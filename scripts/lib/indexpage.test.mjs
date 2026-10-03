#!/usr/bin/env node
// Exercise the generated page's script, including reloads and denied storage.
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
    fire(name, event = {}) { if (!this.disabled) listeners[name]?.(event); },
    focus() {},
  };
}
function visit(storage = new Map(), slugs = ['longest-increasing-subsequence', 'binary-search'], blocked = false) {
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
  const ids = Object.fromEntries(['q', 'clear', 'count', 'empty', 'revision-feedback'].map((id) => [id, element()]));
  const chips = ['all', 'current', 'rebuild'].map((filter) => element({ 'data-filter': filter }));
  const document = {
    querySelectorAll: (selector) => selector === 'li[data-hay]' ? rows : selector === '.chip' ? chips : [],
    getElementById: (id) => ids[id], addEventListener() {},
  };
  const windowEvents = {};
  const localStorage = {
    getItem(k) { if (blocked) throw new Error('storage denied'); return storage.get(k) ?? null; },
    setItem(k, v) { if (blocked) throw new Error('storage denied'); storage.set(k, v); },
  };
  runInNewContext(script, { document, localStorage, window: {
    addEventListener(name, fn) { windowEvents[name] = fn; },
  } });
  return { rows, ids, chips, windowEvents };
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
