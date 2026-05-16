using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ABTW.Systems
{
    /// <summary>
    /// ABTW Glow Effect — soft, perception-reactive glow for symbols, anomalies, and hidden layers.
    /// Glow is never flashy. It is a whisper, not a shout.
    /// </summary>
    public class GlowEffect : MonoBehaviour
    {
        public enum GlowType
        {
            None,
            Subtle,         // Barely visible — anomaly hint
            Forming,        // Symbol forming — soft pulse
            Activation,     // Full activation — warm gold
            HiddenLayer,    // Hidden layer revealed — ethereal white-blue
            Echo,           // Echo state — faded, incomplete
            TrioSync        // Trio synchronized — all three colors
        }

        [Header("Glow Configuration")]
        public GlowType glowType = GlowType.None;
        public bool autoStart = false;
        public float transitionSpeed = 1.5f;

        [Header("Glow Colors")]
        public Color subtleColor      = new Color(0.60f, 0.65f, 0.90f, 0.3f);
        public Color formingColor     = new Color(0.55f, 0.70f, 1.00f, 0.5f);
        public Color activationColor  = new Color(1.00f, 0.85f, 0.20f, 0.8f);
        public Color hiddenLayerColor = new Color(0.85f, 0.90f, 1.00f, 0.6f);
        public Color echoColor        = new Color(0.70f, 0.60f, 0.80f, 0.25f);
        public Color trioSyncColor    = new Color(1.00f, 0.95f, 0.80f, 0.9f);

        [Header("Glow Intensity")]
        public float subtleIntensity      = 0.3f;
        public float formingIntensity     = 0.6f;
        public float activationIntensity  = 1.2f;
        public float hiddenLayerIntensity = 0.8f;
        public float echoIntensity        = 0.2f;
        public float trioSyncIntensity    = 1.5f;

        [Header("Pulse Settings")]
        public bool enablePulse = true;
        public float pulseSpeed = 1.0f;
        public float pulseAmplitude = 0.15f;

        [Header("Renderer References")]
        public Renderer[] targetRenderers;
        public bool useEmissionKeyword = true;
        public string emissionColorProperty = "_EmissionColor";

        [Header("Light Source (optional)")]
        public Light glowLight;
        public bool controlLight = true;

        // Internal
        private GlowType currentGlowType = GlowType.None;
        private float currentIntensity = 0f;
        private float targetIntensity = 0f;
        private Color currentColor;
        private Color targetColor;
        private Coroutine transitionCoroutine;
        private Coroutine pulseCoroutine;
        private bool isPulsing = false;

        // Material property blocks (avoid material instancing)
        private MaterialPropertyBlock[] propertyBlocks;

        private void Awake()
        {
            if (targetRenderers == null || targetRenderers.Length == 0)
                targetRenderers = GetComponentsInChildren<Renderer>();

            propertyBlocks = new MaterialPropertyBlock[targetRenderers.Length];
            for (int i = 0; i < propertyBlocks.Length; i++)
                propertyBlocks[i] = new MaterialPropertyBlock();
        }

        private void Start()
        {
            if (autoStart && glowType != GlowType.None)
                SetGlow(glowType);
            else
                ApplyGlowInstant(GlowType.None);
        }

        // ─── Public API ───────────────────────────────────────────────────────

        public void SetGlow(GlowType type, bool instant = false)
        {
            currentGlowType = type;
            targetColor = GetColorForType(type);
            targetIntensity = GetIntensityForType(type);

            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
            if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);

            if (instant)
            {
                ApplyGlowInstant(type);
            }
            else
            {
                transitionCoroutine = StartCoroutine(TransitionGlow(targetColor, targetIntensity));
            }

            if (enablePulse && type != GlowType.None && type != GlowType.Echo)
                pulseCoroutine = StartCoroutine(PulseRoutine());
        }

        public void ClearGlow(bool instant = false)
        {
            SetGlow(GlowType.None, instant);
        }

        public void FlashGlow(GlowType type, float duration = 0.5f)
        {
            StartCoroutine(FlashRoutine(type, duration));
        }

        // ─── Symbol State Integration ─────────────────────────────────────────

        public void OnSymbolStateChanged(SymbolStateFlow.SymbolState state)
        {
            GlowType mapped = state switch
            {
                SymbolStateFlow.SymbolState.Hidden     => GlowType.None,
                SymbolStateFlow.SymbolState.Glitch     => GlowType.Subtle,
                SymbolStateFlow.SymbolState.Forming    => GlowType.Forming,
                SymbolStateFlow.SymbolState.Activation => GlowType.Activation,
                SymbolStateFlow.SymbolState.Resolved   => GlowType.HiddenLayer,
                SymbolStateFlow.SymbolState.Echo       => GlowType.Echo,
                _ => GlowType.None
            };
            SetGlow(mapped);
        }

        public void OnTrioSynchronized()
        {
            SetGlow(GlowType.TrioSync);
        }

        // ─── Coroutines ───────────────────────────────────────────────────────

        private IEnumerator TransitionGlow(Color toColor, float toIntensity)
        {
            float startIntensity = currentIntensity;
            Color startColor = currentColor;
            float elapsed = 0f;
            float duration = 1f / transitionSpeed;

            while (elapsed < duration)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                currentIntensity = Mathf.Lerp(startIntensity, toIntensity, t);
                currentColor = Color.Lerp(startColor, toColor, t);
                ApplyToRenderers(currentColor, currentIntensity);
                elapsed += Time.deltaTime;
                yield return null;
            }

            currentIntensity = toIntensity;
            currentColor = toColor;
            ApplyToRenderers(currentColor, currentIntensity);
        }

        private IEnumerator PulseRoutine()
        {
            isPulsing = true;
            float baseIntensity = targetIntensity;

            while (currentGlowType != GlowType.None)
            {
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
                float pulsedIntensity = Mathf.Max(0f, baseIntensity + pulse);
                ApplyToRenderers(currentColor, pulsedIntensity);
                yield return null;
            }

            isPulsing = false;
        }

        private IEnumerator FlashRoutine(GlowType type, float duration)
        {
            GlowType previous = currentGlowType;
            SetGlow(type, true);
            yield return new WaitForSeconds(duration);
            SetGlow(previous);
        }

        // ─── Renderer Application ─────────────────────────────────────────────

        private void ApplyToRenderers(Color color, float intensity)
        {
            Color emissionColor = color * intensity;

            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] == null) continue;

                targetRenderers[i].GetPropertyBlock(propertyBlocks[i]);

                if (useEmissionKeyword)
                    propertyBlocks[i].SetColor(emissionColorProperty, emissionColor);

                targetRenderers[i].SetPropertyBlock(propertyBlocks[i]);

                // Enable/disable emission keyword
                foreach (var mat in targetRenderers[i].sharedMaterials)
                {
                    if (mat == null) continue;
                    if (intensity > 0.01f)
                        mat.EnableKeyword("_EMISSION");
                    else
                        mat.DisableKeyword("_EMISSION");
                }
            }

            // Control optional light source
            if (controlLight && glowLight != null)
            {
                glowLight.color = color;
                glowLight.intensity = intensity * 0.5f; // lights are subtler
                glowLight.enabled = intensity > 0.01f;
            }
        }

        private void ApplyGlowInstant(GlowType type)
        {
            currentColor = GetColorForType(type);
            currentIntensity = GetIntensityForType(type);
            targetColor = currentColor;
            targetIntensity = currentIntensity;
            ApplyToRenderers(currentColor, currentIntensity);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        private Color GetColorForType(GlowType type) => type switch
        {
            GlowType.Subtle      => subtleColor,
            GlowType.Forming     => formingColor,
            GlowType.Activation  => activationColor,
            GlowType.HiddenLayer => hiddenLayerColor,
            GlowType.Echo        => echoColor,
            GlowType.TrioSync    => trioSyncColor,
            _ => Color.clear
        };

        private float GetIntensityForType(GlowType type) => type switch
        {
            GlowType.Subtle      => subtleIntensity,
            GlowType.Forming     => formingIntensity,
            GlowType.Activation  => activationIntensity,
            GlowType.HiddenLayer => hiddenLayerIntensity,
            GlowType.Echo        => echoIntensity,
            GlowType.TrioSync    => trioSyncIntensity,
            _ => 0f
        };

        public GlowType CurrentGlowType => currentGlowType;
        public float CurrentIntensity => currentIntensity;
    }
}