#!/usr/bin/env python3
"""
ABTW: The Hidden System — Master Production Pipeline
=====================================================
Connects Office Agent workspace → C:\\ABTW_The_Hidden_System\\ABTW_The_Hidden_System
Generates all required files into correct paths and folders.

Usage:
    python abtw_pipeline.py --mode generate   # Generate all files
    python abtw_pipeline.py --mode validate   # Validate existing structure
    python abtw_pipeline.py --mode sync       # Sync to local Unity project
    python abtw_pipeline.py --mode report     # Print status report
"""

import os
import json
import yaml
import argparse
import logging
import shutil
from pathlib import Path
from datetime import datetime

# ─── LOGGING ──────────────────────────────────────────────────────────────────
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    handlers=[
        logging.StreamHandler(),
        logging.FileHandler("abtw_pipeline.log", encoding="utf-8"),
    ],
)
log = logging.getLogger("ABTW_Pipeline")

# ─── PATHS ────────────────────────────────────────────────────────────────────
WORKSPACE_ROOT = Path(__file__).parent.parent.parent  # /workspace
PRODUCTION_ROOT = WORKSPACE_ROOT / "ABTW_Production"
LOCAL_UNITY_ROOT = Path("C:/ABTW_The_Hidden_System/ABTW_The_Hidden_System")

CONFIG_PATH = PRODUCTION_ROOT / "pipeline" / "pipeline_config.yaml"
WORLD_CONFIG_PATH = PRODUCTION_ROOT / "pipeline" / "world_config.json"

# ─── UNITY PROJECT STRUCTURE ──────────────────────────────────────────────────
UNITY_STRUCTURE = {
    "Assets/ABTW/Core": [
        "GameManager.cs",
        "PlayerMovement.cs",
        "PlayerStats.cs",
        "CameraController.cs",
        "InputHandler.cs",
        "SceneSetup.cs",
        "ChapterOneDirector.cs",
        "MainMenuController.cs",
        "PauseMenu.cs",
    ],
    "Assets/ABTW/Systems/Perception": ["PerceptionSystem.cs"],
    "Assets/ABTW/Systems/Notebook": ["NotebookSystem.cs"],
    "Assets/ABTW/Systems/Glitch": ["GlitchSystem.cs"],
    "Assets/ABTW/Systems/Echo": ["EchoSystem.cs"],
    "Assets/ABTW/Systems/Memory": ["MemoryWalkSystem.cs"],
    "Assets/ABTW/Systems/Trio": ["TrioSystem.cs"],
    "Assets/ABTW/Systems/World": ["WorldManager.cs"],
    "Assets/ABTW/Systems/Spawner": ["AnomalySpawner.cs"],
    "Assets/ABTW/Systems/Save": ["SaveSystem.cs"],
    "Assets/ABTW/Anomalies/Base": ["AnomalyBase.cs"],
    "Assets/ABTW/Anomalies/Shadow": ["ShadowDelayAnomaly.cs"],
    "Assets/ABTW/Anomalies/Light": ["LightInstabilityAnomaly.cs"],
    "Assets/ABTW/Anomalies/Symbol": ["SymbolFragmentAnomaly.cs"],
    "Assets/ABTW/Anomalies/Reflection": ["ReflectionMismatchAnomaly.cs"],
    "Assets/ABTW/Anomalies/Sound": ["SoundEchoAnomaly.cs"],
    "Assets/ABTW/Companions/Lumi": ["LumiCompanion.cs"],
    "Assets/ABTW/Companions/Nela": ["NelaCompanion.cs"],
    "Assets/ABTW/UI/Core": ["UIManager.cs"],
    "Assets/ABTW/UI/Notebook": ["NotebookUI.cs"],
    "Assets/ABTW/UI/HUD": ["HUDController.cs"],
    "Assets/ABTW/Data": [
        "symbol_map.json",
        "character_traits.json",
        "academy_config.yaml",
    ],
    "Assets/ABTW/Visual/Materials": [],
    "Assets/ABTW/Visual/Shaders": [],
    "Assets/ABTW/Visual/Lighting": [],
    "Assets/ABTW/Visual/FX": [],
    "Assets/ABTW/Audio/Ambient": [],
    "Assets/ABTW/Audio/Perception": [],
    "Assets/ABTW/Audio/UI": [],
    "Assets/ABTW/Audio/Music": [],
    "Assets/ABTW/Levels/Chapter01": [],
    "Assets/ABTW/Levels/Test": [],
    "Assets/ABTW/Editor": ["ABTWEditorTools.cs"],
    "Assets/ABTW/Resources": [],
    "Assets/ABTW/Prefabs/Player": [],
    "Assets/ABTW/Prefabs/Anomalies": [],
    "Assets/ABTW/Prefabs/UI": [],
    "Assets/ABTW/ScriptableObjects": [],
}

