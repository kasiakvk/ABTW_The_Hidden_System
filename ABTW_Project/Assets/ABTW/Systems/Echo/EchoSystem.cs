using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ABTW.Core;

namespace ABTW.Systems
{
    /// <summary>
    /// Echo System — the world does not reset, it echoes.
    /// When a path is closed by early recording, an Echo version
    /// appears later: weaker, incomplete, but present.
    /// "You never fully lose content — you see what was there."
    /// </summary>
    public class EchoSystem : MonoBehaviour
    {
        [Header("— ECHO PREFABS —")]
        [SerializeField] private GameObject shadowEchoPrefab;
        [SerializeField] private GameObject symbolEchoPrefab;
        [SerializeField] private GameObject soundEchoPrefab;

        [Header("— SETTINGS —")]
        [SerializeField] private float echoVisibilityDuration = 8f;   // how long echo stays
        [SerializeField] private float echoAlpha              = 0.4f;  // translucent

        // Scheduled echoes: pattern → world time when it should appear
        private readonly List<ScheduledEcho> _scheduled = new();
        private GlitchSystem _glitch;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            _glitch = FindObjectOfType<GlitchSystem>();
        }

        private void Update()
        {
            CheckScheduledEchoes();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Schedule an echo to appear after a delay.</summary>
        public void ScheduleEcho(PatternData pattern, float delay)
        {
            if (pattern.echoScheduled) return;
            pattern.echoScheduled = true;

            _scheduled.Add(new ScheduledEcho
            {
                pattern    = pattern,
                triggerTime = Time.time + delay
            });

            Debug.Log($"[Echo] Scheduled: {pattern.name} in {delay}s");
        }

        /// <summary>Immediately spawn an echo at a world position.</summary>
        public void SpawnEcho(PatternData pattern, Vector3 worldPosition)
        {
            StartCoroutine(SpawnEchoRoutine(pattern, worldPosition));
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void CheckScheduledEchoes()
        {
            for (int i = _scheduled.Count - 1; i >= 0; i--)
            {
                var se = _scheduled[i];
                if (Time.time >= se.triggerTime)
                {
                    _scheduled.RemoveAt(i);
                    TriggerEcho(se.pattern);
                }
            }
        }

        private void TriggerEcho(PatternData pattern)
        {
            // Find a suitable spawn position near the player
            Vector3 playerPos = FindObjectOfType<PlayerMovement>()?.transform.position
                                ?? Vector3.zero;
            Vector3 spawnPos  = playerPos + Random.insideUnitSphere * 4f;
            spawnPos.y        = playerPos.y;

            SpawnEcho(pattern, spawnPos);

            // Add echo entry to notebook
            var notebook = GameManager.Instance?.notebookSystem;
            notebook?.MarkAsEcho(pattern);

            // Notify UI
            GameManager.Instance?.uiManager?.ShowNotification(
                "Echo Detected — a trace of something recorded too early.",
                NotificationType.Echo);

            Debug.Log($"[Echo] Triggered: {pattern.name}");
        }

        private IEnumerator SpawnEchoRoutine(PatternData pattern, Vector3 position)
        {
            // Choose prefab based on anomaly type
            GameObject prefab = pattern.anomalyType switch
            {
                AnomalyType.ShadowDelay        => shadowEchoPrefab,
                AnomalyType.SymbolFragment      => symbolEchoPrefab,
                AnomalyType.SoundEcho           => soundEchoPrefab,
                _                               => shadowEchoPrefab
            };

            if (prefab == null)
            {
                Debug.LogWarning($"[Echo] No prefab for {pattern.anomalyType}");
                yield break;
            }

            // Spawn
            GameObject echo = Instantiate(prefab, position, Quaternion.identity);
            echo.name = $"Echo_{pattern.name}";

            // Make it translucent
            var renderers = echo.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                // Set material to transparent mode
                foreach (var mat in r.materials)
                {
                    mat.SetFloat("_Mode", 3);
                    Color c = mat.color;
                    mat.color = new Color(c.r, c.g, c.b, echoAlpha);
                }
            }

            // Add EchoMarker component so player can identify it
            var marker = echo.AddComponent<EchoMarker>();
            marker.sourcePattern = pattern;

            // Play appearance animation
            var mainRenderer = echo.GetComponentInChildren<Renderer>();
            if (_glitch != null && mainRenderer != null)
                yield return StartCoroutine(
                    _glitch.EchoAppearance(mainRenderer, echoVisibilityDuration));
            else
                yield return new WaitForSeconds(echoVisibilityDuration);

            Destroy(echo);
        }

        // ── Data ─────────────────────────────────────────────────────────────

        private class ScheduledEcho
        {
            public PatternData pattern;
            public float       triggerTime;
        }
    }

    /// <summary>Marker component placed on Echo GameObjects for identification.</summary>
    public class EchoMarker : MonoBehaviour
    {
        public PatternData sourcePattern;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            // Player touched the echo — give small EXP and Stability
            GameManager.Instance?.playerStats.AddEXP(20);
            GameManager.Instance?.playerStats.RaiseStat(StatType.Stability, 1f);
            GameManager.Instance?.uiManager?.ShowNotification(
                $"Echo: {sourcePattern?.name ?? "Unknown"}\n" +
                "This feels incomplete. A pattern ended too early.",
                NotificationType.Echo);

            Debug.Log($"[Echo] Player touched echo: {sourcePattern?.name}");
        }
    }
}