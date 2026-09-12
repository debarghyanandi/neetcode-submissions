/**
 * index.html - the GitHub Pages landing page.
 *
 * It used to be index.md: a 53-row markdown table sorted alphabetically, with a
 * note telling you to use Ctrl+F. That is a list, not an index. Sorted by name,
 * "which stack problems have I done" is unanswerable without reading all of it,
 * and there was nowhere to put the one fact I now need most - whether a
 * problem's visualizer has been redrawn under the current shape rules.
 *
 * So it is a generated HTML page instead. Same palette, radii and faces as the
 * visualizers, read from the same values, because the page and the thing it
 * links to should look like one piece of work.
 *
 * Three rules it follows:
 *
 *   NOTHING IS HARDCODED. Problems, patterns, complexities, file names and the
 *   standard marker are all read from the repo and from state.json, so the page
 *   cannot drift from what is actually there.
 *
 *   THE STATUS IS DERIVED, NOT DECLARED. A problem is green when its visualizer
 *   was built under the current VISUALIZER_FORMAT and red otherwise. There is
 *   no list of "done" slugs to maintain and no way for the page to claim
 *   something that is not true - reskin or rebuild a visualizer and it turns
 *   green on the next run by itself.
 *
 *   IT WORKS WITH NO NETWORK AND NO BUILD. One file, no framework, no CDN
 *   except the font, and it degrades to a perfectly readable list if that font
 *   never arrives. Search is filtering rows that are already on the page.
 */

import { GROUPS, GROUP_NOTE, groupFor, groupRank } from './patterns.mjs';
import { VISUALIZER_FORMAT } from './shapes.mjs';

