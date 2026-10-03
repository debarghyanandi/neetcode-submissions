#!/usr/bin/env node
// Exercise the generated page: persistence, JSON backups and denied storage.
import assert from 'node:assert/strict';
import { test } from 'node:test';
import { runInNewContext } from 'node:vm';
import { buildIndexHtml } from './indexpage.mjs';

const problem = (files = [], slug = 'longest-increasing-subsequence') => ({
  slug, path: `Data Structures & Algorithms/${slug}`, dir: '/missing-index-test-fixture',
  curatedFiles: files, pending: [], hasVisualizer: false,
});
const build = (problems) => buildIndexHtml(problems, { problems: {} }, {
  web: 'https://github.com/example/solutions/blob/main', pages: 'https://example.github.io/solutions',
});
const files = ['optimal.cs', 'suboptimal-2.cs', 'suboptimal-3.cs', 'suboptimal-4.cs', 'suboptimal.cs'];
const html = build([problem(files)]);
const script = html.match(/<script>([\s\S]*?)<\/script>/)[1];
const key = (slug) => `neetcode-submissions:revisions:v1:Data Structures & Algorithms/${slug}`;

function element(attributes = {}) {
  const listeners = {};
  return {
    attributes: { ...attributes }, hidden: false, disabled: false, textContent: '', value: '', style: {},
    getAttribute(name) { return this.attributes[name] ?? null; },
    setAttribute(name, value) { this.attributes[name] = value; },
    addEventListener(name, fn) { listeners[name] = fn; },
    fire(name, event = {}) { if (!this.disabled) return listeners[name]?.(event); },
    focus() {},
    click() { this.clicked = true; return this.fire('click'); }, remove() {},
  };
}
function visit(storage = new Map(), slugs = ['longest-increasing-subsequence', 'binary-search'], blocked = false, options = {}) {
  const rows = slugs.map((slug) => {
    const row = element({ 'data-problem': `Data Structures & Algorithms/${slug}`,
      'data-hay': slug, 'data-status': slug === 'binary-search' ? 'rebuild' : 'current' });
    row.button = element({ 'data-revisions': '0' });
    row.counter = element();
    row.button.querySelector = () => row.counter;
    const name = element();
    name.textContent = slug;
    row.querySelector = (selector) => selector === '.revision' ? row.button : name;
    return row;
  });
  const ids = Object.fromEntries(['q', 'clear', 'count', 'empty', 'revision-feedback', 'revision-backup',
    'backup-message', 'export-state', 'import-state', 'state-file', 'import-mode'].map((id) => [id, element()]));
  ids['import-mode'].value = 'merge';
  const chips = ['all', 'current', 'rebuild'].map((filter) => element({ 'data-filter': filter }));
  const downloads = [], blobs = [], revoked = [];
  const document = {
    querySelectorAll: (selector) => ['li[data-hay]', 'li[data-problem]'].includes(selector) ? rows : selector === '.chip' ? chips : [],
    getElementById: (id) => ids[id], addEventListener() {},
    createElement(tag) { assert.equal(tag, 'a'); const link = element(); link.click = () => downloads.push(link); return link; },
    body: { appendChild() {} },
  };
  const windowEvents = {};
  const localStorage = {
    getItem(k) { if (blocked) throw new Error('storage denied'); return storage.get(k) ?? null; },
    setItem(k, v) { if (blocked || options.failWrite?.(k, v)) throw new Error('storage denied'); storage.set(k, v); },
    removeItem(k) { if (blocked) throw new Error('storage denied'); storage.delete(k); },
    key(i) { if (blocked) throw new Error('storage denied'); return [...storage.keys()][i] ?? null; },
    get length() { if (blocked) throw new Error('storage denied'); return storage.size; },
  };
  const controller = runInNewContext(script, { document, localStorage, window: {
    addEventListener(name, fn) { windowEvents[name] = fn; },
    Blob,
    URL: { createObjectURL(blob) { blobs.push(blob); return 'blob:state-' + blobs.length; }, revokeObjectURL(url) { revoked.push(url); } },
    setTimeout(fn) { fn(); },
  } });
  return { rows, ids, chips, windowEvents, controller, downloads, blobs, revoked };
}

