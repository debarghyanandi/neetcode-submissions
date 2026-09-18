/**
 * Deterministic C# formatting, done by the .NET SDK rather than by a model.
 *
 * Lint used to ask Haiku for spacing and names in the same breath, and that was
 * the bug. Spacing is a function - four spaces a level, a brace on its own line,
 * a space either side of an operator - and you do not call a stochastic model
 * for a function. The prompt's formatting section was, in practice, a polite
 * request: sameShape() ignores whitespace by design, so nothing ever checked
 * whether the model had honoured it. What the request DID do was invite the
 * model to tidy `if (n == 1)` by adding braces, which is a token change, which
 * failed the whole file.
 *
 * Every other language solved this a decade ago and none of the answers involve
 * judgement: gofmt, rustfmt, black, prettier, clang-format. For C# it is the SDK.
 *
 * `dotnet format whitespace --folder` is the exact tool:
 *
 *   --folder     treats the argument as a plain folder of code files. These
 *                submissions have no .csproj and no .sln, and this is the only
 *                subcommand that offers it - plain `dotnet format`, `style` and
 *                `analyzers` all need an MSBuild workspace.
 *   whitespace   runs ONLY whitespace rules. Brace insertion is csharp_prefer_braces
 *                (IDE0011), a *style* rule, so `dotnet format style` would add the
 *                very braces that broke house-robber - and would also apply `var`
 *                preferences and other rewrites. These files are a record of what
 *                was submitted to NeetCode. Whitespace rules move the text around;
 *                style rules change the code.
 *
 * Verified on Windows, SDK 10.0.401, against the real house-robber/suboptimal.cs:
 * K&R braces moved to Allman, `if(` spaced, `int [] dp = new int [n+1]` became
 * `int[] dp = new int[n + 1]`, `dp[i-1]` became `dp[i - 1]` - and the braceless
 * `if (n == 1)` / `return nums[0];` was left braceless.
 *
 * None of which is taken on trust: every result is put through sameShape() by the
 * caller before it is written. A formatter that ever changes a token is refused
 * exactly like a model that does. Same guard, different writer.
 *
 * Formatting is invisible to the rest of the pipeline, which is what makes it safe
 * to run over files lint is not renaming: fingerprint() strips whitespace and
 * solutionPrint() works on significant tokens, so no codePrint, headerSignature,
 * teachSignature or visualizer print moves because a file was reindented.
 */

import { mkdtempSync, writeFileSync, readFileSync, rmSync, existsSync, copyFileSync } from 'node:fs';
import { join } from 'node:path';
import { tmpdir } from 'node:os';
import { execFileSync } from 'node:child_process';
import { REPO } from './scan.mjs';

/** The .editorconfig IS the formatting spec now. Copied in so the temp folder inherits it. */
export const EDITORCONFIG = join(REPO, '.editorconfig');

let available = null;
/** Is there a usable `dotnet` on PATH? Asked once; the answer cannot change mid-run. */
export function dotnetAvailable() {
  if (available !== null) return available;
  try {
    execFileSync('dotnet', ['--version'], { stdio: 'pipe', timeout: 60_000 });
    available = true;
  } catch { available = false; }
  return available;
}

/** Line endings back to whatever the file had. The formatter is free to normalise them. */
export const toEol = (src, eol) => src.replace(/\r\n|\n/g, eol === '\r\n' ? '\r\n' : '\n');

/**
 * Format several C# sources in one invocation.
 *
 * Batched on purpose: `dotnet format` pays a few seconds of SDK start-up per call,
 * which is real money against a model call that takes fifteen, so a folder's files
 * go through together rather than one at a time.
 *
 * @param   {string[]} sources
 * @returns {string[]|null} formatted sources in the same order, or null if the
 *          formatter could not be run at all - never a partial result.
 */
export function formatMany(sources) {
  if (!sources.length) return [];
  if (!dotnetAvailable()) return null;

  let dir = null;
  try {
    dir = mkdtempSync(join(tmpdir(), 'neetfmt-'));
    if (existsSync(EDITORCONFIG)) copyFileSync(EDITORCONFIG, join(dir, '.editorconfig'));
    const names = sources.map((src, i) => {
      const n = `f${i}.cs`;
      writeFileSync(join(dir, n), src, 'utf8');
      return n;
    });
    execFileSync('dotnet', ['format', 'whitespace', '--folder', dir, '--verbosity', 'quiet'], {
      stdio: 'pipe', timeout: 300_000,
      env: { ...process.env, DOTNET_NOLOGO: '1', DOTNET_CLI_TELEMETRY_OPTOUT: '1', DOTNET_SKIP_FIRST_TIME_EXPERIENCE: '1' },
    });
    return names.map((n) => readFileSync(join(dir, n), 'utf8'));
  } catch {
    return null;
  } finally {
    if (dir) { try { rmSync(dir, { recursive: true, force: true }); } catch { /* temp dir */ } }
  }
}
