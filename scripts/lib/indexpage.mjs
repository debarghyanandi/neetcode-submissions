/**
 * index.html - the GitHub Pages landing page.
 *
 * It used to be index.md: a 53-row markdown table sorted alphabetically, with a
 * note telling you to use Ctrl+F. That is a list, not an index. Sorted by name,
 * "which stack problems have I done" is unanswerable without reading all of it,
 * and there was nowhere to put the one fact I now need most - whether a
 * problem's visualizer has been redrawn under the current shape rules.
 *
 * So it is a generated HTML page instead. Its look follows the blue reference the
 * owner chose: cool paper, blue actions, green for done. The visualizers keep their
 * own cream chassis; that is a separate thing and is not changed here.
 *
 * Three rules it follows:
 *
 *   NOTHING IS HARDCODED. Problems, patterns, complexities, file names and the
 *   standard marker are all read from the repo and from state.json, so the page
 *   cannot drift from what is actually there.
 *
 *   THE STATUS IS DERIVED, NOT DECLARED. One field, "standard": green when lint,
 *   classify, teach AND the visualizer are all at the current standard, red when
 *   any one of them is not. The verdict comes from lib/standard.mjs, the same
 *   function the backfill queues on, so the page cannot claim a folder is done
 *   while the queue still holds it. There is no list of "done" slugs to maintain
 *   - finish a folder and it turns green on the next run by itself.
 *
 *   IT WORKS WITH NO NETWORK AND NO BUILD. One file, no framework, no CDN
 *   except the font, and it degrades to a perfectly readable list if that font
 *   never arrives. Search is filtering rows that are already on the page.
 *
 *   THE ORDER IS THE SHEET'S. Groups are the Blueprint sheet's categories, and a
 *   group is drawn only once a problem in it is solved. Inside a group the 📝
 *   pattern notes come first, then the rest, each in sheet order. A solved folder
 *   the sheet does not list goes to the bottom of the group its rules choose.
 */

import { GROUP_NOTE, groupFor, groupRank } from './patterns.mjs';
import { buildSheet, matchSheet } from './sheet.mjs';
import { atStandard, needs } from './standard.mjs';
import { VISUALIZER_FORMAT } from './shapes.mjs';
import { initRevisions } from './revisions.mjs';

