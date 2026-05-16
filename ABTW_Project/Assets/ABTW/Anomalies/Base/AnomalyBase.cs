using UnityEngine;
using ABTW.Core;
using ABTW.Systems;

namespace ABTW.Anomalies
{
    /// <summary>
    /// Base class for all anomalies in ABTW.
    /// Every anomaly exists in the world silently until Perception Mode detects it.
    /// No markers, no UI arrows — only observation.
    /// </summary>
    public abstract class AnomalyBase : MonoBehaviour
    {
        [Header("— ANOMALY IDENTITY —")]
        [SerializeField] protected AnomalyType anomalyType = AnomalyType.ShadowDelay;
        [SerializeField] protected string      anomalyName = "Unknown Anomaly";
        [SerializeField] protected string      description = "";
        [SerializeField] protected int         expReward   = 10;
        [SerializeField] protected int         worldLayer  = 1; // 1–3

        [Header("— PATTERN DATA —")]
        [SerializeField] protected string lumiNote = "";
        [SerializeField] protected string nelaNote = "";
        [SerializeField] protected List<string> observations = new();

        [Header("— STATE —")]
        [SerializeField] protected bool isDetected  = false;
        [SerializeField] protected bool isResolved  = false;
        [SerializeField] protected bool isEvolving  = false;

        // Cached references
        protected GlitchSystem   _glitch;
        protected TrioSystem     _trio;
        protected NotebookSystem _notebook;

        // Properties (used by PerceptionSystem)
        public AnomalyType AnomalyType => anomalyType;
        public int         EXPReward   => expReward;
        public bool        IsDetected  => isDetected;
        public bool        IsResolved  => isResolved;

        // ── Unity ────────────────────────────────────────────────────────────

        protected virtual void Awake()
        {
            _glitch   = FindObjectOfType<GlitchSystem>();
            _trio     = FindObjectOfType<TrioSystem>();
            _notebook = FindObjectOfType<NotebookSystem>();
        }

        protected virtual void Start()
        {
            // Anomalies start invisible / dormant
            SetVisible(false);
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Called by PerceptionSystem when player detects this anomaly.</summary>
        public virtual void OnPerceptionDetected()
        {
            if (isDetected || isResolved) return;
            isDetected = true;

            SetVisible(true);
            OnDetected();

            // Notify Trio system
            _trio?.NotifyAnomalyActive(this);

            // Build pattern data for notebook
            var pattern = BuildPatternData();
            _notebook?.AddPattern(pattern);

            Debug.Log($"[Anomaly] Detected: {anomalyName} (Layer {worldLayer})");
        }

        /// <summary>Called when player records this pattern.</summary>
        public virtual void OnRecorded()
        {
            if (isResolved) return;
            isResolved = true;
            isEvolving = false;

            _trio?.NotifyAnomalyResolved();
            OnResolve();

            Debug.Log($"[Anomaly] Recorded: {anomalyName}");
        }

        /// <summary>Called when player chooses Wait — anomaly evolves.</summary>
        public virtual void OnWaited()
        {
            isEvolving = true;
            OnEvolve();
            Debug.Log($"[Anomaly] Evolving: {anomalyName}");
        }

        // ── Abstract / Virtual ───────────────────────────────────────────────

        /// <summary>Override to implement anomaly-specific detection behaviour.</summary>
        protected abstract void OnDetected();

        /// <summary>Override to implement anomaly-specific evolution (post-Wait).</summary>
        protected abstract void OnEvolve();

        /// <summary>Override to implement anomaly-specific resolution (post-Record).</summary>
        protected abstract void OnResolve();

        // ── Helpers ──────────────────────────────────────────────────────────

        protected virtual void SetVisible(bool visible)
        {
            // Override in subclasses for specific visibility logic
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) r.enabled = visible;
        }

        protected PatternData BuildPatternData()
        {
            return new PatternData
            {
                name         = anomalyName,
                description  = description,
                anomalyType  = anomalyType,
                lumiNote     = lumiNote,
                nelaNote     = nelaNote,
                observations = new List<string>(observations),
                expOnRecord  = expReward,
                expOnDeep    = expReward * 2
            };
        }

        protected void AddObservation(string obs)
        {
            observations.Add(obs);
            Debug.Log($"[Anomaly] Observation: {obs}");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = worldLayer switch
            {
                1 => new Color(1f, 0.8f, 0f, 0.3f),   // gold — layer 1
                2 => new Color(0.5f, 0.2f, 1f, 0.3f), // violet — layer 2
                3 => new Color(0f, 0.8f, 1f, 0.3f),   // cyan — layer 3
                _ => Color.white
            };
            Gizmos.DrawWireSphere(transform.position, 1.5f);
        }
    }
}