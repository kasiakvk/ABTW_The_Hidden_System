using UnityEngine;
using System.Collections;
using ABTW.Systems;

namespace ABTW.Anomalies
{
    /// <summary>
    /// Sound Echo Anomaly — Layer 1 anomaly.
    /// A sound repeats slightly out of sync with its source,
    /// or plays when no source is present.
    ///
    /// GDD: "environmental pauses" / "subtle atmospheric changes"
    /// </summary>
    public class SoundEchoAnomaly : AnomalyBase
    {
        [Header("Audio")]
        [SerializeField] private AudioSource primarySource;
        [SerializeField] private AudioSource echoSource;
        [SerializeField] private float       echoDelay     = 0.35f;
        [SerializeField] private float       echoVolume    = 0.28f;
        [SerializeField] private float       evolveDelay   = 0.65f;
        [SerializeField] private float       evolveVolume  = 0.55f;

        [Header("Detection")]
        [SerializeField] private float detectionRadius = 6f;

        private bool      _evolved;
        private Coroutine _echoLoop;

        // ── AnomalyBase ──────────────────────────────────────────────────────

        protected override void Awake()
        {
            base.Awake();
            anomalyType  = AnomalyType.SoundEcho;
            anomalyLayer = 1;
        }

        protected override void OnDetected()
        {
            Debug.Log("[SoundEcho] Detected — echo begins");
            _echoLoop = StartCoroutine(EchoLoop(echoDelay, echoVolume));

            GameManager.Instance?.uiManager?.ShowNotification(
                "That sound… it repeated.", NotificationType.Perception);
        }

        protected override void OnEvolve()
        {
            _evolved = true;
            Debug.Log("[SoundEcho] Evolving — echo intensifies");

            if (_echoLoop != null) StopCoroutine(_echoLoop);
            _echoLoop = StartCoroutine(EchoLoop(evolveDelay, evolveVolume));

            GameManager.Instance?.uiManager?.ShowNotification(
                "The echo is getting stronger.\nAnd longer.", NotificationType.Perception);
        }

        protected override void OnResolve()
        {
            if (_echoLoop != null) StopCoroutine(_echoLoop);
            StartCoroutine(FadeEchoOut());

            GameManager.Instance?.uiManager?.ShowNotification(
                "Sound echo recorded. Silence restored.", NotificationType.Record);
        }

        // ── Private ──────────────────────────────────────────────────────────

        private IEnumerator EchoLoop(float delay, float volume)
        {
            while (true)
            {
                if (primarySource != null && primarySource.isPlaying)
                {
                    yield return new WaitForSeconds(delay);

                    if (echoSource != null)
                    {
                        echoSource.volume = volume;
                        echoSource.Play();
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator FadeEchoOut()
        {
            float elapsed  = 0f;
            float duration = 1.5f;
            float startVol = echoSource != null ? echoSource.volume : 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (echoSource != null)
                    echoSource.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
                yield return null;
            }

            if (echoSource != null) echoSource.Stop();
            gameObject.SetActive(false);
        }

        protected override PatternData BuildPatternData()
        {
            return new PatternData
            {
                anomalyType = AnomalyType.SoundEcho,
                layer       = anomalyLayer,
                location    = transform.position,
                timestamp   = Time.time,
                notes       = _evolved
                    ? "Echo evolved — sound repeated without source. Possible memory trace."
                    : "Sound echo recorded. Acoustic anomaly stabilised."
            };
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}