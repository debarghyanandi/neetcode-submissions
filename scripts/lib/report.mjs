/**
 * Per-folder run reporting.
 *
 * The run summary used to say only "classify: success". When a folder was
 * skipped, refused or silently no-op'd you had to open four step logs and read
 * them in order to work out what had actually happened to which problem. Each
 * step now records one line per folder, and the summary renders them as a table.
 *
 * Writes to $PIPELINE_REPORT when set (the workflow points it at a temp file)
 * and does nothing at all otherwise, so local runs are unaffected.
 */

import { appendFileSync } from 'node:fs';

const FILE = process.env.PIPELINE_REPORT;
const inCI = !!process.env.GITHUB_ACTIONS;

/**
 * @param step   lint | classify | teach | visualize
 * @param slug   problem folder
 * @param status ok | skipped | refused | failed
 * @param detail short human phrase - what changed, or why it did not
 */
export function report(step, slug, status, detail = '') {
  if (!FILE) return;
  try {
    appendFileSync(FILE, [step, slug, status, String(detail).replace(/[\t\n|]/g, ' ')].join('\t') + '\n');
  } catch { /* reporting must never break the run */ }
}

/** A collapsible section in the Actions log, one per folder. */
export function group(title) {
  if (inCI) console.log(`::group::${title}`);
  else console.log(title);
}

export function endGroup() {
  if (inCI) console.log('::endgroup::');
}
