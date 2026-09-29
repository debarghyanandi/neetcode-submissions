/**
 * The teaching block: a short revision note below the code.
 *
 * Revised 2026-09-29 for how the block is actually used - revising on GitHub,
 * weeks after solving. The reader already solved the problem; they need a
 * reminder of what it asked, the idea in plain words, one worked example, why the
 * complexity is what it is, and the interview material (traps, follow-ups, the
 * trigger). Everything else was cut: provenance and status (the file name and the
 * # / - banner already say it), a variables glossary, an invariant essay, a
 * brute-force paragraph, a C# note, and a bare complexity line the banner repeats.
 *
 * Two shapes:
 *   full   optimal.cs - carries everything that belongs to the PROBLEM (what it
 *          asks, the path from brute force, keywords, follow-ups, trigger). Once.
 *   short  every other file - the problem again (so the file stands alone), its
 *          idea, example, complexity and traps. Follow-ups and the trigger are the same for the whole folder,
 *          so repeating them per file was most of what made a folder a slog.
 *
 * Length is enforced in code (BLOCK_LIMIT), not only asked for in the prompt: the
 * last model ignored "short sentences" for weeks, and a limit that is only a
 * request is a limit that drifts.
 */

const RULE = '='.repeat(80);
const WIDTH = 78;

/** Hard line limits for the whole rendered block, rules and comment markers included. */
// Set from the blocks this replaced, not picked by eye: the Opus 5.5 blocks on main ran a
// median of 73 lines, and the TARGET is about 30% less. A suboptimal or variant file skips the
// sections that belong to the whole problem (path, keywords, follow-ups, trigger), so its
// target is lower.
//
// The model cannot count rendered lines - wrapping is done here - so it lands a few lines either
// side of what it is asked for. Rejecting at the target itself failed 9 of 11 first attempts in
// the first local run (54, 55, 55, 57, 37, 38 ... against 52 / 34), and every retry is a full
// call. So the prompt asks for the target and the hard limit sits a margin above it: only a
// block that really ran long (63, 69) is sent back.
export const BLOCK_TARGET = { full: 52, short: 34 };
export const BLOCK_LIMIT = { full: 60, short: 42 };

/** optimal.cs gets the full block; every other file gets the short one. */
export const roleFor = (file) => (file === 'optimal.cs' ? 'full' : 'short');

/**
 * Sections in their one fixed order. `roles` says which block shape has it.
 * Each `guide` is what the model is told the section is for.
 */
export const SECTIONS = [
  { key: 'problem', marker: 'PROBLEM', roles: ['full', 'short'],
    guide: '2-4 short sentences, enough to recall the problem without opening NeetCode: what the input is, what must be returned, any rule that matters (e.g. "each bar is 1 wide", "return indices, not values"), and one tiny example written as input -> output. From the problem this folder is named after and the code. No constraints, no solution hints.' },
  { key: 'pattern', marker: 'PATTERN', roles: ['full', 'short'],
    guide: 'The pattern name as interviewers say it, under 60 characters, e.g. "Two Pointers (converging) + running max" or "Sliding Window (variable size)".' },
  { key: 'idea', marker: 'IDEA', roles: ['full', 'short'],
    guide: '3-5 short sentences. The core idea of THIS code in plain words, using its variable names, and one sentence on why it is correct. For a file that is not optimal.cs you may name, in one clause, how it differs from optimal.cs.' },
  { key: 'example', marker: 'EXAMPLE', roles: ['full', 'short'],
    guide: 'ONE small input that exercises the main cases (include a tricky case if it fits). 2-4 lines: the input, a compact trace of the key steps, the answer. Every number must be what THIS code produces - work it out, do not guess. Lines are kept as you write them.' },
  { key: 'complexity', marker: 'COMPLEXITY', roles: ['full', 'short'],
    guide: 'Exactly two lines, "Time: <reason>" and "Space: <reason>". Only the reason, in a few words - the values are printed for you. E.g. "Time: each index is visited once by l or r".' },
  { key: 'path', marker: 'PATH TO OPTIMAL', roles: ['full'],
    guide: '2-4 lines, one per step from the brute force to this file, each "approach - complexity - why it is better than the step before", and name the sibling file where one of them IS that step.' },
  { key: 'keywords', marker: 'KEYWORDS', roles: ['full'],
    guide: 'One line: 4-8 comma-separated terms an interviewer or a problem list would use for this problem and technique.' },
  { key: 'watchOut', marker: 'WATCH OUT', roles: ['full', 'short'],
    guide: '2-4 points, each one or two short lines, each starting with "- ": the bug people make with this approach, an input that breaks THIS code as written, or a code comment the code contradicts. Most important section - make each point concrete.' },
  { key: 'followUps', marker: 'FOLLOW-UP', roles: ['full'],
    guide: '3-4 questions an interviewer really asks next: a harder variant, a changed constraint (sorted input, streaming, very large n, less memory), "why does this work", or an alternative technique. Each as a "Q:" line then an "A:" line. The answer is 1-3 sentences: HOW you would change the solution, its new time / space, and the trade-off - enough to say out loud in an interview.' },
  { key: 'trigger', marker: 'TRIGGER', roles: ['full'],
    guide: 'ONE sentence: the signal in a new problem that should make you reach for this pattern.' },
];

