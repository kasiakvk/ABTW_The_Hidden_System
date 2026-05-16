using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ABTW.Core;

namespace ABTW.Systems
{
    /// <summary>
    /// Memory Walk — player revisits past events.
    /// Cannot change decisions. Cannot unlock missed paths fully.
    /// Can only observe, understand, and gain Stability.
    /// "The world does not reset. You learn to see more."
    /// </summary>
    public class MemoryWalkSystem : MonoBehaviour
    {
        [Header("— SETTINGS —")]
        [SerializeField] private float transitionDuration = 1.2f;
        [SerializeField] private float memoryTimeScale    = 0.5f;   // slow motion
        [SerializeField] private float desaturationAmount = 0.6f;   // how grey memory looks

        [Header("— MEMORY SCENE —")]
        [SerializeField] private GameObject memorySceneRoot;        // assign in Inspector
        [SerializeField] private Light[]    memoryLights;           // desaturated lights
        [SerializeField] private Color      memoryLightColor = new Color(0.7f, 0.75f, 0.9f);

        // Recorded memory snapshots
        private readonly List<MemorySnapshot> _memories = new();

        // State
        private bool            _isActive;
        private MemorySnapshot  _currentMemory;
        private GlitchSystem    _glitch;
        private PlayerMovement  _player;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            _glitch = FindObjectOfType<GlitchSystem>();
            _player = FindObjectOfType<PlayerMovement>();

            if (memorySceneRoot != null)
                memorySceneRoot.SetActive(false);
        }

        private void Update()
        {
            // M key to enter/exit Memory Walk
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (_isActive) ExitMemoryWalk();
                else if (_memories.Count > 0) EnterMemoryWalk(_memories[^1]);
            }
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Record a memory snapshot when a pattern is resolved.</summary>
        public void RecordMemory(PatternData pattern, Vector3 playerPosition,
                                 Quaternion playerRotation)
        {
            var snapshot = new MemorySnapshot
            {
                pattern         = pattern,
                playerPosition  = playerPosition,
                playerRotation  = playerRotation,
                timestamp       = Time.time,
                sceneStateName  = pattern.name
            };
            _memories.Add(snapshot);
            Debug.Log($"[Memory] Snapshot recorded: {pattern.name}");
        }

        /// <summary>Enter Memory Walk for a specific memory.</summary>
        public void EnterMemoryWalk(MemorySnapshot memory)
        {
            if (_isActive) return;
            _currentMemory = memory;
            StartCoroutine(EnterRoutine());
        }

        /// <summary>Exit Memory Walk and return to present.</summary>
        public void ExitMemoryWalk()
        {
            if (!_isActive) return;
            StartCoroutine(ExitRoutine());
        }

        public void SetActive(bool active)
        {
            if (active && _memories.Count > 0) EnterMemoryWalk(_memories[^1]);
            else if (!active) ExitMemoryWalk();
        }

        public bool IsActive => _isActive;
        public IReadOnlyList<MemorySnapshot> AllMemories => _memories;

        // ── Private ──────────────────────────────────────────────────────────

        private IEnumerator EnterRoutine()
        {
            _isActive = true;
            GameManager.Instance?.SetMemoryWalk(true);

            // Notify UI
            GameManager.Instance?.uiManager?.ShowNotification(
                "Memory Reconstruction\nThis is not the original moment.",
                NotificationType.Memory);

            // Transition: slow time + desaturate
            if (_glitch != null)
                yield return StartCoroutine(
                    _glitch.MemoryWalkTransition(entering: true, transitionDuration));

            // Activate memory scene
            if (memorySceneRoot != null)
                memorySceneRoot.SetActive(true);

            // Desaturate lights
            ApplyMemoryLighting(true);

            // Move player to memory position (ghost version)
            if (_player != null)
            {
                _player.SetMemoryWalk(true);
                _player.transform.position = _currentMemory.playerPosition;
                _player.transform.rotation = _currentMemory.playerRotation;
            }

            // Spawn ghost anomaly (distorted, non-interactive)
            SpawnGhostAnomaly(_currentMemory);

            Debug.Log($"[Memory] Entered: {_currentMemory.sceneStateName}");
        }

        private IEnumerator ExitRoutine()
        {
            // Transition back
            if (_glitch != null)
                yield return StartCoroutine(
                    _glitch.MemoryWalkTransition(entering: false, transitionDuration));

            // Restore
            ApplyMemoryLighting(false);
            if (memorySceneRoot != null)
                memorySceneRoot.SetActive(false);

            _player?.SetMemoryWalk(false);
            _isActive = false;

            GameManager.Instance?.SetMemoryWalk(false);
            // Stability always gained from Memory Walk
            GameManager.Instance?.playerStats.RaiseStat(StatType.Stability, 3f);

            Debug.Log("[Memory] Exited — +Stability");
        }

        private void ApplyMemoryLighting(bool memoryMode)
        {
            if (memoryLights == null) return;
            foreach (var light in memoryLights)
            {
                if (light == null) continue;
                light.color = memoryMode ? memoryLightColor : Color.white;
                light.intensity *= memoryMode ? (1f - desaturationAmount * 0.3f) : 1f;
            }
        }

        private void SpawnGhostAnomaly(MemorySnapshot memory)
        {
            // Create a simple ghost representation of the past anomaly
            // In full production: instantiate the actual anomaly prefab with ghost shader
            Debug.Log($"[Memory] Ghost anomaly: {memory.pattern.anomalyType} " +
                      $"at {memory.playerPosition} — non-interactive");
        }
    }

    // ── Data ─────────────────────────────────────────────────────────────────

    [System.Serializable]
    public class MemorySnapshot
    {
        public PatternData pattern;
        public Vector3     playerPosition;
        public Quaternion  playerRotation;
        public float       timestamp;
        public string      sceneStateName;
    }
}