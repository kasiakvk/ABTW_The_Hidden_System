"""
ABTW: The Hidden System — Asset Validator
Validates that all required C# scripts, JSON configs, and YAML files exist.
Version: 1.0
"""

import os
import json
from pathlib import Path

PROJECT_ROOT = Path(__file__).parent.parent
ASSETS_ROOT  = PROJECT_ROOT / "Assets" / "ABTW"

REQUIRED_SCRIPTS = [
    "Core/GameManager.cs",
    "Core/PlayerMovement.cs",
    "Core/PlayerStats.cs",
    "Core/CameraController.cs",
    "Core/InputHandler.cs",
    "Core/SceneSetup.cs",
    "Core/ChapterOneDirector.cs",
    "Systems/Perception/PerceptionSystem.cs",
    "Systems/Notebook/NotebookSystem.cs",
    "Systems/Glitch/GlitchSystem.cs",
    "Systems/Echo/EchoSystem.cs",
    "Systems/Memory/MemoryWalkSystem.cs",
    "Systems/Trio/TrioSystem.cs",
    "Systems/World/WorldManager.cs",
    "Systems/Spawner/AnomalySpawner.cs",
    "Systems/Save/SaveSystem.cs",
    "Anomalies/Base/AnomalyBase.cs",
    "Anomalies/Shadow/ShadowDelayAnomaly.cs",
    "Anomalies/Light/LightInstabilityAnomaly.cs",
    "Anomalies/Symbol/SymbolFragmentAnomaly.cs",
    "Anomalies/Reflection/ReflectionMismatchAnomaly.cs",
    "Anomalies/Sound/SoundEchoAnomaly.cs",
    "Companions/Lumi/LumiCompanion.cs",
    "Companions/Nela/NelaCompanion.cs",
    "UI/Core/UIManager.cs",
    "UI/Notebook/NotebookUI.cs",
    "UI/HUD/HUDController.cs",
    "Editor/ABTWEditorTools.cs",
]

REQUIRED_DATA = [
    "Data/symbol_map.json",
    "Data/Stats/character_traits.json",
    "Data/Stats/player_stats_default.json",
    "Data/World/world_states.json",
    "Data/World/academy_config.yaml",
    "Data/World/pipeline_config.yaml",
    "Data/Notebook/notebook_entries.json",
    "Data/Patterns/spawn_table.json",
]

def validate():
    missing = []
    found   = []

    all_required = REQUIRED_SCRIPTS + REQUIRED_DATA
    for rel_path in all_required:
        full = ASSETS_ROOT / rel_path
        if full.exists():
            found.append(rel_path)
        else:
            missing.append(rel_path)

    print(f"\n🌌 ABTW Asset Validator")
    print(f"   Root: {ASSETS_ROOT}")
    print(f"   Found:   {len(found)}/{len(all_required)}")
    print(f"   Missing: {len(missing)}")

    if missing:
        print("\n❌ Missing files:")
        for m in missing:
            print(f"   - {m}")
    else:
        print("\n✅ All required assets present!")

    report = {
        "total":   len(all_required),
        "found":   len(found),
        "missing": missing,
        "status":  "PASS" if not missing else "FAIL"
    }

    report_path = PROJECT_ROOT / "pipeline" / "exports" / "asset_validation_report.json"
    report_path.parent.mkdir(parents=True, exist_ok=True)
    with open(report_path, "w", encoding="utf-8") as f:
        json.dump(report, f, indent=2)

    print(f"\n📄 Report saved: {report_path}")
    return report

if __name__ == "__main__":
    validate()
