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
 * Which method each token belongs to.
 *
 * A local lives inside ONE method. `size` in MaxAreaOfIsland (the running max) and
 * `size` in Dfs (one island's area) are two different variables that happen to share a
 * name, and an honest rename gives them different names - maxArea and areaCount. A
 * file-wide rename map read that as one name becoming two and refused the file twice,
 * so max-area-of-island was never linted.
 *
 * So each member of a type - its signature, parameters and body - is its own scope.
 * A scope ends at a `}` or `;` that lands back at type level. Names that live AT type
 * level (fields, properties, method names) are shared by every method, so they stay
 * in one file-wide scope and must still be renamed the same way everywhere.
 *
 * @returns {{ scopeOf: number[], shared: Set<string> }}
 */
export function memberScopes(toks) {
  const TYPE_KW = new Set(['class', 'struct', 'interface', 'record', 'namespace']);
  const stack = [];                 // one entry per open brace: true = a type body
  let pendingType = false;          // a class/struct/... keyword seen, its `{` not yet
  let parens = 0;
  let scope = 0;
  const scopeOf = new Array(toks.length);
  const shared = new Set();
  const atTypeLevel = () => stack.length === 0 || stack[stack.length - 1] === true;

  for (let i = 0; i < toks.length; i++) {
    const t = toks[i];
    scopeOf[i] = scope;
    if (t.t === 'kw' && TYPE_KW.has(t.v)) pendingType = true;
    if (t.t === 'op') {
      if (t.v === '(') parens++;
      else if (t.v === ')') parens = Math.max(0, parens - 1);
      else if (t.v === '{') { stack.push(pendingType); pendingType = false; if (stack[stack.length - 1]) scope++; }
      else if (t.v === '}') { stack.pop(); if (atTypeLevel()) scope++; }
      else if (t.v === ';' && atTypeLevel()) { pendingType = false; scope++; }
      continue;
    }
    if (t.t === 'id' && atTypeLevel() && parens === 0) shared.add(t.v);
  }
  return { scopeOf, shared };
}

/**
 * Which tokens were inserted and which were dropped, when the two streams are
 * no longer the same length.
 *
 * This exists because of a real failure. The message used to be "token count
 * changed: 145 before, 147 after - something other than names or spacing was
 * edited", and that was the whole of it. It is true and it is useless: it does
 * not say WHICH two tokens, so the model gets the same non-information back on
 * the retry and makes the same edit again. house-robber's submission-0 was
 * refused twice with byte-identical errors, cost two model calls, and was
 * retired as a permanent failure - over one pair of braces the model had added
 * to a braceless `if`.
 *
 * A rejection that carries no new information is a wasted attempt. So name the
 * tokens.
 *
 * Standard LCS, which is O(n*m) - fine on files of a few hundred tokens and
 * capped below so a pathological input degrades to the old message rather than
 * eating the runner's memory.
 *
 * @returns {{added: string[], removed: string[], at: number}} `at` is the index
 *          in A where the two first part company, or -1 if they never do.
 */
export function tokenDelta(A, B) {
  const a = A.map((t) => t.v), b = B.map((t) => t.v);
  const n = a.length, m = b.length;
  if (n > 2000 || m > 2000) return { added: [], removed: [], at: -1 };

  const dp = Array.from({ length: n + 1 }, () => new Int32Array(m + 1));
  for (let i = n - 1; i >= 0; i--)
    for (let j = m - 1; j >= 0; j--)
      dp[i][j] = a[i] === b[j] ? dp[i + 1][j + 1] + 1 : Math.max(dp[i + 1][j], dp[i][j + 1]);

  const added = [], removed = [];
  let i = 0, j = 0, at = -1;
  while (i < n && j < m) {
    if (a[i] === b[j]) { i++; j++; continue; }
    if (at < 0) at = i;
    if (dp[i + 1][j] >= dp[i][j + 1]) removed.push(a[i++]);
    else added.push(b[j++]);
  }
  if ((i < n || j < m) && at < 0) at = i;
  while (i < n) removed.push(a[i++]);
  while (j < m) added.push(b[j++]);
  return { added, removed, at };
}

/**
 * The refusal you get when the token counts differ, written so the retry can
 * act on it. Braces get their own line because they are the common case and
 * because "formatting" is exactly what a model thinks they are.
 */
function countMismatch(A, B) {
  const head = `token count changed: ${A.length} before, ${B.length} after`;
  const d = tokenDelta(A, B);
  if (d.at < 0 || (!d.added.length && !d.removed.length))
    return [`${head} - something other than names or spacing was edited`];

  const show = (xs) => xs.slice(0, 6).map((v) => JSON.stringify(v)).join(' ') + (xs.length > 6 ? ` (+${xs.length - 6} more)` : '');
  const what = [
    d.added.length ? `added ${show(d.added)}` : null,
    d.removed.length ? `removed ${show(d.removed)}` : null,
  ].filter(Boolean).join(', and ');
  const near = A.slice(Math.max(0, d.at - 5), d.at + 5).map((t) => t.v).join(' ');

  const errors = [`${head} - the rewrite ${what}, first near: ${near}`];
  if ([...d.added, ...d.removed].some((v) => v === '{' || v === '}')) {
    errors.push('Braces are structure, not formatting. A body written without braces stays without braces - do not add a { } pair around it, and do not remove a pair that is already there. Move an existing brace to its own line if you like; never change how many there are.');
  }
  return errors;
}

/**
 * Rename local identifiers in a C# source, exactly, by the same rules sameShape()
 * uses to judge a rename.
 *
 * Written for the one-off repair that undid lint's 92 historical renames, and kept
 * because "apply a rename map to C# safely" is a thing worth having once rather
 * than twice. A regex over the text is NOT this: `\bnode\b` also hits `x.node`,
 * `class node`, a string literal and every occurrence in a comment.
 *
 * Only tokens that sameShape would have allowed to be renamed are touched:
 * never a member after a dot, never a declared or constructed type, never a name
 * from the public signature NeetCode generated. Whitespace, comments, strings and
 * every other token come through byte for byte.
 *
 * Comments are handled separately by the caller, on purpose. A rename makes a
 * comment name a variable that no longer exists, so they DO need updating - but a
 * comment is prose, and replacing `result` with `res` inside prose is how you end
 * up with "the res is the longest path". The caller decides which names are safe
 * to touch in text; this function decides nothing about text at all.
 *
 * `opts.includeSignature` lifts the one rule that exists for lint's benefit: normally the
 * names NeetCode generated - the public method and its parameters - are untouchable,
 * because renaming one breaks the submission against the grader. The repair needs the
 * opposite, because the old lint DID rename them: is-anagram shipped as
 * `IsAnagram(string original, string candidate)` when the stub says `(string s, string t)`.
 * Restoring the grader's own names is the only way back, so the repair passes this and
 * nothing else does. Members after a dot and type names stay pinned either way.
 *
 * @param   {string} src
 * @param   {Record<string,string>} map  old name -> new name
 * @param   {{includeSignature?: boolean}} opts
 * @returns {{code: string, applied: Array<[string,string]>}}
 */
export function renameIdentifiers(src, map, opts = {}) {
  const toks = tokenize(src);
  const sig = significant(toks);
  const boilerplate = publicSignatureNames(sig);

  // fixedIdentifier() indexes into the SIGNIFICANT stream (it looks at neighbours,
  // and whitespace is not a neighbour), so walk that stream and remember which
  // entries of the full stream each one came from.
  const sigIndexOf = [];
  for (let i = 0, j = 0; i < toks.length; i++) {
    if (toks[i].t === 'ws' || toks[i].t === 'comment') continue;
    sigIndexOf[j++] = i;
  }

  // A name pinned ANYWHERE in the file is refused everywhere in it, not just at the
  // position that pinned it. `Queue<int> queue = new Queue<int>();` is why: the
  // declaration's `Queue` is followed by `<`, and fixedIdentifier deliberately does
  // not read `<` as the start of a generic argument list - the tokenizer emits it one
  // character at a time, so `l < r` would look identical. sameShape does not care,
  // because `new Queue<..>` pins the name for the comparison; a rewriter does, because
  // nothing stops it renaming the declaration and leaving the construction behind.
  //
  // Refusing by name is the same rule the public signature already uses, and it errs
  // the safe way: at worst a local that happens to share a member's name is left alone.
  const guard = opts.includeSignature ? null : boilerplate;
  const pinned = new Set();
  for (let j = 0; j < sig.length; j++) {
    if (sig[j].t === 'id' && fixedIdentifier(sig, j, guard)) pinned.add(sig[j].v);
  }

  const applied = new Map();
  for (let j = 0; j < sig.length; j++) {
    const t = sig[j];
    if (t.t !== 'id') continue;
    const to = map[t.v];
    if (to == null || to === t.v) continue;
    if (pinned.has(t.v)) continue;                        // member, type, or the grader's name
    toks[sigIndexOf[j]] = { t: 'id', v: to };
    applied.set(t.v, to);
  }
  return { code: toks.map((t) => t.v).join(''), applied: [...applied] };
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
    return { ok: false, renames: [], errors: countMismatch(A, B) };
  }

  const fwd = new Map(), rev = new Map();
  const { scopeOf, shared } = memberScopes(A);
  // A shared name (field, property, method) is one scope for the whole file; a local or
  // a parameter is scoped to its method. Keys carry the scope so the maps never mix them.
  const keyOf = (i, name) => (shared.has(A[i].v) ? 'G' : scopeOf[i]) + ':' + name;

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
    if (fixed) {
      if (a.v !== b.v) {
        errors.push(fixed === 'boilerplate'
          ? `${a.v} -> ${b.v}: ${a.v} is part of the public signature NeetCode gave you - leave it alone`
          : `${fixed} name changed: ${a.v} -> ${b.v} near: ${near}`);
      }
      // Skip the rename bookkeeping below even when the name is UNCHANGED. A
      // member or a type lives in a different namespace from the locals, and
      // recording `root.left -> root.left` put "left" into the rename map - so
      // a local also called `left`, legitimately renamed to `newLeft`, then
      // read as the same name mapping to two different things.
      //
      // That is not a corner case in this repo: every tree solution has locals
      // called left and right sitting beside node.left and node.right. It
      // rejected invert-a-binary-tree twice and left the file unlinted.
      continue;
    }

    // Landing on a contextual keyword is legal C# but a bad name, and in a
    // property setter or an async method it is a compile error.
    if (a.v !== b.v && CONTEXTUAL.has(b.v)) {
      errors.push(`${a.v} renamed to ${b.v}, which is a C# contextual keyword - pick an ordinary name instead`);
      continue;
    }

    const fk = keyOf(i, a.v), rk = keyOf(i, b.v);
    if (fwd.has(fk) && fwd.get(fk) !== b.v) errors.push(`${a.v} renamed inconsistently: ${fwd.get(fk)} then ${b.v}`);
    if (rev.has(rk) && rev.get(rk) !== a.v) errors.push(`two different names both became ${b.v}: ${rev.get(rk)} and ${a.v}`);
    // A local renamed onto the name of a field or method would shadow it - a different program.
    if (!shared.has(a.v) && shared.has(b.v) && a.v !== b.v) errors.push(`${a.v} renamed to ${b.v}, which is already a field or method name in this file`);
    fwd.set(fk, b.v); rev.set(rk, a.v);
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

  // Scope keys stripped back to plain names. The same old name can appear twice with two
  // new names - that is the per-method case above, and both are reported.
  const renames = [...new Set([...fwd].map(([k, v]) => `${k.slice(k.indexOf(':') + 1)}\u0000${v}`))]
    .map((x) => x.split('\u0000'))
    .filter(([k, v]) => k !== v);
  return { ok: errors.length === 0, errors: [...new Set(errors)].slice(0, 12), renames };
}
