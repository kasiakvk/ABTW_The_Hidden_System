using UnityEngine;
using System.Collections;
using ABTW.Core;

namespace ABTW.Systems
{
    /// <summary>
    /// Controls all anomaly visual behaviours.
    /// Rule: 70% normal, 30% anomaly. Never flashy. Only unsettling.
    /// </summary>
    public class GlitchSystem : MonoBehaviour
    {
        [Header("— SHADOW DELAY —")]
        [SerializeField] private float shadowDelaySeconds = 0.35f;  // 0.2–0.5 per GDD
        [SerializeField] private float shadowMaxOffset    = 0.18f;  // subtle drift

        [Header("— LIGHT INSTABILITY —")]
        [SerializeField] private float lightFlickerMin    = 0.82f;
        [SerializeField] private float lightFlickerMax    = 1.00f;
        [SerializeField] private float lightFlickerSpeed  = 2.4f;
        [SerializeField] private Color anomalyLightColor  = new Color(0.48f, 0.31f, 1f); // violet

        [Header("— DISTORTION (post-Wait) —")]
        [SerializeField] private float distortionStrength = 0.06f;
        [SerializeField] private float distortionSpeed    = 1.1f;

        [Header("— TRIO ACTIVATION —")]
        [SerializeField] private float trioWorldDimAmount = 0.15f;  // world dims slightly
        [SerializeField] private float trioPulseDuration  = 2.5f;

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Apply shadow delay to a shadow caster transform.</summary>
        public IEnumerator ShadowDelay(Transform shadowTransform, Transform source)
        {
            while (true)
            {
                // Store source position, wait, then apply to shadow
                Vector3 sourcePos = source.position;
                yield return new WaitForSeconds(shadowDelaySeconds);

                if (shadowTransform == null) yield break;

                // Lerp shadow toward delayed position (subtle, not instant)
                shadowTransform.position = Vector3.Lerp(
                    shadowTransform.position,
                    sourcePos + Vector3.right * shadowMaxOffset * Mathf.Sin(Time.time * 0.5f),
                    Time.deltaTime * 3f);
            }
        }

        /// <summary>Make a light flicker with wrong colour — Layer 1 anomaly.</summary>
        public IEnumerator LightInstability(Light targetLight, float duration)
        {
            if (targetLight == null) yield break;

            Color  originalColor     = targetLight.color;
            float  originalIntensity = targetLight.intensity;
            float  elapsed           = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Flicker intensity
                float flicker = Mathf.Lerp(lightFlickerMin, lightFlickerMax,
                    Mathf.PerlinNoise(Time.time * lightFlickerSpeed, 0f));
                targetLight.intensity = originalIntensity * flicker;

                // Shift colour toward violet at peak anomaly
                float anomalyStrength = Mathf.Sin(t * Mathf.PI); // 0→1→0
                targetLight.color = Color.Lerp(originalColor, anomalyLightColor,
                    anomalyStrength * 0.4f);

                yield return null;
            }

            // Restore
            targetLight.intensity = originalIntensity;
            targetLight.color     = originalColor;
        }

        /// <summary>Post-Wait distortion — shadow "spreads" on wall.</summary>
        public IEnumerator ShadowDistortion(Transform shadowTransform, float duration)
        {
            if (shadowTransform == null) yield break;

            Vector3 originalScale = shadowTransform.localScale;
            float   elapsed       = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Spread: scale X grows, Y shrinks slightly
                float spread = Mathf.Sin(t * Mathf.PI) * distortionStrength;
                shadowTransform.localScale = new Vector3(
                    originalScale.x * (1f + spread * 2f),
                    originalScale.y * (1f - spread * 0.5f),
                    originalScale.z);

                // Drift position
                float drift = Mathf.Sin(Time.time * distortionSpeed) * 0.04f;
                shadowTransform.position += new Vector3(drift, 0f, 0f) * Time.deltaTime;

                yield return null;
            }

            shadowTransform.localScale = originalScale;
        }

        /// <summary>Trio activation — world dims, symbol pulses, soft sound cue.</summary>
        public IEnumerator TrioActivationEffect(GameObject symbolObject)
        {
            // Dim all scene lights slightly
            Light[] sceneLights = FindObjectsOfType<Light>();
            float[] originalIntensities = new float[sceneLights.Length];
            for (int i = 0; i < sceneLights.Length; i++)
                originalIntensities[i] = sceneLights[i].intensity;

            float elapsed = 0f;
            while (elapsed < trioPulseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / trioPulseDuration;

                // Dim → restore
                float dimCurve = Mathf.Sin(t * Mathf.PI) * trioWorldDimAmount;
                for (int i = 0; i < sceneLights.Length; i++)
                    sceneLights[i].intensity = originalIntensities[i] * (1f - dimCurve);

                // Pulse symbol scale
                if (symbolObject != null)
                {
                    float pulse = 1f + Mathf.Sin(t * Mathf.PI * 4f) * 0.08f;
                    symbolObject.transform.localScale = Vector3.one * pulse;
                }

                yield return null;
            }

            // Restore lights
            for (int i = 0; i < sceneLights.Length; i++)
                sceneLights[i].intensity = originalIntensities[i];

            GameManager.Instance?.OnTrioActivated();
        }

        /// <summary>Memory Walk visual — desaturate + slow motion.</summary>
        public IEnumerator MemoryWalkTransition(bool entering, float duration)
        {
            float elapsed = 0f;
            float startScale = entering ? 1f : 0.5f;
            float endScale   = entering ? 0.5f : 1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                Time.timeScale = Mathf.Lerp(startScale, endScale, t);
                yield return null;
            }

            Time.timeScale = endScale;
        }

        /// <summary>Echo appearance — brief, weak, fades quickly.</summary>
        public IEnumerator EchoAppearance(Renderer echoRenderer, float duration)
        {
            if (echoRenderer == null) yield break;

            Material mat = echoRenderer.material;
            Color    col = mat.color;

            // Fade in
            float elapsed = 0f;
            while (elapsed < duration * 0.3f)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 0.45f, elapsed / (duration * 0.3f));
                mat.color = new Color(col.r, col.g, col.b, alpha);
                yield return null;
            }

            // Hold
            yield return new WaitForSeconds(duration * 0.4f);

            // Fade out
            elapsed = 0f;
            while (elapsed < duration * 0.3f)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0.45f, 0f, elapsed / (duration * 0.3f));
                mat.color = new Color(col.r, col.g, col.b, alpha);
                yield return null;
            }

            echoRenderer.gameObject.SetActive(false);
        }
    }
}