# ─── GRAPHIC ASSET REGISTRY ───────────────────────────────────────────────────
GRAPHIC_ASSETS = {
    "Characters": {
        "Astra": [
            "Astra_Idle_Sheet.png",
            "Astra_Walk_Sheet.png",
            "Astra_Notice_Sheet.png",
            "Astra_Expression_Sheet.png",
            "Astra_Silhouette.png",
            "Astra_Portrait_UI.png",
        ],
        "Lumi": [
            "Lumi_Idle_Sheet.png",
            "Lumi_Walk_Sheet.png",
            "Lumi_Notebook_Sheet.png",
            "Lumi_Expression_Sheet.png",
            "Lumi_Portrait_UI.png",
        ],
        "Nela": [
            "Nela_Idle_Sheet.png",
            "Nela_Walk_Sheet.png",
            "Nela_Expression_Sheet.png",
            "Nela_Portrait_UI.png",
        ],
        "Supporting": [
            "ProfQ_Portrait.png",
            "Noctra_Portrait.png",
            "OrionVex_Portrait.png",
            "MrVale_Portrait.png",
            "Nimbus_Sheet.png",
        ],
    },
    "Environments": {
        "EastCorridor": [
            "EastCorridor_Normal.png",
            "EastCorridor_Glitch.png",
            "EastCorridor_Activation.png",
            "EastCorridor_Night.png",
            "EastCorridor_Rain.png",
        ],
        "Observatory": [
            "Observatory_Day.png",
            "Observatory_Night.png",
            "Observatory_Activation.png",
            "Observatory_StarMap.png",
        ],
        "Archive": [
            "Archive_Level1.png",
            "Archive_Level2.png",
            "Archive_Hidden.png",
            "Archive_Mirror.png",
        ],
        "Dormitories": [
            "Dormitory_AstraRoom.png",
            "Dormitory_CommonArea.png",
            "Dormitory_Night.png",
        ],
        "Gardens": [
            "Garden_Autumn.png",
            "Garden_Winter.png",
            "Garden_Spring.png",
            "Garden_Summer.png",
            "Garden_Spiral.png",
        ],
        "Rooftop": [
            "Rooftop_Day.png",
            "Rooftop_Sunset.png",
            "Rooftop_Night.png",
        ],
        "HiddenHalls": [
            "HiddenHall_Entry.png",
            "HiddenHall_Activated.png",
        ],
    },
    "Symbols": {
        "SYM_001_ShadowDelay": [
            "SYM001_Hidden.png",
            "SYM001_Glitch.png",
            "SYM001_Forming.png",
            "SYM001_Activation.png",
            "SYM001_Responding.png",
            "SYM001_Deepening.png",
            "SYM001_Consequence.png",
        ],
        "SYM_002_IncompleteCircle": [
            "SYM002_Hidden.png",
            "SYM002_Glitch.png",
            "SYM002_Forming.png",
            "SYM002_Activation.png",
            "SYM002_Responding.png",
            "SYM002_Deepening.png",
            "SYM002_Consequence.png",
        ],
        "SYM_003_ReflectedWord": [
            "SYM003_Glitch.png",
            "SYM003_Forming.png",
            "SYM003_Activation.png",
        ],
        "SYM_004_ConvergencePoint": [
            "SYM004_Hidden.png",
            "SYM004_Activation.png",
            "SYM004_Consequence.png",
        ],
        "SYM_005_MemoryTrace": [
            "SYM005_Glitch.png",
            "SYM005_Forming.png",
            "SYM005_Activation.png",
        ],
        "SYM_006_GardenSpiral": [
            "SYM006_Forming.png",
            "SYM006_Activation.png",
        ],
        "SYM_007_StarMap": [
            "SYM007_Hidden.png",
            "SYM007_Activation.png",
            "SYM007_Consequence.png",
        ],
        "SYM_008_EchoCorridor": [
            "SYM008_Glitch.png",
            "SYM008_Forming.png",
        ],
    },
    "UI": {
        "Notebook": [
            "Notebook_Cover.png",
            "Notebook_PageBlank.png",
            "Notebook_PagePattern.png",
            "Notebook_PageSymbol.png",
            "Notebook_PageEcho.png",
            "Notebook_Tab_Patterns.png",
            "Notebook_Tab_Symbols.png",
            "Notebook_Tab_Connections.png",
            "Notebook_Tab_Memory.png",
            "Notebook_Button_Record.png",
            "Notebook_Button_Wait.png",
            "Notebook_Button_Revisit.png",
            "Notebook_StatusDot_Unrecorded.png",
            "Notebook_StatusDot_Recorded.png",
            "Notebook_StatusDot_Echo.png",
        ],
        "HUD": [
            "HUD_PerceptionRing.png",
            "HUD_AwarenessIcon.png",
            "HUD_ClarityIcon.png",
            "HUD_ConnectionIcon.png",
            "HUD_TimingIcon.png",
            "HUD_StabilityIcon.png",
            "HUD_TrioSync_Indicator.png",
        ],
        "Menus": [
            "MainMenu_Background.png",
            "MainMenu_Logo.png",
            "PauseMenu_Background.png",
            "ChapterSelect_Background.png",
        ],
    },
    "FX": {
        "Anomalies": [
            "FX_ShadowDelay_Sheet.png",
            "FX_LightFlicker_Sheet.png",
            "FX_ReflectionDistort_Sheet.png",
            "FX_EchoFootstep_Sheet.png",
            "FX_MemoryTrace_Sheet.png",
            "FX_GardenSpiral_Sheet.png",
        ],
        "Activation": [
            "FX_TrioPulse_Sheet.png",
            "FX_SymbolForm_Sheet.png",
            "FX_HiddenLayerReveal_Sheet.png",
            "FX_GoldThread_Sheet.png",
        ],
    },
    "BookIllustrations": {
        "Chapter1": [
            "CH1_P001_AstraEntersCorridor.png",
            "CH1_P002_FirstShadowGlitch.png",
            "CH1_P003_LumiNotebook.png",
            "CH1_P004_NelaWaiting.png",
            "CH1_P005_TrioMeeting.png",
            "CH1_P006_ShadowPattern.png",
            "CH1_P007_NotebookDecision.png",
            "CH1_P008_TrioActivation.png",
            "CH1_P009_HiddenLayerReveal.png",
            "CH1_P010_CorridorEnd.png",
            "CH1_COVER_Full.png",
        ],
    },
}


