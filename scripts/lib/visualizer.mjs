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
      // The blurb sits in a fixed-height header slot. Past ~450 visible
      // characters it pushes past five rendered lines and the layout sprawls.
      const visible = String(sol.blurb || '').replace(/<[^>]+>/g, '');
      if (visible.length > 450) {
        E(where + '.blurb is ' + visible.length + ' visible chars; over 450 it exceeds five rendered lines');
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
        if (!Array.isArray(st.panels)) { E(w + '.panels must be an array'); return; }

        // Panel shapes. Without this, a panel that the renderer will choke on
        // sails through: the file looks finished, and playback dies mid-run on
        // whichever step first contains the bad panel.
        st.panels.forEach((pn, pi) => {
          const pw = w + '.panels[' + pi + ']';
          if (!pn || typeof pn !== 'object') { E(pw + ' is not an object'); return; }
          const arr = (k) => Array.isArray(pn[k]);
          switch (pn.t) {
            case 'chips':
              // The one that bit us: pChips takes ROWS, each with its own items
              // array - not a flat list of chips.
              if (!arr('rows')) { E(pw + " (chips) needs a rows array; pChips takes rows, not chips"); break; }
              pn.rows.forEach((r, ri) => {
                if (!r || typeof r !== 'object') E(pw + '.rows[' + ri + '] is not an object');
                else if (!Array.isArray(r.items)) E(pw + '.rows[' + ri + '].items must be an array - each row wraps its own chips');
              });
              break;
            case 'tiles': case 'bars':
              if (!arr('items')) E(pw + ' (' + pn.t + ') needs an items array');
              else pn.items.forEach((it, ii) => { if (!it || it.v === undefined) E(pw + '.items[' + ii + '] needs a v'); });
              break;
            case 'slots': case 'pills':
              if (!arr('items')) E(pw + ' (' + pn.t + ') needs an items array');
              break;
            case 'ranges':
              if (!arr('items')) E(pw + ' (ranges) needs an items array');
              if (typeof pn.min !== 'number' || typeof pn.max !== 'number') E(pw + ' (ranges) needs numeric min and max');
              break;
            case 'note':
              if (typeof pn.html !== 'string') E(pw + ' (note) needs an html string');
              break;
            default:
              E(pw + " has unknown panel type " + JSON.stringify(pn.t));
          }
        });
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

/**
 * Which solutions belong in a problem's visualizer.
 *
 * Rule, from the repo owner: never visualise a brute force when a real solution
 * exists. Brute force earns its place only when it is all there is.
 *
 * "Brute force" is a recorded classification field, not a guess from the
 * filename or a regex over prose - suboptimal.cs is frequently a genuinely
 * different technique (a prefix-sum scan, a bounded heap) that is worth
 * watching precisely because it is not the naive version.
 */
export function selectForVisualizer(curatedFiles, classification) {
  const known = curatedFiles.filter((f) => classification?.[f]);
  const unknown = curatedFiles.filter((f) => !classification?.[f]);

  const brute = known.filter((f) => classification[f].bruteForce);
  const real = known.filter((f) => !classification[f].bruteForce);

  const chosen = real.length ? real : known;
  const dropped = real.length ? brute : [];

  // Rank best-first so the visualizer's tabs read in the order you revise in.
  const order = (f) => (f.startsWith('optimal.') ? 0 : f.startsWith('optimal-variant') ? 1 : 2);
  chosen.sort((a, b) => order(a) - order(b) || a.localeCompare(b, undefined, { numeric: true }));

  return { chosen, dropped, unclassified: unknown };
}
