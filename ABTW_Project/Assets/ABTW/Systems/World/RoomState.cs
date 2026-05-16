using UnityEngine;
using System.Collections.Generic;
using ABTW.Core;

namespace ABTW.Systems
{
    /// <summary>
    /// RoomState — tracks the complete living state of a room.
    /// Continuity tracking: what happened here, who saw what, symbol states.
    /// Used by SaveSystem and Memory Walk.
    /// </summary>
    public class RoomState : MonoBehaviour
    {
        [System.Serializable]
        public class SymbolRecord
        {
            public string symbolID;
            public string lastState;
            public float lastSeen;
            public bool wasRecorded;
            public bool wasEchoed;
        }

        [System.Serializable]
        public class EventRecord
        {
            public string eventID;
            public string description;
            public float timestamp;
            public string triggeredBy; // "Astra", "Lumi", "Nela", "Trio"
        }

        [System.Serializable]
        public class RoomSaveData
        {
            public string roomID;
            public string roomName;
            public float perceptionLevel;
            public bool hiddenLayerRevealed;
            public int playerVisitCount;
            public float totalTimeSpent;
            public List<SymbolRecord> symbolRecords;
            public List<EventRecord> eventHistory;
            public string lastVisitedTime;
        }

        [Header("Room Identity")]
        public string roomID = "ROOM_001";
        public string roomName = "East Corridor";

        [Header("Visit Tracking")]
        [SerializeField] private int playerVisitCount = 0;
        [SerializeField] private float totalTimeSpent = 0f;
        [SerializeField] private bool playerCurrentlyPresent = false;
        private float visitStartTime = 0f;

        [Header("State Tracking")]
        [SerializeField] private float perceptionLevel = 0f;
        [SerializeField] private bool hiddenLayerRevealed = false;
        [SerializeField] private bool trioActivatedHere = false;

        [Header("Symbol Tracking")]
        public SymbolStateFlow[] trackedSymbols;
        private List<SymbolRecord> symbolRecords = new List<SymbolRecord>();

        [Header("Event History")]
        private List<EventRecord> eventHistory = new List<EventRecord>();
        public int maxEventHistory = 20;

        [Header("References")]
        public EnvironmentDNA environmentDNA;
        public LightingManager lightingManager;

        // Continuity flags
        private Dictionary<string, bool> continuityFlags = new Dictionary<string, bool>();

        private void Awake()
        {
            if (environmentDNA == null)
                environmentDNA = GetComponent<EnvironmentDNA>();

            InitializeSymbolRecords();
        }

        private void Update()
        {
            if (playerCurrentlyPresent)
            {
                totalTimeSpent += Time.deltaTime;
                SyncFromEnvironmentDNA();
            }
        }

        // ─── Visit Tracking ──────────────────────────────────────────────────

        public void OnPlayerEnter()
        {
            playerCurrentlyPresent = true;
            playerVisitCount++;
            visitStartTime = Time.time;

            LogEvent("PLAYER_ENTER", $"Visit #{playerVisitCount}", "Astra");
            Debug.Log($"[RoomState] {roomName} — Player entered (visit #{playerVisitCount})");
        }

        public void OnPlayerExit()
        {
            playerCurrentlyPresent = false;
            float sessionTime = Time.time - visitStartTime;
            LogEvent("PLAYER_EXIT", $"Time spent: {sessionTime:F1}s", "Astra");
            Debug.Log($"[RoomState] {roomName} — Player exited after {sessionTime:F1}s");
        }

        // ─── State Sync ──────────────────────────────────────────────────────

        private void SyncFromEnvironmentDNA()
        {
            if (environmentDNA == null) return;

            perceptionLevel = environmentDNA.PerceptionLevel;

            if (environmentDNA.IsHiddenLayerRevealed && !hiddenLayerRevealed)
            {
                hiddenLayerRevealed = true;
                LogEvent("HIDDEN_LAYER_REVEALED", "Hidden layer became visible", "Trio");
            }
        }

        // ─── Symbol Records ──────────────────────────────────────────────────

        private void InitializeSymbolRecords()
        {
            symbolRecords.Clear();
            if (trackedSymbols == null) return;

            foreach (var symbol in trackedSymbols)
            {
                if (symbol == null) continue;
                symbolRecords.Add(new SymbolRecord
                {
                    symbolID = symbol.symbolID,
                    lastState = symbol.CurrentState.ToString(),
                    lastSeen = -1f,
                    wasRecorded = false,
                    wasEchoed = false
                });
            }
        }

