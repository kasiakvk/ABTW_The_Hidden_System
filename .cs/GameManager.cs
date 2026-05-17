// ============================================================
// ABTW: The Hidden System
// Class: GameManager
// Central hub — connects all systems per Production Bible
// ============================================================
using System;
using System.Collections;
using UnityEngine;

namespace ABTW
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Core Systems")]
        public PerceptionSystem perception;
        public NotebookSystem   notebook;
        public GlitchSystem     glitch;
        public EchoSystem       echo;
        public MemoryWalkSystem memoryWalk;
        public TrioSystem       trio;
        public WorldManager     world;
        public AnomalySpawner   spawner;
        public SaveSystem       save;

        [Header("Player")]
        public PlayerStats playerStats;

        [Header("State")]
        public bool gameStarted = false;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.Log("[GameManager] Initialising ABTW systems...");
            InitialiseSystems();
        }

        private void InitialiseSystems()
        {
            world?.Init();
            playerStats?.Init();
            spawner?.StartSpawning();
            gameStarted = true;
            Debug.Log("[GameManager] All systems online. Game started.");
        }

        // ── Public API ──────────────────────────────────────────────
        public void OnAnomalyDetected(AnomalyBase anomaly)
        {
            Debug.Log($"[GameManager] Anomaly detected: {anomaly.anomalyId}");
            notebook?.NotifyAnomaly(anomaly);
        }

        public void OnPatternRecorded(string patternId)
        {
            Debug.Log($"[GameManager] Pattern recorded: {patternId}");
            playerStats?.AddClarity(10);
            playerStats?.AddExp(25);
            echo?.FlagForEcho(patternId);
            world?.UpdateWorldState();
            save?.SaveGame();
        }

        public void OnPatternWaited(string patternId)
        {
            Debug.Log($"[GameManager] Player waited on: {patternId}");
            playerStats?.AddTiming(5);
            glitch?.EvolveAnomaly(patternId);
            trio?.CheckActivation();
        }

        public void OnTrioActivated()
        {
            Debug.Log("[GameManager] TRIO ACTIVATION!");
            playerStats?.AddConnection(20);
            playerStats?.AddAwareness(10);
            world?.RevealHiddenLayer();
        }

        public void OnMemoryWalkEntered()
        {
            Debug.Log("[GameManager] Memory Walk entered.");
            playerStats?.AddStability(5);
        }
    }
}