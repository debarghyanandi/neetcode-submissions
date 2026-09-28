<#
.SYNOPSIS
  Build ONE slug's visualizer with several models, side by side, as a DRY RUN.

.DESCRIPTION
  Runs scripts/visualize.mjs once per model (and per -Runs repeat) with --backfill but
  WITHOUT --apply, so nothing in the repo changes. Each build is saved to
  .agent/tmp/try/<slug>-visualizer.<model>-<effort>.html (gitignored) for opening side by side.
  Prints a table: pass/fail, wall-clock, $ spent, output/thinking tokens, whether the repair
  attempt was needed.

  --model is passed explicitly, so visualize.mjs uses the same model for the repair attempt too
  (it only swaps to 'opus' when no --model is given). Pass -RepairEffort to pin the repair's
  effort; left empty, the repair uses each model's own default effort.

.EXAMPLE
  .\scripts\compare-visualizer-models.ps1 -Slug binary-tree-diameter
.EXAMPLE
  .\scripts\compare-visualizer-models.ps1 -Slug lru-cache -Runs 2 -Models claude-opus-5-5,claude-sonnet-5-5
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)] [string]   $Slug,
  [string[]] $Models = @('claude-opus-5', 'claude-opus-5-5', 'claude-sonnet-5-5'),
  [ValidateSet('low', 'medium', 'high', 'xhigh', 'max')] [string] $Effort = 'medium',
  [ValidateSet('', 'low', 'medium', 'high', 'xhigh', 'max')] [string] $RepairEffort = '',
  [int]    $Runs = 1,
  # Sonnet 5.5 needs Claude Code >= 2.1.284, Opus 5.5 >= 2.1.280 (code.claude.com/docs/en/model-config).
  [string] $MinCli = '2.1.284',
  [switch] $UpgradeCli
)

$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot          # script lives in <repo>\scripts
Set-Location $repo

# --- 1. Claude Code version gate -------------------------------------------------------------
function Get-CliVersion {
  $raw = (& claude --version 2>$null | Select-Object -First 1)
  if (-not $raw) { return $null }
  return [version](($raw -split '\s+')[0])
}

$cli = Get-CliVersion
if (-not $cli -or $cli -lt [version]$MinCli) {
  Write-Host "Claude Code is $cli; $MinCli or later is needed for these models." -ForegroundColor Yellow
  if ($UpgradeCli) {
    # Exact version, same as CI pins it - not @latest.
    npm install -g "@anthropic-ai/claude-code@$MinCli"
    $cli = Get-CliVersion
    if ($cli -lt [version]$MinCli) { throw "Upgrade failed: still on $cli" }
  } else {
    throw "Re-run with -UpgradeCli, or: npm install -g @anthropic-ai/claude-code@$MinCli"
  }
}
Write-Host "Claude Code $cli" -ForegroundColor Green

# --- 2. One dry-run build per model ----------------------------------------------------------
$logDir = Join-Path $repo '.agent\tmp\try\logs'
New-Item -ItemType Directory -Force -Path $logDir | Out-Null
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$results = @()

foreach ($run in 1..$Runs) {
  foreach ($m in $Models) {
    Write-Host "`n=== $Slug, $m, effort $Effort, run $run/$Runs ===" -ForegroundColor Cyan
    $nodeArgs = @('scripts/visualize.mjs', '--slug', $Slug, '--backfill', '--model', $m, '--effort', $Effort)
    if ($RepairEffort) { $nodeArgs += @('--repair-effort', $RepairEffort) }

    $log = Join-Path $logDir "$Slug-$m-$Effort-run$run-$stamp.log"
    $sw = [Diagnostics.Stopwatch]::StartNew()
    # Continue, so Windows PowerShell 5.1 does not turn node's stderr lines into terminating errors.
    $ErrorActionPreference = 'Continue'
    # Echo each line live AND keep it; the log is written as UTF-8 (5.1's Tee-Object writes UTF-16).
    $out = & node @nodeArgs 2>&1 | ForEach-Object { $line = "$_"; Write-Host $line; $line }
    $exit = $LASTEXITCODE
    $ErrorActionPreference = 'Stop'
    $sw.Stop()
    $out | Out-File -FilePath $log -Encoding utf8

    $text = $out -join "`n"
    # A login failure is the same for every model - stop instead of spending the remaining runs on it.
    if ($text -match 'Failed to authenticate|OAuth|Not logged in|invalid api key') {
      Write-Host "`nClaude Code is not logged in - no model was called. Run 'claude', then /login, and re-run." -ForegroundColor Red
      if ($env:CLAUDE_CODE_OAUTH_TOKEN) { Write-Host "Note: CLAUDE_CODE_OAUTH_TOKEN is set in this shell and overrides /login. Clear it: Remove-Item Env:CLAUDE_CODE_OAUTH_TOKEN" -ForegroundColor Yellow }
      if ($env:ANTHROPIC_API_KEY)       { Write-Host "Note: ANTHROPIC_API_KEY is set in this shell and is used instead of your subscription." -ForegroundColor Yellow }
      exit 1
    }
    $cost     = if ($text -match 'validated\s+\S+\s+\$([\d.]+)') { [decimal]$Matches[1] } else { $null }
    $outTok   = if ($text -match 'total tokens:.*?out ([\d,]+)') { [int]($Matches[1] -replace ',', '') } else { $null }
    $thinking = if ($text -match 'total tokens:.*?thinking ([\d,]+)') { [int]($Matches[1] -replace ',', '') } else { $null }
    $repaired = $text -match 'attempt 2: repairing'
    $saved    = if ($text -match 'saved for comparison: (.+)') { $Matches[1].Trim() } else { '' }

    $results += [pscustomobject]@{
      Model    = $m
      Run      = $run
      Result   = if ($exit -eq 0 -and $saved) { 'PASS' } else { "FAIL ($exit)" }
      Seconds  = [math]::Round($sw.Elapsed.TotalSeconds, 0)
      CostUSD  = $cost
      OutTok   = $outTok
      Thinking = $thinking
      Repaired = $repaired
      Html     = $saved
      Log      = $log
    }
  }
}

# --- 3. Summary ------------------------------------------------------------------------------
Write-Host "`n=== $Slug, effort $Effort ===" -ForegroundColor Green
$results | Format-Table Model, Run, Result, Seconds, CostUSD, OutTok, Thinking, Repaired -AutoSize

if ($Runs -gt 1) {
  $results | Group-Object Model | ForEach-Object {
    $ok = @($_.Group | Where-Object Result -eq 'PASS')
    [pscustomobject]@{
      Model      = $_.Name
      PassRate   = "$($ok.Count)/$($_.Count)"
      AvgSeconds = [math]::Round((($_.Group | Measure-Object Seconds -Average).Average), 0)
      AvgCostUSD = if ($ok.Count) { [math]::Round((($ok | Measure-Object CostUSD -Average).Average), 4) } else { $null }
      AvgThink   = if ($ok.Count) { [math]::Round((($ok | Measure-Object Thinking -Average).Average), 0) } else { $null }
    }
  } | Format-Table -AutoSize
}

$csv = Join-Path $logDir "$Slug-compare-$stamp.csv"
$results | Export-Csv -NoTypeInformation -Path $csv
Write-Host "CSV: $csv"
Write-Host "Open the saved .html files side by side to judge fidelity - cost and pass/fail do not show that."