const TITLES = { followUps: 'FOLLOW-UP AN INTERVIEWER WILL ASK' };
const sectionsFor = (role) => SECTIONS.filter((s) => s.roles.includes(role));

/**
 * Read the plain-text reply ("@@ NAME" markers) into an object.
 * The markers a role does not use are refused, so a short block cannot grow the
 * full block's sections back.
 * @returns {{ out: object|null, errors: string[] }}
 */
export function parseTeachText(text, role = 'full') {
  const errors = [];
  const wanted = sectionsFor(role);
  const byMarker = new Map(SECTIONS.map((s) => [s.marker, s]));
  const lines = String(text ?? '').replace(/\r\n/g, '\n').split('\n');
  const blocks = [];
  let cur = null;
  for (const line of lines) {
    const m = line.match(/^\s*@@\s*(.+?)\s*$/);
    if (m) { cur = { head: m[1].toUpperCase(), body: [] }; blocks.push(cur); continue; }
    if (cur) cur.body.push(line);   // anything before the first marker is ignored
  }

  const out = {};
  const order = [];
  for (const b of blocks) {
    const s = byMarker.get(b.head);
    if (!s) { errors.push(`unknown marker "@@ ${b.head}"`); continue; }
    if (!s.roles.includes(role)) { errors.push(`"@@ ${b.head}" does not belong in this file's block - leave it out`); continue; }
    const body = b.body.join('\n').replace(/^\n+|\s+$/g, '');
    if (!body.trim()) { errors.push(`"@@ ${b.head}" is empty`); continue; }
    if (out[s.key] !== undefined) { errors.push(`"@@ ${b.head}" appears twice`); continue; }
    order.push(s.key);
    out[s.key] = body;
  }
  for (const s of wanted) if (out[s.key] === undefined) errors.push(`missing "@@ ${s.marker}"`);

  const pos = new Map(SECTIONS.map((s, i) => [s.key, i]));
  for (let i = 1; i < order.length; i++) {
    if (pos.get(order[i]) < pos.get(order[i - 1])) { errors.push(`sections out of order: "${order[i]}" after "${order[i - 1]}"`); break; }
  }

  if (out.pattern) out.pattern = out.pattern.split('\n')[0].trim();

  if (out.complexity !== undefined) {
    const t = out.complexity.match(/^\s*Time\s*:\s*(.+)$/im), sp = out.complexity.match(/^\s*Space\s*:\s*(.+)$/im);
    if (!t || !sp) errors.push('COMPLEXITY needs exactly a "Time: <reason>" line and a "Space: <reason>" line');
    else out.complexity = { time: t[1].trim(), space: sp[1].trim() };
  }

  if (out.followUps !== undefined) {
    const list = [];
    let q = null;
    for (const l of out.followUps.split('\n')) {
      const qm = l.match(/^\s*Q\s*:\s*(.*)$/i), am = l.match(/^\s*A\s*:\s*(.*)$/i);
      if (qm) { q = { question: qm[1].trim(), answer: '' }; list.push(q); }
      else if (am && q) q.answer = am[1].trim();
      else if (q && l.trim()) { if (q.answer) q.answer += ' ' + l.trim(); else q.question += ' ' + l.trim(); }
    }
    if (list.length < 3 || list.length > 4) errors.push(`${list.length} follow-up questions - need 3 or 4, each as a "Q:" line then an "A:" line`);
    if (list.some((f) => !f.question || !f.answer)) errors.push('a follow-up is missing its "Q:" or its "A:"');
    out.followUps = list;
  }

  return { out: errors.length ? null : out, errors };
}

