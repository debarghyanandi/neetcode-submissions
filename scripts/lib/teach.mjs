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

/**
 * The block has a FIXED set of sections in a FIXED order.
 *
 * It used to be a free array: the model picked three to nine sections and titled them.
 * Two models on the same file then produced different sections in a different order, and
 * Sonnet simply left out the interviewer follow-ups that Opus had chosen to write. Reading
 * fifty blocks for revision only works if every block has the same shape. So the schema
 * names each section, and the schema - not the prompt - decides which exist and in what order.
 *
 * The one flexible slot is keyDetails: 0-2 sections the model titles itself, for the
 * problem-specific trick. It always sits in the same place.
 */
const text = (description) => ({ type: 'string', description });

export const SECTIONS_SCHEMA = {
  type: 'object',
  additionalProperties: false,
  properties: {
    pattern: text('The pattern name for the banner, e.g. "Sliding Window / Greedy - track the running minimum". Under 60 chars.'),
    whyThisPattern: text('2-4 sentences. What in the PROBLEM STATEMENT points at this pattern, and why the pattern solves it. Name this file\'s variables.'),
    bruteForce: text('2-4 sentences. If STATUS is Optimal or an Optimal variant: the simplest CORRECT approach a person would write first, its complexity, and why it loses. Not a broken version of this code. If STATUS is Suboptimal: the better approach, and exactly why this file loses to it.'),
    invariant: text('2-4 sentences. The invariant and the correctness argument: what stays true at each step of THIS code, and why that makes the answer right.'),
    keyDetails: {
      type: 'array', minItems: 0, maxItems: 2,
      description: 'Zero to two problem-specific tricks worth their own section, 2-4 sentences each. Only a point no other field covers. Empty is the normal answer - never pad.',
      items: {
        type: 'object', additionalProperties: false,
        properties: { title: text('SHORT UPPERCASE HEADING, plain words.'), body: text('The detail. Newlines allowed.') },
        required: ['title', 'body'],
      },
    },
    watchOut: text('2-5 sentences. What can go wrong in THIS code AS WRITTEN: input that breaks it, a bug waiting to happen, a comment the code contradicts. Each point is made here and nowhere else.'),
    followUps: {
      type: 'array', minItems: 2, maxItems: 4,
      description: 'Follow-up questions an interviewer will ask: how would the solution CHANGE - less memory, no recursion, a variant of the problem, much larger input. Never a point already made in watchOut or keyDetails.',
      items: {
        type: 'object', additionalProperties: false,
        properties: { question: text('The question, as the interviewer would say it. One sentence.'), answer: text('1-2 sentences: the answer, then the trade-off.') },
        required: ['question', 'answer'],
      },
    },
    trigger: text('ONE sentence: the signal in a new problem that should make you reach for this pattern next time.'),
    csharpNote: text('1-2 sentences. One C#-specific point about this code - the right collection, a costly API, a better idiom - that no other field already made. Grounded in the code, not folklore.'),
  },
  required: ['pattern', 'whyThisPattern', 'bruteForce', 'invariant', 'keyDetails', 'watchOut', 'followUps', 'trigger', 'csharpNote'],
};

/**
 * The model's fields, in the one order every block uses. COMPLEXITY is added after these
 * by buildTeachingBlock, from the stored classification.
 */
export function toSections(out, ctx) {
  const suboptimal = /^Suboptimal/.test(ctx?.status ?? '');
  const followUps = (out.followUps ?? [])
    .map((f, i) => `${i + 1}. ${String(f.question).trim()}\n   ${String(f.answer).trim()}`)
    .join('\n');
  return [
    { title: 'WHY THIS PATTERN', body: out.whyThisPattern },
    { title: suboptimal ? 'BETTER APPROACH' : 'BRUTE FORCE', body: out.bruteForce },
    { title: 'INVARIANT', body: out.invariant },
    ...(out.keyDetails ?? []),
    { title: 'WATCH OUT', body: out.watchOut },
    { title: 'FOLLOW-UP AN INTERVIEWER WILL ASK', body: followUps },
    { title: 'TRIGGER', body: out.trigger },
    { title: 'C# NOTE', body: out.csharpNote },
  ].filter((x) => x.body && String(x.body).trim());
}

export const TEACH_INSTRUCTIONS = (ctx) => [
  'You are writing the study preamble for ONE C# solution file, given on stdin.',
  '',
  'The reader is the person who wrote it, revising weeks later for an interview. Write for recall',
  'and for the follow-up an interviewer would ask.',
  '',
  'Fill every field of the output schema. The sections and their order are fixed by the schema -',
  'do not add headings of your own inside a field.',
  '',
  'Facts already printed in the banner ABOVE your sections. Never restate any of them:',
  `  PATTERN is your "pattern" field.`,
  `  SOURCE  : ${ctx.source}`,
  `  STATUS  : ${ctx.status}`,
  `  COMPLEXITY is emitted separately as ${ctx.time} time / ${ctx.space} space. Do not write it again.`,
  '',
  'Rules:',
  '- Never repeat a point across two fields. If a follow-up would repeat WATCH OUT or a key detail,',
  '  choose a different follow-up.',
  '- Every claim must be grounded in the code you were given or in the algorithm itself.',
  '  Do not assert performance folklore about the runtime, the JIT, or the compiler - if you',
  '  cannot show it from the code, leave it out.',
  '- Name the concrete variables and values from THIS file, not a generic template.',
  '- You are NOT given the problem\'s constraints. Never invent input sizes or limits ("a 300x300 grid").',
  '- Short. The whole block should read in two minutes. Each field has its own job; a point that fits',
  '  two fields goes in the first one and is left out of the other.',
  '- If a comment in the code states something the code contradicts, say so in watchOut.',
  '- Plain English for a reader whose second language is English: short sentences, common words,',
  '  no word play. Keep real technical terms, and explain one in plain words the first time.',
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
