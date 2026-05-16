# 💻 ACADEMY BEYOND THIS WORLD: THE HIDDEN SYSTEM
## MASTER PROGRAMMING BIBLE — v1.0

> *"The system responds to perception. The code must respond to the system."*

---

## TABLE OF CONTENTS
1. [Architecture Overview](#1-architecture-overview)
2. [Project Structure](#2-project-structure)
3. [Core Systems](#3-core-systems)
4. [System Flow & Connections](#4-system-flow--connections)
5. [Script Reference](#5-script-reference)
6. [Data Structures](#6-data-structures)
7. [Build Pipeline](#7-build-pipeline)
8. [Development Phases](#8-development-phases)

---

## 1. ARCHITECTURE OVERVIEW

### Master Flow
```
GameManager
  → SceneSetup
  → Player enters Level
  → AnomalySpawner activates
  → PerceptionSystem listens
  → GlitchSystem triggers anomaly
  → NotebookSystem opens decision
  → Player chooses (Record / Wait)
  → EchoSystem OR TrioSystem reacts
  → WorldManager updates state
  → SaveSystem saves
```

### System Layers
| Layer | Description | Key Scripts |
|---|---|---|
| Core Logic | Activation, symbol states, trio mechanics | `GameManager.cs`, `PlayerStats.cs` |
| Character Modules | Astra, Lumi, Nela behaviour | `PlayerMovement.cs`, `LumiCompanion.cs`, `NelaCompanion.cs` |
| Environment Systems | Corridor, library, dormitory DNA | `WorldManager.cs`, `EnvironmentDNA.cs` |
| UI / UX | Notebook, HUD, symbol maps | `NotebookUI.cs`, `HUDController.cs` |
| Visual Effects | Shader effects, anomaly FX | `GlitchSystem.cs`, `PerceptionSystem.cs` |
| Data / Config | JSON / YAML world data | `academy_config.yaml`, `symbol_map.json` |

---

## 2. PROJECT STRUCTURE

```
C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System\
│
├── Assets\
│   └── ABTW\
│       ├── Core\
│       │   ├── GameManager.cs
│       │   ├── PlayerMovement.cs
│       │   ├── PlayerStats.cs
│       │   ├── CameraController.cs
│       │   ├── InputHandler.cs
│       │   ├── SceneSetup.cs
│       │   └── ChapterOneDirector.cs
│       │
│       ├── Systems\
│       │   ├── Perception\
│       │   │   └── PerceptionSystem.cs
│       │   ├── Notebook\
│       │   │   └── NotebookSystem.cs
│       │   ├── Glitch\
│       │   │   └── GlitchSystem.cs
│       │   ├── Echo\
│       │   │   └── EchoSystem.cs
│       │   ├── Memory\
│       │   │   └── MemoryWalkSystem.cs
│       │   ├── Trio\
│       │   │   └── TrioSystem.cs
│       │   ├── World\
│       │   │   └── WorldManager.cs
│       │   ├── Spawner\
│       │   │   └── AnomalySpawner.cs
│       │   └── Save\
│       │       └── SaveSystem.cs
│       │
│       ├── Anomalies\
│       │   ├── Base\
│       │   │   └── AnomalyBase.cs
│       │   ├── Shadow\
│       │   │   └── ShadowDelayAnomaly.cs
│       │   ├── Light\
│       │   │   └── LightInstabilityAnomaly.cs
│       │   ├── Symbol\
│       │   │   └── SymbolFragmentAnomaly.cs
│       │   ├── Reflection\
│       │   │   └── ReflectionMismatchAnomaly.cs
│       │   └── Sound\
│       │       └── SoundEchoAnomaly.cs
│       │
│       ├── Companions\
│       │   ├── Lumi\
│       │   │   └── LumiCompanion.cs
│       │   └── Nela\
│       │       └── NelaCompanion.cs
│       │
│       ├── UI\
│       │   ├── Core\
│       │   │   └── UIManager.cs
│       │   ├── Notebook\
│       │   │   └── NotebookUI.cs
│       │   ├── HUD\
│       │   │   └── HUDController.cs
│       │   └── Fonts\
│       │
│       ├── Levels\
│       │   ├── Chapter01\
│       │   │   ├── EastCorridor.unity  ← START HERE
│       │   │   ├── Classroom.unity
│       │   │   ├── Archive.unity
│       │   │   └── Observatory.unity
│       │   └── Test\
│       │       └── Prototype.unity
│       │
│       ├── Visual\
│       │   ├── Materials\
│       │   ├── Shaders\
│       │   ├── Lighting\
│       │   └── FX\
│       │
│       ├── Audio\
│       │   ├── Ambient\
│       │   ├── Perception\
│       │   ├── UI\
│       │   └── Music\
│       │
│       ├── Data\
│       │   ├── Patterns\
│       │   ├── Stats\
│       │   ├── Notebook\
│       │   └── World\
│       │       ├── academy_config.yaml
│       │       └── symbol_map.json
│       │
│       ├── Prefabs\
│       │   ├── Player\
│       │   ├── Anomalies\
│       │   └── UI\
│       │
│       ├── Resources\
│       ├── Editor\
│       └── ScriptableObjects\
│
├── Packages\
├── ProjectSettings\
├── Builds\
│   └── ABTW_Windows\
│       ├── ABTW.exe
│       └── ABTW_Data\
│
└── Pipeline\
    ├── Generate-ABTW-Structure.ps1
    ├── validate_canon.py
    └── export_data.py
```

---

## 3. CORE SYSTEMS

### 3.1 GameManager.cs
Central hub connecting all systems. Attach to a **persistent GameObject** in every scene.

```csharp
namespace ABTW.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Systems")]
        public PerceptionSystem  perception;
        public NotebookSystem    notebook;
        public GlitchSystem      glitch;
        public EchoSystem        echo;
        public MemoryWalkSystem  memory;
        public TrioSystem        trio;
        public WorldManager      world;
        public AnomalySpawner    spawner;
        public SaveSystem        save;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
```

### 3.2 PerceptionSystem.cs
Activates when player stands still for `stillThreshold` seconds.

```csharp
// Trigger: player stops moving for 2+ seconds
// Effect: Time.timeScale = 0.7f (slow motion)
// Notifies: GlitchSystem.OnPerceptionActive()
```

**Key parameters:**
- `slowMotionScale = 0.7f`
- `stillThreshold = 2f` seconds

### 3.3 NotebookSystem.cs
Core mechanic — reality-affecting decision system.

```csharp
public enum PatternState { Unrecorded, Recorded, Echo, DeepRecord }

// Record() → removes glitch, flags echo, +EXP
// Wait()   → evolves glitch, checks trio activation
// DeepRecord() → unlocks hidden layer (Trio only)
```

### 3.4 GlitchSystem.cs
Manages anomaly spawning and evolution.

```csharp
// TriggerGlitch(prefab, position) → spawns anomaly
// RemoveActiveGlitch()            → destroys anomaly
// EvolveGlitch()                  → increases intensity
// OnPerceptionActive()            → makes glitch detectable
```

### 3.5 EchoSystem.cs
Spawns echo anomalies after a delay when patterns are recorded early.

```csharp
// FlagForEcho() → starts coroutine
// SpawnEchoAfterDelay() → waits echoDelay seconds, then spawns
// echoDelay = 30f seconds (default)
```

### 3.6 TrioSystem.cs
Monitors for synchronisation conditions.

```csharp
// Activation: GlitchActive == true AND player does NOT press any key for 3 seconds
// Result: ActivateTrio() → calls notebook.DeepRecord()
```

### 3.7 WorldManager.cs
Tracks time of day, season, and hidden layer state.

```csharp
public enum TimeOfDay  { Morning, Afternoon, Sunset, Night, Rain }
public enum Season     { Autumn, Winter, Spring, Summer }

// UnlockHiddenLayer() → HiddenLayerUnlocked = true
```

### 3.8 SaveSystem.cs
JSON-based save/load using `Application.persistentDataPath`.

```csharp
// Save() → writes abtw_save.json
// Load() → returns SaveData object
// SaveData fields: academyRank, exp, awareness, clarity,
//                 hiddenLayerUnlocked, patternState
```

### 3.9 ShadowDelayAnomaly.cs
Tutorial anomaly — shadow moves 0.4s after its source.

```csharp
// AnomalyID = "SYM_001_ShadowDelay"
// delaySeconds = 0.4f
// Buffers position and applies with delay each frame
```

---

## 4. SYSTEM FLOW & CONNECTIONS

### Perception Flow
```
PlayerMovement.Update()
  → player stops for 2s
  → PerceptionSystem.ActivatePerceptionMode()
  → Time.timeScale = 0.7f
  → GlitchSystem.OnPerceptionActive()
```

### Glitch Flow
```
GlitchSystem.TriggerGlitch()
  → Instantiate anomaly prefab
  → GlitchActive = true
  → NotebookSystem UI becomes available
```

### Notebook Decision Flow
```
RECORD button:
  NotebookSystem.Record()
  → GlitchSystem.RemoveActiveGlitch()
  → EchoSystem.FlagForEcho()
  → PlayerStats.AddEXP(50)

WAIT button:
  NotebookSystem.Wait()
  → GlitchSystem.EvolveGlitch()
  → TrioSystem.CheckActivation()
```

### Trio Activation Flow
```
TrioSystem.Update()
  → GlitchActive == true
  → No input for 3 seconds
  → ActivateTrio()
  → NotebookSystem.DeepRecord()
  → WorldManager.UnlockHiddenLayer()
```

### Echo Flow
```
EchoSystem.FlagForEcho()
  → StartCoroutine(SpawnEchoAfterDelay)
  → Wait 30 seconds
  → Instantiate echo prefab at Vector3.zero
```

---

## 5. SCRIPT REFERENCE

### Complete Script List

#### Core (7 scripts)
| Script | Namespace | Purpose |
|---|---|---|
| `GameManager.cs` | ABTW.Core | Central hub, singleton, DontDestroyOnLoad |
| `PlayerMovement.cs` | ABTW.Core | Movement + perception mode trigger |
| `PlayerStats.cs` | ABTW.Core | ScriptableObject — all player stats |
| `CameraController.cs` | ABTW.Core | 3rd person cinematic camera |
| `InputHandler.cs` | ABTW.Core | Input abstraction layer |
| `SceneSetup.cs` | ABTW.Core | Scene initialisation |
| `ChapterOneDirector.cs` | ABTW.Core | Chapter 1 narrative director |

#### Systems (9 scripts)
| Script | Namespace | Purpose |
|---|---|---|
| `PerceptionSystem.cs` | ABTW.Systems | Slow motion, anomaly detection mode |
| `NotebookSystem.cs` | ABTW.Systems | Record/Wait/DeepRecord decisions |
| `GlitchSystem.cs` | ABTW.Systems | Anomaly spawn, evolve, remove |
| `EchoSystem.cs` | ABTW.Systems | Delayed echo anomaly spawning |
| `MemoryWalkSystem.cs` | ABTW.Systems | Past event reconstruction |
| `TrioSystem.cs` | ABTW.Systems | Trio synchronisation detection |
| `WorldManager.cs` | ABTW.Systems | Time, season, hidden layer state |
| `AnomalySpawner.cs` | ABTW.Systems | Anomaly spawn table management |
| `SaveSystem.cs` | ABTW.Systems | JSON save/load |

#### Anomalies (6 scripts)
| Script | Namespace | Purpose |
|---|---|---|
| `AnomalyBase.cs` | ABTW.Anomalies | Abstract base class |
| `ShadowDelayAnomaly.cs` | ABTW.Anomalies | Shadow 0.4s delay (tutorial) |
| `LightInstabilityAnomaly.cs` | ABTW.Anomalies | Light flicker anomaly |
| `SymbolFragmentAnomaly.cs` | ABTW.Anomalies | Incomplete symbol appearance |
| `ReflectionMismatchAnomaly.cs` | ABTW.Anomalies | Mirror shows different content |
| `SoundEchoAnomaly.cs` | ABTW.Anomalies | Footstep echo delay |

#### Companions (2 scripts)
| Script | Namespace | Purpose |
|---|---|---|
| `LumiCompanion.cs` | ABTW.Companions | Lumi AI — analysis, notebook |
| `NelaCompanion.cs` | ABTW.Companions | Nela AI — timing, alignment |

#### UI (3 scripts)
| Script | Namespace | Purpose |
|---|---|---|
| `UIManager.cs` | ABTW.UI | UI state management |
| `NotebookUI.cs` | ABTW.UI | Notebook panel, Record/Wait buttons |
| `HUDController.cs` | ABTW.UI | Perception ring, stat indicators |

---

## 6. DATA STRUCTURES

### PlayerStats (ScriptableObject)
```csharp
[CreateAssetMenu(menuName = "ABTW/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Range(0,100)] public float Awareness  = 0f;
    [Range(0,100)] public float Clarity    = 0f;
    [Range(0,100)] public float Connection = 0f;
    [Range(0,100)] public float Timing     = 0f;
    [Range(0,100)] public float Stability  = 50f;
    public int AcademyRank = 1;
    public int EXP         = 0;
}
```

### SaveData (JSON)
```json
{
  "academyRank": 1,
  "exp": 0,
  "awareness": 0.0,
  "clarity": 0.0,
  "hiddenLayerUnlocked": false,
  "patternState": "Unrecorded"
}
```

### symbol_map.json (excerpt)
```json
{
  "version": "1.0",
  "canon_lock": true,
  "golden_rule": "Symbols never fully explain themselves. They respond to perception.",
  "symbol_states": ["Hidden","Glitch","Forming","Activation","Responding","Deepening","Consequence"],
  "symbols": [
    {
      "id": "SYM_001",
      "name": "The Delayed Shadow",
      "category": "Layer1_Anomaly",
      "location": "East Corridor",
      "trio_required": false,
      "hidden_layer": false,
      "color_glitch": "#6666CC",
      "color_activation": "#FFD700"
    }
  ]
}
```

### academy_config.yaml (excerpt)
```yaml
version: "1.0"
canon_lock: true
world:
  name: "Academy Beyond This World"
  golden_rule: "The Academy does not reveal itself. It responds to perception."
  anomaly_ratio: 0.30

time_states:
  - id: night
    anomaly_multiplier: 2.0
    emotional_tone: "Perception depth. Silence."
  - id: rain
    anomaly_multiplier: 1.8
    emotional_tone: "Reflection. Instability."
```

---

## 7. BUILD PIPELINE

### Minimum Build Requirements (Level 1)
```
Core/
    GameManager.cs
    PlayerMovement.cs

Systems/
    PerceptionSystem.cs
    NotebookSystem.cs
    GlitchSystem.cs

Anomalies/
    ShadowDelayAnomaly.cs

UI/
    NotebookUI.cs

Levels/
    EastCorridor.unity
```

### Build Checklist
#### Gameplay
- [ ] Player moves correctly
- [ ] Camera follows player
- [ ] Glitch trigger activates
- [ ] Perception Mode activates (2s still)
- [ ] Notebook opens (TAB key)
- [ ] RECORD button works
- [ ] WAIT button works

#### Systems
- [ ] Decision changes world state
- [ ] Glitch disappears after RECORD
- [ ] Glitch evolves after WAIT
- [ ] Echo spawns after delay
- [ ] Trio activates after 3s wait

#### UI
- [ ] Notebook panel visible
- [ ] Text updates correctly
- [ ] Buttons respond to click

#### Build
- [ ] Scene starts without errors
- [ ] No null reference exceptions
- [ ] .exe runs on target platform

### Unity Build Steps
```
File → Build Settings
→ Add EastCorridor.unity to scenes
→ Platform: PC, Mac & Linux Standalone
→ Target Platform: Windows
→ Architecture: x86_64
→ Build
```

---

## 8. DEVELOPMENT PHASES

### Phase 1 — Prototype (Weeks 1–4)
- [ ] Corridor level geometry
- [ ] Astra movement + camera
- [ ] 1 glitch (ShadowDelayAnomaly)
- [ ] Notebook UI (Record/Wait)
- [ ] Basic decision system

### Phase 2 — Alpha (Weeks 5–8)
- [ ] Trio system
- [ ] Echo system
- [ ] Memory Walk (basic)
- [ ] PlayerStats tracking
- [ ] Save/Load

### Phase 3 — Beta (Weeks 9–12)
- [ ] All Chapter 1 anomalies
- [ ] Lumi + Nela companions
- [ ] Full symbol system
- [ ] Audio integration
- [ ] Lighting polish

### Phase 4 — Gold (Weeks 13–16)
- [ ] Full Chapter 1 levels
- [ ] Performance optimisation
- [ ] QA pass
- [ ] Build + packaging

---

### VS Code Extensions Required
```
ms-dotnettools.csharp          # C# language support
unity.unity-debug              # Unity debugger
esbenp.prettier-vscode         # Code formatting
redhat.vscode-yaml             # YAML support
zainchen.json                  # JSON viewer
slevesque.shader               # Shader language support
ritwickdey.liveserver          # Live server for web UI tests
```

### PowerShell Setup
```powershell
# Fix execution policy (run as Administrator)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Run project generator
.\ABTW_Production\scripts\Generate-ABTW-Structure.ps1 -RootPath "C:\ABTW_The_Hidden_System\ABTW_The_Hidden_System"

# Dry run first
.\Generate-ABTW-Structure.ps1 -DryRun -Verbose
```

---

*"This is not a game about power. This is a game about perception."*

**Version 1.0 — Full Canon Lock**
