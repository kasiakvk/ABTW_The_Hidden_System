using UnityEngine;
using System.Collections;
using ABTW.Systems;

namespace ABTW.Anomalies
{
    /// <summary>
    /// Reflection Mismatch Anomaly — Layer 1 anomaly.
    /// A reflective surface (mirror, window, puddle) shows a slightly
    /// different version of the scene — wrong angle, delayed, or missing objects.
    ///
    /// GDD: "shifted reflections" / "reflection instability"
    /// </summary>
    public class ReflectionMismatchAnomaly : AnomalyBase
    {
        [Header("Reflection Settings")]
        [SerializeField] private Renderer reflectionSurface;
        [SerializeField] private float    mismatchIntensity = 0.08f;   // UV offset amount
        [SerializeField] private float    flickerSpeed      = 2.5f;
        [SerializeField] private float    evolveIntensity   = 0.22f;

        [Header("Colors")]
        [SerializeField] private Color normalTint  = new Color(0.9f, 0.9f, 1.0f, 0.6f);
        [SerializeField] private Color glitchTint  = new Color(0.7f, 0.6f, 1.0f, 0.8f);  // violet

        private MaterialPropertyBlock _mpb;
        private Coroutine _flickerRoutine;
        private float _currentOffset;
        private bool  _evolved;

        // ── AnomalyBase ──────────────────────────────────────────────────────

        protected override void Awake()
        {
            base.Awake();
            anomalyType  = AnomalyType.ReflectionMismatch;
            anomalyLayer = 1;
            _mpb = new MaterialPropertyBlock();
        }

        protected override void OnDetected()
        {
            Debug.Log("[ReflectionMismatch] Detected — reflection offset begins");
            _flickerRoutine = StartCoroutine(FlickerRoutine(mismatchIntensity));

            GameManager.Instance?.uiManager?.ShowNotification(
                "The reflection… doesn't match.", NotificationType.Perception);
        }

        protected override void OnEvolve()
        {
            _evolved = true;
            Debug.Log("[ReflectionMismatch] Evolving — reflection shows alternate scene");

            if (_flickerRoutine != null) StopCoroutine(_flickerRoutine);
            _flickerRoutine = StartCoroutine(FlickerRoutine(evolveIntensity));

            GameManager.Instance?.uiManager?.ShowNotification(
                "The reflection is showing something else.\nSomething that isn't here.",
                NotificationType.Perception);
        }

        protected override void OnResolve()
        {
            if (_flickerRoutine != null) StopCoroutine(_flickerRoutine);
            StartCoroutine(RestoreRoutine());

            string note = _evolved
                ? "Reflection mismatch resolved after evolution. Alternate scene glimpsed."
                : "Reflection mismatch recorded. Surface stabilised.";

            GameManager.Instance?.uiManager?.ShowNotification(note, NotificationType.Record);
        }

        // ── Private ──────────────────────────────────────────────────────────

        private IEnumerator FlickerRoutine(float intensity)
        {
            float t = 0f;
            while (true)
            {
                t += Time.deltaTime * flickerSpeed;

                // Oscillating UV offset simulates reflection mismatch
                float offsetX = Mathf.Sin(t * 1.3f) * intensity;
                float offsetY = Mathf.Cos(t * 0.9f) * intensity * 0.5f;
                _currentOffset = intensity;

                ApplyOffset(offsetX, offsetY, _evolved ? glitchTint : normalTint);
                yield return null;
            }
        }

        private IEnumerator RestoreRoutine()
        {
            float elapsed  = 0f;
            float duration = 1.8f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float offset = Mathf.Lerp(_currentOffset, 0f, t);
                Color tint   = Color.Lerp(_evolved ? glitchTint : normalTint, normalTint, t);
                ApplyOffset(offset * Mathf.Sin(elapsed * 10f), 0f, tint);
                yield return null;
            }

            ApplyOffset(0f, 0f, normalTint);
            gameObject.SetActive(false);
        }

        private void ApplyOffset(float x, float y, Color tint)
        {
            if (reflectionSurface == null) return;
            reflectionSurface.GetPropertyBlock(_mpb);
            _mpb.SetVector("_MainTex_ST", new Vector4(1f, 1f, x, y));
            _mpb.SetColor("_Color", tint);
            reflectionSurface.SetPropertyBlock(_mpb);
        }

        protected override PatternData BuildPatternData()
        {
            return new PatternData
            {
                anomalyType = AnomalyType.ReflectionMismatch,
                layer       = anomalyLayer,
                location    = transform.position,
                timestamp   = Time.time,
                notes       = _evolved
                    ? "Reflection showed alternate scene. Possible hidden layer access point."
                    : "Reflection mismatch recorded. Surface returned to normal."
            };
        }
    }
}