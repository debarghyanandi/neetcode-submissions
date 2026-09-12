/**
 * The shape catalogue: what a problem can be made of, what can draw each of
 * those things, and which of them the pipeline refuses to let slide.
 *
 * This file exists because the visualizer chassis used to own only linear
 * panels - seven constructors, six of which are a row of boxes. Nothing was
 * broken, and every tree problem still came out as "nodes grouped by depth",
 * because that was the least-wrong drawing available. The animation was right
 * and the shape was a lie.
 *
 * The split, deliberately:
 *
 *   classify  reports WHICH STRUCTURES the code uses, from a fixed enum
 *   visualize decides HOW TO DRAW THEM, freely, from the panel catalogue
 *   validate  checks only that nothing handed over went undrawn
 *
 * That is why the enum lives here and the panel list lives here too: the check
 * at the end is the same table read backwards, so the three steps cannot drift
 * apart by being edited separately.
 *
 * ADDING A STRUCTURE (graph with a real node-link panel, DP, whatever is next):
 * add it to STRUCTURES, give it an entry in RENDERABLE_BY, and if a row of
 * boxes would be a real loss for it, add it to ENFORCED. If it needs a panel
 * that does not exist yet, add the panel to PANELS and its renderer to the
 * chassis. Nothing else in the pipeline needs touching - visualize builds its
 * prompt from this file and validate builds its check from it.
 */

/**
 * The version of the shape contract a visualizer was built under.
 *
 * Same job as HEADER_FORMAT and LINT_FORMAT: it is how a backfill knows what it
 * has already redone, and how it knows to stop. Without it the only question
 * anyone could ask was "does a visualizer file exist", which is true of every
 * array-shaped one built before the structural panels existed - so the backfill
 * would have considered the whole repo finished on the day it started.
 *
 * Bump this whenever a change would make an already-built visualizer worth
 * rebuilding: a new panel that better suits a structure already in the enum, or
 * a change to what coverage enforces. Do NOT bump it for a new structure that
 * nothing in the repo uses yet.
 *
 *   1  seven linear panels, no coverage check
 *   2  structural panels, structures recorded, coverage enforced
 */
export const VISUALIZER_FORMAT = 2;

/**
 * What a solution can be made of.
 *
 * Free text was never an option. A model that must answer from a list is one
 * whose answer can be ranked, diffed and disagreed with; one that writes prose
 * is one you have to trust. Same reasoning as the complexity enum.
 *
 * `call-stack` is the odd one out and earns its place: recursion is a mechanism
 * rather than a data structure, but a recursive descent IS a stack, and a
 * recursive solution drawn without its frames is the single most common way one
 * of these animations ends up teaching nothing.
 */
export const STRUCTURES = [
  'array',
  'string',
  'matrix',
  'interval',
  'linked-list',
  'tree',
  'graph',
  'trie',
  'stack',
  'queue',
  'deque',
  'heap',
  'call-stack',
  'hash-map',
  'hash-set',
  'union-find',
  'dp-table',
  'bitmask',
];

/** One line each, for the classify prompt. Plain, and about the CODE, not the problem title. */
export const STRUCTURE_HELP = {
  'array': 'an array or list walked, indexed or two-pointered',
  'string': 'a string walked or built character by character',
  'matrix': 'a 2-D array, grid or board',
  'interval': 'pairs treated as [start, end] ranges',
  'linked-list': 'nodes joined by next/prev references',
  'tree': 'a binary tree, BST or any parent-child node structure',
  'graph': 'adjacency list or matrix, edges, BFS/DFS over nodes',
  'trie': 'a prefix tree',
  'stack': 'an explicit LIFO - Stack<T>, or a List used as one with push/pop at the end',
  'queue': 'an explicit FIFO - Queue<T>, including a BFS frontier',
  'deque': 'a double-ended queue - LinkedList<T> used at both ends, a monotonic deque',
  'heap': 'a priority queue - PriorityQueue<T,TPriority>, or a hand-rolled heap',
  'call-stack': 'THE CODE RECURSES. Report this whenever a method calls itself, even if the recursion walks a tree you already reported',
  'hash-map': 'Dictionary, a frequency count, a seen-value map, memoisation',
  'hash-set': 'HashSet, membership testing',
  'union-find': 'disjoint set, find/union with parent array',
  'dp-table': 'a table of subproblem answers filled in order, 1-D or 2-D',
  'bitmask': 'integers used as sets of bits, shifting and masking',
};

