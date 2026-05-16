# ABTW_Project — MERGED STRUCTURE
## Academy Beyond This World: The Hidden System
### Unity Game Project — v1.0 MERGED

---

## ✅ MERGE STATUS
This project is the **unified master** combining:
- `ABTW_Unity/` — original flat script structure (50 files)
- `ABTW_Project/` — organized production structure (89 files)

**Result:** All scripts, prefabs, configs, levels, shaders, and data in one clean hierarchy.

---

## 📁 PROJECT STRUCTURE

```
ABTW_Project/
├── Assets/
│   ├── ABTW/
│   │   ├── Core/               ← GameManager, PlayerMovement, AstraController, etc.
│   │   ├── Systems/
│   │   │   ├── Perception/     ← PerceptionSystem.cs
│   │   │   ├── Notebook/       ← NotebookSystem.cs
│   │   │   ├── Glitch/         ← GlitchSystem.cs
│   │   │   ├── Echo/           ← EchoSystem.cs
│   │   │   ├── Memory/         ← MemoryWalkSystem.cs
│   │   │   ├── Trio/           ← TrioSystem.cs
│   │   │   ├── World/          ← WorldManager, EnvironmentDNA, LightingManager
│   │   │   ├── Spawner/        ← AnomalySpawner.cs
│   │   │   └── Save/           ← SaveSystem.cs, SaveData.json
│   │   ├── Anomalies/
│   │   │   ├── Base/           ← AnomalyBase.cs
│   │   │   ├── Shadow/         ← ShadowDelayAnomaly.cs + ShadowGlitch.prefab
│   │   │   ├── Light/          ← LightInstabilityAnomaly.cs
│   │   │   ├── Symbol/         ← SymbolFragmentAnomaly.cs
│   │   │   ├── Reflection/     ← ReflectionMismatchAnomaly.cs
│   │   │   └── Sound/          ← SoundEchoAnomaly.cs
│   │   ├── Companions/
│   │   │   ├── Lumi/           ← LumiCompanion.cs, LumiAnalyzer.cs, Lumi.prefab
│   │   │   └── Nela/           ← NelaCompanion.cs, NelaActionMaker.cs, Nela.prefab
│   │   ├── UI/
│   │   │   ├── Core/           ← UIManager.cs, DialogueSystem.cs, UI_Canvas.prefab
│   │   │   ├── Notebook/       ← NotebookUI.cs, NotebookUI.tsx, UI_Notebook.prefab
│   │   │   └── HUD/            ← HUDController.cs, SymbolHUD.cs, UI_HUD.prefab
│   │   ├── Levels/
│   │   │   ├── Chapter01/      ← EastCorridor.unity ✅ (START HERE)
│   │   │   │                      Classroom.unity, Archive.unity, Observatory.unity
│   │   │   └── Test/           ← Prototype.unity (debug scene)
│   │   ├── Visual/
│   │   │   ├── Shaders/        ← SoftLightShader.shader, PerceptionShader.shader
│   │   │   ├── Lighting/       ← LightingProfile.asset
│   │   │   └── FX/             ← GlowEffect.cs
│   │   ├── Data/
│   │   │   ├── Patterns/       ← symbol_map.json, PatternsDatabase.asset
│   │   │   ├── Stats/          ← character_traits.json, PlayerStats.asset
│   │   │   ├── Notebook/       ← NotebookEntries.asset
│   │   │   └── World/          ← academy_config.yaml, WorldStates.asset
│   │   └── Resources/          ← GameConfig.asset, DefaultSettings.asset, InitScene.prefab
│   └── Editor/                 ← ABTWEditorTools.cs (NOT included in build)
├── Packages/
│   └── manifest.json           ← All Unity dependencies
└── ProjectSettings/
    └── ProjectSettings.asset
```

---

## 🚀 HOW TO OPEN IN UNITY

1. Open **Unity Hub**
2. Click **Add project from disk**
3. Select the `ABTW_Project/` folder
4. Unity version: **2022.3 LTS** or newer recommended
5. Wait for package import (manifest.json handles dependencies automatically)
6. Open `Assets/ABTW/Levels/Chapter01/EastCorridor.unity`
7. Press **Play**

---

## 🎮 MINIMUM BUILD (Level 1 only)

For the first playable vertical slice, you only need:

| System | File |
|--------|------|
| Entry point | `Core/GameManager.cs` |
| Player | `Core/PlayerMovement.cs` |
| Perception | `Systems/Perception/PerceptionSystem.cs` |
| Anomaly | `Systems/Glitch/GlitchSystem.cs` |
| Decision | `Systems/Notebook/NotebookSystem.cs` |
| Level | `Levels/Chapter01/EastCorridor.unity` |

---

## 🧠 CORE GAME LOOP

```
Player enters corridor
    → PerceptionSystem detects stillness (2 sec)
    → GlitchSystem triggers ShadowDelayAnomaly
    → NotebookSystem opens UI
    → Player chooses: RECORD or WAIT
        RECORD → world stabilizes → EchoSystem flags for later
        WAIT   → GlitchSystem evolves → TrioSystem activates
    → WorldManager updates state
    → SaveSystem records decision
```

---

## 📖 CANON RULE

> *"The Academy does not reveal itself. It responds to perception."*
> — ABTW Legendary Master World Bible v1.0

All game systems must obey this principle.
No combat. No spectacle. Only observation, understanding, and alignment.

---

## 🔒 VERSION
- **v1.0 MERGED** — May 2025
- Source A: `ABTW_Unity/` (50 files — original scripts)
- Source B: `ABTW_Project/` (89 files — production structure)
- Master: `ABTW_Project/` (89 files — this folder)