# ============================================================
# ABTW: The Hidden System — PowerShell Script 02
# PURPOSE: Generate all C# script stub files into correct paths
# RUN: After 01_Setup_Project_Structure.ps1
# ============================================================

param(
    [string]$RootPath = "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System\Assets\ABTW",
    [switch]$DryRun
)

# Map: relative path => class name
$scripts = @{
    "Core\GameManager.cs"                        = "GameManager"
    "Core\PlayerMovement.cs"                     = "PlayerMovement"
    "Core\PlayerStats.cs"                        = "PlayerStats"
    "Core\CameraController.cs"                   = "CameraController"
    "Core\InputHandler.cs"                       = "InputHandler"
    "Core\SceneSetup.cs"                         = "SceneSetup"
    "Core\ChapterOneDirector.cs"                 = "ChapterOneDirector"
    "Core\MainMenuController.cs"                 = "MainMenuController"
    "Core\PauseMenu.cs"                          = "PauseMenu"
    "Systems\Perception\PerceptionSystem.cs"     = "PerceptionSystem"
    "Systems\Notebook\NotebookSystem.cs"         = "NotebookSystem"
    "Systems\Glitch\GlitchSystem.cs"             = "GlitchSystem"
    "Systems\Echo\EchoSystem.cs"                 = "EchoSystem"
    "Systems\Memory\MemoryWalkSystem.cs"         = "MemoryWalkSystem"
    "Systems\Trio\TrioSystem.cs"                 = "TrioSystem"
    "Systems\World\WorldManager.cs"              = "WorldManager"
    "Systems\Spawner\AnomalySpawner.cs"          = "AnomalySpawner"
    "Systems\Save\SaveSystem.cs"                 = "SaveSystem"
    "Anomalies\Base\AnomalyBase.cs"              = "AnomalyBase"
    "Anomalies\Shadow\ShadowDelayAnomaly.cs"     = "ShadowDelayAnomaly"
    "Anomalies\Light\LightInstabilityAnomaly.cs" = "LightInstabilityAnomaly"
    "Anomalies\Symbol\SymbolFragmentAnomaly.cs"  = "SymbolFragmentAnomaly"
    "Anomalies\Reflection\ReflectionMismatchAnomaly.cs" = "ReflectionMismatchAnomaly"
    "Anomalies\Sound\SoundEchoAnomaly.cs"        = "SoundEchoAnomaly"
    "Companions\Lumi\LumiCompanion.cs"           = "LumiCompanion"
    "Companions\Nela\NelaCompanion.cs"           = "NelaCompanion"
    "UI\Core\UIManager.cs"                       = "UIManager"
    "UI\Notebook\NotebookUI.cs"                  = "NotebookUI"
    "UI\HUD\HUDController.cs"                    = "HUDController"
    "Editor\ABTWEditorTools.cs"                  = "ABTWEditorTools"
}

function Get-StubContent($className) {
    $isEditor = $className -eq "ABTWEditorTools"
    $namespace = if ($isEditor) { "UnityEditor" } else { "UnityEngine" }
    $baseClass = if ($isEditor) { "" } else { " : MonoBehaviour" }
    $editorUsing = if ($isEditor) { "`nusing UnityEditor;" } else { "" }
    return @"
// ============================================================
// ABTW: The Hidden System
// Class: $className
// Auto-generated stub — implement per Production Bible
// ============================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;$editorUsing

namespace ABTW
{
    public class $className$baseClass
    {
        // TODO: Implement $className per ABTW Programming Bible
        // Reference: docs/ABTW_Programming_Bible.md

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

Write-Host "`n🌌 ABTW C# Script Generator" -ForegroundColor Cyan
Write-Host "Target: $RootPath" -ForegroundColor Yellow
$created = 0
$skipped = 0

foreach ($entry in $scripts.GetEnumerator()) {
    $filePath = Join-Path $RootPath $entry.Key
    $className = $entry.Value
    $dir = Split-Path $filePath -Parent

    if ($DryRun) {
        Write-Host "  [DRY RUN] $($entry.Key)" -ForegroundColor Gray
        continue
    }

    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }

    if (-not (Test-Path $filePath)) {
        $content = Get-StubContent $className
        Set-Content -Path $filePath -Value $content -Encoding UTF8
        Write-Host "  ✅ $($entry.Key)" -ForegroundColor Green
        $created++
    } else {
        Write-Host "  ⏭  Exists: $($entry.Key)" -ForegroundColor DarkGray
        $skipped++
    }
}

Write-Host "`n📊 Summary: $created created, $skipped skipped" -ForegroundColor Cyan
Write-Host "✅ C# stubs ready!" -ForegroundColor Green