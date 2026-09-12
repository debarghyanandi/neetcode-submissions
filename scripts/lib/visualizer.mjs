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
import { coverage } from './shapes.mjs';

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
export function validate(problemSource, structures = []) {
  const problems = [];
  const tmp = join(REPO, '.agent', 'tmp');
  mkdirSync(tmp, { recursive: true });
  const harness = join(tmp, 'validate-problem.cjs');

  const code = `
${helpersFromChassis()}
${problemSource}
const out = { errors: [], stats: {}, solutions: [] };
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
      // Which shapes this solution actually draws, for the coverage check in
      // the parent process. Collected here because this is the only place the
      // definition is ever really run.
      const seenTypes = new Set();
      out.solutions.push({
        where: where, label: sol.label || null, types: [],
        override: (sol.shapeOverride && typeof sol.shapeOverride === 'object')
          ? { skip: sol.shapeOverride.skip, reason: sol.shapeOverride.reason } : null,
        __types: seenTypes,
      });
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

        // Markup where markup is not read.
        //
        // 'msg', a panel 'title' and pNote's 'html' are rendered as HTML.
        // Everything else - a chip's text, a stack frame's sub, a node's value -
        // goes through esc() and is rendered as plain text, so "&lt;" written
        // there arrives on screen as the five characters &lt; and "<b>" as the
        // three characters <b>. It is a small, ugly, entirely silent fault: the
        // panel renders, the layout is right, and one label reads as gibberish.
        // Cheap to catch here, so it is caught here rather than in review.
        (function scan(v, path, depth){
          if (depth > 6 || v === null || v === undefined) return;
          if (typeof v === 'string'){
            if (/&(?:lt|gt|amp|quot|apos|nbsp|#\\d+);/i.test(v) || /<\\/?[a-z][a-z0-9]*(?:\\s[^>]*)?>/i.test(v)){
              E(path + ' is rendered as plain text but contains markup: ' + JSON.stringify(v.slice(0, 60)) +
                ' - write the characters themselves. Only msg, a panel title and pNote html are HTML.');
            }
            return;
          }
          if (Array.isArray(v)){ v.forEach(function (x, i){ scan(x, path + '[' + i + ']', depth + 1); }); return; }
          if (typeof v === 'object'){
            for (const key of Object.keys(v)){
              if (key === 'title' || key === 'html') continue;   // rendered as HTML on purpose
              scan(v[key], path + '.' + key, depth + 1);
            }
          }
        })(st.panels, w + '.panels', 0);

        // Panel shapes. Without this, a panel that the renderer will choke on
        // sails through: the file looks finished, and playback dies mid-run on
        // whichever step first contains the bad panel.
        st.panels.forEach((pn, pi) => {
          const pw = w + '.panels[' + pi + ']';
          if (!pn || typeof pn !== 'object') { E(pw + ' is not an object'); return; }
          const arr = (k) => Array.isArray(pn[k]);
          seenTypes.add(pn.t);
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

            // ---- structural panels ----
            // These are the ones with real geometry behind them, so a wrong
            // shape does not throw - it draws a plausible-looking wrong
            // picture. A parent id with no node behind it silently turns one
            // tree into a forest, and nobody reports a forest as a crash.
            case 'tree': {
              if (!arr('nodes')) { E(pw + ' (tree) needs a nodes array'); break; }
              const ids = new Set();
              pn.nodes.forEach(function (n, ni) {
                if (!n || typeof n !== 'object') { E(pw + '.nodes[' + ni + '] is not an object'); return; }
                if (n.id === undefined || n.id === null) E(pw + '.nodes[' + ni + '] needs an id');
                else if (ids.has(String(n.id))) E(pw + '.nodes[' + ni + '] repeats the id ' + JSON.stringify(String(n.id)));
                else ids.add(String(n.id));
              });
              pn.nodes.forEach(function (n, ni) {
                if (n && n.parent !== undefined && n.parent !== null && !ids.has(String(n.parent))) {
                  E(pw + '.nodes[' + ni + '] has parent ' + JSON.stringify(String(n.parent)) +
                    ', which is not a node in this panel - it would silently be drawn as a second root');
                }
                if (n && n.side !== undefined && n.side !== null && n.side !== '' && n.side !== 'L' && n.side !== 'R') {
                  E(pw + '.nodes[' + ni + '].side must be "L" or "R", got ' + JSON.stringify(n.side));
                }
              });
              const roots = pn.nodes.filter(function (n) { return n && (n.parent === undefined || n.parent === null); });
              if (pn.nodes.length && !roots.length) E(pw + ' (tree) has no root - every node names a parent, so nothing can be drawn');
              (pn.tags || []).forEach(function (t, ti) {
                if (t && t.id !== undefined && !ids.has(String(t.id))) {
                  E(pw + '.tags[' + ti + '] points at id ' + JSON.stringify(String(t.id)) + ', which is not in this panel');
                }
              });
              break;
            }
            case 'list': {
              if (!arr('rows')) { E(pw + ' (list) needs a rows array; pList takes rows, not nodes'); break; }
              pn.rows.forEach(function (r, ri) {
                if (!r || typeof r !== 'object') { E(pw + '.rows[' + ri + '] is not an object'); return; }
                if (!Array.isArray(r.nodes)) { E(pw + '.rows[' + ri + '].nodes must be an array - each row wraps its own nodes'); return; }
                r.nodes.forEach(function (n, ni) {
                  if (!n || n.v === undefined) E(pw + '.rows[' + ri + '].nodes[' + ni + '] needs a v');
                });
                (r.tags || []).forEach(function (t, ti) {
                  if (t && t.idx !== undefined && t.idx !== null &&
                      (!Number.isInteger(t.idx) || t.idx >= r.nodes.length)) {
                    E(pw + '.rows[' + ri + '].tags[' + ti + '].idx is ' + t.idx +
                      ', outside 0..' + (r.nodes.length - 1) + ' - the marker would not be drawn. Use -1 for "not on the list".');
                  }
                });
                if (r.cycleTo !== undefined && r.cycleTo !== null &&
                    (!Number.isInteger(r.cycleTo) || r.cycleTo < 0 || r.cycleTo >= r.nodes.length)) {
                  E(pw + '.rows[' + ri + '].cycleTo is ' + r.cycleTo + ', outside 0..' + (r.nodes.length - 1));
                }
              });
              break;
            }
            case 'stack': {
              if (!arr('buckets')) { E(pw + ' (stack) needs a buckets array; pStack takes buckets, not items'); break; }
              pn.buckets.forEach(function (b, bi) {
                if (!b || typeof b !== 'object') { E(pw + '.buckets[' + bi + '] is not an object'); return; }
                if (!Array.isArray(b.items)) { E(pw + '.buckets[' + bi + '].items must be an array'); return; }
                if (b.mode !== undefined && b.mode !== 'stack' && b.mode !== 'queue') {
                  E(pw + '.buckets[' + bi + '].mode must be "stack" or "queue", got ' + JSON.stringify(b.mode));
                }
                b.items.forEach(function (it, ii) {
                  if (!it || it.text === undefined || it.text === null) {
                    E(pw + '.buckets[' + bi + '].items[' + ii + '] needs a text');
                  }
                });
              });
              break;
            }
            case 'grid': {
              if (!arr('rows')) { E(pw + ' (grid) needs a rows array of arrays'); break; }
              let width = -1;
              pn.rows.forEach(function (r, ri) {
                if (!Array.isArray(r)) { E(pw + '.rows[' + ri + '] must be an array - a grid is an array of rows'); return; }
                if (width < 0) width = r.length;
                else if (r.length !== width) {
                  E(pw + '.rows[' + ri + '] has ' + r.length + ' cells but row 0 has ' + width +
                    ' - a ragged grid draws blank cells and reads as a rendering fault');
                }
              });
              if (pn.colLabels && Array.isArray(pn.colLabels) && width >= 0 && pn.colLabels.length < width) {
                E(pw + '.colLabels has ' + pn.colLabels.length + ' entries for ' + width + ' columns');
              }
              if (pn.rowLabels && Array.isArray(pn.rowLabels) && pn.rowLabels.length < pn.rows.length) {
                E(pw + '.rowLabels has ' + pn.rowLabels.length + ' entries for ' + pn.rows.length + ' rows');
              }
              if (pn.box) {
                const b = pn.box;
                for (const kk of ['r0','c0','r1','c1']) {
                  if (!Number.isInteger(b[kk])) E(pw + '.box.' + kk + ' must be an integer');
                }
                if (Number.isInteger(b.r1) && b.r1 >= pn.rows.length) E(pw + '.box.r1 is outside the grid');
                if (Number.isInteger(b.c1) && width >= 0 && b.c1 >= width) E(pw + '.box.c1 is outside the grid');
              }
              break;
            }
            case 'heap': {
              if (!arr('items')) { E(pw + ' (heap) needs an items array - the backing array in index order'); break; }
              pn.items.forEach(function (it, ii) {
                if (it !== null && typeof it === 'object' && it.v === undefined) {
                  E(pw + '.items[' + ii + '] is an object without a v');
                }
              });
              (pn.tags || []).forEach(function (t, ti) {
                if (t && t.idx !== undefined && (!Number.isInteger(t.idx) || t.idx < 0 || t.idx >= pn.items.length)) {
                  E(pw + '.tags[' + ti + '].idx is ' + t.idx + ', outside 0..' + (pn.items.length - 1));
                }
              });
              break;
            }
            default:
              E(pw + " has unknown panel type " + JSON.stringify(pn.t));
          }
        });
      });
      out.stats[where] = steps.length + ' steps';
    });
  }
} catch (e) { E('fatal: ' + e.message); }
for (const s of out.solutions) { s.types = Array.from(s.__types); delete s.__types; }
process.stdout.write(JSON.stringify(out));
`;
  writeFileSync(harness, code, 'utf8');
  let res;
  try {
    res = JSON.parse(execFileSync(process.execPath, [harness], { encoding: 'utf8', timeout: 30000, stdio: ['pipe','pipe','pipe'] }));
  } catch (e) {
    const err = String(e.stderr || e.message).split('\n').slice(0, 6).join(' | ');
    return { errors: [`the definition does not even run: ${err}`], stats: {}, waived: [] };
  } finally {
    try { rmSync(harness); } catch { /* best effort */ }
  }

  // ---- coverage ----
  //
  // Everything above asks "will this render". This asks the question that the
  // seven-linear-panel era could not: "is the thing the code is made of
  // actually on screen". It runs here rather than in the harness because the
  // answer depends on what classify recorded, which the harness has no business
  // knowing - the harness runs the definition, this decides whether the
  // definition was the right drawing.
  //
  // Only structures that HAVE a renderer are ever enforced, and any panel that
  // can draw one satisfies it. Which panel remains the model's call.
  const waived = [];
  for (const sol of res.solutions ?? []) {
    const c = coverage(structures, sol.types ?? [], sol.override);
    for (const e of c.errors) res.errors.push(`${sol.where}: ${e}`);
    for (const w of c.waived) waived.push(`${sol.label ?? sol.where}: ${w} (declared: ${String(sol.override?.reason ?? '').trim()})`);
  }
  res.waived = waived;
  return res;
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

/**
 * Follow a rename into the visualizer's own text.
 *
 * A visualizer names the file each panel animates, in a badge the reader can
 * see. classify renames curated files - a tie now goes to the solution you
 * wrote, so two panels can swap - and the HTML does not follow on its own.
 *
 * It used to, by accident: a rename invalidated the recorded code prints, so
 * the next run rebuilt the whole animation and wrote fresh labels. Re-keying
 * those prints stopped that Opus call and, with it, the only thing keeping the
 * labels honest. The animation was never wrong - just the caption - so this
 * fixes the caption and leaves the rest alone.
 *
 * One pass with the old names longest-first, so a straight swap between two
 * names cannot be applied twice.
 *
 * @param {string} html
 * @param {Map<string,string>|Record<string,string>} renames old name -> new name
 * @returns {{html: string, changed: number}}
 */
export function renameInVisualizer(html, renames) {
  const map = new Map(renames instanceof Map ? renames : Object.entries(renames));
  const moved = [...map].filter(([from, to]) => from !== to);
  if (!moved.length) return { html, changed: 0 };

  const esc = (x) => x.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  const alternation = moved
    .map(([from]) => from)
    .sort((a, b) => b.length - a.length)
    .map(esc)
    .join('|');

  let changed = 0;
  const out = html.replace(
    new RegExp(`(?<![A-Za-z0-9_.\\-])(${alternation})(?![A-Za-z0-9_\\-])`, 'g'),
    (m) => { changed++; return map.get(m) ?? m; },
  );
  return { html: out, changed };
}
