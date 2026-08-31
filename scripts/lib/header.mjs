/**
 * Generating and replacing the banner header at the top of a solution file.
 *
 * The banner character carries provenance, following the convention already in
 * this repo:  #  you marked it as your own,  -  no marker found.
 *
 * On wording: a missing //My solution marker means "unmarked", not "not yours".
 * You can forget to type it. A header that flatly asserts you didn't solve
 * something you did is worse than one that reports only what it knows, so the
 * unmarked banner says no marker was found and stops there.
 */

const WIDTH = 74;

/** A banner rule: // ####...  or  // ----... */
const RULE_RE = /^\s*\/\/\s*[#\-=]{8,}\s*$/;
const COMMENT_RE = /^\s*\/\//;

/**
 * Strip a leading banner block, if present.
 *
 * Matches both styles already in the repo (# rules and - rules) and anything we
 * generate. Only touches a block that STARTS the file and is bounded by two
 * rules - a stray comment at the top is left alone.
 */
export function stripHeader(src) {
  const eol = src.includes('\r\n') ? '\r\n' : '\n';
  const lines = src.split(/\r?\n/);

  let i = 0;
  while (i < lines.length && lines[i].trim() === '') i++;
  if (i >= lines.length || !RULE_RE.test(lines[i])) return { body: src, eol, had: false };

  let j = i + 1;
  while (j < lines.length && COMMENT_RE.test(lines[j]) && !RULE_RE.test(lines[j])) j++;
  if (j >= lines.length || !RULE_RE.test(lines[j])) return { body: src, eol, had: false };

  j++;                                            // past the closing rule
  while (j < lines.length && lines[j].trim() === '') j++;
  return { body: lines.slice(j).join(eol), eol, had: true };
}

const wrap = (text, width) => {
  const out = [];
  let line = '';
  for (const w of String(text).split(/\s+/).filter(Boolean)) {
    if (line && (line + ' ' + w).length > width) { out.push(line); line = w; }
    else line = line ? line + ' ' + w : w;
  }
  if (line) out.push(line);
  return out;
};

/**
 * Where this solution sits relative to its siblings. Deterministic - derived
 * from the ranked list, never asked of the model.
 */
function standing(name, ranked) {
  if (ranked.length === 1) return 'the only solution in this folder';
  const me = ranked.findIndex((r) => r.name === name);
  const best = ranked[0];
  if (me === 0) {
    const next = ranked[1];
    return `ranks above ${next.name} (${next.time} time / ${next.space} space)`;
  }
  if (ranked[me].time === best.time && ranked[me].space === best.space) {
    return `ties with ${best.name} on ${best.time} time / ${best.space} space`;
  }
  return `ranks below ${best.name} (${best.time} time / ${best.space} space)`;
}

/**
 * @param name      final filename, e.g. optimal.cs
 * @param origin    filename it came from, e.g. submission-3.cs (null if already curated)
 * @param sol       classifier output for this file
 * @param selfMark  did the source carry a //My solution marker
 * @param ranked    [{name,time,space}] every solution in the folder, best first
 */
export function buildHeader(name, origin, sol, selfMark, ranked) {
  const ch = selfMark ? '#' : '-';
  const rule = `// ${ch.repeat(WIDTH)}`;
  const L = (t = '') => `// ${ch}  ${t}`.trimEnd();

  // Only a raw NeetCode file is "from submission-N"; a curated file that changed
  // rank is "was optimal.cs". Saying "from suboptimal" for the latter is wrong.
  let where = '';
  if (origin && origin !== name) {
    where = /^submission-\d+\./.test(origin)
      ? ` (from ${origin.replace(/\.cs$/, '')})`
      : ` (was ${origin})`;
  }
  const provenance = selfMark
    ? `YOU SOLVED THIS YOURSELF - marked '//My solution'${where}`
    : `No '//My solution' marker in the source${where}`;

  return [
    rule,
    L(`${name.padEnd(22)}${sol.time} time / ${sol.space} space`),
    L(`${sol.algorithm}   [${sol.approachKey}]`),
    L(standing(name, ranked)),
    L(),
    L(provenance),
    ...(sol.correct ? [] : [L(), L('*** flagged as INCORRECT by classification - check before trusting ***')]),
    L(),
    ...wrap(sol.note, WIDTH - 4).map(L),
    rule,
  ].join('\n');
}

/** Replace whatever header the file has with a freshly generated one. */
export function applyHeader(src, header) {
  const { body, eol } = stripHeader(src);
  return header.split('\n').join(eol) + eol + eol + body.replace(/^(\r?\n)+/, '');
}