# ─── PIPELINE FUNCTIONS ───────────────────────────────────────────────────────

def load_config() -> dict:
    """Load pipeline configuration from YAML."""
    if CONFIG_PATH.exists():
        with open(CONFIG_PATH, "r", encoding="utf-8") as f:
            return yaml.safe_load(f)
    log.warning("Config not found, using defaults.")
    return {}


def load_world_config() -> dict:
    """Load world configuration from JSON."""
    if WORLD_CONFIG_PATH.exists():
        with open(WORLD_CONFIG_PATH, "r", encoding="utf-8") as f:
            return json.load(f)
    log.warning("World config not found.")
    return {}


def generate_folder_structure(base: Path, structure: dict) -> None:
    """Create all folders and placeholder files."""
    log.info(f"Generating folder structure under: {base}")
    for folder, files in structure.items():
        folder_path = base / folder
        folder_path.mkdir(parents=True, exist_ok=True)
        for filename in files:
            file_path = folder_path / filename
            if not file_path.exists():
                file_path.touch()
                log.info(f"  Created placeholder: {folder}/{filename}")
    log.info("Folder structure generation complete.")


def validate_structure(base: Path, structure: dict) -> dict:
    """Validate that all expected files exist. Returns report dict."""
    report = {"missing": [], "present": [], "total": 0}
    for folder, files in structure.items():
        for filename in files:
            file_path = base / folder / filename
            report["total"] += 1
            if file_path.exists():
                report["present"].append(str(file_path))
            else:
                report["missing"].append(str(file_path))
    return report


