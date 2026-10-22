/**
 * The Blueprint of Mastery DSA sheet, as the index reads it.
 *
 * Three files under sheet/ hold it, and they are the only source the order comes from:
 *
 *   sheet.json    every problem on the sheet, in sheet order. `n` is its position and
 *                 `category` is its group.
 *   notes.json    the pattern-note problems (the 📝 ones), keyed by sheet slug.
 *   aliases.json  repo folder slug -> sheet slug, only where NeetCode's name differs.
 *
 * A folder the sheet lists is placed by the sheet, never by a heuristic. Only a solved
 * folder the sheet does not list falls through to the rules in patterns.mjs, and such a
 * folder goes to the bottom of whichever group those rules choose.
 *
 * Everything here is pure except loadSheet(), which reads the three files. A missing
 * file is an empty sheet, and the caller says so. A file that exists but does not parse
 * throws, because a silently wrong order is the one failure this exists to prevent.
 */

import { existsSync, readFileSync } from 'node:fs';
import { join } from 'node:path';

/**
 * @param {{problems?: object[], notes?: object[], aliases?: Record<string, string>}} parts
 *   the parsed contents of the three files
 * @returns the lookup tables the index needs
 */
export function buildSheet({ problems = [], notes = [], aliases = {} } = {}) {
  const bySlug = new Map();
  const categoryCounts = new Map();
  problems.forEach((p, i) => {
    bySlug.set(p.slug, {
      n: Number.isInteger(p.n) ? p.n : i + 1,
      category: p.category,
      title: p.title ?? p.slug,
      slug: p.slug,
    });
    categoryCounts.set(p.category, (categoryCounts.get(p.category) ?? 0) + 1);
  });

  const noteBySlug = new Map();
  for (const x of notes) {
    noteBySlug.set(x.slug, { pattern: x.pattern ?? '', takeaway: x.takeaway ?? '' });
  }

  return {
    bySlug,
    noteBySlug,
    aliases: { ...aliases },
    categoryCounts,
    count: bySlug.size,
    noteCount: noteBySlug.size,
  };
}

const readJson = (file) => (existsSync(file) ? JSON.parse(readFileSync(file, 'utf8')) : null);

/** Read sheet/ under the repo root. */
export function loadSheet(repoRoot) {
  const dir = join(repoRoot, 'sheet');
  const sheet = readJson(join(dir, 'sheet.json'));
  const notes = readJson(join(dir, 'notes.json'));
  const aliases = readJson(join(dir, 'aliases.json'));
  return buildSheet({
    problems: sheet?.problems ?? [],
    notes: notes?.notes ?? [],
    aliases: aliases?.aliases ?? {},
  });
}

/**
 * The sheet entry for a repo folder, or null when the sheet does not list it.
 * The alias is consulted first, so the folder `two-integer-sum` finds `two-sum`.
 * Object.hasOwn stops a folder named like an Object prototype key from matching.
 *
 * @returns {{n, category, title, slug, sheetSlug, note: {pattern, takeaway} | null} | null}
 */
export function matchSheet(sheet, folderSlug) {
  const want = Object.hasOwn(sheet.aliases, folderSlug) ? sheet.aliases[folderSlug] : folderSlug;
  const entry = sheet.bySlug.get(want);
  if (!entry) return null;
  return { ...entry, sheetSlug: want, note: sheet.noteBySlug.get(want) ?? null };
}