test('multiple solutions stay in two ordered rows, including the requested 2 + 3 split', () => {
  for (let n = 1; n <= 5; n++) {
    const rendered = build([problem(files.slice(0, n))]);
    const sourceRows = [...rendered.matchAll(/<span class="source-row">([\s\S]*?)<\/span>/g)];
    assert.equal(sourceRows.length, n === 1 ? 1 : 2);
    assert.deepEqual(sourceRows.map((r) => [...r[1].matchAll(/class="src"/g)].length),
      n === 1 ? [1] : [Math.floor(n / 2), Math.ceil(n / 2)]);
    assert.deepEqual([...rendered.matchAll(/class="src"[^>]*>([^<]+)<\/a>/g)].map((r) => r[1]),
      files.slice(0, n).map((f) => f.replace(/\.cs$/, '')));
    assert.match(rendered, /Data%20Structures%20%26%20Algorithms/);
  }
});

test('every problem, including an uncurated problem, receives a stable revision key and button', () => {
  const rendered = build([problem(files), problem([], 'new-problem')]);
  assert.equal([...rendered.matchAll(/class="revision revision-tone"/g)].length, 2);
  assert.match(rendered, /data-problem="Data Structures &amp; Algorithms\/new-problem"/);
  assert.match(rendered, /mark-std rebuild[^>]+title="Needs rebuilt: needs:/);
  assert.match(rendered, /class="sr-only">Needs rebuilt:/);
});

test('four clicks reach green, cap at four, and only change the selected problem', () => {
  const storage = new Map();
  const { rows } = visit(storage);
  assert.equal(rows[0].counter.textContent, '0/4');
  assert.equal(rows[0].button.hidden, false);
  for (let n = 1; n <= 4; n++) {
    rows[0].button.fire('click');
    assert.equal(rows[0].button.getAttribute('data-revisions'), String(n));
    assert.equal(rows[0].counter.textContent, `${n}/4`);
    assert.equal(storage.get(key('longest-increasing-subsequence')), String(n));
  }
  assert.equal(rows[0].button.disabled, true);
  rows[0].button.fire('click');
  assert.equal(rows[0].counter.textContent, '4/4');
  assert.equal(rows[1].counter.textContent, '0/4');
  assert.match(rows[0].button.getAttribute('aria-label'), /Revision goal complete/);
});

test('reloads, reordering and newly indexed problems preserve existing revisions', () => {
  const storage = new Map();
  visit(storage).rows[0].button.fire('click');
  const { rows } = visit(storage, ['new-problem', 'binary-search', 'longest-increasing-subsequence']);
  assert.deepEqual(rows.map((row) => row.counter.textContent), ['0/4', '0/4', '1/4']);
});

test('bad stored values are safe and unavailable storage still allows revisions', () => {
  for (const [value, expected] of [['garbage', '0/4'], ['2.5', '0/4'], ['-1', '0/4'], ['99', '4/4']]) {
    assert.equal(visit(new Map([[key('longest-increasing-subsequence'), value]])).rows[0].counter.textContent, expected);
  }
  const { rows, ids } = visit(new Map(), undefined, true);
  rows[0].button.fire('click');
  assert.equal(rows[0].counter.textContent, '1/4');
  assert.match(ids['revision-feedback'].textContent, /saved only for this visit/);
});

test('other tabs update progress and clearing storage resets it', () => {
  const storage = new Map();
  const { rows, windowEvents } = visit(storage);
  storage.set(key('binary-search'), '3');
  windowEvents.storage({ key: key('binary-search') });
  assert.equal(rows[1].counter.textContent, '3/4');
  storage.clear();
  windowEvents.storage({ key: null });
  assert.deepEqual(rows.map((r) => r.counter.textContent), ['0/4', '0/4']);
});

test('search and pipeline-status filters still work after revision setup', () => {
  const { rows, ids, chips } = visit();
  chips[2].fire('click');
  assert.deepEqual(rows.map((r) => r.hidden), [true, false]);
  assert.equal(ids.count.textContent, '1 of 2');
  chips[0].fire('click');
  ids.q.value = 'subsequence';
  ids.q.fire('input');
  assert.deepEqual(rows.map((r) => r.hidden), [false, true]);
});

const path = (slug) => `Data Structures & Algorithms/${slug}`;
const stateFile = (revisions, extra = {}) => JSON.stringify({
  format: 'neetcode-submissions-revisions', version: 1, revisions, ...extra,
});

test('export downloads a portable JSON backup, including progress outside the current index', async () => {
  const storage = new Map([[key('future-problem'), '3'], ['another-app', 'private'],
    ['neetcode-submissions:cloud:v1:unused', 'private']]);
  const app = visit(storage);
  app.rows[0].button.fire('click');
  app.ids['export-state'].fire('click');
  assert.equal(app.downloads.length, 1);
  assert.match(app.downloads[0].download, /^neetcode-revisions-\d{4}-\d{2}-\d{2}\.json$/);
  assert.deepEqual(app.revoked, ['blob:state-1']);
  assert.equal(app.blobs[0].type, 'application/json');
  const backup = JSON.parse(await app.blobs[0].text());
  assert.equal(backup.format, 'neetcode-submissions-revisions');
  assert.equal(backup.version, 1);
  assert.equal(new Date(backup.exportedAt).toISOString(), backup.exportedAt);
  assert.deepEqual(backup.revisions, {
    [path('binary-search')]: 0, [path('future-problem')]: 3, [path('longest-increasing-subsequence')]: 1,
  });
});

test('an exported file restores progress in a fresh browser and survives new folders and regeneration', () => {
  const original = visit();
  original.rows[0].button.fire('click');
  original.rows[0].button.fire('click');
  const storage = new Map(), other = visit(storage);
  other.controller.importState(JSON.stringify(original.controller.exportState()));
  assert.deepEqual(other.rows.map((r) => r.counter.textContent), ['2/4', '0/4']);
  assert.deepEqual(visit(storage, ['new-problem', 'longest-increasing-subsequence']).rows.map((r) => r.counter.textContent), ['0/4', '2/4']);
});

test('merge keeps higher counts, preserves absent problems, and stores not-yet-indexed problems', () => {
  const storage = new Map([[key('longest-increasing-subsequence'), '3'], [key('binary-search'), '2']]);
  const app = visit(storage);
  app.controller.importState(stateFile({ [path('longest-increasing-subsequence')]: 1, [path('future-problem')]: 4 }));
  assert.deepEqual(app.rows.map((r) => r.counter.textContent), ['3/4', '2/4']);
  assert.equal(visit(storage, ['future-problem']).rows[0].counter.textContent, '4/4');
  app.controller.importState(stateFile({ [path('longest-increasing-subsequence')]: 4 }));
  assert.equal(app.rows[0].counter.textContent, '4/4');
});

test('replace restores lower and zero counts, resets absent problems, and preserves unrelated storage', () => {
  const storage = new Map([[key('longest-increasing-subsequence'), '4'], [key('binary-search'), '3'],
    [key('old-problem'), '2'], ['another-app', 'keep']]);
  const app = visit(storage);
  app.controller.importState(stateFile({ [path('longest-increasing-subsequence')]: 1 }), 'replace');
  assert.deepEqual(app.rows.map((r) => r.counter.textContent), ['1/4', '0/4']);
  assert.equal(app.rows[0].button.disabled, false);
  assert.equal(storage.has(key('old-problem')), false);
  assert.equal(storage.get('another-app'), 'keep');
  app.controller.importState(stateFile({ [path('longest-increasing-subsequence')]: 0 }), 'replace');
  assert.equal(app.rows[0].counter.textContent, '0/4');
  app.controller.importState(stateFile({}), 'replace');
  assert.equal(storage.has(key('longest-increasing-subsequence')), false);
});

test('invalid, foreign and unsupported files never change any progress', () => {
  const storage = new Map([[key('longest-increasing-subsequence'), '2']]), app = visit(storage);
  const invalid = ['{', 'null', '[]', '{}', stateFile({}, { version: 2 }), stateFile({}, { format: 'foreign' }),
    stateFile([], {}), stateFile({ 'bad/path/extra': 1 }), stateFile({ 'bad\\topic/problem': 1 }),
    stateFile({ [path('binary-search')]: 1, [path('longest-increasing-subsequence')]: '3' }),
    ...[-1, 5, 1.5, null, true].map((n) => stateFile({ [path('longest-increasing-subsequence')]: n })),
    ' '.repeat(1024 * 1024 + 1)];
  for (const text of invalid) {
    assert.throws(() => app.controller.importState(text, 'replace'));
    assert.equal(storage.get(key('longest-increasing-subsequence')), '2');
    assert.equal(storage.size, 1);
    assert.equal(app.rows[0].counter.textContent, '2/4');
  }
});

test('the file-picker handler imports JSON, reports errors, and allows selecting the same file again', async () => {
  const app = visit(), input = app.ids['state-file'];
  app.ids['import-state'].fire('click');
  assert.equal(input.clicked, true);
  input.files = [{ size: 200, text: async () => '\uFEFF' + stateFile({ [path('longest-increasing-subsequence')]: 3 }) }];
  input.value = 'fake-path';
  await input.fire('change');
  assert.equal(app.rows[0].counter.textContent, '3/4');
  assert.equal(input.value, '');
  assert.match(app.ids['backup-message'].textContent, /Merged/);
  app.ids['import-mode'].value = 'replace';
  app.ids['import-mode'].fire('change');
  assert.match(app.ids['backup-message'].textContent, /missing from the file reset to 0/);
  input.files = [{ size: 1, text: async () => '{' }];
  await input.fire('change');
  assert.match(app.ids['backup-message'].textContent, /not valid JSON/);
  assert.equal(app.rows[0].counter.textContent, '3/4');
  assert.equal(app.ids['import-state'].disabled, false);
  input.files = [{ size: 1024 * 1024 + 1, text() { throw new Error('must not read oversized file'); } }];
  await input.fire('change');
  assert.match(app.ids['backup-message'].textContent, /smaller than 1 MB/);
});

test('storage failures retain progress for this visit and still allow exporting a backup', async () => {
  const app = visit(new Map(), undefined, true);
  app.rows[0].button.fire('click');
  app.controller.importState(stateFile({ [path('binary-search')]: 3 }));
  assert.deepEqual(app.rows.map((r) => r.counter.textContent), ['1/4', '3/4']);
  assert.match(app.ids['backup-message'].textContent, /only for this visit/);
  app.ids['export-state'].fire('click');
  assert.equal(JSON.parse(await app.blobs[0].text()).revisions[path('binary-search')], 3);
});

test('a partial storage write is rolled back and the imported state remains exportable in memory', () => {
  let writes = 0;
  const storage = new Map([[key('longest-increasing-subsequence'), '2'], [key('binary-search'), '1']]);
  const app = visit(storage, undefined, false, { failWrite: () => ++writes === 2 });
  const saved = app.controller.importState(stateFile({ [path('longest-increasing-subsequence')]: 4, [path('binary-search')]: 3 }), 'replace');
  assert.equal(saved, false);
  assert.deepEqual([...storage], [[key('longest-increasing-subsequence'), '2'], [key('binary-search'), '1']]);
  assert.equal(app.controller.exportState().revisions[path('longest-increasing-subsequence')], 4);
  assert.match(app.ids['backup-message'].textContent, /only for this visit/);
});
