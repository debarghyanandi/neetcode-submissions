/**
 * usage.mjs - what a call actually spent, in tokens rather than one dollar figure.
 *
 * total_cost_usd alone cannot say WHY a call was expensive. These four numbers can:
 *   in           fresh input (the prompt and stdin)
 *   cacheWrite   input written to the prompt cache (1.25x input price)
 *   cacheRead    input read back from the cache (0.1x input price)
 *   out          output, INCLUDING thinking - the most expensive kind of token
 * A big cacheWrite on every call is fixed overhead. A big out is thinking or a long answer.
 */

/**
 * Prompt caching OFF for every model call the pipeline makes.
 *
 * On a subscription, `claude -p` writes a 1-hour cache entry at 2x the input price. The pipeline
 * never reads one back: a normal structured call is ONE model request, and no two calls share a
 * prompt (each file, folder and model differs). Measured 2026-09 on the same inputs:
 *   teach (Opus)        input $0.032 with the cache  ->  $0.016 without
 *   visualize (Sonnet)  input $0.063 with the cache  ->  $0.032 without
 * Every script imports this module, so local runs and CI behave the same.
 * PIPELINE_PROMPT_CACHE=1 leaves caching on, for a comparison.
 */
if (process.env.PIPELINE_PROMPT_CACHE !== '1' && process.env.DISABLE_PROMPT_CACHING === undefined) {
  process.env.DISABLE_PROMPT_CACHING = '1';
}

export const EFFORTS = ['low', 'medium', 'high', 'xhigh', 'max'];

/** --effort passthrough. No flag means the model's default effort. */
export function effortArgs(effort) {
  if (!effort) return [];
  if (!EFFORTS.includes(effort)) throw new Error(`--effort must be one of ${EFFORTS.join(', ')}; got "${effort}"`);
  return ['--effort', effort];
}

export function usageOf(env) {
  const u = env?.usage ?? {};
  return {
    in: u.input_tokens ?? 0,
    cacheWrite: u.cache_creation_input_tokens ?? 0,
    cacheRead: u.cache_read_input_tokens ?? 0,
    out: u.output_tokens ?? 0,
    // Thinking is billed as output and is part of `out`, not extra to it.
    thinking: u.output_tokens_details?.thinking_tokens ?? 0,
    // Which cache lifetime the writes used. A 1-hour write costs 2x input, a 5-minute one 1.25x.
    ttl: (u.cache_creation?.ephemeral_1h_input_tokens ?? 0) > 0 ? '1h'
      : (u.cache_creation?.ephemeral_5m_input_tokens ?? 0) > 0 ? '5m' : null,
  };
}

export const addUsage = (a, b) => ({
  in: (a?.in ?? 0) + (b?.in ?? 0),
  cacheWrite: (a?.cacheWrite ?? 0) + (b?.cacheWrite ?? 0),
  cacheRead: (a?.cacheRead ?? 0) + (b?.cacheRead ?? 0),
  out: (a?.out ?? 0) + (b?.out ?? 0),
  thinking: (a?.thinking ?? 0) + (b?.thinking ?? 0),
  ttl: b?.ttl ?? a?.ttl ?? null,
});

const n = (x) => Number(x ?? 0).toLocaleString('en-US');

export const usageLine = (u) =>
  `tokens: in ${n(u?.in)} · cache write ${n(u?.cacheWrite)}${u?.ttl ? ` (${u.ttl})` : ''} · cache read ${n(u?.cacheRead)} · out ${n(u?.out)}${u?.thinking ? ` (thinking ${n(u.thinking)})` : ''}`;

/**
 * The lean call: our own one-line system prompt, and no tools.
 *
 * Without these, every call carries Claude Code's default system prompt and every tool
 * description - about 25-33k tokens, written to the cache at 2x input price on a cold
 * start. On a Sonnet teach call that was ~90% of the cost. None of it is needed: every
 * script hands the code over on stdin, and no prompt asks the model to read a file.
 *
 * This is NOT --bare. --bare stops reading the subscription login; these two flags do not.
 * PIPELINE_FULL_PROMPT=1 turns them off, for a before/after comparison or a quick rollback.
 */
export const SYSTEM_PROMPT =
  'You are one non-interactive step in a build pipeline. You have no tools and no file access: ' +
  'everything you need is in the prompt and on stdin. Reply only through the required structured JSON output.';

export const leanArgs = () =>
  process.env.PIPELINE_FULL_PROMPT === '1' ? [] : ['--system-prompt', SYSTEM_PROMPT, '--tools', ''];
