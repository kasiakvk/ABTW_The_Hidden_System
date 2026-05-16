using UnityEngine;
using System.Collections;
using ABTW.Systems;

namespace ABTW.Anomalies
{
    /// <summary>
    /// Light Instability — ceiling light flickers with wrong colour (violet).
    /// Layer 1 anomaly. Appears after Shadow Delay is noticed.
    /// The light doesn't break — it shifts. Subtly wrong.
    /// </summary>
    public class LightInstabilityAnomaly : AnomalyBase
    {
        [Header("— LIGHT SPECIFIC —")]
        [SerializeField] private Light targetLight;
        [SerializeField] private float flickerDuration   = 8f;
        [SerializeField] private float evolutionDuration  = 20f;
        [SerializeField] private float symbolRevealDelay  = 3f;   // after Wait: symbol appears

        [Header("— SYMBOL (Layer 2) —")]
        [SerializeField] private GameObject symbolObject;         // revealed post-Wait

        private Coroutine _flickerRoutine;

        // ── Override ─────────────────────────────────────────────────────────

        protected override void Awake()
        {
            base.Awake();
            anomalyType = AnomalyType.LightInstability;
            anomalyName = "Light Instability";
            worldLayer  = 1;

            if (symbolObject != null)
                symbolObject.SetActive(false);
        }

        protected override void OnDetected()
        {
            AddObservation("Light colour shift: warm → violet");
            AddObservation("Flicker pattern: irregular");
            AddObservation("Source: ceiling fixture, East Corridor");

            if (_glitch != null && targetLight != null)
                _flickerRoutine = StartCoroutine(
                    _glitch.LightInstability(targetLight, flickerDuration));

            GameManager.Instance?.uiManager?.ShowNotification(
                "The light is wrong.", NotificationType.Perception);
        }

        protected override void OnEvolve()
        {
            // Stop normal flicker, start extended evolution
            if (_flickerRoutine != null) StopCoroutine(_flickerRoutine);

            if (_glitch != null && targetLight != null)
                _flickerRoutine = StartCoroutine(
                    _glitch.LightInstability(targetLight, evolutionDuration));

            // After delay, reveal symbol (Layer 2 unlock)
            StartCoroutine(RevealSymbolAfterDelay());

            AddObservation("Light evolution: extended instability");
            AddObservation("Pattern: not random — structured");
        }

        protected override void OnResolve()
        {
            if (_flickerRoutine != null) StopCoroutine(_flickerRoutine);

            // Restore light to normal
            if (targetLight != null)
            {
                targetLight.color     = Color.white;
                targetLight.intensity = 1f;
            }

            // Hide symbol if it was revealed
            if (symbolObject != null)
                symbolObject.SetActive(false);

            GameManager.Instance?.uiManager?.ShowNotification(
                "The light stabilised.", NotificationType.Record);
        }

        // ── Private ──────────────────────────────────────────────────────────

        private IEnumerator RevealSymbolAfterDelay()
        {
            yield return new WaitForSeconds(symbolRevealDelay);

            if (symbolObject == null) yield break;

            symbolObject.SetActive(true);
            AddObservation("Symbol emerged: incomplete — requires Trio");

            // Notify player
            GameManager.Instance?.uiManager?.ShowNotification(
                "Something appeared on the wall…", NotificationType.Perception);

            // Pulse the symbol
            StartCoroutine(PulseSymbol());
        }

        private IEnumerator PulseSymbol()
        {
            if (symbolObject == null) yield break;

            float elapsed = 0f;
            float duration = 6f;

            while (elapsed < duration && !isResolved)
            {
                elapsed += Time.deltaTime;
                float pulse = 1f + Mathf.Sin(elapsed * 2.5f) * 0.06f;
                symbolObject.transform.localScale = Vector3.one * pulse;
                yield return null;
            }

            symbolObject.transform.localScale = Vector3.one;
        }
    }
}