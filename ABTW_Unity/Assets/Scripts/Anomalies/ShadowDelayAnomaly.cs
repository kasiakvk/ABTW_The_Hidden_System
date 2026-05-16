using UnityEngine;
using System.Collections;
using ABTW.Systems;

namespace ABTW.Anomalies
{
    /// <summary>
    /// Shadow Delay — the first anomaly in Chapter 1.
    /// The shadow moves 0.35 seconds after its source.
    /// Subtle. Unsettling. Never flashy.
    /// </summary>
    public class ShadowDelayAnomaly : AnomalyBase
    {
        [Header("— SHADOW DELAY SPECIFIC —")]
        [SerializeField] private Transform shadowTransform;   // the shadow object
        [SerializeField] private Transform sourceTransform;   // what casts the shadow
        [SerializeField] private int       occurrenceCount = 3; // must be seen 3× per GDD
        [SerializeField] private float     evolutionScale  = 1.8f; // how much shadow spreads post-Wait

        private int       _occurrencesSeen = 0;
        private Coroutine _delayRoutine;
        private Coroutine _evolutionRoutine;

        // ── Override ─────────────────────────────────────────────────────────

        protected override void OnDetected()
        {
            _occurrencesSeen++;
            AddObservation($"Shadow movement mismatch #{_occurrencesSeen}");
            AddObservation($"Timing offset: ~{_glitch != null ? "0.35" : "0.3"}s");

            // Start the delay coroutine
            if (_glitch != null && shadowTransform != null && sourceTransform != null)
                _delayRoutine = StartCoroutine(
                    _glitch.ShadowDelay(shadowTransform, sourceTransform));

            // After 3 occurrences, open notebook
            if (_occurrencesSeen >= occurrenceCount)
            {
                AddObservation("Pattern confirmed: repeating delay");
                GameManager.Instance?.notebookSystem?.AddPattern(BuildPatternData());
                GameManager.Instance?.uiManager?.ShowNotification(
                    "Something is repeating…", NotificationType.Perception);
            }
        }

        protected override void OnEvolve()
        {
            // Stop simple delay, start distortion
            if (_delayRoutine != null) StopCoroutine(_delayRoutine);

            if (_glitch != null && shadowTransform != null)
                _evolutionRoutine = StartCoroutine(
                    _glitch.ShadowDistortion(shadowTransform, duration: 12f));

            // Shadow "spreads" on wall — scale up
            if (shadowTransform != null)
                StartCoroutine(EvolveShadowScale());

            AddObservation("Shadow distortion: spreading pattern");
            GameManager.Instance?.uiManager?.ShowNotification(
                "The shadow is changing…", NotificationType.Wait);
        }

        protected override void OnResolve()
        {
            // Stop all coroutines
            if (_delayRoutine    != null) StopCoroutine(_delayRoutine);
            if (_evolutionRoutine != null) StopCoroutine(_evolutionRoutine);

            // Return shadow to normal position
            if (shadowTransform != null && sourceTransform != null)
                StartCoroutine(RestoreShadow());

            GameManager.Instance?.uiManager?.ShowNotification(
                "The shadow corrected itself.", NotificationType.Record);
        }

        // ── Private ──────────────────────────────────────────────────────────

        private IEnumerator EvolveShadowScale()
        {
            if (shadowTransform == null) yield break;

            Vector3 originalScale = shadowTransform.localScale;
            Vector3 targetScale   = originalScale * evolutionScale;
            float   elapsed       = 0f;
            float   duration      = 4f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                shadowTransform.localScale = Vector3.Lerp(
                    originalScale, targetScale, elapsed / duration);
                yield return null;
            }
        }

        private IEnumerator RestoreShadow()
        {
            if (shadowTransform == null || sourceTransform == null) yield break;

            Vector3 currentPos   = shadowTransform.position;
            Vector3 correctPos   = sourceTransform.position + Vector3.down * 0.01f;
            Vector3 currentScale = shadowTransform.localScale;
            Vector3 normalScale  = Vector3.one;
            float   elapsed      = 0f;
            float   duration     = 1.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                shadowTransform.position   = Vector3.Lerp(currentPos,   correctPos,  t);
                shadowTransform.localScale = Vector3.Lerp(currentScale, normalScale, t);
                yield return null;
            }

            SetVisible(false);
        }
    }
}