/**
 * The panels the chassis can draw, with the exact call shape.
 *
 * `renders` is the claim this file is really making: which structures this
 * panel is an honest drawing of. It is what the coverage check reads, and it is
 * what the prompt suggests from. Everything else here is prompt material.
 */
export const PANELS = [
  {
    t: 'tiles', fn: 'pTiles',
    sig: 'pTiles(title, values, decorate, extra)',
    use: 'a flat array or string as a row of boxes. decorate(i, v) returns {cls}. extra takes {ptrs:[{name,idx,cls}], bracket:{from,to,dashed}, focus, narrow}.',
    renders: ['array', 'string', 'dp-table', 'bitmask'],
    snippet: "pTiles('nums', nums, (i) => ({cls: i === lo ? 'hot' : ''}), {ptrs: [{name: 'l', idx: lo}, {name: 'r', idx: hi}]})",
  },
  {
    t: 'bars', fn: 'pBars',
    sig: 'pBars(title, values, decorate, extra)',
    use: 'the same data as a histogram. Right when the VALUE is a height, a price or a width.',
    renders: ['array'],
    snippet: "pBars('heights', h, (i) => ({cls: i === best ? 'hot' : ''}), {fill: {from: l, to: r, level: water}})",
  },
  {
    t: 'chips', fn: 'pChips',
    sig: 'pChips(title, rows)',
    use: 'labelled rows of small boxes. rows is an array of ROWS, each {left, right, items:[{text, sub, cls, fresh}], empty}. One row is still a one-element array. This is the right panel for a map, a set, a counter, and for a graph adjacency list.',
    renders: ['hash-map', 'hash-set', 'string', 'graph', 'union-find', 'bitmask'],
    snippet: "pChips('seen so far', [{left: 'value -> index', items: mapChips(seen, hotKey)}])",
  },
  {
    t: 'slots', fn: 'pSlots',
    sig: 'pSlots(title, items)',
    use: 'the output being built, dashes until written. items: [{text, cls}].',
    renders: ['array'],
    snippet: "pSlots('answer', out.map((v) => ({text: v === null ? '' : v})))",
  },
  {
    t: 'pills', fn: 'pPills',
    sig: 'pPills(title, items)',
    use: 'scalar readouts - l, r, sum, best. items: [{k, v, cls}]. Never the only panel.',
    renders: [],
    snippet: "pPills('state', [{k: 'sum', v: sum}, {k: 'best', v: best, cls: 'good'}])",
  },
  {
    t: 'ranges', fn: 'pRanges',
    sig: 'pRanges(title, items, min, max)',
    use: 'intervals on a shared timeline. items: [{from, to, label, cls}]; min and max are numbers.',
    renders: ['interval'],
    snippet: "pRanges('intervals', iv.map((x, i) => ({from: x[0], to: x[1], label: x[0] + '-' + x[1], cls: i === k ? 'hot' : ''})), 0, 20)",
  },
  {
    t: 'note', fn: 'pNote',
    sig: 'pNote(title, html)',
    use: 'a free-text panel. html is a string.',
    renders: [],
    snippet: "pNote('why', 'The left half is sorted, so the target is in it only when <code>nums[l] &lt;= t</code>.')",
  },
  {
    t: 'tree', fn: 'pTree',
    sig: 'pTree(title, nodes, opts)',
    use: 'a real tree with real edges. nodes: [{id, v, parent, side:"L"|"R", cls, sub, edgeCls}] - id is any unique string, parent is another node\'s id or null for the root, and DEPTH IS DERIVED FROM THE PARENT CHAIN, never stated. A node with v null draws as a dashed ∅. opts: {tags:[{name, id, cls}] for markers floating above a node, links:[{from, to, cls, label}] for extra non-tree edges, square:true for boxes instead of circles, empty, scale}. Build nodes from a LeetCode level-order array with treeFromLevelOrder(vals).',
    renders: ['tree', 'trie', 'graph', 'union-find'],
    snippet: "pTree('tree', nodes.map((n) => ({...n, cls: n.id === curId ? 'hot' : ''})), {tags: [{name: 'root', id: curId, cls: 'accent'}], scale: 0.85})",
  },
  {
    t: 'list', fn: 'pList',
    sig: 'pList(title, rows, opts)',
    use: 'a linked list as boxes joined by arrows. rows is an array of ROWS - two rows for a merge, one for everything else. Each row is {label, nodes:[{id, v, cls, sub, fresh}], tail:true for a trailing ∅, arrows:[cls-or-{cls,text} per gap, in order], tags:[{name, idx, cls, below}] for prev/cur/next markers, cycleTo:index to draw the back-edge of a cycle, empty}. An arrow is a pointer: give it "ghost" when it is about to be rewired and "back" when it now points the other way.',
    renders: ['linked-list'],
    snippet: "pList('list', [{nodes: cells, tail: true, arrows: arrowCls, tags: [{name: 'prev', idx: p}, {name: 'cur', idx: c, cls: 'accent'}]}])",
  },
  {
    t: 'stack', fn: 'pStack',
    sig: 'pStack(title, buckets)',
    use: 'a bucket with a floor and an open top - things sit in it and come off the end. buckets is an array, so two stacks stand side by side. Each is {label, items:[{text, sub, cls, fresh, popped}], mode:"stack"|"queue", empty, top, foot}. In stack mode items[0] is the BOTTOM and the last item is the top; in queue mode items[0] is the FRONT. This is also the panel for recursion: one frame per item, innermost call on top.',
    renders: ['stack', 'queue', 'deque', 'call-stack'],
    snippet: "pStack('call stack', [{label: 'frames', items: frames.map((f) => ({text: f.node, sub: 'depth ' + f.d})), empty: 'no frames'}])",
  },
  {
    t: 'grid', fn: 'pGrid',
    sig: 'pGrid(title, rows, opts)',
    use: 'a matrix drawn as a matrix. rows is an array of arrays; a cell is a scalar or {v, cls, sub, fresh}. opts: {rowLabels, colLabels, box:{r0,c0,r1,c1,dashed} to bracket a live sub-rectangle, scale}.',
    renders: ['matrix', 'dp-table', 'graph'],
    snippet: "pGrid('matrix', m.map((row, r) => row.map((v, c) => ({v, cls: r === rr && c === cc ? 'hot' : ''}))), {box: {r0: 0, c0: lo, r1: rows - 1, c1: hi, dashed: true}})",
  },
  {
    t: 'heap', fn: 'pHeap',
    sig: 'pHeap(title, items, opts)',
    use: 'a heap as BOTH the tree and the array that stores it, index-linked. items is the backing array in index order; an item is a scalar or {v, cls, sub, fresh}. opts: {tags:[{name, idx, cls}], caption, showArray:false to drop the array row, empty, scale}. Use this rather than a chip row for anything sifting up or down - the sift path only reads as a path in the tree view, and the index arithmetic only reads in the array view.',
    renders: ['heap'],
    snippet: "pHeap('min-heap', heap.map((v, i) => ({v, cls: i === sift ? 'hot' : ''})), {tags: [{name: 'sift', idx: sift, cls: 'accent'}]})",
  },
];