/** Lines of text the model may write: the block limit minus markers, rules and section headings. */
const textBudget = (role) => BLOCK_TARGET[role] - 5 - sectionsFor(role).filter((s) => s.key !== 'problem' && s.key !== 'pattern').length;

export const TEACH_INSTRUCTIONS = (ctx) => {
  const role = ctx.role ?? 'full';
  const secs = sectionsFor(role);
  const siblings = (ctx.siblings ?? []).map((s) => `  ${s.name.padEnd(24)}${s.time} time / ${s.space} space${s.name === ctx.file ? '   <- this file' : ''}`);
  return [
    'You are writing a short REVISION NOTE for ONE C# solution file, given on stdin. It goes below the code.',
    '',
    `The problem is the NeetCode problem this folder is named after: "${ctx.slug}".`,
    'The reader solved it weeks ago and is revising for an interview. They remember the problem once',
    'reminded; they need the idea, one worked example, why the complexity holds, and what an',
    'interviewer will probe. Short and easy to read beats complete.',
    '',
    'Files in this folder (the file name already says optimal / suboptimal; the complexity is already',
    'printed above the code):',
    ...siblings,
    '',
    role === 'full'
      ? 'This is optimal.cs, so its note carries everything about the PROBLEM: what it asks, the path from brute force, keywords, follow-ups, the trigger.'
      : 'This is NOT optimal.cs. optimal.cs carries the path from brute force, keywords, follow-ups and the trigger. Write ONLY the sections below - no comparison section, no follow-ups.',
    '',
    'Rules:',
    // The rendered block adds the comment markers, three rules and one heading per body section.
    `- Aim for about ${textBudget(role) - 4} lines of text in total (76 characters each), and never more than ${textBudget(role)}.`,
    '  Rich but not padded: every line should be worth re-reading before an interview.',
    '- Plain English for a reader whose second language is English: short sentences, common words.',
    '  Use the standard DSA interview terms (two pointers, sliding window, monotonic stack, prefix sum,',
    '  memoization, BFS/DFS, heap, amortized O(1), ...) and explain an uncommon one in a few words.',
    '- Ground every claim in this code or the algorithm. Name its real variables. Never invent input',
    '  limits; you are not given the constraints.',
    '- Never make the same point in two sections.',
    '- Plain ASCII. No markdown, no backticks, no emoji.',
    '',
    'OUTPUT FORMAT. Reply with ONLY these sections, in exactly this order, each starting with its',
    'marker line. No text before the first marker, no JSON, no code fences.',
    '',
    ...secs.flatMap((s) => [`@@ ${s.marker}`, `   ${s.guide}`]),
  ].join('\n');
};

const wrap = (text, width) => {
  const out = [];
  for (const para of String(text).split(/\n/)) {
    if (!para.trim()) { out.push(''); continue; }
    const indent = (para.match(/^\s*/) || [''])[0].slice(0, 6);
    // A "- " point wraps under its own text, not under the dash.
    const hang = /^\s*- /.test(para) ? indent + '  ' : indent;
    let line = '', first = true;
    for (const w of para.trim().split(/\s+/)) {
      const lead = first ? indent : hang;
      if (line && (line + ' ' + w).length > width - lead.length) { out.push(lead + line); line = w; first = false; }
      else line = line ? line + ' ' + w : w;
    }
    if (line) out.push((first ? indent : hang) + line);
  }
  return out;
};