def generate_graphic_checklist(output_path: Path) -> None:
    """Generate PNG asset checklist as markdown."""
    lines = [
        "# 🎨 ABTW — PNG Graphic Asset Checklist",
        f"> Generated: {datetime.now().strftime('%Y-%m-%d %H:%M')}",
        "",
        "Mark each asset as `[x]` when completed and delivered to the correct folder.",
        "",
    ]
    total = 0
    for category, subcategories in GRAPHIC_ASSETS.items():
        lines.append(f"## {category}")
        lines.append("")
        for subcat, assets in subcategories.items():
            lines.append(f"### {subcat}")
            for asset in assets:
                lines.append(f"- [ ] `{asset}`")
                total += 1
            lines.append("")
    lines.append("---")
    lines.append(f"**Total assets: {total}**")
    output_path.parent.mkdir(parents=True, exist_ok=True)
    with open(output_path, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    log.info(f"Graphic checklist written: {output_path} ({total} assets)")


def sync_to_unity(source: Path, target: Path) -> None:
    """Sync generated files to local Unity project path."""
    if not target.exists():
        log.warning(f"Unity project path not found: {target}")
        log.warning("Skipping sync. Run from Windows with Unity project present.")
        return
    log.info(f"Syncing {source} → {target}")
    for item in source.rglob("*"):
        if item.is_file():
            rel = item.relative_to(source)
            dest = target / rel
            dest.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(item, dest)
            log.info(f"  Synced: {rel}")
    log.info("Sync complete.")


def print_report(base: Path, structure: dict) -> None:
    """Print a human-readable status report."""
    report = validate_structure(base, structure)
    present = len(report["present"])
    missing = len(report["missing"])
    total = report["total"]
    pct = (present / total * 100) if total > 0 else 0
    print("\n" + "=" * 60)
    print("  ABTW PRODUCTION PIPELINE — STATUS REPORT")
    print("=" * 60)
    print(f"  Base path : {base}")
    print(f"  Total     : {total}")
    print(f"  Present   : {present} ({pct:.1f}%)")
    print(f"  Missing   : {missing}")
    print("=" * 60)
    if missing > 0:
        print("\n  MISSING FILES:")
        for m in report["missing"][:20]:
            print(f"    ✗ {m}")
        if missing > 20:
            print(f"    ... and {missing - 20} more")
    else:
        print("\n  ✅ All files present!")
    print()


# ─── MAIN ─────────────────────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(description="ABTW Production Pipeline")
    parser.add_argument(
        "--mode",
        choices=["generate", "validate", "sync", "report"],
        default="generate",
        help="Pipeline mode",
    )
    parser.add_argument(
        "--base",
        type=str,
        default=str(PRODUCTION_ROOT),
        help="Base output directory",
    )
    args = parser.parse_args()
    base = Path(args.base)

    log.info(f"ABTW Pipeline starting — mode: {args.mode}")

    if args.mode == "generate":
        generate_folder_structure(base / "ABTW_Unity", UNITY_STRUCTURE)
        generate_graphic_checklist(
            PRODUCTION_ROOT / "checklists" / "PNG_Asset_Checklist.md"
        )
        log.info("Generation complete.")

    elif args.mode == "validate":
        report = validate_structure(base / "ABTW_Unity", UNITY_STRUCTURE)
        log.info(
            f"Validation: {len(report['present'])}/{report['total']} files present, "
            f"{len(report['missing'])} missing."
        )

    elif args.mode == "sync":
        sync_to_unity(base / "ABTW_Unity", LOCAL_UNITY_ROOT)

    elif args.mode == "report":
        print_report(base / "ABTW_Unity", UNITY_STRUCTURE)

    log.info("Pipeline finished.")


if __name__ == "__main__":
    main()