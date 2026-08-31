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
  if (i >= lines.length) return { body: src, eol, had: false };

  // A /* ... */ teaching block, which is what the visualiser-era headers use.
  if (/^\s*\/\*/.test(lines[i])) {
    let k = i;
    while (k < lines.length && !/\*\//.test(lines[k])) k++;
    if (k >= lines.length) return { body: src, eol, had: false };   // unterminated - leave it alone
    k++;
    while (k < lines.length && lines[k].trim() === '') k++;
    return { body: lines.slice(k).join(eol), eol, had: true };
  }

  if (!RULE_RE.test(lines[i])) return { body: src, eol, had: false };

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
 * The stable part of a header: everything except the model's prose.
 *
 * Model wording varies between runs on identical input. Regenerating headers
 * unconditionally therefore produces a diff every night forever - a commit that
 * reworders your files and changes nothing. Compare on this instead, and leave
 * a header alone when only the prose would move.
 */
export function headerSignature(name, sol, selfMark, ranked) {
  return JSON.stringify({
    name,
    time: sol.time,
    space: sol.space,
    algorithm: sol.algorithm,
    approachKey: sol.approachKey,
    correct: sol.correct,
    selfMark: !!selfMark,
    standing: standing(name, ranked),
  });
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

  // Every content line wraps to the rule width. One unwrapped line - the
  // algorithm plus a long approachKey is the usual offender - juts out past the
  // banner and makes the whole block look broken.
  const W = WIDTH - 4;
  const W$ = (t) => wrap(t, W).map(L);

  // Keep the approachKey with the algorithm when it fits, give it its own
  // line when it doesn't, rather than letting wrap() split it mid-slug.
  const algo = `${sol.algorithm}   [${sol.approachKey}]`.length <= W
    ? [L(`${sol.algorithm}   [${sol.approachKey}]`)]
    : [...W$(sol.algorithm), L(`[${sol.approachKey}]`)];

  // The title line is column-aligned on purpose, so it must not go through
  // wrap() - that collapses runs of spaces and destroys the padding.
  const title = `${name.padEnd(22)}${sol.time} time / ${sol.space} space`;
  const titleLines = title.length <= W
    ? [L(title)]
    : [L(name), L(`${sol.time} time / ${sol.space} space`)];

  return [
    rule,
    ...titleLines,
    ...algo,
    ...W$(standing(name, ranked)),
    L(),
    ...W$(provenance),
    ...(sol.correct ? [] : [L(), ...W$('*** flagged as INCORRECT by classification - check before trusting ***')]),
    L(),
    ...W$(sol.note),
    rule,
  ].join('\n');
}

/** Replace whatever header the file has with a freshly generated one. */
export function applyHeader(src, header) {
  const { body, eol } = stripHeader(src);
  return header.split('\n').join(eol) + eol + eol + body.replace(/^(\r?\n)+/, '');
}
