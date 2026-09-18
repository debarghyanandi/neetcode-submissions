#!/usr/bin/env node
/**
 * compile-check.mjs - do the solution files actually compile?
 *
 * The .cs files are the point of this repo. Everything else - the header, the
 * teaching block, the visualizer, the index - is generated commentary that a
 * backfill can rebuild from scratch. The code cannot be rebuilt: it is what was
 * written and submitted, and a pipeline that has been rewriting it in place for
 * weeks owes a proof that it is still valid C#.
 *
 * Nothing else in the pipeline checks this. sameShape() proves a rewrite changed
 * no token, which is a statement about two files being the same - it says nothing
 * about whether either one compiles. A rename onto a keyword, a mangled generic,
 * a lost brace: all of them pass every check in this repo and fail at the grader.
 *
 * HOW. Every solution is `public class Solution { ... }`, so they collide if you
 * throw them at one compiler. Each file is therefore wrapped in a namespace of its
 * own, which makes one project out of the whole repo and one `dotnet build` out of
 * ~120 files - seconds rather than the minutes a per-file build would take.
 *
 * NeetCode's stubs reference types it supplies and the file does not - ListNode,
 * TreeNode, Node, Interval. Those are declared per namespace, and skipped for a
 * file that declares its own (lru-cache writes its own Node).
 *
 *   node scripts/compile-check.mjs            # every curated file AND every raw submission
 *   node scripts/compile-check.mjs --slug two-integer-sum
 *   node scripts/compile-check.mjs --keep     # leave the temp project for poking at
 */

import { readFileSync, writeFileSync, mkdirSync, rmSync, existsSync } from 'node:fs';
import { join } from 'node:path';
import { tmpdir } from 'node:os';
import { execFileSync } from 'node:child_process';
import { loadState, scanRepo } from './lib/scan.mjs';
import { stripHeader, solutionBody } from './lib/header.mjs';
import { splitTrailingTeach } from './lib/teach.mjs';

const argv = process.argv.slice(2);
const arg = (n, d = null) => (argv.includes(n) ? argv[argv.indexOf(n) + 1] : d);
const onlyRaw = arg('--slug');
const only = onlyRaw ? onlyRaw.split(',').map((s) => s.trim()).filter(Boolean) : null;
const keep = argv.includes('--keep');

/** What NeetCode hands you and the submission does not declare. */
const HELPERS = {
  ListNode: 'public class ListNode { public int val; public ListNode next; public ListNode(int val = 0, ListNode next = null) { this.val = val; this.next = next; } }',
  TreeNode: 'public class TreeNode { public int val; public TreeNode left; public TreeNode right; public TreeNode(int val = 0, TreeNode left = null, TreeNode right = null) { this.val = val; this.left = left; this.right = right; } }',
  Node: 'public class Node { public int val; public IList<Node> neighbors; public Node next; public Node random; public Node prev; public Node child; public Node(int val = 0) { this.val = val; this.neighbors = new List<Node>(); } }',
  Interval: 'public class Interval { public int start; public int end; public Interval(int start = 0, int end = 0) { this.start = start; this.end = end; } }',
};

const state = loadState();
let problems = scanRepo(state).filter((p) => p.curatedFiles.length || p.pending.length);
if (only) problems = problems.filter((p) => only.includes(p.slug));
if (!problems.length) { console.log('\nNothing to compile.\n'); process.exit(0); }

const dir = join(tmpdir(), `neetcc-${Date.now()}`);
mkdirSync(join(dir, 'src'), { recursive: true });

writeFileSync(join(dir, 'p.csproj'), [
  '<Project Sdk="Microsoft.NET.Sdk">',
  '  <PropertyGroup>',
  '    <OutputType>Library</OutputType>',
  '    <TargetFramework>net8.0</TargetFramework>',
  '    <Nullable>disable</Nullable>',
  '    <ImplicitUsings>enable</ImplicitUsings>',
  '    <NoWarn>CS0168;CS0219;CS0414;CS8321;CS0649</NoWarn>',   // unused locals are the author's business
  '    <EnableDefaultCompileItems>true</EnableDefaultCompileItems>',
  '  </PropertyGroup>',
  '</Project>',
].join('\n'), 'utf8');

