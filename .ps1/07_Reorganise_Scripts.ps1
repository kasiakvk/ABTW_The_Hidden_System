
# ============================================================
# ABTW: The Hidden System — PowerShell Script 07
# PURPOSE: Reorganise C# scripts from flat ABTW_Unity/Assets/Scripts/
#          into the canonical ABTW/ folder hierarchy.
#          Also creates any missing stub .cs files.
# RUN:     .\07_Reorganise_Scripts.ps1
#          .\07_Reorganise_Scripts.ps1 -DryRun      (preview only)
#          .\07_Reorganise_Scripts.ps1 -CreateStubs (generate missing stubs)
# ============================================================

param(
    [string]$UnityRoot    = "C:\ABTW_The_Hidden_System\ABTW_Unity",
    [switch]$DryRun,
    [switch]$CreateStubs
)

# ── Colour helpers ────────────────────────────────────────────────────────────
function Write-OK   { param($m) Write-Host "  ✅ $m" -ForegroundColor Green   }
function Write-Skip { param($m) Write-Host "  ⏭  $m" -ForegroundColor DarkGray }
function Write-Warn { param($m) Write-Host "  ⚠️  $m" -ForegroundColor Yellow  }
function Write-Fail { param($m) Write-Host "  ❌ $m" -ForegroundColor Red     }
function Write-Info { param($m) Write-Host "  ℹ️  $m" -ForegroundColor Cyan    }
function Write-New  { param($m) Write-Host "  🆕 $m" -ForegroundColor Magenta }

Write-Host "`n🌌 ABTW Script Reorganiser" -ForegroundColor Magenta
Write-Host "   Unity root : $UnityRoot" -ForegroundColor Yellow
if ($DryRun)      { Write-Host "   MODE      : DRY RUN (no files moved)" -ForegroundColor Cyan }
if ($CreateStubs) { Write-Host "   STUBS     : Will create missing stub files" -ForegroundColor Cyan }

if (-not (Test-Path $UnityRoot)) {
    Write-Fail "Unity root not found: $UnityRoot"
    exit 1
}

$AssetsRoot  = Join-Path $UnityRoot "Assets"
$OldScripts  = Join-Path $AssetsRoot "Scripts"   # flat source folder
$ABTWRoot    = Join-Path $AssetsRoot "ABTW"       # canonical destination root

# ── Canonical destination map ─────────────────────────────────────────────────
# Key   = script filename (case-insensitive match)
# Value = destination subfolder relative to Assets\ABTW\
$destMap = @{
    # ── Core ──────────────────────────────────────────────────────────────────
    "GameManager.cs"              = "Core"
    "PlayerMovement.cs"           = "Core"
    "PlayerStats.cs"              = "Core"
    "CameraController.cs"         = "Core"
    "InputHandler.cs"             = "Core"
    "SceneSetup.cs"               = "Core"
    "ChapterOneDirector.cs"       = "Core"
    "MainMenuController.cs"       = "Core"
    "PauseMenu.cs"                = "Core"
    "SaveManager.cs"              = "Core"
    "ActivationLogic.cs"          = "Core"
    "SymbolStateFlow.cs"          = "Core"

    # ── Systems ───────────────────────────────────────────────────────────────
    "PerceptionSystem.cs"         = "Systems\Perception"
    "NotebookSystem.cs"           = "Systems\Notebook"
    "GlitchSystem.cs"             = "Systems\Glitch"
    "EchoSystem.cs"               = "Systems\Echo"
    "MemoryWalkSystem.cs"         = "Systems\Memory"
    "TrioSystem.cs"               = "Systems\Trio"
    "WorldManager.cs"             = "Systems\World"
    "AnomalySpawner.cs"           = "Systems\Spawner"
    "SaveSystem.cs"               = "Systems\Save"
    "EnvironmentDNA.cs"           = "Systems"
    "LightingManager.cs"          = "Systems"
    "RoomState.cs"                = "Systems"
    "FogController.cs"            = "Systems"
    "GlowEffect.cs"               = "Systems"

    # ── Anomalies ─────────────────────────────────────────────────────────────
    "AnomalyBase.cs"              = "Anomalies\Base"
    "ShadowDelayAnomaly.cs"       = "Anomalies\Shadow"
    "LightInstabilityAnomaly.cs"  = "Anomalies\Light"
    "SymbolFragmentAnomaly.cs"    = "Anomalies\Symbol"
    "ReflectionMismatchAnomaly.cs"= "Anomalies\Reflection"
    "SoundEchoAnomaly.cs"         = "Anomalies\Sound"

    # ── Companions ────────────────────────────────────────────────────────────
    "AstraController.cs"          = "Companions\Astra"
    "LumiCompanion.cs"            = "Companions\Lumi"
    "LumiAnalyzer.cs"             = "Companions\Lumi"
    "NelaCompanion.cs"            = "Companions\Nela"
    "NelaActionMaker.cs"          = "Companions\Nela"

    # ── UI ────────────────────────────────────────────────────────────────────
    "UIManager.cs"                = "UI\Core"
    "NotebookUI.cs"               = "UI\Notebook"
    "HUDController.cs"            = "UI\HUD"
    "SymbolHUD.cs"                = "UI\HUD"
    "DialogueSystem.cs"           = "UI"

    # ── Editor ────────────────────────────────────────────────────────────────
    "ABTWEditorTools.cs"          = "Editor"
}

