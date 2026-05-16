using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using ABTW.Systems;

namespace ABTW.Core
{
    /// <summary>
    /// SaveSystem — Persistent save/load for ABTW.
    ///
    /// Saves:
    ///   - Player stats (5 stats + rank + EXP)
    ///   - Recorded patterns list
    ///   - World state (time, weather, season)
    ///   - Chapter progress
    ///   - Echo flags (which paths were closed early)
    ///
    /// Format: JSON → Application.persistentDataPath/abtw_save.json
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        // ── Singleton ────────────────────────────────────────────────────────

        public static SaveSystem Instance { get; private set; }

        // ── Save Data Structure ──────────────────────────────────────────────

        [Serializable]
        public class SaveData
        {
            // Stats
            public float awareness;
            public float clarity;
            public float connection;
            public float timing;
            public float stability;
            public int   rank;
            public int   exp;

            // Progress
            public int   currentChapter;
            public int   anomaliesFound;
            public int   patternsRecorded;

            // Recorded patterns
            public List<string> recordedPatterns = new List<string>();
            public List<string> echoFlags        = new List<string>();

            // World state
            public string timeOfDay;
            public string weather;
            public string season;

            // Meta
            public string saveTimestamp;
            public string gameVersion = "0.1";
        }

        // ── Inspector ────────────────────────────────────────────────────────

        [Header("— SETTINGS —")]
        [SerializeField] private bool autoSaveEnabled  = true;
        [SerializeField] private float autoSaveInterval = 120f;  // seconds
        [SerializeField] private string saveFileName    = "abtw_save.json";

        // ── State ────────────────────────────────────────────────────────────

        private string   _savePath;
        private SaveData _currentSave;
        private float    _autoSaveTimer;

        // ── Events ───────────────────────────────────────────────────────────

        public event Action OnSaved;
        public event Action OnLoaded;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _savePath = Path.Combine(Application.persistentDataPath, saveFileName);
            Debug.Log($"[Save] Save path: {_savePath}");
        }

        private void Update()
        {
            if (!autoSaveEnabled) return;
            _autoSaveTimer += Time.deltaTime;
            if (_autoSaveTimer >= autoSaveInterval)
            {
                _autoSaveTimer = 0f;
                Save();
            }
        }

        // ── Public API ───────────────────────────────────────────────────────

        public void Save()
        {
            _currentSave = BuildSaveData();

            try
            {
                string json = JsonUtility.ToJson(_currentSave, prettyPrint: true);
                File.WriteAllText(_savePath, json);
                PlayerPrefs.SetInt("ABTW_SaveExists", 1);
                PlayerPrefs.Save();
                OnSaved?.Invoke();
                Debug.Log("[Save] Game saved.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Save] Failed to save: {e.Message}");
            }
        }

        public bool Load()
        {
            if (!File.Exists(_savePath))
            {
                Debug.Log("[Save] No save file found.");
                return false;
            }

            try
            {
                string json = File.ReadAllText(_savePath);
                _currentSave = JsonUtility.FromJson<SaveData>(json);
                ApplySaveData(_currentSave);
                OnLoaded?.Invoke();
                Debug.Log("[Save] Game loaded.");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Save] Failed to load: {e.Message}");
                return false;
            }
        }

        public bool SaveExists() => File.Exists(_savePath);

        public void DeleteSave()
        {
            if (File.Exists(_savePath))
                File.Delete(_savePath);
            PlayerPrefs.DeleteKey("ABTW_SaveExists");
            Debug.Log("[Save] Save deleted.");
        }

        // ── Private — Build ──────────────────────────────────────────────────

        private SaveData BuildSaveData()
        {
            var data = new SaveData
            {
                saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            // Stats
            var stats = GameManager.Instance?.playerStats;
            if (stats != null)
            {
                data.awareness  = stats.Awareness;
                data.clarity    = stats.Clarity;
                data.connection = stats.Connection;
                data.timing     = stats.Timing;
                data.stability  = stats.Stability;
                data.rank       = stats.CurrentRank;
                data.exp        = stats.CurrentEXP;
            }

            // Progress
            var gm = GameManager.Instance;
            if (gm != null)
            {
                data.anomaliesFound   = gm.AnomaliesFound;
                data.patternsRecorded = gm.PatternsRecorded;
                data.currentChapter   = 1;
            }

            // Patterns
            var nb = GameManager.Instance?.notebookSystem;
            if (nb != null)
            {
                foreach (var pattern in nb.GetAllPatterns())
                {
                    if (pattern.status == PatternStatus.Recorded)
                        data.recordedPatterns.Add(pattern.anomalyType.ToString());
                    if (pattern.status == PatternStatus.Echo)
                        data.echoFlags.Add(pattern.anomalyType.ToString());
                }
            }

            // World state
            var world = WorldManager.Instance;
            if (world != null)
            {
                data.timeOfDay = world.CurrentTime.ToString();
                data.weather   = world.CurrentWeather.ToString();
                data.season    = world.CurrentSeason.ToString();
            }

            return data;
        }

        private void ApplySaveData(SaveData data)
        {
            // Stats
            var stats = GameManager.Instance?.playerStats;
            if (stats != null)
            {
                stats.SetStat("Awareness",  data.awareness);
                stats.SetStat("Clarity",    data.clarity);
                stats.SetStat("Connection", data.connection);
                stats.SetStat("Timing",     data.timing);
                stats.SetStat("Stability",  data.stability);
                stats.SetRankDirectly(data.rank);
                stats.SetEXPDirectly(data.exp);
            }

            // World state
            var world = WorldManager.Instance;
            if (world != null)
            {
                if (Enum.TryParse<TimeOfDay>(data.timeOfDay, out var time))
                    world.SetTime(time);
                if (Enum.TryParse<WeatherType>(data.weather, out var weather))
                    world.SetWeather(weather);
                if (Enum.TryParse<Season>(data.season, out var season))
                    world.SetSeason(season);
            }

            Debug.Log($"[Save] Applied save: Rank {data.rank}, " +
                      $"Patterns recorded: {data.recordedPatterns.Count}");
        }
    }
}