/** structure -> the panel `t` values that are an honest drawing of it. */
export const RENDERABLE_BY = (() => {
  const m = Object.fromEntries(STRUCTURES.map((s) => [s, []]));
  for (const p of PANELS) for (const s of p.renders) m[s].push(p.t);
  return m;
})();

/**
 * The structures the pipeline will actually fail a run over.
 *
 * The test is not "does this structure have a panel" - it is "would a row of
 * boxes be a real loss here". An array genuinely IS a row of boxes, and a
 * frequency map genuinely IS a row of chips, so neither is listed: enforcing
 * those would be ceremony. A tree drawn as rows, a stack drawn as chips or a
 * recursion drawn with no frames are all losses, so those are.
 */
export const ENFORCED = new Set([
  'matrix', 'linked-list', 'tree', 'graph', 'trie',
  'stack', 'queue', 'deque', 'heap', 'call-stack',
]);

export const isStructure = (s) => STRUCTURES.includes(s);
export const panelsFor = (s) => RENDERABLE_BY[s] ?? [];

/**
 * Which structures this run must show, and what would count as showing each.
 *
 * A structure with no renderer yet is dropped rather than enforced - listing
 * something in ENFORCED before its panel exists would fail every run with no
 * way to pass, which is a worse failure than the one this is preventing.
 */
export function required(structures) {
  return [...new Set(structures)]
    .filter((s) => ENFORCED.has(s) && panelsFor(s).length)
    .map((s) => ({ structure: s, panels: panelsFor(s) }));
}

/**
 * The coverage verdict.
 *
 * Deliberately weak: ANY panel that can draw the structure satisfies it. The
 * check is "the heap is visible somewhere", never "you must call pHeap". Which
 * panel to reach for stays a design decision, and design decisions are the
 * model's. This only refuses the one outcome that has actually happened -
 * everything rendered as a row of boxes.
 *
 * @param {string[]} structures   union of what classify recorded for the files in this run
 * @param {string[]} seenTypes    panel `t` values that appear anywhere in this solution
 * @param {{skip?: string[], reason?: string}} override  declared escape hatch, if any
 */
