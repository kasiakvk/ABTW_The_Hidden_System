using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace ABTW.Core
{
    /// <summary>
    /// ABTW SaveManager (Extended) — persists world state, character stats,
    /// symbol states, notebook entries, echo records, and continuity tracking.
    /// The world remembers everything. So does the save system.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        // ─── Save Data Structures ─────────────────────────────────────────────

        [Serializable]
        public class CharacterSaveData
        {
            public float awareness;
            public float clarity;
            public float connection;
            public float timing;
            public float stability;
            public int academyRank;
            public int totalExp;
            public string currentRoom;
            public Vector3Data position;
        }

        [Serializable]
        public class SymbolSaveData
        {
            public string symbolId;
            public string state;        // Hidden / Glitch / Forming / Activation / Resolved / Echo
            public bool trioActivated;
            public bool notebookRecorded;
            public bool echoGenerated;
            public string roomId;
            public long timestampTicks;
        }

        [Serializable]
        public class NotebookEntrySaveData
        {
            public string entryId;
            public string patternName;
            public string status;       // Unrecorded / Recorded / Echo / DeepRecord
            public List<string> observations;
            public string lumiNote;
            public string nelaNote;
            public string echoNote;
            public bool isEcho;
            public long recordedAtTicks;
        }

        [Serializable]
        public class RoomSaveData
        {
            public string roomId;
            public string environmentState;
            public string lightingPreset;
            public float perceptionLevel;
            public float anomalyIntensity;
            public bool hiddenLayerRevealed;
            public bool trioActivationOccurred;
            public int visitCount;
            public List<string> symbolsDiscovered;
            public List<string> eventHistory;
        }

        [Serializable]
        public class WorldSaveData
        {
            public string currentSeason;
            public string currentTimeOfDay;
            public string currentWeather;
            public int dayCount;
            public float worldPerceptionLevel;
            public bool hiddenSystemPartiallyRevealed;
            public List<string> globalEventsTriggered;
        }

        [Serializable]
        public class EchoSaveData
        {
            public string echoId;
            public string originalSymbolId;
            public string echoType;     // Fragment / Trace / Residual
            public bool encountered;
            public long generatedAtTicks;
        }

        [Serializable]
        public class TrioSaveData
        {
            public float synchronizationLevel;
            public int totalActivations;
            public List<string> activationRoomIds;
            public bool lumiUnlocked;
            public bool nelaUnlocked;
        }

        [Serializable]
        public class MasterSaveData
        {
            public string saveVersion = "2.0";
            public string saveId;
            public long savedAtTicks;
            public string chapterProgress;  // e.g. "Book1_Chapter2"
            public int totalPlaytimeSeconds;

            public CharacterSaveData astra;
            public TrioSaveData trio;
            public WorldSaveData world;
            public List<SymbolSaveData> symbols;
            public List<NotebookEntrySaveData> notebookEntries;
            public List<RoomSaveData> rooms;
            public List<EchoSaveData> echoes;
            public List<string> decisionsLog;   // chronological decision history
        }

        [Serializable]
        public class Vector3Data
        {
            public float x, y, z;
            public Vector3Data(Vector3 v) { x = v.x; y = v.y; z = v.z; }
            public Vector3 ToVector3() => new Vector3(x, y, z);
        }

        // ─── Configuration ────────────────────────────────────────────────────

        [Header("Save Configuration")]
        public string saveFileName = "abtw_save.json";
        public bool useEncryption = false;          // future-proof flag
        public bool autoSaveEnabled = true;
        public float autoSaveInterval = 120f;       // seconds

        [Header("Debug")]
        public bool verboseLogging = false;

        // ─── Internal State ───────────────────────────────────────────────────

        private MasterSaveData currentSave;
        private string savePath;
        private float autoSaveTimer = 0f;
        private bool isDirty = false;

        // Events
        public event Action OnSaveCompleted;
        public event Action OnLoadCompleted;
        public event Action<string> OnSaveError;

        // ─── Lifecycle ────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, saveFileName);
            InitializeNewSave();
        }

        private void Update()
        {
            if (!autoSaveEnabled || !isDirty) return;

            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                autoSaveTimer = 0f;
                SaveGame();
            }
        }

        // ─── Save / Load ──────────────────────────────────────────────────────

        public bool SaveGame()
        {
            try
            {
                currentSave.savedAtTicks = DateTime.UtcNow.Ticks;
                CollectLiveData();

                string json = JsonUtility.ToJson(currentSave, prettyPrint: true);
                File.WriteAllText(savePath, json);

                isDirty = false;
                Log($"Game saved to {savePath}");
                OnSaveCompleted?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Save failed: {ex.Message}");
                OnSaveError?.Invoke(ex.Message);
                return false;
            }
        }

        public bool LoadGame()
        {
            try
            {
                if (!File.Exists(savePath))
                {
                    Log("No save file found. Starting fresh.");
                    InitializeNewSave();
                    return false;
                }

                string json = File.ReadAllText(savePath);
                currentSave = JsonUtility.FromJson<MasterSaveData>(json);

                if (currentSave == null)
                {
                    Debug.LogWarning("[SaveManager] Save file corrupt. Initializing new save.");
                    InitializeNewSave();
                    return false;
                }

                ApplyLoadedData();
                Log($"Game loaded. Chapter: {currentSave.chapterProgress}");
                OnLoadCompleted?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Load failed: {ex.Message}");
                InitializeNewSave();
                return false;
            }
        }

        public bool HasSaveFile() => File.Exists(savePath);

        public void DeleteSave()
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                InitializeNewSave();
                Log("Save file deleted.");
            }
        }

        // ─── Data Collection (Live → Save) ────────────────────────────────────

        private void CollectLiveData()
        {
            // Collect from AstraController
            var astra = FindObjectOfType<Companions.AstraController>();
            if (astra != null)
            {
                currentSave.astra.awareness   = astra.Awareness;
                currentSave.astra.clarity     = astra.Clarity;
                currentSave.astra.connection  = astra.Connection;
                currentSave.astra.timing      = astra.Timing;
                currentSave.astra.stability   = astra.Stability;
                currentSave.astra.academyRank = astra.AcademyRank;
                currentSave.astra.totalExp    = astra.TotalExp;
                currentSave.astra.position    = new Vector3Data(astra.transform.position);
            }

            // Collect from WorldManager
            var world = FindObjectOfType<Systems.WorldManager>();
            if (world != null)
            {
                currentSave.world.currentSeason    = world.CurrentSeason.ToString();
                currentSave.world.currentTimeOfDay = world.CurrentTimeOfDay.ToString();
                currentSave.world.currentWeather   = world.CurrentWeather.ToString();
            }

            // Collect symbol states
            var symbols = FindObjectsOfType<Systems.SymbolStateFlow>();
            currentSave.symbols.Clear();
            foreach (var sym in symbols)
            {
                currentSave.symbols.Add(new SymbolSaveData
                {
                    symbolId         = sym.SymbolId,
                    state            = sym.CurrentState.ToString(),
                    trioActivated    = sym.TrioActivated,
                    notebookRecorded = sym.NotebookRecorded,
                    echoGenerated    = sym.EchoGenerated,
                    roomId           = sym.RoomId,
                    timestampTicks   = DateTime.UtcNow.Ticks
                });
            }

            // Collect room states
            var rooms = FindObjectsOfType<Systems.RoomState>();
            currentSave.rooms.Clear();
            foreach (var room in rooms)
            {
                currentSave.rooms.Add(new RoomSaveData
                {
                    roomId                  = room.RoomId,
                    environmentState        = room.CurrentEnvironmentState.ToString(),
                    lightingPreset          = room.CurrentLightingPreset.ToString(),
                    perceptionLevel         = room.PerceptionLevel,
                    anomalyIntensity        = room.AnomalyIntensity,
                    hiddenLayerRevealed     = room.HiddenLayerRevealed,
                    trioActivationOccurred  = room.TrioActivationOccurred,
                    visitCount              = room.VisitCount,
                    symbolsDiscovered       = new List<string>(room.DiscoveredSymbolIds),
                    eventHistory            = new List<string>(room.EventHistory)
                });
            }
        }

        // ─── Data Application (Save → Live) ───────────────────────────────────

        private void ApplyLoadedData()
        {
            var astra = FindObjectOfType<Companions.AstraController>();
            if (astra != null && currentSave.astra != null)
            {
                astra.SetStats(
                    currentSave.astra.awareness,
                    currentSave.astra.clarity,
                    currentSave.astra.connection,
                    currentSave.astra.timing,
                    currentSave.astra.stability);
                astra.SetRank(currentSave.astra.academyRank, currentSave.astra.totalExp);

                if (currentSave.astra.position != null)
                    astra.transform.position = currentSave.astra.position.ToVector3();
            }
        }

        // ─── Incremental Updates (called by game systems) ─────────────────────

        public void RecordDecision(string decisionKey, string choice)
        {
            string entry = $"{DateTime.UtcNow:HH:mm:ss} | {decisionKey} → {choice}";
            currentSave.decisionsLog.Add(entry);
            MarkDirty();
            Log($"Decision recorded: {entry}");
        }

        public void RecordNotebookEntry(NotebookEntrySaveData entry)
        {
            var existing = currentSave.notebookEntries.Find(e => e.entryId == entry.entryId);
            if (existing != null)
                currentSave.notebookEntries.Remove(existing);

            currentSave.notebookEntries.Add(entry);
            MarkDirty();
        }

        public void RecordEcho(string originalSymbolId, string echoType)
        {
            currentSave.echoes.Add(new EchoSaveData
            {
                echoId           = $"ECHO_{originalSymbolId}_{DateTime.UtcNow.Ticks}",
                originalSymbolId = originalSymbolId,
                echoType         = echoType,
                encountered      = false,
                generatedAtTicks = DateTime.UtcNow.Ticks
            });
            MarkDirty();
        }

        public void UpdateChapterProgress(string chapter)
        {
            currentSave.chapterProgress = chapter;
            MarkDirty();
        }

        public void IncrementPlaytime(int seconds)
        {
            currentSave.totalPlaytimeSeconds += seconds;
        }

        // ─── Queries ──────────────────────────────────────────────────────────

        public bool WasSymbolRecorded(string symbolId)
        {
            var sym = currentSave.symbols.Find(s => s.symbolId == symbolId);
            return sym != null && sym.notebookRecorded;
        }

        public bool WasDecisionMade(string decisionKey)
        {
            return currentSave.decisionsLog.Exists(d => d.Contains(decisionKey));
        }

        public string GetChapterProgress() => currentSave?.chapterProgress ?? "Book1_Chapter1";

        public List<NotebookEntrySaveData> GetAllNotebookEntries()
            => currentSave?.notebookEntries ?? new List<NotebookEntrySaveData>();

        public List<EchoSaveData> GetPendingEchoes()
            => currentSave?.echoes?.FindAll(e => !e.encountered) ?? new List<EchoSaveData>();

        // ─── Initialization ───────────────────────────────────────────────────

        private void InitializeNewSave()
        {
            currentSave = new MasterSaveData
            {
                saveId           = Guid.NewGuid().ToString(),
                savedAtTicks     = DateTime.UtcNow.Ticks,
                chapterProgress  = "Book1_Chapter1",
                totalPlaytimeSeconds = 0,

                astra = new CharacterSaveData
                {
                    awareness  = 0.1f,
                    clarity    = 0.0f,
                    connection = 0.0f,
                    timing     = 0.0f,
                    stability  = 0.5f,
                    academyRank = 1,
                    totalExp   = 0
                },

                trio = new TrioSaveData
                {
                    synchronizationLevel = 0f,
                    totalActivations     = 0,
                    activationRoomIds    = new List<string>(),
                    lumiUnlocked         = false,
                    nelaUnlocked         = false
                },

                world = new WorldSaveData
                {
                    currentSeason                = "Autumn",
                    currentTimeOfDay             = "Morning",
                    currentWeather               = "Clear",
                    dayCount                     = 1,
                    worldPerceptionLevel         = 0f,
                    hiddenSystemPartiallyRevealed = false,
                    globalEventsTriggered        = new List<string>()
                },

                symbols        = new List<SymbolSaveData>(),
                notebookEntries = new List<NotebookEntrySaveData>(),
                rooms          = new List<RoomSaveData>(),
                echoes         = new List<EchoSaveData>(),
                decisionsLog   = new List<string>()
            };
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        private void MarkDirty()
        {
            isDirty = true;
            autoSaveTimer = 0f; // reset auto-save countdown on any change
        }

        private void Log(string msg)
        {
            if (verboseLogging) Debug.Log($"[SaveManager] {msg}");
        }

        private void OnApplicationQuit()
        {
            if (isDirty) SaveGame();
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused && isDirty) SaveGame();
        }
    }
}