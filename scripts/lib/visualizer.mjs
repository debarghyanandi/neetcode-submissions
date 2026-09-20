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

/**
 * The code panel is the reader's file. The model does not transcribe it.
 *
 * Strip leading and trailing blank lines from a solution body (header and
 * teaching block already gone) and return the remaining lines. That array is
 * what the visualizer highlights, and what the prompt numbers 1-based so
 * simulate() can point at it.
 */
export function panelLines(body) {
  const lines = String(body ?? '').split(/\r?\n/);
  let a = 0, b = lines.length - 1;
  while (a <= b && !lines[a].trim()) a++;
  while (b >= a && !lines[b].trim()) b--;
  return a > b ? [] : lines.slice(a, b + 1);
}

/** Numbered listing handed to the model. Line N here is code[N-1] after inject. */
export function numberedListing(lines) {
  const width = String((lines ?? []).length).length;
  return (lines ?? []).map((l, i) => `${String(i + 1).padStart(width, ' ')}|${l}`).join('\n');
}

function matchBracket(src, open) {
  const openCh = src[open], closeCh = openCh === '[' ? ']' : '}';
  let depth = 0, quote = null;
  for (let i = open; i < src.length; i++) {
    const c = src[i];
    if (quote) {
      if (c === '\\') { i++; continue; }
      if (c === quote) quote = null;
      continue;
    }
    if (c === '"' || c === "'" || c === '`') { quote = c; continue; }
    if (c === openCh) depth++;
    else if (c === closeCh) {
      depth--;
      if (depth === 0) return i;
    }
  }
  return -1;
}

function findCodeArrays(src) {
  const hits = [];
  const re = /\bcode\s*:/g;
  let m;
  while ((m = re.exec(src))) {
    let k = m.index + m[0].length;
    while (k < src.length && /\s/.test(src[k])) k++;
    if (src[k] !== '[') continue;
    const end = matchBracket(src, k);
    if (end < 0) continue;
    hits.push({ start: m.index, arrayStart: k, arrayEnd: end });
  }
  return hits;
}

function indentAt(src, index) {
  let i = index;
  while (i > 0 && src[i - 1] !== '\n' && src[i - 1] !== '\r') i--;
  const pad = src.slice(i, index);
  return /^\s*$/.test(pad) ? pad : '    ';
}

function formatCodeArray(lines, indent) {
  const inner = `${indent}  `;
  if (!(lines ?? []).length) return '[]';
  return `[\n${lines.map((l) => `${inner}${JSON.stringify(l)},`).join('\n')}\n${indent}]`;
}