const units = [];
for (const p of problems) {
  // Raw submissions are included, and they are the ones this guard is really for.
  // A curated file has already been through the pipeline; a submission that landed
  // ten seconds ago has not, and it is about to have a classification, a teaching
  // block and a visualizer bought for it. If it does not compile, that is worth
  // knowing before the money is spent, not after.
  for (const file of [...p.curatedFiles, ...p.pending.filter((x) => !x.supersededBy).map((x) => x.file)]) {
    const raw = readFileSync(join(p.dir, file), 'utf8');
    const body = solutionBody(raw).trim();
    if (!body) { console.log(`  skipped ${p.slug}/${file} - no code once the header and block are stripped`); continue; }
    const ns = `N${units.length}`;
    const declares = (t) => new RegExp(`\\bclass\\s+${t}\\b`).test(body);
    const needs = Object.keys(HELPERS).filter((t) => new RegExp(`\\b${t}\\b`).test(body) && !declares(t));
    units.push({ slug: p.slug, file, ns });
    writeFileSync(join(dir, 'src', `${ns}.cs`), [
      'using System;', 'using System.Collections.Generic;', 'using System.Linq;', 'using System.Text;',
      `namespace ${ns}`, '{',
      ...needs.map((t) => '    ' + HELPERS[t]),
      body.split('\n').map((l) => (l ? '    ' + l : l)).join('\n'),
      '}', '',
    ].join('\n'), 'utf8');
  }
}

const raw = units.filter((u) => /^submission-/.test(u.file)).length;
console.log(`\ncompiling ${units.length} file(s) from ${problems.length} folder(s)` +
  (raw ? ` (${units.length - raw} curated, ${raw} raw submission(s))` : '') + '\n');

let out = '', failed = false;
try {
  out = execFileSync('dotnet', ['build', join(dir, 'p.csproj'), '-v', 'quiet', '--nologo'], {
    encoding: 'utf8', stdio: 'pipe', timeout: 600_000,
    env: { ...process.env, DOTNET_NOLOGO: '1', DOTNET_CLI_TELEMETRY_OPTOUT: '1' },
  });
} catch (e) { out = String(e.stdout || '') + String(e.stderr || ''); failed = true; }

// Map every diagnostic back from its namespace to the file a person can open.
const byNs = new Map(units.map((u) => [u.ns, u]));
const errors = [];
for (const line of out.split('\n')) {
  const m = line.match(/src[\\/](N\d+)\.cs\((\d+),(\d+)\):\s*(error|warning)\s+(\S+):\s*(.*?)(?:\s*\[.*)?$/);
  if (!m || m[4] !== 'error') continue;
  const u = byNs.get(m[1]);
  errors.push({ where: u ? `${u.slug}/${u.file}` : m[1], line: Number(m[2]) - 1, code: m[5], msg: m[6].trim() });
}

if (!errors.length && !failed) {
  console.log(`  all ${units.length} file(s) compile.\n`);
  if (!keep) rmSync(dir, { recursive: true, force: true });
  process.exit(0);
}

if (!errors.length) {
  console.log('  the build failed but produced no per-file error - raw output follows:\n');
  console.log(out.split('\n').slice(-25).join('\n'));
} else {
  const grouped = new Map();
  for (const e of errors) (grouped.get(e.where) ?? grouped.set(e.where, []).get(e.where)).push(e);
  console.log(`  ${errors.length} error(s) in ${grouped.size} file(s):\n`);
  for (const [where, es] of grouped) {
    console.log(`  ${where}`);
    for (const e of es.slice(0, 6)) console.log(`      line ${e.line}: ${e.code} ${e.msg}`);
    if (es.length > 6) console.log(`      ... and ${es.length - 6} more`);
  }
}
console.log(keep ? `\ntemp project kept at ${dir}\n` : '');
if (!keep) rmSync(dir, { recursive: true, force: true });
process.exit(1);
