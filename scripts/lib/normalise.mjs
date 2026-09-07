import { createHash } from 'node:crypto';
import { shapeForm } from './csharp.mjs';

/**
 * Strip C#/C-family comments from source, respecting string and char literals.
 *
 * Why a state machine and not a regex: `string s = "// not a comment";` and
 * `char c = '"';` both defeat every regex you would reach for first. Getting
 * this wrong makes the fingerprint below silently unreliable, and a silently
 * unreliable dedupe is worse than none.
 *
 * Handles: // line, /* block *\/, "quoted", @"verbatim" (doubled "" escape),
 * 'c', and backslash escapes inside quoted strings.
 */
export function stripComments(src) {
  const out = [];
  const n = src.length;
  let i = 0;

  while (i < n) {
    const c = src[i];
    const d = src[i + 1];

    // --- comments ---
    if (c === '/' && d === '/') {
      while (i < n && src[i] !== '\n') i++;
      continue;                     // leave the \n for the whitespace pass
    }
    if (c === '/' && d === '*') {
      i += 2;
      while (i < n && !(src[i] === '*' && src[i + 1] === '/')) i++;
      i += 2;
      out.push(' ');                // a block comment can join two tokens
      continue;
    }

    // --- verbatim string: @"..."  where "" is a literal quote ---
    if (c === '@' && d === '"') {
      out.push(c, d);
      i += 2;
      while (i < n) {
        if (src[i] === '"') {
          if (src[i + 1] === '"') { out.push('"', '"'); i += 2; continue; }
          out.push('"'); i++; break;
        }
        out.push(src[i]); i++;
      }
      continue;
    }

    // --- quoted string / char literal ---
    if (c === '"' || c === "'") {
      const quote = c;
      out.push(c);
      i++;
      while (i < n) {
        if (src[i] === '\\') { out.push(src[i], src[i + 1]); i += 2; continue; }
        out.push(src[i]);
        if (src[i] === quote) { i++; break; }
        if (src[i] === '\n') { i++; break; }   // unterminated - bail, don't hang
        i++;
      }
      continue;
    }

    out.push(c);
    i++;
  }

  return out.join('');
}

/**
 * Comment-free, whitespace-free form. Two files that differ only in
 * formatting, indentation, or commentary collapse to the same string.
 */
export function normalise(src) {
  return stripComments(src).replace(/\s+/g, '');
}

/** SHA-256 of the normalised form. This is the identity of a solution. */
export function fingerprint(src) {
  return createHash('sha256').update(normalise(src), 'utf8').digest('hex');
}

/** Short form for logs and state files. Collision risk is irrelevant at this scale. */
export function shortPrint(src) {
  return fingerprint(src).slice(0, 12);
}

/**
 * The identity of a SOLUTION, as opposed to the identity of a file.
 *
 * Two submissions are the same solution when they are the same logic - the
 * same technique, the same structure - whatever the variables are called and
 * whatever you wrote in the comments. That has to be the test rather than a
 * hash of the text, because lint itself renames variables: without this, a
 * resubmission of code lint had already tidied would read as a brand new
 * solution and be filed as optimal-variant.cs alongside its own original.
 *
 * C# only. Anything else falls back to the text hash, which is strictly safer:
 * it can miss a duplicate, never invent one.
 */
export function solutionPrint(src, ext = 'cs') {
  if (String(ext).toLowerCase() !== 'cs') return fingerprint(src);
  try {
    return createHash('sha256').update('shape1|' + shapeForm(src), 'utf8').digest('hex');
  } catch {
    return fingerprint(src);
  }
}
