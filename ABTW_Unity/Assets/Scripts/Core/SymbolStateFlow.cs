using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ABTW.Core
{
    /// <summary>
    /// Manages symbol state transitions: Glitch → Forming → Activation → Resolved
    /// Each symbol in the Academy follows this flow based on player perception.
    /// </summary>
    public class SymbolStateFlow : MonoBehaviour
    {
        public enum SymbolState
        {
            Hidden,       // Not yet visible
            Glitch,       // Flickering, unstable — first detection
            Forming,      // Pattern emerging — player is noticing
            Activation,   // Full symbol visible — Trio synchronized
            Resolved,     // Recorded in notebook — stabilized
            Echo          // Recorded too early — fragment remains
        }

        [Header("Symbol Identity")]
        public string symbolID = "SYM_001";
        public string symbolName = "Unknown Symbol";
        [TextArea(2, 4)]
        public string symbolMeaning = "";

        [Header("State")]
        [SerializeField] private SymbolState currentState = SymbolState.Hidden;
        [SerializeField] private SymbolState previousState = SymbolState.Hidden;

        [Header("Transition Timings")]
        public float glitchDuration = 2.5f;
        public float formingDuration = 4f;
        public float activationHoldTime = 6f;
        public float echoFadeDuration = 3f;

        [Header("Perception Requirements")]
        [Range(0f, 1f)] public float glitchTriggerAwareness = 0.3f;
        [Range(0f, 1f)] public float formingTriggerClarity = 0.5f;
        [Range(0f, 1f)] public float activationTriggerConnection = 0.7f;

        [Header("Visual References")]
        public Renderer symbolRenderer;
        public ParticleSystem glitchParticles;
        public ParticleSystem activationParticles;
        public Light symbolLight;

        [Header("Events")]
        public UnityEngine.Events.UnityEvent OnGlitchStart;
        public UnityEngine.Events.UnityEvent OnForming;
        public UnityEngine.Events.UnityEvent OnActivation;
        public UnityEngine.Events.UnityEvent OnResolved;
        public UnityEngine.Events.UnityEvent OnEcho;

        // State history for Memory Walk
        private List<(SymbolState state, float timestamp)> stateHistory
            = new List<(SymbolState, float)>();

        private Coroutine stateCoroutine;
        private MaterialPropertyBlock propBlock;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        public SymbolState CurrentState => currentState;
        public List<(SymbolState state, float timestamp)> StateHistory => stateHistory;

        private void Awake()
        {
            propBlock = new MaterialPropertyBlock();
        }

        private void Start()
        {
            ApplyVisualState(currentState);
        }

        // ─── State Transitions ───────────────────────────────────────────────

        public void TriggerGlitch()
        {
            if (currentState == SymbolState.Hidden || currentState == SymbolState.Echo)
                TransitionTo(SymbolState.Glitch);
        }

        public void TriggerForming()
        {
            if (currentState == SymbolState.Glitch)
                TransitionTo(SymbolState.Forming);
        }

        public void TriggerActivation()
        {
            if (currentState == SymbolState.Forming)
                TransitionTo(SymbolState.Activation);
        }

        public void RecordSymbol()
        {
            if (currentState == SymbolState.Activation)
            {
                TransitionTo(SymbolState.Resolved);
            }
            else if (currentState == SymbolState.Glitch || currentState == SymbolState.Forming)
            {
                // Recorded too early → Echo
                TransitionTo(SymbolState.Echo);
            }
        }

        private void TransitionTo(SymbolState newState)
        {
            previousState = currentState;
            currentState = newState;

            // Log history
            stateHistory.Add((newState, Time.time));

            Debug.Log($"[SymbolStateFlow] {symbolName}: {previousState} → {newState}");

            if (stateCoroutine != null) StopCoroutine(stateCoroutine);
            stateCoroutine = StartCoroutine(StateRoutine(newState));

            ApplyVisualState(newState);
            FireStateEvent(newState);
        }

        private IEnumerator StateRoutine(SymbolState state)
        {
            switch (state)
            {
                case SymbolState.Glitch:
                    yield return StartCoroutine(GlitchRoutine());
                    break;

                case SymbolState.Forming:
                    yield return StartCoroutine(FormingRoutine());
                    break;

                case SymbolState.Activation:
                    yield return new WaitForSeconds(activationHoldTime);
                    // Auto-decay if not recorded
                    if (currentState == SymbolState.Activation)
                        TransitionTo(SymbolState.Echo);
                    break;

                case SymbolState.Echo:
                    yield return StartCoroutine(EchoFadeRoutine());
                    break;
            }
        }

        private IEnumerator GlitchRoutine()
        {
            float elapsed = 0f;
            while (elapsed < glitchDuration)
            {
                // Flicker effect
                float flicker = Mathf.Sin(Time.time * 12f) * 0.5f + 0.5f;
                SetAlpha(flicker * 0.6f);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator FormingRoutine()
        {
            float elapsed = 0f;
            while (elapsed < formingDuration)
            {
                float t = elapsed / formingDuration;
                SetAlpha(Mathf.Lerp(0.3f, 0.8f, t));
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator EchoFadeRoutine()
        {
            float elapsed = 0f;
            while (elapsed < echoFadeDuration)
            {
                float t = elapsed / echoFadeDuration;
                SetAlpha(Mathf.Lerp(0.4f, 0f, t));
                elapsed += Time.deltaTime;
                yield return null;
            }
            // Return to hidden after echo
            currentState = SymbolState.Hidden;
            gameObject.SetActive(false);
        }

        // ─── Visuals ─────────────────────────────────────────────────────────

        private void ApplyVisualState(SymbolState state)
        {
            if (symbolRenderer == null) return;

            Color emissionColor;
            switch (state)
            {
                case SymbolState.Hidden:
                    SetAlpha(0f);
                    if (symbolLight) symbolLight.enabled = false;
                    if (glitchParticles) glitchParticles.Stop();
                    if (activationParticles) activationParticles.Stop();
                    return;

                case SymbolState.Glitch:
                    emissionColor = new Color(0.2f, 0.2f, 0.8f, 1f); // cool blue
                    if (glitchParticles) glitchParticles.Play();
                    if (symbolLight) { symbolLight.enabled = true; symbolLight.intensity = 0.5f; }
                    break;

                case SymbolState.Forming:
                    emissionColor = new Color(0.4f, 0.6f, 1f, 1f); // soft blue-white
                    if (glitchParticles) glitchParticles.Stop();
                    if (symbolLight) symbolLight.intensity = 1f;
                    break;

                case SymbolState.Activation:
                    emissionColor = new Color(1f, 0.95f, 0.7f, 1f); // warm gold
                    if (activationParticles) activationParticles.Play();
                    if (symbolLight) { symbolLight.color = Color.yellow; symbolLight.intensity = 2f; }
                    break;

                case SymbolState.Resolved:
                    emissionColor = new Color(0.6f, 1f, 0.6f, 1f); // soft green
                    if (activationParticles) activationParticles.Stop();
                    if (symbolLight) symbolLight.intensity = 0.3f;
                    break;

                case SymbolState.Echo:
                    emissionColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // grey fragment
                    if (symbolLight) symbolLight.intensity = 0.2f;
                    break;

                default:
                    emissionColor = Color.white;
                    break;
            }

            symbolRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor(EmissionColor, emissionColor);
            symbolRenderer.SetPropertyBlock(propBlock);
        }

        private void SetAlpha(float alpha)
        {
            if (symbolRenderer == null) return;
            symbolRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(Alpha, alpha);
            symbolRenderer.SetPropertyBlock(propBlock);
        }

        private void FireStateEvent(SymbolState state)
        {
            switch (state)
            {
                case SymbolState.Glitch:      OnGlitchStart?.Invoke(); break;
                case SymbolState.Forming:     OnForming?.Invoke(); break;
                case SymbolState.Activation:  OnActivation?.Invoke(); break;
                case SymbolState.Resolved:    OnResolved?.Invoke(); break;
                case SymbolState.Echo:        OnEcho?.Invoke(); break;
            }
        }

        // ─── Perception Integration ──────────────────────────────────────────

        /// <summary>
        /// Called by PerceptionSystem when player awareness changes.
        /// </summary>
        public void OnAwarenessChanged(float awareness)
        {
            if (currentState == SymbolState.Hidden && awareness >= glitchTriggerAwareness)
                TriggerGlitch();
        }

        public void OnClarityChanged(float clarity)
        {
            if (currentState == SymbolState.Glitch && clarity >= formingTriggerClarity)
                TriggerForming();
        }

        public void OnConnectionChanged(float connection)
        {
            if (currentState == SymbolState.Forming && connection >= activationTriggerConnection)
                TriggerActivation();
        }

        // ─── Save/Load ───────────────────────────────────────────────────────

        public string GetSaveState() => currentState.ToString();

        public void LoadSaveState(string savedState)
        {
            if (System.Enum.TryParse(savedState, out SymbolState loaded))
            {
                currentState = loaded;
                ApplyVisualState(currentState);
            }
        }
    }
}