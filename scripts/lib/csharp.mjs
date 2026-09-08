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
 * Is the identifier at `i` one that must NOT be renamed?
 *
 * Shared by sameShape and shapeForm so the comparison and the hash can never
 * drift apart. Fixed means: renaming it makes a different program, not a
 * tidier one.
 *
 *   x.Length          a member name belongs to somebody else's API
 *   class Solution    the declared type
 *   new Queue<..>     the type being constructed
 *   TreeNode node     the type in a declaration - an identifier directly
 *                     followed by another identifier is `Type name` in C#
 *
 * Deliberately NOT "followed by <": the tokenizer emits `<` one character at a
 * time, so `l < = r` and `t < nums[m]` both look like the start of a generic
 * argument list. `Queue<TreeNode>` is pinned anyway by the `new Queue<..>` that
 * has to construct it, and by consistency across the two.
 *
 * The last three matter more than they look. Without them `List<int> seen`
 * and `HashSet<int> seen` are a consistent one-to-one rename of each other,
 * so the guard would wave through a lint rewrite that turned an O(n) Contains
 * into an O(1) one - a complexity change, dressed as tidier naming.
 */
/**
 * The names NeetCode wrote, which are not yours to change.
 *
 * Every solution starts from a stub: `public bool IsValidBST(TreeNode root)`.
 * The method name and its parameters are the grader's interface - rename one
 * and the submission no longer compiles against it. Everything else in the
 * file is yours: locals, and the private helpers you added.
 *
 * Pinned by NAME across the whole file, not just at the signature. Pinning only
 * the declaration would let a rewrite rename `root` in the body and leave the
 * parameter behind, which is a broken file. Pinning the name everywhere also
 * settles the case that first exposed this: a helper `DFS(TreeNode root, ...)`
 * where the model sensibly renamed the helper's `root` to `node` and left the
 * public one alone. Both edits were right, and sameShape - which reads a flat
 * stream of tokens and knows nothing of scope - could only see one name
 * becoming two. Now neither is renamed, and there is nothing to disagree about.
 */
export function publicSignatureNames(toks) {
  const fixed = new Set();

  for (let i = 0; i < toks.length; i++) {
    if (!(toks[i].t === 'kw' && toks[i].v === 'public')) continue;

    // Walk the declaration head to its parameter list. A field or property
    // never gets there: it hits `{`, `;` or `=` first and is abandoned.
    let open = -1;
    for (let j = i + 1; j < toks.length; j++) {
      const t = toks[j];
      if (t.t === 'op' && t.v === '(') { open = j; break; }
      if (t.t === 'kw' || t.t === 'id') continue;
      if (t.t === 'op' && '<>[],.?'.includes(t.v)) continue;
      break;
    }
    if (open < 0) continue;

    // The method's own name is the identifier immediately before the list.
    if (toks[open - 1]?.t === 'id') fixed.add(toks[open - 1].v);

    // A parameter name is the identifier that closes its slot: `int[] nums,`
    // or `TreeNode root)`. Depth-tracked, so a default value or a generic
    // argument inside the list cannot be mistaken for one.
    let depth = 0;
    for (let k = open; k < toks.length; k++) {
      const t = toks[k];
      if (t.t === 'op' && t.v === '(') { depth++; continue; }
      if (t.t === 'op' && t.v === ')') { if (--depth === 0) break; continue; }
      if (depth !== 1 || t.t !== 'id') continue;
      const next = toks[k + 1];
      if (next && next.t === 'op' && (next.v === ',' || next.v === ')')) fixed.add(t.v);
    }
  }
  return fixed;
}

function fixedIdentifier(toks, i, boilerplate) {
  const prev = toks[i - 1], next = toks[i + 1];
  if (boilerplate && boilerplate.has(toks[i].v)) return 'boilerplate';
  if (prev && prev.t === 'op' && prev.v === '.') return 'member';
  if (prev && prev.t === 'kw' &&
      (prev.v === 'class' || prev.v === 'struct' || prev.v === 'interface' || prev.v === 'namespace')) return 'type';
  if (prev && prev.t === 'kw' && prev.v === 'new') return 'type';
  if (next && next.t === 'id') return 'type';
  return null;
}

/**
 * A canonical form in which two files that are the same solution collapse to
 * the same string, whatever the variables are called.
 *
 * Every identifier is replaced by the order in which it first appears, so
 * `left/right/mid` and `l/r/m` come out identical. Comments and whitespace are
 * gone with them. Held literal, because renaming one of these is a different
 * program rather than a tidier one: keywords, operators, literals, a member
 * name after a dot, and a declared type name. That is the same set sameShape()
 * refuses to see renamed, deliberately - the two answer the same question, one
 * as a comparison and one as a hash.
 *
 * Rare false collapse worth knowing about: two files differing ONLY by swapping
 * the roles of two variables in a non-commutative spot, with nothing earlier to
 * pin which is which, canonicalise the same. `l - r` fixed to `r - l` on the
 * first line either name appears is the shape of it.
 */
export function shapeForm(src) {
  const toks = significant(tokenize(src));
  const boilerplate = publicSignatureNames(toks);
  const seen = new Map();
  const out = [];

  for (let i = 0; i < toks.length; i++) {
    const t = toks[i];
    if (t.t !== 'id') { out.push(t.v); continue; }
    if (fixedIdentifier(toks, i, boilerplate)) { out.push(t.v); continue; }
    if (!seen.has(t.v)) seen.set(t.v, `$${seen.size}`);
    out.push(seen.get(t.v));
  }
  return out.join(' ');
}

/**
 * @returns {{ok: boolean, errors: string[], renames: Array<[string,string]>}}
 */
export function sameShape(before, after) {
  const A = significant(tokenize(before));
  const B = significant(tokenize(after));
  const boilerplate = publicSignatureNames(A);
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

    // A member name belongs to some other API, and a type name decides what the
    // program costs. Renaming either is a different program, not a tidier one.
    const fixed = fixedIdentifier(A, i, boilerplate);
    if (fixed && a.v !== b.v) {
      errors.push(fixed === 'boilerplate'
        ? `${a.v} -> ${b.v}: ${a.v} is part of the public signature NeetCode gave you - leave it alone`
        : `${fixed} name changed: ${a.v} -> ${b.v} near: ${near}`);
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
