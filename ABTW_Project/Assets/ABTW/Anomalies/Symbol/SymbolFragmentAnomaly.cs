using UnityEngine;
using System.Collections;
using ABTW.Systems;

namespace ABTW.Anomalies
{
    /// <summary>
    /// Symbol Fragment Anomaly — Layer 2 anomaly.
    /// An incomplete symbol appears on a surface (wall, floor, ceiling).
    /// Requires Trio activation to fully reveal.
    ///
    /// GDD: "incomplete symbols" / "symbolic emergence"
    /// </summary>
    public class SymbolFragmentAnomaly : AnomalyBase
    {
        [Header("Symbol Settings")]
        [SerializeField] private Renderer symbolRenderer;
        [SerializeField] private float    revealDuration  = 3f;
        [SerializeField] private float    pulseSpeed      = 1.2f;
        [SerializeField] private Color    symbolColor     = new Color(0.85f, 0.75f, 0.3f, 1f); // gold
        [SerializeField] private Color    trioRevealColor = new Color(0.6f, 0.4f, 1.0f, 1f);   // violet

        [Header("Fragment State")]
        [SerializeField] [Range(0f, 1f)] private float initialVisibility = 0.15f;

        private MaterialPropertyBlock _mpb;
        private float _currentAlpha;
        private bool  _trioRevealed;
        private Coroutine _pulseRoutine;

        // ── AnomalyBase ──────────────────────────────────────────────────────

        protected override void Awake()
        {
            base.Awake();
            anomalyType  = AnomalyType.SymbolFragment;
            anomalyLayer = 2;
            _mpb = new MaterialPropertyBlock();
        }

        protected override void OnDetected()
        {
            Debug.Log("[SymbolFragment] Detected — fragment partially visible");
            SetSymbolAlpha(initialVisibility);
            _pulseRoutine = StartCoroutine(PulseRoutine());

            GameManager.Instance?.uiManager?.ShowNotification(
                "A symbol… incomplete.", NotificationType.Symbol);
        }

        protected override void OnEvolve()
        {
            Debug.Log("[SymbolFragment] Evolving — symbol grows clearer");
            StartCoroutine(RevealRoutine(initialVisibility, 0.45f));

            GameManager.Instance?.uiManager?.ShowNotification(
                "The symbol is becoming clearer.\nWait…", NotificationType.Symbol);
        }

        protected override void OnResolve()
        {
            if (_pulseRoutine != null) StopCoroutine(_pulseRoutine);

            if (_trioRevealed)
            {
                // Full reveal via Trio
                StartCoroutine(RevealRoutine(_currentAlpha, 1f, trioRevealColor));
                GameManager.Instance?.uiManager?.ShowNotification(
                    "Symbol fully revealed.\nThe system responds.", NotificationType.TrioActivation);
            }
            else
            {
                // Recorded early — symbol fades
                StartCoroutine(FadeOutRoutine());
                GameManager.Instance?.uiManager?.ShowNotification(
                    "Symbol recorded.\nFragment stabilised.", NotificationType.Record);
            }
        }

        // ── Public ───────────────────────────────────────────────────────────

        /// <summary>Called by TrioSystem when trio activates near this anomaly.</summary>
        public void OnTrioActivated()
        {
            _trioRevealed = true;
            OnResolve();
        }

        // ── Private ──────────────────────────────────────────────────────────

        private IEnumerator PulseRoutine()
        {
            float t = 0f;
            while (true)
            {
                t += Time.deltaTime * pulseSpeed;
                float alpha = initialVisibility + Mathf.Sin(t * Mathf.PI * 2f) * 0.05f;
                SetSymbolAlpha(Mathf.Clamp01(alpha));
                yield return null;
            }
        }

        private IEnumerator RevealRoutine(float fromAlpha, float toAlpha,
            Color? targetColor = null)
        {
            float elapsed = 0f;
            Color startColor = symbolColor;
            Color endColor   = targetColor ?? symbolColor;

            while (elapsed < revealDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / revealDuration;
                SetSymbolAlpha(Mathf.Lerp(fromAlpha, toAlpha, t));
                SetSymbolColor(Color.Lerp(startColor, endColor, t));
                yield return null;
            }

            _currentAlpha = toAlpha;
        }

        private IEnumerator FadeOutRoutine()
        {
            float elapsed = 0f;
            float startAlpha = _currentAlpha;
            float duration = 2f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                SetSymbolAlpha(Mathf.Lerp(startAlpha, 0f, elapsed / duration));
                yield return null;
            }

            gameObject.SetActive(false);
        }

        private void SetSymbolAlpha(float alpha)
        {
            _currentAlpha = alpha;
            if (symbolRenderer == null) return;
            symbolRenderer.GetPropertyBlock(_mpb);
            Color c = symbolColor;
            c.a = alpha;
            _mpb.SetColor("_Color", c);
            symbolRenderer.SetPropertyBlock(_mpb);
        }

        private void SetSymbolColor(Color color)
        {
            symbolColor = color;
            SetSymbolAlpha(_currentAlpha);
        }

        protected override PatternData BuildPatternData()
        {
            return new PatternData
            {
                anomalyType  = AnomalyType.SymbolFragment,
                layer        = anomalyLayer,
                location     = transform.position,
                timestamp    = Time.time,
                notes        = _trioRevealed
                    ? "Symbol fully revealed through Trio synchronisation."
                    : "Symbol fragment recorded. Deeper version may exist."
            };
        }
    }
}