function insertCodeBeforeSimulate(src, panels) {
  let n = 0;
  const out = src.replace(/(\n)([ \t]*)simulate\s*\(/g, (all, nl, indent) => {
    if (n >= panels.length) return all;
    const lit = formatCodeArray(panels[n], indent);
    n++;
    return `${nl}${indent}code: ${lit},\n${indent}simulate(`;
  });
  return { src: out, injected: n };
}

/**
 * Overwrite every solution's `code` array with the matching .cs file.
 *
 * The model used to transcribe that array. Opus at medium paraphrased it
 * (binary-tree-diameter, 2026-09-17): invented comments, reflowed a statement,
 * every line number still resolved, and a repair paid for a second generation.
 * The file is already on disk. The script writes it. The model keeps parse(),
 * panels and steps, and points `lines` at the numbered listing.
 *
 * If the model omitted `code` entirely, one is inserted before each simulate().
 *
 * @returns {{src: string, injected: number, error?: string}}
 */
export function injectCodePanels(src, bodies) {
  const text = String(src ?? '');
  const panels = (bodies ?? []).map(panelLines);
  if (!panels.length) return { src: text, injected: 0 };

  const hits = findCodeArrays(text);
  if (hits.length === 0) {
    const inserted = insertCodeBeforeSimulate(text, panels);
    if (inserted.injected !== panels.length) {
      return { src: text, injected: 0, error: `could not insert code panels: found ${inserted.injected} simulate() for ${panels.length} file(s)` };
    }
    return inserted;
  }
  if (hits.length !== panels.length) {
    return { src: text, injected: 0, error: `could not inject code panels: found ${hits.length} code array(s) for ${panels.length} file(s)` };
  }

  let out = text;
  for (let i = hits.length - 1; i >= 0; i--) {
    const indent = indentAt(out, hits[i].start);
    const lit = formatCodeArray(panels[i], indent);
    out = out.slice(0, hits[i].arrayStart) + lit + out.slice(hits[i].arrayEnd + 1);
  }
  return { src: out, injected: hits.length };
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
/**
 * What ONE solution must draw: its own file's recorded structures, when classify recorded them.
 *
 * The check used to hold every solution to the UNION of all files. reverse-a-linked-list has an
 * iterative file (linked-list) and a recursive one (linked-list + call-stack), so the iterative tab
 * was rejected for not drawing a call stack it does not have. The retry went to Opus, which only
 * wrote a waiver saying so: $0.49 and 3.5 minutes spent proving the rule wrong. The union is still
 * the fallback for a solution with nothing recorded, so an older classification is not let off.
 */
export function structuresFor(si, structures = [], perSolution = null) {
  const own = Array.isArray(perSolution) ? perSolution[si] : null;
  return Array.isArray(own) && own.length ? own : structures;
}


/**
 * Does this code panel contain a method the layer view can fold?
 *
 * The chassis renders the code panel as one collapsible layer per method, and it
 * works this out by PARSING the code array - no schema field, nothing the generator
 * has to declare. When the parse finds nothing the layers are simply off, which is
 * the right fallback for a bare fragment and the wrong one for a whole solution.
 *
 * house-robber-ii is why this is now checked. Its panel began at
 * `int n = nums.Length;` - the method signature was missing - so findLayers() found
 * no method, the "show all" control hid itself and the whole layer view silently
 * went away. Every line in the panel WAS a line of the file, so the fidelity check
 * above was satisfied; starting halfway down simply was not something it asked about.
 *
 * The rule below mirrors findLayers() in the chassis. That duplication is real, and
 * visualizer.test.mjs pins it: it pulls findLayers out of the chassis text and
 * asserts the two agree on the same fixtures, so they cannot drift apart quietly.
 */
const CTRL_WORD = /^\s*(?:if|else|for|foreach|while|do|switch|case|return|using|lock|try|catch|finally|new)\b/;

export function hasMethodSignature(code) {
  const lines = Array.isArray(code) ? code.map((l) => String(l ?? '')) : [];
  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    if (line.indexOf('(') < 0) continue;            // a signature has parameters
    if (/;\s*$/.test(line)) continue;               // a call, not a declaration
    if (CTRL_WORD.test(line)) continue;             // if (...) is not a method
    const m = line.match(/(\w+)\s*\(/);
    if (!m) continue;
    if (!line.slice(0, line.indexOf(m[0])).trim()) continue;   // needs a return type
    let open = -1;
    for (let k = i; k < Math.min(i + 3, lines.length); k++) {
      if (lines[k].indexOf(';') >= 0) break;
      if (lines[k].indexOf('{') >= 0) { open = k; break; }
    }
    if (open < 0) continue;
    let depth = 0, end = -1;
    for (let k = open; k < lines.length; k++) {
      for (const ch of lines[k]) { if (ch === '{') depth++; else if (ch === '}') depth--; }
      if (depth === 0) { end = k; break; }
    }
    if (end < 0 || end - i < 2) continue;           // too small to be worth folding
    return true;
  }
  return false;
}

export function validate(problemSource, structures = [], perSolution = null, sourceBodies = null) {
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
        code: sol.code.slice(),
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
            // NAMED tags only. The first version matched any <Word>, which in a
            // C# repo means it flagged Stack<TreeNode>, Queue<TreeNode> and
            // List<int> - ordinary labels - as markup. That rejection cost a
            // full Opus retry on invert-a-binary-tree before anyone noticed,
            // and it was inconsistent too: Dictionary<int, Node> slipped
            // through because of the space. A generic type is not a tag.
            if (/&(?:lt|gt|amp|quot|apos|nbsp|#\\d+);/i.test(v) ||
                /<\\/?(?:a|b|i|u|p|em|strong|code|span|div|br|hr|small|sub|sup|pre|kbd|mark|ul|ol|li|img|h[1-6])\\b[^>]*>/i.test(v)){
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
  (res.solutions ?? []).forEach((sol, si) => {
    const c = coverage(structuresFor(si, structures, perSolution), sol.types ?? [], sol.override);
    for (const e of c.errors) res.errors.push(`${sol.where}: ${e}`);
    for (const w of c.waived) waived.push(`${sol.label ?? sol.where}: ${w} (declared: ${String(sol.override?.reason ?? '').trim()})`);
  });
  // ---- code fidelity ----
  //
  // visualize.mjs overwrites each code array from disk before it calls validate.
  // This check is the injector's gate: if a line here is not in the file,
  // injectCodePanels wrote the wrong thing. The model used to transcribe this
  // array (and paraphrased it); that path is gone.
  // The code panel is supposed to BE the reader's file, so that a line number in
  // a msg means the same line in the editor. Opus at medium effort was measured
  // (2026-09-17, binary-tree-diameter) inventing explanatory comments and
  // reflowing one statement across two lines - every line number still resolved,
  // every check above passed, and the panel quietly stopped being the file.
  // Nothing but a verbatim comparison catches that, so here it is.
  // The panel has to start where the method starts, or the layer view vanishes.
  (res.solutions ?? []).forEach((sol) => {
    if (hasMethodSignature(sol.code)) return;
    res.errors.push(`${sol.where}.code has no method signature - it starts inside the method body. ` +
      'Begin the panel at the declaration line (e.g. "public int Rob(int[] nums)") and its opening ' +
      'brace, and include the closing brace, so the code panel can fold one layer per method.');
  });

  if (Array.isArray(sourceBodies)) {
    (res.solutions ?? []).forEach((sol, si) => {
      const body = sourceBodies[si];
      if (typeof body !== 'string') return;
      const key = (l) => String(l).replace(/\s+/g, ' ').trim();
      const fileLines = new Set(body.split('\n').map(key).filter(Boolean));
      const strays = (sol.code ?? [])
        .map((l, i) => ({ i: i + 1, l }))
        .filter(({ l }) => key(l) && !fileLines.has(key(l)));
      for (const { i, l } of strays.slice(0, 8)) {
        res.errors.push(`${sol.where}.code[${i}] is not a line of the solution file: ` +
          `${JSON.stringify(String(l).slice(0, 80))} - the code panel must be the file itself, copied ` +
          'verbatim. Do not add comments, reword them, reflow a statement across lines or join two lines.');
      }
      if (strays.length > 8) {
        res.errors.push(`${sol.where}.code has ${strays.length} lines that are not in the solution file ` +
          '(first 8 listed) - copy the file verbatim.');
      }
    });
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