# ── Stub template ─────────────────────────────────────────────────────────────
function Get-StubContent {
    param([string]$ClassName, [string]$SubFolder)

    $isEditor   = $ClassName -eq "ABTWEditorTools"
    $baseClass  = if ($isEditor) { "" } else { " : MonoBehaviour" }
    $editorUsing= if ($isEditor) { "`nusing UnityEditor;" } else { "" }

    return @"
// ============================================================
// ABTW: The Hidden System
// Class  : $ClassName
// Folder : Assets/ABTW/$SubFolder
// Status : Stub — implement per ABTW Programming Bible
// Ref    : docs/ABTW_Programming_Bible.md
// ============================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;$editorUsing

namespace ABTW
{
    public class $ClassName$baseClass
    {
        // TODO: Implement $ClassName per ABTW Programming Bible

        private void Awake()
        {
            // Initialization
        }

        private void Start()
        {
            // Setup
        }

        private void Update()
        {
            // Per-frame logic
        }
    }
}
"@
}

# ── Step 1: Move existing scripts ─────────────────────────────────────────────
$moved   = 0
$skipped = 0
$notFound= 0

Write-Host "`n📦 Step 1 — Moving existing scripts from Assets\Scripts\" -ForegroundColor Cyan

if (-not (Test-Path $OldScripts)) {
    Write-Warn "Assets\Scripts\ folder not found — skipping move step."
} else {
    # Collect all .cs files recursively from old Scripts folder
    $allOldScripts = Get-ChildItem -Path $OldScripts -Recurse -Filter "*.cs"

    foreach ($file in $allOldScripts) {
        $name = $file.Name

        if ($destMap.ContainsKey($name)) {
            $relDest  = $destMap[$name]
            $destDir  = Join-Path $ABTWRoot $relDest
            $destPath = Join-Path $destDir $name

            if ($DryRun) {
                Write-Info "[DRY RUN] Move: $name → ABTW\$relDest\"
                $moved++
                continue
            }

            # Create destination directory if needed
            if (-not (Test-Path $destDir)) {
                New-Item -ItemType Directory -Path $destDir -Force | Out-Null
            }

            if (Test-Path $destPath) {
                Write-Skip "Already at destination: $name"
                $skipped++
            } else {
                Move-Item -Path $file.FullName -Destination $destPath -Force
                Write-OK "Moved: $name → ABTW\$relDest\"
                $moved++
            }
        } else {
            Write-Warn "No mapping for: $name (left in place)"
            $notFound++
        }
    }

    # Clean up empty subdirectories in old Scripts folder
    if (-not $DryRun) {
        $emptyDirs = Get-ChildItem -Path $OldScripts -Recurse -Directory |
                     Where-Object { (Get-ChildItem $_.FullName -Recurse -File).Count -eq 0 } |
                     Sort-Object FullName -Descending

        foreach ($dir in $emptyDirs) {
            Remove-Item $dir.FullName -Force -ErrorAction SilentlyContinue
            Write-Info "Removed empty dir: $($dir.FullName.Replace($UnityRoot,''))"
        }

        # Remove Scripts root if now empty
        if ((Get-ChildItem $OldScripts -Recurse -File).Count -eq 0) {
            Remove-Item $OldScripts -Recurse -Force -ErrorAction SilentlyContinue
            Write-OK "Removed empty Assets\Scripts\ folder"
        }
    }
}

