using UnityEngine;
using ABTW.Systems;
using ABTW.Anomalies;
using ABTW.Companions;
using ABTW.UI;

namespace ABTW.Core
{
    /// <summary>
    /// Chapter 1 — East Corridor scene setup.
    /// Attach this to a SceneSetup GameObject in your Unity scene.
    /// It wires all systems together and places anomalies correctly.
    ///
    /// SETUP INSTRUCTIONS:
    /// 1. Create empty GameObject → name it "SceneSetup"
    /// 2. Attach this script
    /// 3. Assign all serialized fields in Inspector
    /// 4. Press Play
    /// </summary>
    public class SceneSetup : MonoBehaviour
    {
        [Header("═══ PLAYER ═══")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform  playerSpawnPoint;

        [Header("═══ COMPANIONS ═══")]
        [SerializeField] private GameObject lumiPrefab;
        [SerializeField] private GameObject nelaPrefab;

        [Header("═══ ANOMALIES — CHAPTER 1 ═══")]
        [Tooltip("Shadow Delay anomaly — place near corridor door")]
        [SerializeField] private GameObject shadowDelayPrefab;
        [SerializeField] private Transform  shadowDelaySpawn;

        [Tooltip("Light Instability — ceiling fixture")]
        [SerializeField] private GameObject lightInstabilityPrefab;
        [SerializeField] private Transform  lightInstabilitySpawn;

        [Header("═══ HIDDEN LAYER ═══")]
        [SerializeField] private GameObject hiddenLayerRoot;

        [Header("═══ SYSTEMS ═══")]
        [SerializeField] private GameManager    gameManagerPrefab;
        [SerializeField] private PlayerStats    playerStatsSO;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            SetupSystems();
            SpawnPlayer();
            SpawnCompanions();
            SpawnAnomalies();
            SetupHiddenLayer();

            Debug.Log("[SceneSetup] Chapter 1 — East Corridor ready.");
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void SetupSystems()
        {
            // Ensure GameManager exists (singleton)
            if (GameManager.Instance == null && gameManagerPrefab != null)
            {
                var gm = Instantiate(gameManagerPrefab);
                if (playerStatsSO != null)
                    gm.playerStats = playerStatsSO;
            }
        }

        private void SpawnPlayer()
        {
            if (playerPrefab == null) { Debug.LogWarning("[Setup] No player prefab!"); return; }

            Vector3 spawnPos = playerSpawnPoint != null
                ? playerSpawnPoint.position : Vector3.zero;

            var player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            player.name = "Astra";
            Debug.Log("[Setup] Player spawned.");
        }

        private void SpawnCompanions()
        {
            Vector3 basePos = playerSpawnPoint != null
                ? playerSpawnPoint.position : Vector3.zero;

            if (lumiPrefab != null)
            {
                var lumi = Instantiate(lumiPrefab,
                    basePos + new Vector3(-1.8f, 0f, -0.5f), Quaternion.identity);
                lumi.name = "Lumi";
            }

            if (nelaPrefab != null)
            {
                var nela = Instantiate(nelaPrefab,
                    basePos + new Vector3(1.8f, 0f, -0.5f), Quaternion.identity);
                nela.name = "Nela";
            }

            Debug.Log("[Setup] Companions spawned.");
        }

        private void SpawnAnomalies()
        {
            // Shadow Delay — first anomaly player encounters
            if (shadowDelayPrefab != null && shadowDelaySpawn != null)
            {
                var shadow = Instantiate(shadowDelayPrefab,
                    shadowDelaySpawn.position, shadowDelaySpawn.rotation);
                shadow.name = "Anomaly_ShadowDelay_01";
            }

            // Light Instability — second anomaly
            if (lightInstabilityPrefab != null && lightInstabilitySpawn != null)
            {
                var light = Instantiate(lightInstabilityPrefab,
                    lightInstabilitySpawn.position, lightInstabilitySpawn.rotation);
                light.name = "Anomaly_LightInstability_01";
            }

            Debug.Log("[Setup] Anomalies placed.");
        }

        private void SetupHiddenLayer()
        {
            // Hidden Layer starts inactive — Trio system activates it
            if (hiddenLayerRoot != null)
                hiddenLayerRoot.SetActive(false);

            // Wire to TrioSystem
            var trio = FindObjectOfType<TrioSystem>();
            // hiddenLayerRoot is assigned directly in TrioSystem inspector
        }
    }
}