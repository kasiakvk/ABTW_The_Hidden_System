using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ABTW.Anomalies;

namespace ABTW.Systems
{
    /// <summary>
    /// AnomalySpawner — Dynamic anomaly placement system.
    ///
    /// Reads WorldManager state (time/weather/season) and adjusts:
    ///   - spawn rate via GetAnomalyMultiplier()
    ///   - anomaly type weights (Night → more ShadowDelay, Rain → more SoundEcho)
    ///
    /// Spawn points are defined as child transforms named "SpawnPoint_XX".
    /// </summary>
    public class AnomalySpawner : MonoBehaviour
    {
        // ── Inspector ────────────────────────────────────────────────────────

        [Header("— SPAWN SETTINGS —")]
        [SerializeField] private float  baseSpawnInterval = 45f;
        [SerializeField] private int    maxActiveAnomalies = 4;
        [SerializeField] private bool   spawnOnStart = true;

        [Header("— PREFABS —")]
        [SerializeField] private GameObject shadowDelayPrefab;
        [SerializeField] private GameObject lightInstabilityPrefab;
        [SerializeField] private GameObject symbolFragmentPrefab;
        [SerializeField] private GameObject reflectionMismatchPrefab;
        [SerializeField] private GameObject soundEchoPrefab;

        [Header("— SPAWN POINTS —")]
        [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

        // ── State ────────────────────────────────────────────────────────────

        private List<GameObject> _activeAnomalies = new List<GameObject>();
        private Coroutine        _spawnRoutine;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Start()
        {
            // Auto-collect child spawn points if none assigned
            if (spawnPoints.Count == 0)
                CollectSpawnPoints();

            if (spawnOnStart)
                SpawnInitialAnomalies();

            _spawnRoutine = StartCoroutine(SpawnLoop());
        }

        // ── Public ───────────────────────────────────────────────────────────

        public void ForceSpawn(AnomalyType type)
        {
            var point = GetRandomSpawnPoint();
            if (point == null) return;
            SpawnAnomaly(type, point.position);
        }

        public void ClearAllAnomalies()
        {
            foreach (var a in _activeAnomalies)
                if (a != null) Destroy(a);
            _activeAnomalies.Clear();
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void CollectSpawnPoints()
        {
            foreach (Transform child in transform)
                if (child.name.StartsWith("SpawnPoint"))
                    spawnPoints.Add(child);
        }

        private void SpawnInitialAnomalies()
        {
            // Always start with ShadowDelay — Chapter 1 GDD requirement
            var firstPoint = spawnPoints.Count > 0 ? spawnPoints[0] : transform;
            SpawnAnomaly(AnomalyType.ShadowDelay, firstPoint.position);
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                float multiplier = WorldManager.Instance != null
                    ? WorldManager.Instance.GetAnomalyMultiplier()
                    : 1f;

                float interval = baseSpawnInterval / multiplier;
                yield return new WaitForSeconds(interval);

                CleanupDestroyedAnomalies();

                if (_activeAnomalies.Count < maxActiveAnomalies)
                {
                    var type  = GetWeightedRandomType();
                    var point = GetRandomSpawnPoint();
                    if (point != null)
                        SpawnAnomaly(type, point.position);
                }
            }
        }

        private void SpawnAnomaly(AnomalyType type, Vector3 position)
        {
            GameObject prefab = GetPrefabForType(type);
            if (prefab == null)
            {
                // Fallback: create primitive placeholder
                prefab = CreatePlaceholderPrefab(type);
            }

            var instance = Instantiate(prefab, position, Quaternion.identity, transform);
            _activeAnomalies.Add(instance);
            Debug.Log($"[Spawner] Spawned {type} at {position}");
        }

        private GameObject GetPrefabForType(AnomalyType type)
        {
            switch (type)
            {
                case AnomalyType.ShadowDelay:         return shadowDelayPrefab;
                case AnomalyType.LightInstability:    return lightInstabilityPrefab;
                case AnomalyType.SymbolFragment:      return symbolFragmentPrefab;
                case AnomalyType.ReflectionMismatch:  return reflectionMismatchPrefab;
                case AnomalyType.SoundEcho:           return soundEchoPrefab;
                default:                              return shadowDelayPrefab;
            }
        }

        private GameObject CreatePlaceholderPrefab(AnomalyType type)
        {
            // Runtime placeholder when prefab not assigned
            var go = new GameObject($"Anomaly_{type}");

            switch (type)
            {
                case AnomalyType.ShadowDelay:
                    go.AddComponent<ShadowDelayAnomaly>();
                    break;
                case AnomalyType.LightInstability:
                    go.AddComponent<LightInstabilityAnomaly>();
                    break;
                case AnomalyType.SymbolFragment:
                    go.AddComponent<SymbolFragmentAnomaly>();
                    break;
                case AnomalyType.ReflectionMismatch:
                    go.AddComponent<ReflectionMismatchAnomaly>();
                    break;
                case AnomalyType.SoundEcho:
                    go.AddComponent<SoundEchoAnomaly>();
                    break;
            }

            return go;
        }

        /// <summary>
        /// Weighted random anomaly type based on current world state.
        /// Night → ShadowDelay more likely.
        /// Rain  → SoundEcho more likely.
        /// </summary>
        private AnomalyType GetWeightedRandomType()
        {
            var world = WorldManager.Instance;

            // Build weight table
            var weights = new Dictionary<AnomalyType, float>
            {
                { AnomalyType.ShadowDelay,        1.0f },
                { AnomalyType.LightInstability,   1.0f },
                { AnomalyType.SymbolFragment,     0.6f },
                { AnomalyType.ReflectionMismatch, 0.8f },
                { AnomalyType.SoundEcho,          0.7f }
            };

            if (world != null)
            {
                // Night boosts shadow anomalies
                if (world.CurrentTime == TimeOfDay.Night)
                    weights[AnomalyType.ShadowDelay] += 1.5f;

                // Rain boosts sound echo
                if (world.CurrentWeather == WeatherType.Rain)
                    weights[AnomalyType.SoundEcho] += 1.2f;

                // Autumn boosts memory/symbol traces
                if (world.CurrentSeason == Season.Autumn)
                    weights[AnomalyType.SymbolFragment] += 0.8f;

                // Fog reduces reflection visibility
                if (world.CurrentWeather == WeatherType.Fog)
                    weights[AnomalyType.ReflectionMismatch] -= 0.4f;
            }

            // Weighted random selection
            float total = 0f;
            foreach (var w in weights.Values) total += Mathf.Max(0f, w);

            float roll = Random.Range(0f, total);
            float cumulative = 0f;

            foreach (var kvp in weights)
            {
                cumulative += Mathf.Max(0f, kvp.Value);
                if (roll <= cumulative)
                    return kvp.Key;
            }

            return AnomalyType.ShadowDelay;
        }

        private Transform GetRandomSpawnPoint()
        {
            if (spawnPoints.Count == 0) return transform;
            return spawnPoints[Random.Range(0, spawnPoints.Count)];
        }

        private void CleanupDestroyedAnomalies()
        {
            _activeAnomalies.RemoveAll(a => a == null);
        }
    }
}