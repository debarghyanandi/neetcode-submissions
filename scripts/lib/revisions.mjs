/** Embedded by indexpage.mjs so every pipeline regeneration keeps file backups. */
export function initRevisions() {
  'use strict';
  const prefix = 'neetcode-submissions:revisions:v1:';
  const format = 'neetcode-submissions-revisions';
  const maxBytes = 1024 * 1024;
  const rows = Array.from(document.querySelectorAll('li[data-problem]'));
  const panel = document.getElementById('revision-backup');
  const message = document.getElementById('backup-message');
  const exportButton = document.getElementById('export-state');
  const importButton = document.getElementById('import-state');
  const fileInput = document.getElementById('state-file');
  const mode = document.getElementById('import-mode');
  const feedback = document.getElementById('revision-feedback');
  const pathOf = (row) => row.getAttribute('data-problem');
  const validPath = (path) => typeof path === 'string' && path.length <= 300 &&
    /^[^/\\\u0000-\u001f]+\/[^/\\\u0000-\u001f]+$/.test(path);
  const validCount = (n) => Number.isInteger(n) && n >= 0 && n <= 4;
  const countOf = (value) => Number.isInteger(Number(value)) ? Math.max(0, Math.min(4, Number(value))) : 0;
  let storageAvailable = true;

  function status(text, error = false) {
    message.textContent = text;
    panel.setAttribute('data-backup-state', error ? 'error' : 'ready');
  }
  function storedEntries() {
    const entries = new Map();
    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i);
      if (key?.startsWith(prefix) && validPath(key.slice(prefix.length))) {
        entries.set(key.slice(prefix.length), localStorage.getItem(key));
      }
    }
    return entries;
  }
  function readStored() {
    try { return new Map([...storedEntries()].map(([path, value]) => [path, countOf(value)])); }
    catch { storageAvailable = false; return new Map(); }
  }
  let counts = readStored();
  function snapshot() {
    const result = readStored();
    // Memory includes imports and clicks even if browser storage is unavailable.
    counts.forEach((n, path) => result.set(path, n));
    rows.forEach((row) => { if (!result.has(pathOf(row))) result.set(pathOf(row), 0); });
    return result;
  }
  function render() {
    rows.forEach((row) => {
      const n = counts.get(pathOf(row)) ?? 0;
      const button = row.querySelector('.revision');
      const title = row.querySelector('.name').textContent;
      button.setAttribute('data-revisions', String(n));
      button.querySelector('.revision-count').textContent = n + '/4';
      button.disabled = n === 4;
      button.hidden = false;
      button.setAttribute('aria-label', title + ': ' + n + ' of 4 revisions. ' +
        (n === 4 ? 'Revision goal complete.' : 'Mark revised once more.'));
      button.title = n === 4 ? 'Four revisions complete' : 'Mark revision ' + (n + 1) + ' of 4';
    });
  }
  function exportState() {
    return {
      format, version: 1, exportedAt: new Date().toISOString(),
      revisions: Object.fromEntries([...snapshot()].sort(([a], [b]) => a.localeCompare(b))),
    };
  }
  function validate(text) {
    if (typeof text !== 'string' || text.length > maxBytes) throw new Error('Choose a JSON state file smaller than 1 MB.');
    let data;
    try { data = JSON.parse(text.replace(/^\uFEFF/, '')); }
    catch { throw new Error('This file is not valid JSON. Choose an exported state file.'); }
    if (data?.format !== format || data.version !== 1 || !data.revisions ||
        typeof data.revisions !== 'object' || Array.isArray(data.revisions)) {
      throw new Error('This is not a supported NeetCode revision state file.');
    }
    const entries = Object.entries(data.revisions);
    if (entries.length > 5000 || !entries.every(([path, n]) => validPath(path) && validCount(n))) {
      throw new Error('The file contains invalid problem paths or revision counts. Counts must be whole numbers from 0 to 4.');
    }
    return new Map(entries);
  }
  function persist(next, replace) {
    let before;
    try {
      before = storedEntries();
      next.forEach((n, path) => localStorage.setItem(prefix + path, String(n)));
      if (replace) before.forEach((value, path) => {
        if (!next.has(path)) localStorage.removeItem(prefix + path);
      });
      storageAvailable = true;
      return true;
    } catch {
      storageAvailable = false;
      // A quota error must not leave half of a restored backup in storage.
      if (before) {
        const touched = new Set([...before.keys(), ...next.keys()]);
        touched.forEach((path) => {
          try {
            if (before.has(path)) localStorage.setItem(prefix + path, before.get(path));
            else localStorage.removeItem(prefix + path);
          } catch { /* The visible status still explains that persistence failed. */ }
        });
      }
      return false;
    }
  }
  function importState(text, importMode = 'merge') {
    const incoming = validate(text); // Validate the entire file before any change.
    if (importMode !== 'merge' && importMode !== 'replace') throw new Error('Choose merge or replace.');
    const next = importMode === 'replace' ? new Map(incoming) : snapshot();
    if (importMode === 'merge') incoming.forEach((n, path) => next.set(path, Math.max(next.get(path) ?? 0, n)));
    const saved = persist(next, importMode === 'replace');
    counts = next;
    render();
    const result = (importMode === 'replace' ? 'Restored' : 'Merged') + ' state from ' + incoming.size + ' problem(s).';
    status(result + (saved ? ' Saved in this browser.' : ' Saved only for this visit; browser storage is unavailable. Export a backup before leaving.'), !saved);
    return saved;
  }

  rows.forEach((row) => row.querySelector('.revision').addEventListener('click', () => {
    const path = pathOf(row), previous = counts.get(path) ?? 0;
    if (previous >= 4) return;
    const n = previous + 1;
    counts.set(path, n);
    let saved = true;
    try { localStorage.setItem(prefix + path, String(n)); storageAvailable = true; }
    catch { saved = false; storageAvailable = false; }
    render();
    feedback.textContent = row.querySelector('.name').textContent + ': ' + n + ' of 4 revisions.' +
      (saved ? '' : ' Browser storage is unavailable; saved only for this visit.');
    if (!saved) status('Saved only for this visit; browser storage is unavailable. Export a backup before leaving.', true);
  }));
  window.addEventListener('storage', (event) => {
    if (event.key === null || event.key?.startsWith(prefix)) { counts = readStored(); render(); }
  });
  exportButton.addEventListener('click', () => {
    let url;
    try {
      const data = exportState();
      const filename = 'neetcode-revisions-' + data.exportedAt.slice(0, 10) + '.json';
      url = window.URL.createObjectURL(new window.Blob([JSON.stringify(data, null, 2) + '\n'], { type: 'application/json' }));
      const link = document.createElement('a');
      link.href = url;
      link.download = filename;
      document.body.appendChild(link);
      try { link.click(); } finally { link.remove(); }
      status('Export requested: ' + filename + '. Keep the file to restore your progress in another browser or PC.');
    } catch { status('Could not export state. Try again.', true); }
    finally { if (url) window.setTimeout(() => window.URL.revokeObjectURL(url), 1000); }
  });
  importButton.addEventListener('click', () => fileInput.click());
  fileInput.addEventListener('change', async () => {
    const file = fileInput.files?.[0];
    if (!file) return;
    const importMode = mode.value;
    importButton.disabled = true;
    mode.disabled = true;
    try {
      if (file.size > maxBytes) throw new Error('Choose a JSON state file smaller than 1 MB.');
      importState(await file.text(), importMode);
    } catch (error) { status(error.message || 'Could not read this state file. Try again.', true); }
    finally { fileInput.value = ''; importButton.disabled = false; mode.disabled = false; }
  });
  mode.addEventListener('change', () => status(mode.value === 'replace'
    ? 'Replace restores the file exactly. Problems missing from the file reset to 0.'
    : 'Merge keeps the higher count for each problem. Other progress stays saved.'));
  panel.hidden = false;
  render();
  status(storageAvailable ? 'Saved in this browser. Use a JSON file to move progress to another browser or PC.'
    : 'Browser storage is unavailable. Use Export state to keep progress from this visit.', !storageAvailable);
  return { exportState, importState };
}