// A closing comment marker inside generated prose would end the block early.
const safe = (s) => String(s).replace(/\*\//g, '* /');

/**
 * Render the block. `out` is parseTeachText's object; ctx carries role, time, space.
 */
export function buildTeachingBlock(out, ctx) {
  const role = ctx.role ?? 'full';
  // Labels are column-aligned, so the label must never go through wrap().
  const labelled = (label, value) => {
    const head = ' ' + label.padEnd(8) + ': ';
    const cont = ' '.repeat(head.length);
    // One paragraph: a line break the model left mid-sentence must not survive into the banner.
    const flat = String(value).split('\n').map((l) => l.trim()).filter(Boolean).join(' ');
    return wrap(safe(flat), WIDTH - head.length).map((l, i) => (i ? cont + l.trim() : head + l.trim()));
  };
  const body = (t) => wrap(safe(t), WIDTH - 2).map((l) => (l ? '  ' + l : ''));

  const lines = ['/*', RULE];
  lines.push(...labelled('PROBLEM', out.problem));
  lines.push(...labelled('PATTERN', out.pattern), RULE);

  for (const s of sectionsFor(role)) {
    if (s.key === 'problem' || s.key === 'pattern') continue;
    lines.push(TITLES[s.key] ?? s.marker);
    if (s.key === 'complexity') {
      const tag = (k, v) => `${k.padEnd(6)}${v}`;
      const w = Math.max(tag('Time', ctx.time).length, tag('Space', ctx.space).length) + 2;
      // Aligned columns, so the label part must not go through wrap(); only the reason wraps.
      for (const [k, v, why] of [['Time', ctx.time, out.complexity.time], ['Space', ctx.space, out.complexity.space]]) {
        const head = '  ' + tag(k, v).padEnd(w);
        wrap(safe(why), WIDTH - head.length).forEach((l, i) => lines.push((i ? ' '.repeat(head.length) : head) + l.trim()));
      }
      continue;
    }
    if (s.key === 'followUps') {
      out.followUps.forEach((f, i) => {
        lines.push(...wrap(safe(`${i + 1}. ${f.question}`), WIDTH - 2).map((l, j) => (j ? '     ' + l : '  ' + l)));
        lines.push(...wrap(safe(`-> ${f.answer}`), WIDTH - 5).map((l, j) => (j ? '        ' + l : '     ' + l)));
      });
      continue;
    }
    lines.push(...body(out[s.key]));
  }
  lines.push(RULE, '*/');
  return lines.join('\n');
}

/** A rendered block over its role's limit, as a retry instruction; null when it fits. */
export function lengthProblem(block, role) {
  const n = block.split('\n').length, max = BLOCK_LIMIT[role];
  return n > max ? `the note rendered to ${n} lines; the limit is ${max}. Shorten the longest sections - cut words, keep every section` : null;
}

/** STATUS text from the filename the ranking produced. */
export const statusFor = (name) =>
  name.startsWith('optimal-variant') ? 'Optimal variant - ties the best complexity by another route'
  : name.startsWith('optimal') ? 'Optimal'
  : 'Suboptimal';

/** SOURCE text. Absence of the marker is reported as absence, never as authorship. */
export function sourceFor(origin, name, prov) {
  const from = origin && /^submission-\d+\./.test(origin) ? ` (${origin.replace(/\.cs$/, '')})` : '';
  const why = prov && prov.evidence ? ` - ${prov.evidence}` : '';
  if (!prov || prov.selfMarked === null || prov.selfMarked === undefined) {
    return `Provenance unknown${from} - no marker and no earlier annotation`;
  }
  return prov.selfMarked
    ? `YOUR OWN SOLUTION${from}${why}`
    : `Reference solution - not one you solved yourself${from}${why}`;
}

/**
 * Split off a teaching block sitting at the END of a file.
 *
 * The block lives below the code, so replacing it must not disturb the short
 * banner at the top - that belongs to classify.mjs and carries different
 * information. Requires a full-width = rule inside the block before claiming
 * it: a file may legitimately end in some other block comment, and eating
 * someone's trailing comment would be a silent, permanent loss.
 */
export function splitTrailingTeach(src) {
  const eol = src.includes('\r\n') ? '\r\n' : '\n';
  const lines = src.split(/\r?\n/);

  let end = lines.length - 1;
  while (end >= 0 && lines[end].trim() === '') end--;
  if (end < 0 || lines[end].trim() !== '*/') return { code: src, eol, had: false };

  let start = end;
  while (start >= 0 && lines[start].trim() !== '/*') start--;
  if (start < 0) return { code: src, eol, had: false };

  if (!lines.slice(start, end + 1).some((l) => /^={60,}$/.test(l.trim()))) {
    return { code: src, eol, had: false };
  }

  let k = start - 1;
  while (k >= 0 && lines[k].trim() === '') k--;
  return { code: lines.slice(0, k + 1).join(eol), eol, had: true };
}