const esc = (s) => String(s ?? '')
  .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
  .replace(/"/g, '&quot;');

/** Percent-encode each path segment; the topic folder has a space and an &. */
const linkPath = (p) => p.split('/').map(encodeURIComponent).join('/');

const nice = (slug) => slug.replace(/-/g, ' ').replace(/\b\w/g, (c) => c.toUpperCase());

/**
 * Has this visualizer been drawn under the current shape rules?
 *
 *   current  built by the pipeline at the current format
 *   stale    a visualizer exists, but from before the structural panels - so it
 *            draws a tree as rows of boxes. This is what the backfill is for.
 *   none     no visualizer at all
 */
function standing(p, state) {
  if (!p.hasVisualizer) return 'none';
  const rec = state.problems[p.slug]?.visualizer;
  return (rec?.v ?? 1) === VISUALIZER_FORMAT ? 'current' : 'stale';
}

const STATUS_TEXT = {
  current: 'current',
  stale: 'needs reshape',
  none: 'no visualizer',
};

export function buildIndexHtml(problems, state, opts = {}) {
  const { web, pages, patternOf } = opts;

  const rows = problems.map((p) => {
    const cls = state.problems[p.slug]?.classification ?? {};
    const best = cls['optimal.cs'] ?? Object.values(cls)[0] ?? null;
    const structures = [...new Set(Object.values(cls).flatMap((c) => c.structures ?? []))];
    const pattern = patternOf ? patternOf(p) : '';
    return {
      slug: p.slug,
      title: nice(p.slug),
      group: groupFor({ slug: p.slug, pattern, structures }),
      pattern,
      structures,
      time: best?.time ?? null,
      space: best?.space ?? null,
      files: p.curatedFiles,
      path: p.path,
      hasVisualizer: p.hasVisualizer,
      visualizerFile: `${p.slug}-visualizer.html`,
      status: standing(p, state),
    };
  });

  const byGroup = new Map();
  for (const r of rows) {
    if (!byGroup.has(r.group)) byGroup.set(r.group, []);
    byGroup.get(r.group).push(r);
  }
  for (const list of byGroup.values()) list.sort((a, b) => a.title.localeCompare(b.title));
  const groups = [...byGroup.keys()].sort((a, b) => groupRank(a) - groupRank(b));

  const total = rows.length;
  const current = rows.filter((r) => r.status === 'current').length;

  const sectionHtml = (name) => {
    const list = byGroup.get(name);
    const done = list.filter((r) => r.status === 'current').length;
    return `
<section class="group" data-group="${esc(name)}">
  <div class="group-head">
    <h2>${esc(name)}</h2>
    <span class="group-count"><b class="gdone">${done}</b> / <b>${list.length}</b> reshaped</span>
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
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Patrick+Hand&family=Courier+Prime:wght@400;700&display=swap">
<style>
/* The same tokens as scripts/templates/visualizer.chassis.html. The index and
   the thing it opens should look like one piece of work, so the values are
   copied rather than approximated. Single visual world, painted explicitly. */
:root{
  --paper:#FBF5E9; --paper-sunk:#F5EDDD;
  --ink:#211E1A; --pencil:#8A8175; --pencil-light:#C9C0AF;
  --orange:#F5A93F; --orange-soft:#FBE3BC;
  --red:#C94F2E; --green:#5E8C61;
  --hand:'Patrick Hand','Segoe Print','Bradley Hand',cursive;
  --mono:'Courier Prime',ui-monospace,'Courier New',monospace;
  --sk-lg:255px 18px 225px 18px / 18px 225px 18px 255px;
  --sk-md:14px 8px 12px 10px / 10px 12px 8px 14px;
  --sk-sm:8px 5px 9px 6px / 6px 9px 5px 8px;
}
*{box-sizing:border-box;}
html{scroll-behavior:smooth;}
body{
  margin:0;background:var(--paper);color:var(--ink);
  font-family:var(--hand);font-size:17px;line-height:1.5;
  padding-block:30px 60px;padding-left:clamp(16px,3.5vw,52px);padding-right:clamp(16px,3.5vw,52px);
}
.wrap{max-width:1120px;margin:0 auto;}
a{color:inherit;}

/* ---------- masthead ---------- */
.masthead{display:flex;flex-wrap:wrap;gap:18px 40px;align-items:flex-end;justify-content:space-between;
  padding-bottom:16px;border-bottom:2px solid var(--ink);}
.eyebrow{margin:0 0 2px;font-size:15px;color:var(--pencil);transform:rotate(-1deg);transform-origin:left bottom;}
h1{margin:0;font-size:clamp(30px,3.6vw,44px);line-height:1.02;font-weight:400;text-wrap:balance;}
.lede{margin:9px 0 0;max-width:64ch;}
.lede code{font-family:var(--mono);font-size:13px;background:var(--orange-soft);padding:0 4px;border-radius:var(--sk-sm);}
.meter{min-width:230px;display:flex;flex-direction:column;gap:7px;}
.meter-line{display:flex;justify-content:space-between;align-items:baseline;gap:12px;font-size:15px;color:var(--pencil);}
.meter-line b{font-family:var(--mono);font-size:22px;font-weight:700;color:var(--ink);font-variant-numeric:tabular-nums;}
.bar{height:14px;border:2px solid var(--ink);border-radius:var(--sk-sm);background:var(--paper);overflow:hidden;}
.bar span{display:block;height:100%;background:var(--green);}

/* ---------- controls ---------- */
.controls{position:sticky;top:0;z-index:20;background:var(--paper);
  display:flex;flex-wrap:wrap;gap:12px 16px;align-items:center;
  margin:0 -6px 22px;padding:14px 6px 12px;border-bottom:2px dashed var(--pencil-light);}
.search{flex:1 1 300px;display:flex;align-items:center;gap:10px;
  border:2px solid var(--ink);border-radius:var(--sk-md);background:var(--paper);padding:6px 14px;}
.search input{flex:1;min-width:0;border:0;outline:0;background:transparent;
  font-family:var(--hand);font-size:17px;color:var(--ink);}
.search input::placeholder{color:var(--pencil-light);}
.search .mark{font-size:16px;color:var(--pencil);}
.search .clear{border:0;background:transparent;cursor:pointer;font-family:var(--hand);font-size:17px;color:var(--pencil);padding:0 2px;}
.search .clear:hover{color:var(--red);}
.chips{display:flex;gap:8px;flex-wrap:wrap;}
.chip{font-family:var(--hand);font-size:15px;line-height:1;color:var(--ink);background:var(--paper);
  border:2px solid var(--ink);border-radius:var(--sk-sm);padding:7px 12px 9px;cursor:pointer;white-space:nowrap;}
.chip:hover{background:var(--orange-soft);}
.chip[aria-pressed="true"]{background:var(--orange);}
.chip:focus-visible,.search input:focus-visible{outline:3px solid var(--orange);outline-offset:2px;}
.count{font-family:var(--mono);font-size:13px;color:var(--pencil);font-variant-numeric:tabular-nums;margin-left:auto;}

/* ---------- groups ---------- */
.group{border:2px solid var(--ink);border-radius:var(--sk-lg);padding:18px 22px 8px;margin-bottom:20px;}
.group-head{display:flex;flex-wrap:wrap;gap:8px 18px;align-items:baseline;justify-content:space-between;}
.group h2{margin:0;font-size:24px;font-weight:400;transform:rotate(-.5deg);transform-origin:left center;}
.group-count{font-family:var(--mono);font-size:13px;color:var(--pencil);font-variant-numeric:tabular-nums;white-space:nowrap;}
.group-count b{font-weight:700;color:var(--ink);}
.group-count .gdone{color:var(--green);}
.group-note{margin:4px 0 12px;font-size:15px;color:var(--pencil);max-width:70ch;}
ul{list-style:none;margin:0;padding:0;}
li{display:flex;flex-wrap:wrap;gap:8px 16px;align-items:center;padding:12px 2px;}
li + li{border-top:2px dashed var(--pencil-light);}
.who{flex:1 1 340px;min-width:0;}
.name{font-size:19px;text-decoration:none;border-bottom:2px solid var(--pencil-light);}
.name:hover{border-bottom-color:var(--orange);background:var(--orange-soft);}
.why{display:block;font-size:14px;color:var(--pencil);margin-top:2px;}
.why.none{font-style:italic;color:var(--pencil-light);}
.meta{display:flex;flex-wrap:wrap;gap:7px;align-items:center;margin-left:auto;}
.cx{font-family:var(--mono);font-size:11.5px;line-height:1;padding:6px 8px 7px;
  border:2px solid var(--pencil-light);border-radius:var(--sk-sm);color:var(--pencil);white-space:nowrap;}
.src{font-family:var(--mono);font-size:11.5px;line-height:1;padding:6px 9px 7px;text-decoration:none;
  border:2px solid var(--ink);border-radius:var(--sk-sm);white-space:nowrap;}
.src:hover{background:var(--orange-soft);}
.run{font-family:var(--hand);font-size:15px;line-height:1;padding:6px 12px 8px;text-decoration:none;
  border:2px solid var(--ink);border-radius:var(--sk-sm);background:var(--orange-soft);white-space:nowrap;}
.run:hover{background:var(--orange);}
.run.off{border-color:var(--pencil-light);color:var(--pencil-light);background:transparent;pointer-events:none;}
/* The standard marker. Shape as well as colour, so it still reads when the
   page is printed or the reader cannot separate red from green. */
.mark-std{display:inline-flex;align-items:center;gap:6px;font-family:var(--mono);font-size:11px;line-height:1;
  padding:6px 9px 7px;border:2px solid;border-radius:var(--sk-sm);white-space:nowrap;}
.mark-std .dot{width:9px;height:9px;border-radius:50%;flex:none;}
.mark-std.current{border-color:var(--green);color:var(--green);}
.mark-std.current .dot{background:var(--green);}
.mark-std.stale{border-color:var(--red);color:var(--red);}
.mark-std.stale .dot{background:var(--red);}
.mark-std.none{border-color:var(--pencil-light);color:var(--pencil-light);}
.mark-std.none .dot{background:var(--pencil-light);}

.empty{display:none;border:2px dashed var(--pencil-light);border-radius:var(--sk-lg);padding:26px 22px;text-align:center;color:var(--pencil);}
.foot{margin-top:28px;padding-top:14px;border-top:2px solid var(--ink);
  display:flex;flex-wrap:wrap;gap:8px 20px;justify-content:space-between;font-size:14px;color:var(--pencil);}
.foot code{font-family:var(--mono);font-size:12px;}
@media (max-width:640px){ .meta{margin-left:0;} .count{margin-left:0;} }
@media (prefers-reduced-motion: reduce){html{scroll-behavior:auto;}}
</style>
</head>
<body>
<div class="wrap">

  <header class="masthead">
    <div>
      <p class="eyebrow">debarghyanandi / neetcode-submissions</p>
      <h1>NeetCode Solutions</h1>
      <p class="lede">${total} problems in C#, grouped the way the roadmap groups them. Every one carries its
        complexity, a teaching block in the source, and a step-through visualizer you can run in the browser.</p>
    </div>
    <div class="meter">
      <div class="meter-line"><span>visualizers at current standard</span><b><span id="curCount">${current}</span> / ${total}</b></div>
      <div class="bar"><span id="bar" style="width:${total ? (current / total * 100).toFixed(1) : 0}%"></span></div>
      <div class="meter-line"><span>red means the structure is still drawn as rows of boxes</span></div>
    </div>
  </header>

  <div class="controls">
    <label class="search">
      <span class="mark" aria-hidden="true">&#8981;</span>
      <input id="q" type="search" placeholder="Search a problem, a pattern, or a technique…" autocomplete="off" aria-label="Search problems and patterns">
      <button class="clear" id="clear" type="button" title="Clear" aria-label="Clear search" hidden>&times;</button>
    </label>
    <div class="chips" role="group" aria-label="Filter by visualizer standard">
      <button class="chip" id="f-all" data-filter="all" aria-pressed="true">All</button>
      <button class="chip" id="f-stale" data-filter="stale" aria-pressed="false">Needs reshape</button>
      <button class="chip" id="f-current" data-filter="current" aria-pressed="false">Current</button>
    </div>
    <span class="count" id="count"></span>
  </div>

${groups.map(sectionHtml).join('\n')}

  <p class="empty" id="empty">Nothing matches that. Try a pattern name — <em>sliding window</em>, <em>heap</em>, <em>monotonic</em>.</p>

  <footer class="foot">
    <span>Generated by <code>scripts/apply.mjs</code>. Grouping rules live in <code>scripts/lib/patterns.mjs</code>.</span>
    <span>Visualizer standard <code>v${VISUALIZER_FORMAT}</code></span>
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
</script>
</body>
</html>
`;
}

function rowHtml(r, web, pages) {
  // Everything the search should match, lowercased once here rather than on
  // every keystroke: title, slug, pattern group, the teaching block's PATTERN
  // line, and the structures classify recorded.
  const hay = [r.title, r.slug, r.group, r.pattern, r.structures.join(' '), STATUS_TEXT[r.status]]
    .join(' ').toLowerCase();

  const cx = (r.time && r.space)
    ? `<span class="cx" title="time / space">${esc(r.time)} &middot; ${esc(r.space)}</span>` : '';

  const src = r.files.length && web
    ? r.files.map((f) =>
        `<a class="src" href="${web}/${linkPath(`${r.path}/${f}`)}" target="_blank" rel="noopener noreferrer">${esc(f.replace(/\.cs$/, ''))}</a>`
      ).join('')
    : '<span class="cx">not curated yet</span>';

  const run = (r.hasVisualizer && pages)
    ? `<a class="run" href="${pages}/${linkPath(`${r.path}/${r.visualizerFile}`)}" target="_blank" rel="noopener noreferrer">&#9654; run</a>`
    : '<span class="run off">no visualizer</span>';

  const why = r.pattern
    ? `<span class="why">${esc(r.pattern)}</span>`
    : '<span class="why none">not taught yet</span>';

  return `    <li data-hay="${esc(hay)}" data-status="${r.status}">
      <span class="who">
        <a class="name" href="https://neetcode.io/problems/${esc(r.slug)}" target="_blank" rel="noopener noreferrer">${esc(r.title)}</a>
        ${why}
      </span>
      <span class="meta">
        ${cx}${src}${run}
        <span class="mark-std ${r.status}" title="visualizer shape standard"><i class="dot"></i>${STATUS_TEXT[r.status]}</span>
      </span>
    </li>`;
}
