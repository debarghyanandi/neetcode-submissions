/**
 * A small C# tokenizer, and a check that one version of a file differs from
 * another ONLY by whitespace, comments and renamed local variables.
 *
 * This is the whole safety story for the lint step. A model asked to "improve
 * variable names" can quietly change a comparison, drop a branch, or swap two
 * arguments, and the result still compiles and still looks like your code. The
 * fix is not to trust it harder - it is to make the class of edit checkable.
 *
 * Allowed: whitespace, comments, and a consistent one-to-one renaming of local
 * identifiers. Refused: any change to keywords, literals, operators, member
 * names after a dot, declared type or method names, or the order of anything.
 */

const KEYWORDS = new Set([
  'abstract','as','base','bool','break','byte','case','catch','char','checked','class','const',
  'continue','decimal','default','delegate','do','double','else','enum','event','explicit','extern',
  'false','finally','fixed','float','for','foreach','goto','if','implicit','in','int','interface',
  'internal','is','lock','long','namespace','new','null','object','operator','out','override',
  'params','private','protected','public','readonly','ref','return','sbyte','sealed','short',
  'sizeof','stackalloc','static','string','struct','switch','this','throw','true','try','typeof',
  'uint','ulong','unchecked','unsafe','ushort','using','var','virtual','void','volatile','while',
]);

// C# contextual keywords are ordinary identifiers everywhere except the one
// place each is special: `value` in a property setter, `await` in an async
// method, `where` in a generic constraint. The tokenizer must NOT call them
// keywords - doing so made a perfectly legal rename look like the model had
// swapped an identifier for a keyword, and the file was refused. They are
// still poor names, so a rename that lands on one is refused by its own rule
// below, with a message that says what to do about it.
const CONTEXTUAL = new Set([
  'value','var','get','set','init','when','where','yield','async','await','nameof',
  'record','dynamic','partial','from','select','into','orderby','join','let','on','equals',
  'by','ascending','descending','global','alias','add','remove','args','managed','unmanaged',
  'notnull','required','scoped','file','and','or','not','with',
]);

/** Tokens: ws | comment | str | char | num | id | op */
export function tokenize(src) {
  const out = [];
  let i = 0;
  const n = src.length;

  while (i < n) {
    const c = src[i];

    if (/\s/.test(c)) { let j = i; while (j < n && /\s/.test(src[j])) j++; out.push({ t: 'ws', v: src.slice(i, j) }); i = j; continue; }

    if (c === '/' && src[i + 1] === '/') { let j = i; while (j < n && src[j] !== '\n') j++; out.push({ t: 'comment', v: src.slice(i, j) }); i = j; continue; }
    if (c === '/' && src[i + 1] === '*') { let j = i + 2; while (j < n && !(src[j] === '*' && src[j + 1] === '/')) j++; j += 2; out.push({ t: 'comment', v: src.slice(i, j) }); i = j; continue; }

    // @"verbatim", with "" as an escaped quote
    if (c === '@' && src[i + 1] === '"') {
      let j = i + 2;
      while (j < n) { if (src[j] === '"') { if (src[j + 1] === '"') { j += 2; continue; } j++; break; } j++; }
      out.push({ t: 'str', v: src.slice(i, j) }); i = j; continue;
    }
    if (c === '"' || c === "'") {
      const q = c; let j = i + 1;
      while (j < n) { if (src[j] === '\\') { j += 2; continue; } if (src[j] === q) { j++; break; } if (src[j] === '\n') break; j++; }
      out.push({ t: q === '"' ? 'str' : 'char', v: src.slice(i, j) }); i = j; continue;
    }

    if (/[0-9]/.test(c)) { let j = i; while (j < n && /[0-9a-fA-FxX._]/.test(src[j])) j++; out.push({ t: 'num', v: src.slice(i, j) }); i = j; continue; }

    if (/[A-Za-z_$]/.test(c)) {
      let j = i; while (j < n && /[A-Za-z0-9_]/.test(src[j])) j++;
      const v = src.slice(i, j);
      out.push({ t: KEYWORDS.has(v) ? 'kw' : 'id', v }); i = j; continue;
    }

    out.push({ t: 'op', v: c }); i++;
  }
  return out;
}

/** Tokens that carry meaning - whitespace and comments are not among them. */
const significant = (toks) => toks.filter((t) => t.t !== 'ws' && t.t !== 'comment');

/**
 * @returns {{ok: boolean, errors: string[], renames: Array<[string,string]>}}
 */
export function sameShape(before, after) {
  const A = significant(tokenize(before));
  const B = significant(tokenize(after));
  const errors = [];

  if (A.length !== B.length) {
    return { ok: false, renames: [], errors: [`token count changed: ${A.length} before, ${B.length} after - something other than names or spacing was edited`] };
  }

  const fwd = new Map(), rev = new Map();

  for (let i = 0; i < A.length; i++) {
    const a = A[i], b = B[i];
    const near = A.slice(Math.max(0, i - 4), i + 4).map((t) => t.v).join(' ');

    if (a.t !== b.t) { errors.push(`token ${i} changed kind (${a.t} -> ${b.t}) near: ${near}`); continue; }

    if (a.t !== 'id') {
      if (a.v !== b.v) errors.push(`${a.t} changed: ${JSON.stringify(a.v)} -> ${JSON.stringify(b.v)} near: ${near}`);
      continue;
    }

    // A member name after a dot belongs to some other API - renaming it is a
    // different program, not a tidier one. Same for a declared type or method.
    const prev = A[i - 1];
    const isMember = prev && prev.t === 'op' && prev.v === '.';
    const isDeclName = prev && prev.t === 'kw' && (prev.v === 'class' || prev.v === 'struct' || prev.v === 'interface' || prev.v === 'namespace');
    if ((isMember || isDeclName) && a.v !== b.v) {
      errors.push(`${isMember ? 'member' : 'type'} name changed: ${a.v} -> ${b.v} near: ${near}`);
      continue;
    }

    // Landing on a contextual keyword is legal C# but a bad name, and in a
    // property setter or an async method it is a compile error.
    if (a.v !== b.v && CONTEXTUAL.has(b.v)) {
      errors.push(`${a.v} renamed to ${b.v}, which is a C# contextual keyword - pick an ordinary name instead`);
      continue;
    }

    if (fwd.has(a.v) && fwd.get(a.v) !== b.v) errors.push(`${a.v} renamed inconsistently: ${fwd.get(a.v)} then ${b.v}`);
    if (rev.has(b.v) && rev.get(b.v) !== a.v) errors.push(`two different names both became ${b.v}: ${rev.get(b.v)} and ${a.v}`);
    fwd.set(a.v, b.v); rev.set(b.v, a.v);
  }

  // Comments are exempt from the token comparison above, which means the model
  // is free to rewrite them - and free to delete them. Notes you left for
  // yourself are not the model's to discard, so losing one is a hard failure.
  // Rewording is still allowed: a rename can make a comment name a variable
  // that no longer exists.
  const commentsBefore = tokenize(before).filter((t) => t.t === 'comment').length;
  const commentsAfter = tokenize(after).filter((t) => t.t === 'comment').length;
  if (commentsAfter < commentsBefore) {
    errors.push(`${commentsBefore - commentsAfter} comment(s) deleted - comments may be reworded, never removed`);
  }

  const renames = [...fwd].filter(([k, v]) => k !== v);
  return { ok: errors.length === 0, errors: [...new Set(errors)].slice(0, 12), renames };
}