# ── Step 2: Create missing stub files ─────────────────────────────────────────
$stubsCreated = 0
$stubsExist   = 0

if ($CreateStubs) {
    Write-Host "`n🆕 Step 2 — Creating missing stub .cs files" -ForegroundColor Cyan

    foreach ($entry in $destMap.GetEnumerator()) {
        $name     = $entry.Key
        $relDest  = $entry.Value
        $destDir  = Join-Path $ABTWRoot $relDest
        $destPath = Join-Path $destDir $name

        # Also check old Scripts folder
        $oldPath  = Get-ChildItem -Path $OldScripts -Recurse -Filter $name -ErrorAction SilentlyContinue |
                    Select-Object -First 1 -ExpandProperty FullName

        if ((Test-Path $destPath) -or ($oldPath -and (Test-Path $oldPath))) {
            Write-Skip "Exists: $name"
            $stubsExist++
            continue
        }

        $className = [System.IO.Path]::GetFileNameWithoutExtension($name)
        $content   = Get-StubContent -ClassName $className -SubFolder $relDest

        if ($DryRun) {
            Write-Info "[DRY RUN] Would create stub: $name → ABTW\$relDest\"
            $stubsCreated++
            continue
        }

        if (-not (Test-Path $destDir)) {
            New-Item -ItemType Directory -Path $destDir -Force | Out-Null
        }

        Set-Content -Path $destPath -Value $content -Encoding UTF8
        Write-New "Created stub: $name → ABTW\$relDest\"
        $stubsCreated++
    }
} else {
    Write-Host "`n  (Skipping stub creation — use -CreateStubs to generate missing files)" -ForegroundColor DarkGray
}

# ── Step 3: Verify final structure ────────────────────────────────────────────
Write-Host "`n🔍 Step 3 — Verifying canonical structure" -ForegroundColor Cyan

$verified = 0
$absent   = 0

foreach ($entry in $destMap.GetEnumerator()) {
    $name     = $entry.Key
    $relDest  = $entry.Value
    $destPath = Join-Path $ABTWRoot $relDest | Join-Path -ChildPath $name

    if (Test-Path $destPath) {
        $verified++
    } else {
        Write-Warn "Missing: ABTW\$relDest\$name"
        $absent++
    }
}

# ── Summary ───────────────────────────────────────────────────────────────────
Write-Host "`n📊 Summary" -ForegroundColor Magenta
Write-Host "   Scripts moved      : $moved"         -ForegroundColor Green
Write-Host "   Already in place   : $skipped"       -ForegroundColor DarkGray
Write-Host "   No mapping found   : $notFound"      -ForegroundColor $(if ($notFound -gt 0) { "Yellow" } else { "DarkGray" })
if ($CreateStubs) {
Write-Host "   Stubs created      : $stubsCreated"  -ForegroundColor Magenta
Write-Host "   Stubs already exist: $stubsExist"    -ForegroundColor DarkGray
}
Write-Host "   Verified present   : $verified / $($destMap.Count)" -ForegroundColor $(if ($absent -eq 0) { "Green" } else { "Yellow" })
Write-Host "   Still missing      : $absent"        -ForegroundColor $(if ($absent -gt 0) { "Yellow" } else { "DarkGray" })

if ($absent -gt 0) {
    Write-Host "`n  💡 Run with -CreateStubs to generate stub files for missing scripts." -ForegroundColor Cyan
}

Write-Host "`n✅ Script reorganisation complete!`n" -ForegroundColor Green