export function coverage(structures, seenTypes, override) {
  const seen = new Set(seenTypes);
  const skip = new Set((override?.skip ?? []).map(String));
  const reason = String(override?.reason ?? '').trim();
  const errors = [];
  const waived = [];

  for (const { structure, panels } of required(structures)) {
    if (panels.some((t) => seen.has(t))) continue;
    if (skip.has(structure)) {
      // An override is a claim, and a claim with no argument behind it is just
      // a way of turning the check off. Make it cost a sentence.
      if (reason.length < 25) {
        errors.push(
          `shapeOverride skips "${structure}" but its reason is too short to be a reason. ` +
          `Say in one sentence why this code does not materialise a ${structure}.`);
      } else {
        waived.push(structure);
      }
      continue;
    }
    errors.push(
      `the code uses ${structure} and nothing in this solution draws one. ` +
      `Add a panel of type ${panels.join(' or ')} (${panels.map((t) => PANELS.find((p) => p.t === t).fn).join(' / ')}), ` +
      `or declare shapeOverride: {skip: ['${structure}'], reason: '...'} on this solution if the code genuinely never materialises one.`);
  }
  return { errors, waived };
}

// ---------------------------------------------------------------- prompt text

/**
 * The panel catalogue, as the visualize prompt sees it.
 *
 * Every panel is listed every time. Narrowing the list to the detected
 * structures would be a quiet way of making the choice for the model, and the
 * whole point of splitting evidence from design is that it does not. The
 * detected ones are marked, not filtered.
 */
export function catalogueSection(structures = []) {
  const need = new Set(structures);
  const hot = new Set(required(structures).flatMap((r) => r.panels));

  const lines = ['PANEL CATALOGUE - every panel the chassis can draw. Get the shapes exactly right;',
                 'the renderer does not tolerate a wrong one.', ''];
  for (const p of PANELS) {
    const mark = hot.has(p.t) ? '  <- suits this problem' : '';
    lines.push(`  ${p.sig}${mark}`);
    lines.push(`      ${p.use}`);
    lines.push(`      e.g. ${p.snippet}`);
    lines.push('');
  }
  lines.push('Every panel also accepts `scale` in its options: a number from 0.7 to 1 that draws it');
  lines.push('smaller. Use it when a structure is wide or deep enough to need scrolling - a tree of');
  lines.push('fifteen nodes, a 6x6 grid - so the whole thing is on screen at once. It is clamped at');
  lines.push('0.7, so you cannot make a panel unreadable, and 1 is right for anything that already fits.');
  if (need.size) {
    lines.push('');
    lines.push('Shared state classes, on any item of any panel, so the colour language is one language:');
    lines.push('  hot (live, orange) · in-window (inside the current window, pale) · good (settled, green)');
    lines.push('  done (finished, ink) · dim (ruled out) · ghost (not yet real) · fresh (just written, animates)');
  }
  return lines.join('\n');
}

/**
 * The coverage contract, as the visualize prompt sees it.
 * Spelled out up front because a rule discovered only through a rejection costs
 * a whole extra Opus call to learn.
 */
export function contractSection(structures) {
  const req = required(structures);
  if (!structures.length) {
    return [
      'This problem has no structures on record (classify has not run since the structures field',
      'was added). Choose panels on the merits: draw each structure the code builds as the thing',
      'it is - a tree as a tree, a stack as a bucket, a matrix as a grid - not as a row of boxes.',
    ].join('\n');
  }
  const out = [
    `WHAT THIS CODE IS MADE OF, from classify: ${structures.join(', ')}.`,
    '',
    'You choose the panels. The only rule is that nothing on that list goes undrawn.',
  ];
  if (req.length) {
    out.push('', 'These must each be visible in at least one panel of EVERY solution, or the run is rejected:');
    for (const { structure, panels } of req) {
      out.push(`  ${structure.padEnd(12)} -> ${panels.map((t) => PANELS.find((p) => p.t === t).fn).join(' or ')}`);
    }
    out.push('',
      'Any one of the listed panels satisfies it - this is a check that the structure is on screen,',
      'not an instruction to use a particular panel. If the code genuinely never materialises one of',
      'them, say so on that solution instead of drawing a fake:',
      "    shapeOverride: {skip: ['tree'], reason: 'one sentence on why not'}");
  }
  const soft = structures.filter((s) => !ENFORCED.has(s));
  if (soft.length) {
    out.push('', `Also present, and worth drawing though not enforced: ${soft.join(', ')}.`);
  }
  return out.join('\n');
}
