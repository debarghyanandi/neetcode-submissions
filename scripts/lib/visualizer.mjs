/**
 * Splicing and validating a generated PROBLEM definition into the visualizer
 * chassis.
 *
 * The chassis is 32KB of design system and playback engine lifted verbatim from
 * the visualizers already in this repo, so a generated one is visually identical
 * to the hand-built ones by construction rather than by asking a model to match
 * a style it cannot see.
 *
 * The only generated part is the PROBLEM object. That is also the only part that
 * can be silently broken: a visualizer that throws on step 3 still looks like a
 * finished 40KB file. Hence validate() below, which actually runs the thing.
 */

import { readFileSync, existsSync, writeFileSync, mkdirSync, rmSync } from 'node:fs';
import { join, dirname } from 'node:path';
import { execFileSync } from 'node:child_process';
import { REPO } from './scan.mjs';

const CHASSIS = join(REPO, 'scripts', 'templates', 'visualizer.chassis.html');
const MARKER = '/*__PROBLEM__*/';

export const loadChassis = () => readFileSync(CHASSIS, 'utf8');

export function splice(problemSource) {
  const chassis = loadChassis();
  if (!chassis.includes(MARKER)) throw new Error(`chassis is missing ${MARKER}`);
  return chassis.replace(MARKER, problemSource);
}

/** The shared helpers the PROBLEM object is allowed to call. */
function helpersFromChassis() {
  const c = loadChassis();
  const start = c.indexOf('<script>');
  const end = c.indexOf(MARKER);
  if (start < 0 || end < 0) throw new Error('cannot locate helper region in chassis');
  return c.slice(start + '<script>'.length, end);
}

/**
 * Run the generated definition for real and report what breaks.
 *
 * Checks, in the order a reader would care about them:
 *   parses at all -> has the required shape -> the default input is accepted
 *   -> every solution simulates -> every step is renderable
 *
 * A step whose `lines` point outside its own `code` array is the interesting
 * failure: the visualizer highlights nothing and looks merely dull, so nobody
 * reports it.
 */
export function validate(problemSource) {
  const problems = [];
  const tmp = join(REPO, '.agent', 'tmp');
  mkdirSync(tmp, { recursive: true });
  const harness = join(tmp, 'validate-problem.cjs');

  const code = `
${helpersFromChassis()}
${problemSource}
const out = { errors: [], stats: {} };
const E = (m) => out.errors.push(m);
try {
  if (typeof PROBLEM !== 'object' || !PROBLEM) E('PROBLEM is not an object');
  for (const k of ['title','note','inputs','parse','shuffle','solutions']) {
    if (PROBLEM[k] === undefined) E('PROBLEM.' + k + ' is missing');
  }
  if (!Array.isArray(PROBLEM.inputs) || !PROBLEM.inputs.length) E('PROBLEM.inputs must be a non-empty array');
  if (!Array.isArray(PROBLEM.solutions) || !PROBLEM.solutions.length) E('PROBLEM.solutions must be a non-empty array');

  const raw = {};
  for (const inp of (PROBLEM.inputs || [])) {
    if (!inp.id) E('an input has no id');
    raw[inp.id] = inp.value;
  }

  let parsed = null;
  try { parsed = PROBLEM.parse(raw); } catch (e) { E('parse() threw on its own default input: ' + e.message); }
  if (parsed && !parsed.ok) E('parse() rejects its own default input: ' + parsed.msg);

  try { const s = PROBLEM.shuffle(); if (!s || typeof s !== 'object') E('shuffle() did not return an object'); }
  catch (e) { E('shuffle() threw: ' + e.message); }

  if (parsed && parsed.ok) {
    PROBLEM.solutions.forEach((sol, si) => {
      const where = 'solutions[' + si + ']' + (sol && sol.label ? ' (' + sol.label + ')' : '');
      for (const k of ['label','badge','blurb','code','simulate']) {
        if (sol[k] === undefined) E(where + '.' + k + ' is missing');
      }
      if (!Array.isArray(sol.code) || !sol.code.length) { E(where + '.code must be a non-empty array'); return; }
      let steps = null;
      try { steps = sol.simulate(parsed.value); } catch (e) { E(where + '.simulate() threw: ' + e.message); return; }
      if (!Array.isArray(steps) || !steps.length) { E(where + '.simulate() returned no steps'); return; }
      if (steps.length > 4000) E(where + '.simulate() produced ' + steps.length + ' steps - runaway loop?');
      steps.forEach((st, k) => {
        const w = where + '.steps[' + k + ']';
        if (!st || typeof st !== 'object') { E(w + ' is not an object'); return; }
        if (!Array.isArray(st.lines)) E(w + '.lines must be an array');
        else for (const ln of st.lines) {
          if (!Number.isInteger(ln) || ln < 1 || ln > sol.code.length) {
            E(w + '.lines has ' + ln + ', outside code lines 1..' + sol.code.length + ' - highlights nothing');
          }
        }
        if (typeof st.msg !== 'string' || !st.msg.trim()) E(w + '.msg is empty');
        if (!Array.isArray(st.panels)) E(w + '.panels must be an array');
      });
      out.stats[where] = steps.length + ' steps';
    });
  }
} catch (e) { E('fatal: ' + e.message); }
process.stdout.write(JSON.stringify(out));
`;
  writeFileSync(harness, code, 'utf8');
  try {
    const res = execFileSync(process.execPath, [harness], { encoding: 'utf8', timeout: 30000, stdio: ['pipe','pipe','pipe'] });
    return JSON.parse(res);
  } catch (e) {
    const err = String(e.stderr || e.message).split('\n').slice(0, 6).join(' | ');
    return { errors: [`the definition does not even run: ${err}`], stats: {} };
  } finally {
    try { rmSync(harness); } catch { /* best effort */ }
  }
}