const esc = (s) => String(s ?? '')
  .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
  .replace(/"/g, '&quot;');

/** Percent-encode each path segment; the topic folder has a space and an &. */
const linkPath = (p) => p.split('/').map(encodeURIComponent).join('/');

const nice = (slug) => slug.replace(/-/g, ' ').replace(/\b\w/g, (c) => c.toUpperCase());

/** A stable anchor for a group. */
const groupId = (name) => 'g-' + name.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');

/**
 * ONE field, covering the whole pipeline.
 *
 * This used to report only the visualizer's shape version, which meant a folder
 * could read "current" here while the backfill queue held it for three other
 * reasons - unformatted code, a header from older rules, a teaching block written
 * for a classification that had since changed. One green word, three kinds of
 * unfinished underneath it.
 *
 * The verdict now comes from lib/standard.mjs, the same function select-backfill
 * queues on, so the page and the queue cannot disagree:
 *
 *   current  lint, classify, teach AND visualizer all at the current standard
 *   rebuild  any one of them is not - a backfill pass fixes it
 */
function standing(p, state) {
  return atStandard(p, state) ? 'current' : 'rebuild';
}

const STATUS_TEXT = {
  current: 'Current',
  rebuild: 'Needs rebuilt',
};

export function buildIndexHtml(problems, state, opts = {}) {
  const { web, pages, patternOf, sheet = buildSheet() } = opts;

  const rows = problems.map((p) => {
    const cls = state.problems[p.slug]?.classification ?? {};
    const best = cls['optimal.cs'] ?? Object.values(cls)[0] ?? null;
    const structures = [...new Set(Object.values(cls).flatMap((c) => c.structures ?? []))];
    // Every file's recorded technique, joined. A folder whose only solution is the
    // space-optimised one has no dp-table to give it away, but classify still called
    // it dynamic programming - and that sentence is the most reliable signal there is.
    const algorithm = Object.values(cls).map((c) => c.algorithm ?? '').filter(Boolean).join(' · ');
    const pattern = patternOf ? patternOf(p) : '';
    // The sheet decides first. Only a folder it does not list is filed by the rules.
    const onSheet = matchSheet(sheet, p.slug);
    return {
      slug: p.slug,
      title: nice(p.slug),
      group: onSheet ? onSheet.category : groupFor({ slug: p.slug, pattern, structures, algorithm }),
      sheetN: onSheet ? onSheet.n : null,
      note: onSheet ? onSheet.note : null,
      pattern,
      structures,
      time: best?.time ?? null,
      space: best?.space ?? null,
      files: p.curatedFiles,
      path: p.path,
      hasVisualizer: p.hasVisualizer,
      visualizerFile: `${p.slug}-visualizer.html`,
      status: standing(p, state),
      needs: needs(p, state),
    };
  });

  const byGroup = new Map();
  for (const r of rows) {
    if (!byGroup.has(r.group)) byGroup.set(r.group, []);
    byGroup.get(r.group).push(r);
  }
  // Inside a group: the 📝 pattern notes first, then the rest of the sheet, each in sheet
  // order. A folder the sheet does not list goes last, by title.
  const tier = (r) => (r.sheetN == null ? 2 : r.note ? 0 : 1);
  for (const list of byGroup.values()) {
    list.sort((a, b) => tier(a) - tier(b) || (a.sheetN ?? 0) - (b.sheetN ?? 0) || a.title.localeCompare(b.title));
  }
  const groups = [...byGroup.keys()].sort((a, b) => groupRank(a) - groupRank(b));

  const total = rows.length;
  const current = rows.filter((r) => r.status === 'current').length;
  const notesDone = rows.filter((r) => r.note).length;
  const sheetDone = rows.filter((r) => r.sheetN != null).length;

  const sectionHtml = (name) => {
    const list = byGroup.get(name);
    const done = list.filter((r) => r.status === 'current').length;
    return `
<section class="group" id="${groupId(name)}" data-group="${esc(name)}">
  <div class="group-head">
    <h2>${esc(name)}</h2>
    <span class="group-count"><b class="gdone">${done}</b> / <b>${list.length}</b> at standard</span>
  </div>
  <p class="group-note">${esc(GROUP_NOTE[name] || '')}</p>
  <ul>
${list.map((r) => rowHtml(r, web, pages)).join('\n')}
  </ul>
</section>`;
  };

  return `<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>NeetCode Solutions — by pattern</title>
<meta name="description" content="${total} NeetCode problems in C#, grouped by pattern, each with its source and a step-through visualizer.">
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=JetBrains+Mono:wght@400;500&display=swap">
<style>
/* Blue reference theme: cool near-white paper, blue actions, green for done.
   Colours were sampled from the reference screenshot. Solid hairline borders and
   soft radii throughout; no hand-drawn tilts. */
:root{
  --bg:#f9fafe; --panel:#ffffff; --panel-soft:#f1f5fd;
  --ink:#182028; --muted:#5b6a84; --faint:#8d9ab2;
  --line:#dfe6f3; --line-soft:#edf2fa;
  --blue:#2870f8; --blue-deep:#1d5ae0; --blue-soft:#e4edff; --blue-edge:#c3d6fb;
  --green:#28b878; --green-soft:#dff6ea;
  --red:#e5484d; --red-soft:#fde4e4;
  --radius:14px; --radius-sm:9px;
  --sans:'Inter',system-ui,-apple-system,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;
  --mono:'JetBrains Mono',ui-monospace,SFMono-Regular,Menlo,Consolas,monospace;
}
*{box-sizing:border-box;}
html{scroll-behavior:smooth;}
body{
  margin:0;background:var(--bg);color:var(--ink);
  font-family:var(--sans);font-size:15px;line-height:1.5;
  padding:28px clamp(16px,3vw,48px) 64px;
}
/* Stretched across the screen, with a generous measure on very wide displays. */
.wrap{max-width:1480px;margin:0 auto;min-width:0;}
a{color:inherit;}

/* ---------- masthead ---------- */
.masthead{display:grid;grid-template-columns:minmax(0,1fr) minmax(300px,440px);gap:24px 48px;
  align-items:end;padding-bottom:22px;}
.eyebrow{margin:0 0 6px;font-size:13px;color:var(--faint);}
h1{margin:0;font-size:clamp(28px,3.2vw,40px);line-height:1.1;font-weight:600;letter-spacing:-.02em;}
.lede{margin:10px 0 0;max-width:68ch;color:var(--muted);}
.meter{background:var(--panel);border:1px solid var(--line);border-radius:var(--radius);
  padding:18px 20px 16px;box-shadow:0 1px 2px rgba(24,32,40,.04);}
.meter-top{display:flex;justify-content:space-between;align-items:baseline;gap:12px;color:var(--muted);font-size:14px;}
.meter-figure{font-family:var(--mono);font-size:26px;font-weight:500;color:var(--ink);font-variant-numeric:tabular-nums;}
.bar{height:10px;border-radius:999px;background:var(--line-soft);overflow:hidden;margin:12px 0 10px;}
.bar span{display:block;height:100%;background:var(--green);border-radius:999px;}
.meter-hint{margin:0;font-size:12.5px;color:var(--faint);}
.stats{display:flex;flex-wrap:wrap;gap:8px 18px;margin-top:14px;padding-top:12px;
  border-top:1px solid var(--line-soft);font-size:13px;color:var(--muted);}
.stat b{font-family:var(--mono);font-size:15px;font-weight:500;color:var(--ink);}
.stat em{font-style:normal;}

/* ---------- controls ---------- */
.controls{position:sticky;top:0;z-index:20;background:var(--bg);
  display:flex;flex-wrap:wrap;gap:12px;align-items:center;
  margin:0 0 16px;padding:14px 0 12px;border-bottom:1px solid var(--line);}
.search{flex:1 1 320px;display:flex;align-items:center;gap:10px;min-height:42px;
  background:var(--panel);border:1px solid var(--line);border-radius:12px;padding:0 14px;}
.search:focus-within{border-color:var(--blue);}
.search input{flex:1;min-width:0;border:0;outline:0;background:transparent;
  font-family:var(--sans);font-size:15px;color:var(--ink);}
.search input::placeholder{color:var(--faint);}
.search .mark{font-size:15px;color:var(--faint);}
.search .clear{border:0;background:transparent;cursor:pointer;font-family:var(--sans);font-size:18px;color:var(--faint);padding:0 2px;}
.search .clear:hover{color:var(--red);}
.chips{display:flex;gap:8px;flex-wrap:wrap;}
.chip{font-family:var(--sans);font-size:14px;line-height:1;color:var(--ink);background:var(--panel);
  border:1px solid var(--line);border-radius:10px;padding:10px 14px;cursor:pointer;white-space:nowrap;}
.chip:hover{border-color:var(--blue-edge);background:var(--blue-soft);}
.chip[aria-pressed="true"]{background:var(--blue);border-color:var(--blue);color:#fff;}
.chip:focus-visible,.search input:focus-visible{outline:3px solid var(--blue-edge);outline-offset:2px;}
.count{font-family:var(--mono);font-size:12.5px;color:var(--faint);font-variant-numeric:tabular-nums;margin-left:auto;}

/* ---------- backup and revision key ---------- */
.backup-panel{display:flex;flex-wrap:wrap;align-items:center;gap:10px 14px;margin:0 0 16px;
  padding:10px 14px;border:1px solid var(--blue-edge);border-radius:12px;background:var(--blue-soft);}
.backup-message{flex:1 1 300px;margin:0;font-size:13.5px;color:var(--muted);}
.backup-panel[data-backup-state="error"]{border-color:var(--red);background:var(--red-soft);}
.backup-panel[data-backup-state="error"] .backup-message{color:var(--red);}
.backup-actions{display:flex;flex-wrap:wrap;align-items:center;gap:8px;max-width:100%;}
.backup-mode{display:flex;flex-wrap:wrap;align-items:center;gap:6px;font-size:13px;color:var(--muted);}
.backup-mode select{font-family:var(--sans);font-size:13px;padding:6px 8px;max-width:100%;
  border:1px solid var(--line);border-radius:8px;background:var(--panel);color:var(--ink);}
.backup-button{font-family:var(--sans);font-size:13.5px;font-weight:500;line-height:1;padding:9px 13px;
  border:1px solid var(--line);border-radius:9px;background:var(--panel);color:var(--ink);cursor:pointer;}
.backup-button.primary{background:var(--blue);border-color:var(--blue);color:#fff;}
.backup-button:hover:not(:disabled){border-color:var(--blue-edge);background:var(--blue-soft);}
.backup-button.primary:hover:not(:disabled){background:var(--blue-deep);border-color:var(--blue-deep);}
.backup-button:disabled{opacity:.55;cursor:default;}
.backup-button:focus-visible,.backup-mode select:focus-visible{outline:3px solid var(--blue-edge);outline-offset:2px;}

/* Revision tones: grey at zero, then a warm climb to green at four. */
.revision-tone{--revision-fill:var(--panel-soft);--revision-edge:#c9d3e6;}
.revision-tone[data-revisions="1"]{--revision-fill:#fde4e4;--revision-edge:#e5484d;}
.revision-tone[data-revisions="2"]{--revision-fill:#ffeccc;--revision-edge:#f59f00;}
.revision-tone[data-revisions="3"]{--revision-fill:#fff6c7;--revision-edge:#d9b400;}
.revision-tone[data-revisions="4"]{--revision-fill:var(--green-soft);--revision-edge:var(--green);}
.revision-key{display:flex;flex-wrap:wrap;align-items:center;gap:6px 14px;margin:0 0 20px;font-size:13px;color:var(--muted);}
.revision-key .step{display:inline-flex;align-items:center;gap:6px;}
.revision-key .swatch{width:10px;height:10px;border-radius:50%;background:var(--revision-fill);border:1px solid var(--revision-edge);}

/* ---------- groups ---------- */
.group{background:var(--panel);border:1px solid var(--line);border-radius:var(--radius);
  padding:18px 22px 6px;margin-bottom:16px;box-shadow:0 1px 2px rgba(24,32,40,.03);}
.group-head{display:flex;flex-wrap:wrap;gap:6px 18px;align-items:baseline;justify-content:space-between;}
.group h2{margin:0;font-size:19px;font-weight:600;letter-spacing:-.01em;}
.group-count{font-size:13px;color:var(--muted);white-space:nowrap;font-variant-numeric:tabular-nums;}
.group-count b{font-family:var(--mono);font-weight:500;color:var(--ink);}
.group-count .gdone{color:var(--green);}
.group-note{margin:4px 0 8px;font-size:13.5px;color:var(--muted);max-width:72ch;}
ul{list-style:none;margin:0;padding:0;}
li{display:flex;flex-wrap:wrap;gap:8px 16px;align-items:center;padding:13px 0;}
/* The filter works by setting li.hidden, and the display:flex above beats the
   browser's own [hidden] rule - so every row stayed on screen while the count
   under the chips correctly read "2 of 62". Same for the group sections. */
[hidden]{display:none !important;}
li + li{border-top:1px solid var(--line-soft);}
.who{flex:1 1 280px;min-width:0;}
.name-line{display:flex;flex-wrap:wrap;gap:6px 10px;align-items:center;min-width:0;}
.name{font-size:15.5px;font-weight:600;text-decoration:none;color:var(--ink);}
.name:hover{color:var(--blue);text-decoration:underline;text-underline-offset:3px;}
.sheet-no{font-family:var(--mono);font-size:11.5px;color:var(--faint);white-space:nowrap;}
/* The pattern-note tag: a blue pill on every problem the Blueprint sheet marks as a note. */
.note-badge{font-size:12.5px;line-height:1;padding:5px 10px 6px;border:1px solid var(--blue-edge);
  border-radius:999px;background:var(--blue-soft);color:var(--blue-deep);font-weight:500;
  white-space:nowrap;cursor:help;}
.note-badge:hover{background:#d5e4ff;}
.why{display:block;font-size:13px;color:var(--muted);margin-top:3px;}
.why.none{font-style:italic;color:var(--faint);}
.meta{display:flex;flex-wrap:wrap;gap:10px;align-items:center;margin-left:auto;min-width:0;max-width:100%;}
.sources{display:flex;flex-direction:column;gap:6px;min-width:0;max-width:100%;}
.source-row{display:flex;flex-wrap:wrap;gap:6px;justify-content:flex-end;}
.actions{display:flex;gap:8px;align-items:center;}
.cx{font-family:var(--mono);font-size:11.5px;line-height:1;padding:6px 8px 7px;
  border:1px solid var(--line);border-radius:7px;background:var(--panel);color:var(--muted);white-space:nowrap;}
.src{font-family:var(--mono);font-size:11.5px;line-height:1;padding:6px 9px 7px;text-decoration:none;
  border:1px solid var(--blue-edge);border-radius:7px;background:#f3f7ff;color:var(--ink);white-space:nowrap;}
.src:hover{background:var(--blue-soft);border-color:var(--blue);}
.run{font-size:13px;font-weight:500;line-height:1;padding:8px 12px 8px;text-decoration:none;
  border-radius:8px;background:var(--blue);color:#fff;white-space:nowrap;}
.run:hover{background:var(--blue-deep);}
.run.off{background:var(--line-soft);color:var(--faint);pointer-events:none;}
/* The pipeline verdict stays available to screen readers and in the tooltip. */
.mark-std{display:inline-flex;align-items:center;justify-content:center;width:12px;height:24px;flex:none;}
.mark-std .dot{width:9px;height:9px;border-radius:50%;}
.mark-std.current .dot{background:var(--green);}
.mark-std.rebuild .dot{background:var(--red);}
.revision{font-family:var(--sans);font-size:12.5px;line-height:1;padding:8px 10px 8px;
  border:1px solid var(--revision-edge);border-radius:8px;background:var(--revision-fill);
  color:var(--ink);white-space:nowrap;cursor:pointer;}
.revision .revision-count{font-family:var(--mono);font-size:11px;margin-left:5px;color:var(--muted);}
.revision:hover:not(:disabled){border-color:var(--ink);}
.revision:disabled{cursor:default;}
.revision:focus-visible,.src:focus-visible,.run:focus-visible{outline:3px solid var(--blue-edge);outline-offset:2px;}
.sr-only{position:absolute;width:1px;height:1px;padding:0;margin:-1px;overflow:hidden;
  clip-path:inset(50%);white-space:nowrap;border:0;}

.empty{display:none;border:1px dashed var(--line);border-radius:var(--radius);padding:26px 22px;
  text-align:center;color:var(--muted);background:var(--panel);}
.foot{margin-top:24px;padding-top:14px;border-top:1px solid var(--line);
  display:flex;flex-wrap:wrap;gap:8px 20px;justify-content:space-between;font-size:12.5px;color:var(--faint);}
.foot code{font-family:var(--mono);font-size:11.5px;}

@media (max-width:860px){
  .masthead{grid-template-columns:minmax(0,1fr);}
}
@media (max-width:640px){
  .meta{margin-left:0;justify-content:flex-start;} .count{margin-left:0;}
  .source-row{justify-content:flex-start;} .cx{white-space:normal;overflow-wrap:anywhere;}
  .src{white-space:normal;overflow-wrap:anywhere;max-width:100%;}
}
@media (prefers-reduced-motion: reduce){html{scroll-behavior:auto;}}
</style>
</head>
<body>
<div class="wrap">

  <header class="masthead">
    <div>
      <p class="eyebrow">debarghyanandi / neetcode-submissions</p>
      <h1>NeetCode Solutions</h1>
      <p class="lede">${total} problems in C#, grouped the way the Blueprint sheet groups them, in sheet order. Every one carries its
        complexity, a teaching block in the source, and a step-through visualizer you can run in the browser.</p>
    </div>
    <div class="meter">
      <div class="meter-top"><span>at current standard</span><b class="meter-figure"><span id="curCount">${current}</span> / ${total}</b></div>
      <div class="bar" aria-hidden="true"><span id="bar" style="width:${total ? (current / total * 100).toFixed(1) : 0}%"></span></div>
      <p class="meter-hint">red means lint, classify, teach or the visualizer is not yet at the current standard</p>
      <div class="stats">
        <span class="stat"><b>${notesDone}</b> / ${sheet.noteCount} <em>📝 pattern notes solved</em></span>
        <span class="stat"><b>${sheetDone}</b> / ${sheet.count} <em>of the Blueprint sheet</em></span>
      </div>
    </div>
  </header>

  <div class="controls">
    <label class="search">
      <span class="mark" aria-hidden="true">&#8981;</span>
      <input id="q" type="search" placeholder="Search a problem, a pattern, or a technique…" autocomplete="off" aria-label="Search problems and patterns">
      <button class="clear" id="clear" type="button" title="Clear" aria-label="Clear search" hidden>&times;</button>
    </label>
    <div class="chips" role="group" aria-label="Filter by standard">
      <button class="chip" id="f-all" data-filter="all" aria-pressed="true">All</button>
      <button class="chip" id="f-rebuild" data-filter="rebuild" aria-pressed="false">Needs rebuilt</button>
      <button class="chip" id="f-current" data-filter="current" aria-pressed="false">Current</button>
    </div>
    <span class="count" id="count"></span>
  </div>

  <div class="backup-panel" id="revision-backup" hidden>
    <p class="backup-message" id="backup-message" role="status" aria-live="polite"></p>
    <div class="backup-actions">
      <label class="backup-mode">Import mode
        <select id="import-mode" aria-describedby="backup-message">
          <option value="merge">Merge · keep higher counts</option>
          <option value="replace">Replace · restore file exactly</option>
        </select>
      </label>
      <button class="backup-button primary" id="export-state" type="button">Export state</button>
      <button class="backup-button" id="import-state" type="button">Import state</button>
      <input id="state-file" type="file" accept=".json,application/json" aria-label="Choose revision state file" hidden>
    </div>
  </div>

  <p class="revision-key">
    <span>Revisions:</span>
    ${['not revised', 'once', 'twice', 'three times', 'four times'].map((label, n) =>
      `<span class="step revision-tone" data-revisions="${n}"><i class="swatch" aria-hidden="true"></i>${n} &middot; ${label}</span>`
    ).join('\n    ')}
    <span>Click Revised after each review.</span>
  </p>
  <span class="sr-only" id="revision-feedback" role="status"></span>

${groups.map(sectionHtml).join('\n')}

  <p class="empty" id="empty">Nothing matches that. Try a pattern name — <em>sliding window</em>, <em>heap</em>, <em>monotonic</em>.</p>

  <footer class="foot">
    <span>Generated by <code>scripts/apply.mjs</code>. Order comes from <code>sheet/</code>; unlisted problems are filed by <code>scripts/lib/patterns.mjs</code>.</span>
    <span>Standard: lint, classify, teach and visualizer <code>v${VISUALIZER_FORMAT}</code></span>
  </footer>
</div>

<script>
(function(){
  'use strict';
  var rows = [].slice.call(document.querySelectorAll('li[data-hay]'));
  var groups = [].slice.call(document.querySelectorAll('.group'));
  var q = document.getElementById('q');
  var clear = document.getElementById('clear');
  var count = document.getElementById('count');
  var empty = document.getElementById('empty');
  var filter = 'all';
  function apply(){
    var needle = q.value.trim().toLowerCase();
    var terms = needle ? needle.split(/\\s+/) : [];
    var shown = 0;
    rows.forEach(function(li){
      var hay = li.getAttribute('data-hay');
      var okText = terms.every(function(t){ return hay.indexOf(t) >= 0; });
      var okFilter = filter === 'all' || li.getAttribute('data-status') === filter;
      var on = okText && okFilter;
      li.hidden = !on;
      if (on) shown++;
    });
    // A group with nothing left in it is noise, not structure.
    groups.forEach(function(g){
      var any = [].slice.call(g.querySelectorAll('li[data-hay]')).some(function(li){ return !li.hidden; });
      g.hidden = !any;
    });
    empty.style.display = shown ? 'none' : 'block';
    count.textContent = shown === rows.length ? rows.length + ' problems' : shown + ' of ' + rows.length;
    clear.hidden = !q.value;
  }

  q.addEventListener('input', apply);
  clear.addEventListener('click', function(){ q.value = ''; q.focus(); apply(); });
  // "/" focuses search, the way every list you already use behaves.
  document.addEventListener('keydown', function(e){
    if (e.key === '/' && document.activeElement !== q){ e.preventDefault(); q.focus(); }
    if (e.key === 'Escape' && document.activeElement === q){ q.value = ''; apply(); }
  });
  [].slice.call(document.querySelectorAll('.chip')).forEach(function(btn){
    btn.addEventListener('click', function(){
      filter = btn.getAttribute('data-filter');
      [].slice.call(document.querySelectorAll('.chip')).forEach(function(b){
        b.setAttribute('aria-pressed', String(b === btn));
      });
      apply();
    });
  });
  apply();
})();
(${initRevisions.toString()})();
</script>
</body>
</html>
`;
}

function rowHtml(r, web, pages) {
  // Everything the search should match, lowercased once here rather than on
  // every keystroke: title, slug, pattern group, the teaching block's PATTERN
  // line, and the structures classify recorded.
  const hay = [r.title, r.slug, r.group, r.pattern, r.structures.join(' '), STATUS_TEXT[r.status],
    r.note ? `${r.note.pattern} ${r.note.takeaway}` : '']
    .join(' ').toLowerCase();

  const cx = (r.time && r.space)
    ? `<span class="cx" title="time / space">${esc(r.time)} &middot; ${esc(r.space)}</span>` : '';

  const sourceLinks = r.files.length && web
    ? r.files.map((f) =>
        `<a class="src" href="${web}/${linkPath(`${r.path}/${f}`)}" target="_blank" rel="noopener noreferrer">${esc(f.replace(/\.cs$/, ''))}</a>`
      ) : [];
  // Explicit rows keep five solutions at 2 + 3, instead of pushing the whole
  // metadata block below the title. On phones each row can wrap further.
  const split = Math.floor(sourceLinks.length / 2);
  const sourceRows = sourceLinks.length > 1
    ? [sourceLinks.slice(0, split), sourceLinks.slice(split)] : [sourceLinks];
  const src = sourceLinks.length
    ? `<span class="sources">${sourceRows.map((links) => `<span class="source-row">${links.join('')}</span>`).join('')}</span>`
    : '<span class="cx">not curated yet</span>';

  const run = (r.hasVisualizer && pages)
    ? `<a class="run" href="${pages}/${linkPath(`${r.path}/${r.visualizerFile}`)}" target="_blank" rel="noopener noreferrer">&#9654; run</a>`
    : '<span class="run off">no visualizer</span>';

  const why = r.pattern
    ? `<span class="why">${esc(r.pattern)}</span>`
    : '<span class="why none">not taught yet</span>';

  const statusDetail = r.status === 'current'
    ? 'lint, classify, teach and visualizer all at the current standard'
    : 'needs: ' + r.needs.join(', ');

  const note = r.note
    ? `<span class="note-badge" title="${esc(r.note.takeaway)}">📝 Pattern note${r.note.pattern ? ' · ' + esc(r.note.pattern) : ''}</span>`
    : '';

  return `    <li data-hay="${esc(hay)}" data-status="${r.status}" data-problem="${esc(r.path)}">
      <span class="who">
        <span class="name-line">
          <a class="name" href="https://neetcode.io/problems/${esc(r.slug)}" target="_blank" rel="noopener noreferrer">${esc(r.title)}</a>
          ${r.sheetN != null ? `<span class="sheet-no" title="Position on the Blueprint sheet">#${r.sheetN}</span>` : ''}
          ${note}
        </span>
        ${why}
      </span>
      <span class="meta">
        ${cx}${src}
        <span class="actions">
          ${run}
          <span class="mark-std ${r.status}" title="${esc(STATUS_TEXT[r.status] + ': ' + statusDetail)}"><i class="dot" aria-hidden="true"></i><span class="sr-only">${STATUS_TEXT[r.status]}: ${esc(statusDetail)}</span></span>
          <button class="revision revision-tone" type="button" data-revisions="0" hidden>Revised<span class="revision-count">0/4</span></button>
        </span>
      </span>
    </li>`;
}
