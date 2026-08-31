/**
 * The teaching block: a /* ... *\/ preamble carrying why the pattern applies,
 * not just what the code does.
 *
 * Split of responsibility, same as everywhere else in this pipeline:
 *   deterministic   the banner (PATTERN / SOURCE / STATUS) and COMPLEXITY,
 *                   all of which we already know from classification
 *   the model       the sections that require reading and judgement
 *
 * The model chooses its own sections. It is told what the banner already
 * states so it does not restate it - redundancy is the failure mode when
 * sections are free-form.
 */

const RULE = '='.repeat(80);
const WIDTH = 78;

export const SECTIONS_SCHEMA = {
  type: 'object',
  additionalProperties: false,
  properties: {
    pattern: {
      type: 'string',
      description: 'The pattern name for the banner, e.g. "Sliding Window / Greedy - track the running minimum". Under 60 chars.',
    },
    sections: {
      type: 'array',
      minItems: 3,
      maxItems: 9,
      items: {
        type: 'object',
        additionalProperties: false,
        properties: {
          title: { type: 'string', description: 'SHORT UPPERCASE HEADING, e.g. WHY THIS PATTERN, BRUTE FORCE, INVARIANT, ALGORITHM, TRIGGER, WATCH OUT, or one you choose that fits this problem.' },
          body: { type: 'string', description: 'Prose or numbered steps. Newlines allowed. No heading inside.' },
        },
        required: ['title', 'body'],
      },
    },
  },
  required: ['pattern', 'sections'],
};

export const TEACH_INSTRUCTIONS = (ctx) => [
  'You are writing the study preamble for ONE C# solution file, given on stdin.',
  '',
  'The reader is the person who wrote it, revising weeks later. Write for recall and for the',
  'follow-up an interviewer would ask - the correctness argument, the invariant, the trap.',
  '',
  'Facts already printed in the banner ABOVE your sections. Never restate any of them:',
  `  PATTERN is your "pattern" field.`,
  `  SOURCE  : ${ctx.source}`,
  `  STATUS  : ${ctx.status}`,
  `  COMPLEXITY is emitted separately as ${ctx.time} time / ${ctx.space} space. Do NOT write a complexity section.`,
  '',
  'Rules:',
  '- Choose the sections this problem actually needs. Three to nine. Omit any section you would',
  '  have to pad. A short block with four real sections beats nine with filler.',
  '- Never repeat a point across two sections, and never repeat the banner.',
  '- Every claim must be grounded in the code you were given or in the algorithm itself.',
  '  Do not assert performance folklore about the runtime, the JIT, or the compiler - if you',
  '  cannot show it from the code, leave it out.',
  '- Name the concrete variables and values from THIS file, not a generic template.',
  '- If the solution is not optimal, say what the better approach is and why this one loses.',
  '- Plain ASCII. No markdown, no backticks, no emoji.',
].join('\n');

const wrap = (text, width) => {
  const out = [];
  for (const para of String(text).split(/\n/)) {
    if (!para.trim()) { out.push(''); continue; }
    const indent = (para.match(/^\s*/) || [''])[0].slice(0, 6);
    let line = '';
    for (const w of para.trim().split(/\s+/)) {
      if (line && (line + ' ' + w).length > width - indent.length) { out.push(indent + line); line = w; }
      else line = line ? line + ' ' + w : w;
    }
    if (line) out.push(indent + line);
  }
  return out;
};

// A closing comment marker inside generated prose would end the block early.
const safe = (s) => String(s).replace(/\*\//g, '* /');

export function buildTeachingBlock({ pattern, sections }, ctx) {
  // Labels are column-aligned, so the label must never go through wrap() -
  // wrap collapses runs of spaces and silently destroys the alignment.
  const labelled = (label, value) => {
    const head = ' ' + label.padEnd(8) + ': ';
    const cont = ' '.repeat(head.length);
    return wrap(safe(value), WIDTH - head.length).map((l, i) => (i ? cont + l.trim() : head + l.trim()));
  };

  const lines = [
    '/*',
    RULE,
    ...labelled('PATTERN', pattern),
    ...labelled('SOURCE', ctx.source),
    ...labelled('STATUS', ctx.status),
    RULE,
  ];

  for (const s of sections) {
    lines.push(safe(String(s.title).toUpperCase().trim()));
    lines.push(...wrap(safe(s.body), WIDTH).map((l) => (l ? '  ' + l : '')));
  }

  lines.push('COMPLEXITY');
  lines.push(`  Time  : ${ctx.time}`);
  lines.push(`  Space : ${ctx.space}`);
  lines.push(RULE);
  lines.push('*/');
  return lines.join('\n');
}

/** STATUS text from the filename the ranking produced. */
export const statusFor = (name) =>
  name.startsWith('optimal-variant') ? 'Optimal variant - ties the best complexity by another route'
  : name.startsWith('optimal') ? 'Optimal'
  : 'Suboptimal';

/** SOURCE text. Absence of the marker is reported as absence, never as authorship. */
export function sourceFor(origin, name, selfMark) {
  const from = origin && /^submission-\d+\./.test(origin) ? ` (${origin.replace(/\.cs$/, '')})` : '';
  return selfMark
    ? `YOUR OWN SOLUTION${from}, marked '//My solution'`
    : `No '//My solution' marker in the source${from} - provenance unknown`;
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