        public void UpdateSymbolRecord(string symbolID, SymbolStateFlow.SymbolState newState)
        {
            var record = symbolRecords.Find(r => r.symbolID == symbolID);
            if (record == null)
            {
                record = new SymbolRecord { symbolID = symbolID };
                symbolRecords.Add(record);
            }

            record.lastState = newState.ToString();
            record.lastSeen = Time.time;

            if (newState == SymbolStateFlow.SymbolState.Resolved)
            {
                record.wasRecorded = true;
                LogEvent("SYMBOL_RECORDED", $"Symbol {symbolID} recorded", "Astra");
            }
            else if (newState == SymbolStateFlow.SymbolState.Echo)
            {
                record.wasEchoed = true;
                LogEvent("SYMBOL_ECHO", $"Symbol {symbolID} became echo", "Astra");
            }
        }

        // ─── Event Logging ───────────────────────────────────────────────────

        public void LogEvent(string eventID, string description, string triggeredBy = "System")
        {
            if (eventHistory.Count >= maxEventHistory)
                eventHistory.RemoveAt(0);

            eventHistory.Add(new EventRecord
            {
                eventID = eventID,
                description = description,
                timestamp = Time.time,
                triggeredBy = triggeredBy
            });
        }

        public void LogTrioActivation()
        {
            trioActivatedHere = true;
            LogEvent("TRIO_ACTIVATION", "Trio synchronized in this room", "Trio");
        }

        // ─── Continuity Flags ────────────────────────────────────────────────

        public void SetFlag(string flagID, bool value)
        {
            continuityFlags[flagID] = value;
        }

        public bool GetFlag(string flagID)
        {
            return continuityFlags.TryGetValue(flagID, out bool val) && val;
        }

        // ─── Save / Load ─────────────────────────────────────────────────────

        public RoomSaveData GetSaveData()
        {
            // Sync symbol states before saving
            if (trackedSymbols != null)
            {
                foreach (var symbol in trackedSymbols)
                {
                    if (symbol != null)
                        UpdateSymbolRecord(symbol.symbolID, symbol.CurrentState);
                }
            }

            return new RoomSaveData
            {
                roomID = roomID,
                roomName = roomName,
                perceptionLevel = perceptionLevel,
                hiddenLayerRevealed = hiddenLayerRevealed,
                playerVisitCount = playerVisitCount,
                totalTimeSpent = totalTimeSpent,
                symbolRecords = new List<SymbolRecord>(symbolRecords),
                eventHistory = new List<EventRecord>(eventHistory),
                lastVisitedTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        public void LoadSaveData(RoomSaveData data)
        {
            if (data == null) return;

            perceptionLevel = data.perceptionLevel;
            hiddenLayerRevealed = data.hiddenLayerRevealed;
            playerVisitCount = data.playerVisitCount;
            totalTimeSpent = data.totalTimeSpent;
            symbolRecords = data.symbolRecords ?? new List<SymbolRecord>();
            eventHistory = data.eventHistory ?? new List<EventRecord>();

            // Restore symbol states
            if (trackedSymbols != null)
            {
                foreach (var symbol in trackedSymbols)
                {
                    if (symbol == null) continue;
                    var record = symbolRecords.Find(r => r.symbolID == symbol.symbolID);
                    if (record != null)
                        symbol.LoadSaveState(record.lastState);
                }
            }

            // Restore environment DNA perception level
            if (environmentDNA != null)
                environmentDNA.ReceivePerception(perceptionLevel * 10f);

            Debug.Log($"[RoomState] {roomName} — state loaded (visits: {playerVisitCount})");
        }

        // ─── Public Accessors ────────────────────────────────────────────────

        public int VisitCount => playerVisitCount;
        public float TimeSpent => totalTimeSpent;
        public bool HiddenLayerRevealed => hiddenLayerRevealed;
        public bool TrioActivatedHere => trioActivatedHere;
        public float PerceptionLevel => perceptionLevel;
        public List<EventRecord> EventHistory => eventHistory;
        public List<SymbolRecord> SymbolRecords => symbolRecords;

        // ─── Trigger Zone ────────────────────────────────────────────────────

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                OnPlayerEnter();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                OnPlayerExit();
        }

        // ─── Debug ───────────────────────────────────────────────────────────

        [ContextMenu("Print Room State")]
        public void PrintRoomState()
        {
            Debug.Log($"=== ROOM STATE: {roomName} ===\n" +
                      $"Visits: {playerVisitCount}\n" +
                      $"Time Spent: {totalTimeSpent:F1}s\n" +
                      $"Perception: {perceptionLevel:F2}\n" +
                      $"Hidden Layer: {hiddenLayerRevealed}\n" +
                      $"Trio Activated: {trioActivatedHere}\n" +
                      $"Symbols Tracked: {symbolRecords.Count}\n" +
                      $"Events Logged: {eventHistory.Count}");
        }
    }
}