#!/usr/bin/env node
/**
 * probe-cli.mjs - find out exactly which part of the classify.mjs invocation
 * your installed Claude Code rejects.
 *
 * Escalating probes, each adding ONE thing to the previous. The first FAIL is
 * the culprit; everything after it is noise. Each call is a few tokens.
 *
 *   node scripts/probe-cli.mjs
 */

import { execFileSync, spawnSync } from 'node:child_process';
import { platform } from 'node:process';

const SCHEMA = JSON.stringify({
  type: 'object',
  additionalProperties: false,
  properties: { word: { type: 'string' } },
  required: ['word'],
});

const PROMPT = 'Reply with the single word: ok';

const PROBES = [
  ['A  bare invocation',        ['-p', PROMPT]],
  ['B  + --output-format json', ['-p', PROMPT, '--output-format', 'json']],
  ['C  + --json-schema',        ['-p', PROMPT, '--output-format', 'json', '--json-schema', SCHEMA]],
  ['D  + --permission-mode',    ['-p', PROMPT, '--output-format', 'json', '--json-schema', SCHEMA, '--permission-mode', 'dontAsk']],
  ['E  + --max-turns 1',        ['-p', PROMPT, '--output-format', 'json', '--json-schema', SCHEMA, '--permission-mode', 'dontAsk', '--max-turns', '1']],
  ['F  + stdin payload',        ['-p', PROMPT, '--output-format', 'json', '--json-schema', SCHEMA, '--permission-mode', 'dontAsk', '--max-turns', '1'], 'some text on stdin'],
  // The lean call (lib/usage.mjs leanArgs). --max-turns 3, because structured output itself
  // may take a turn. PASS here means structured_output still arrives with no tools.
  ['G  + --tools ""',          ['-p', PROMPT, '--output-format', 'json', '--json-schema', SCHEMA, '--permission-mode', 'dontAsk', '--max-turns', '3', '--tools', ''], 'some text on stdin'],
  ['H  + --system-prompt',      ['-p', PROMPT, '--output-format', 'json', '--json-schema', SCHEMA, '--permission-mode', 'dontAsk', '--max-turns', '3', '--tools', '', '--system-prompt', 'You are one non-interactive step in a build pipeline. Reply only through the required structured JSON output.'], 'some text on stdin'],
];

console.log(`\nplatform ${platform}   node ${process.version}`);
const v = spawnSync('claude', ['--version'], { encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'] });
console.log(`claude --version -> ${(v.stdout || v.stderr || String(v.error)).trim().split('\n')[0]}`);
console.log(`spawn resolution -> ${v.error ? `ERROR ${v.error.code}` : 'ok'}\n`);

let firstFailure = null;

for (const [label, args, input] of PROBES) {
  process.stdout.write(`${label.padEnd(30)} `);
  try {
    const out = execFileSync('claude', args, {
      encoding: 'utf8',
      input: input ?? '',
      maxBuffer: 8 * 1024 * 1024,
      stdio: ['pipe', 'pipe', 'pipe'],
      timeout: 180000,
    });
    const flat = out.replace(/\s+/g, ' ').trim();
    let extra = '';
    try {
      const j = JSON.parse(out);
      if (j.structured_output !== undefined) extra = `  structured_output=${JSON.stringify(j.structured_output)}`;
      if (j.usage) extra += `\n${' '.repeat(31)}turns ${j.num_turns} · cache write ${j.usage.cache_creation_input_tokens ?? 0} · cache read ${j.usage.cache_read_input_tokens ?? 0} · in ${j.usage.input_tokens ?? 0} · out ${j.usage.output_tokens ?? 0}`;
      else if (j.result !== undefined) extra = `  result=${JSON.stringify(String(j.result).slice(0, 60))}`;
    } catch { /* plain text probe */ }
    console.log(`PASS   ${flat.slice(0, 70)}${extra}`);
  } catch (e) {
    console.log('FAIL');
    console.log(`     exit status : ${e.status ?? '(none)'}`);
    console.log(`     error code  : ${e.code ?? '(none)'}`);
    const err = String(e.stderr ?? '').trim();
    const out = String(e.stdout ?? '').trim();
    if (err) console.log(`     stderr      : ${err.slice(0, 700)}`);
    if (out) console.log(`     stdout      : ${out.slice(0, 700)}`);
    if (!err && !out) console.log(`     message     : ${e.message.slice(0, 400)}`);
    if (!firstFailure) firstFailure = label;
    console.log('');
  }
}

console.log(firstFailure
  ? `\nFirst failure: ${firstFailure}. That is the thing to fix; ignore later failures.\n`
  : '\nAll probes passed - the flags are fine, so the problem is in the payload or the schema.\n');
