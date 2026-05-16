using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ABTW.Core;

namespace ABTW.Systems
{
    /// <summary>
    /// Manages Perception Mode — the core mechanic of ABTW.
    /// When active: world slows slightly, anomalies become detectable,
    /// camera breathes. No flashy effects — only attention.
    /// </summary>
    public class PerceptionSystem : MonoBehaviour
    {
        [Header("— SETTINGS —")]
        [SerializeField] private float detectionRadius    = 8f;
        [SerializeField] private float cameraBreathAmount = 0.02f;  // subtle zoom
        [SerializeField] private float cameraBreathSpeed  = 1.2f;
        [SerializeField] private LayerMask anomalyLayer;

        [Header("— VISUAL —")]
        [SerializeField] private Material perceptionOverlayMat;     // slight desaturation
        [SerializeField] private float    desaturationTarget = 0.35f;

        // State
        private bool    _isActive;
        private float   _activeTimer;
        private float   _maxDuration;
        private Camera  _cam;
        private float   _baseFOV;
        private float   _breathPhase;

        // Detected anomalies this session
        private readonly List<AnomalyBase> _detectedThisSession = new();

        // ── Unity ────────────────────────────────────────────────────────────

        private void Awake()
        {
            _cam     = Camera.main;
            _baseFOV = _cam != null ? _cam.fieldOfView : 60f;
        }

        private void Update()
        {
            // Toggle with P key (or via GameManager)
            if (Input.GetKeyDown(KeyCode.P))
                Toggle();

            if (!_isActive) return;

            _activeTimer += Time.deltaTime;
            HandleCameraBreath();
            ScanForAnomalies();

            // Auto-deactivate when duration expires
            if (_activeTimer >= _maxDuration)
                SetActive(false);
        }

        // ── Public API ───────────────────────────────────────────────────────

        public void SetActive(bool active)
        {
            _isActive = active;

            if (active)
            {
                _activeTimer = 0f;
                _maxDuration = GameManager.Instance != null
                    ? GameManager.Instance.playerStats.PerceptionModeMaxDuration
                    : 10f;

                // Slow player slightly
                var pm = FindObjectOfType<PlayerMovement>();
                pm?.SetPerceiving(true);

                Debug.Log($"[Perception] Mode ON — max {_maxDuration:F1}s");
            }
            else
            {
                // Restore camera FOV
                if (_cam != null) _cam.fieldOfView = _baseFOV;
                _breathPhase = 0f;

                var pm = FindObjectOfType<PlayerMovement>();
                pm?.SetPerceiving(false);

                Debug.Log("[Perception] Mode OFF");
            }
        }

        public void Toggle() => SetActive(!_isActive);

        public bool IsActive => _isActive;

        // ── Private ──────────────────────────────────────────────────────────

        private void HandleCameraBreath()
        {
            if (_cam == null) return;
            _breathPhase += Time.deltaTime * cameraBreathSpeed;
            float breath = Mathf.Sin(_breathPhase) * cameraBreathAmount;
            _cam.fieldOfView = _baseFOV - breath * 3f; // subtle ±0.06° equivalent
        }

        private void ScanForAnomalies()
        {
            Collider[] hits = Physics.OverlapSphere(
                transform.position, detectionRadius, anomalyLayer);

            foreach (var hit in hits)
            {
                var anomaly = hit.GetComponent<AnomalyBase>();
                if (anomaly == null || _detectedThisSession.Contains(anomaly)) continue;

                float dist = Vector3.Distance(transform.position, hit.transform.position);
                float awarenessBonus = GameManager.Instance != null
                    ? GameManager.Instance.playerStats.awareness / 100f
                    : 0f;

                // Higher awareness = detect from further away
                float effectiveRadius = detectionRadius * (0.5f + awarenessBonus * 0.5f);
                if (dist <= effectiveRadius)
                {
                    anomaly.OnPerceptionDetected();
                    _detectedThisSession.Add(anomaly);
                    GameManager.Instance?.OnAnomalyFound(anomaly.AnomalyType, anomaly.EXPReward);

                    // Trigger notice animation on player
                    FindObjectOfType<PlayerMovement>()?.PlayNoticeAnimation();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.5f, 0.2f, 1f, 0.25f);
            Gizmos.DrawSphere(transform.position, detectionRadius);
        }
    }
}