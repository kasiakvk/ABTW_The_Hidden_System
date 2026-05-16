using UnityEngine;
using System.Collections;

namespace ABTW.Core
{
    /// <summary>
    /// ABTW Core Rule: Attention + Understanding + Alignment = Response
    /// Manages the three-layer activation system of the Academy.
    /// </summary>
    public class ActivationLogic : MonoBehaviour
    {
        [Header("Activation Thresholds")]
        [Range(0f, 1f)] public float attentionThreshold = 0.6f;
        [Range(0f, 1f)] public float understandingThreshold = 0.5f;
        [Range(0f, 1f)] public float alignmentThreshold = 0.7f;

        [Header("Current State")]
        [SerializeField] private float attentionLevel = 0f;
        [SerializeField] private float understandingLevel = 0f;
        [SerializeField] private float alignmentLevel = 0f;

        [Header("Response Settings")]
        public float responseIntensity = 1f;
        public float activationCooldown = 3f;
        public AnimationCurve responseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Events")]
        public UnityEngine.Events.UnityEvent OnActivationTriggered;
        public UnityEngine.Events.UnityEvent OnActivationFailed;
        public UnityEngine.Events.UnityEvent OnPartialActivation;

        private bool isActivated = false;
        private bool isCoolingDown = false;
        private float activationScore = 0f;

        // Layer states
        public enum ActivationLayer { None, Attention, Understanding, Alignment, Full }
        private ActivationLayer currentLayer = ActivationLayer.None;

        public float AttentionLevel => attentionLevel;
        public float UnderstandingLevel => understandingLevel;
        public float AlignmentLevel => alignmentLevel;
        public float ActivationScore => activationScore;
        public ActivationLayer CurrentLayer => currentLayer;
        public bool IsActivated => isActivated;

        private void Update()
        {
            EvaluateActivation();
        }

        /// <summary>
        /// Core formula: Attention + Understanding + Alignment = Response
        /// </summary>
        private void EvaluateActivation()
        {
            if (isCoolingDown) return;

            // Calculate weighted activation score
            float attentionWeight = 0.35f;
            float understandingWeight = 0.35f;
            float alignmentWeight = 0.30f;

            activationScore = (attentionLevel * attentionWeight)
                            + (understandingLevel * understandingWeight)
                            + (alignmentLevel * alignmentWeight);

            // Determine current layer
            UpdateActivationLayer();

            // Check for full activation
            if (attentionLevel >= attentionThreshold &&
                understandingLevel >= understandingThreshold &&
                alignmentLevel >= alignmentThreshold)
            {
                if (!isActivated)
                {
                    TriggerFullActivation();
                }
            }
            else if (activationScore > 0.4f)
            {
                OnPartialActivation?.Invoke();
            }
        }

        private void UpdateActivationLayer()
        {
            if (attentionLevel >= attentionThreshold &&
                understandingLevel >= understandingThreshold &&
                alignmentLevel >= alignmentThreshold)
            {
                currentLayer = ActivationLayer.Full;
            }
            else if (attentionLevel >= attentionThreshold &&
                     understandingLevel >= understandingThreshold)
            {
                currentLayer = ActivationLayer.Alignment;
            }
            else if (attentionLevel >= attentionThreshold)
            {
                currentLayer = ActivationLayer.Understanding;
            }
            else if (attentionLevel > 0.1f)
            {
                currentLayer = ActivationLayer.Attention;
            }
            else
            {
                currentLayer = ActivationLayer.None;
            }
        }

        private void TriggerFullActivation()
        {
            isActivated = true;
            float intensity = responseCurve.Evaluate(activationScore) * responseIntensity;
            Debug.Log($"[ActivationLogic] FULL ACTIVATION — Score: {activationScore:F2}, Intensity: {intensity:F2}");
            OnActivationTriggered?.Invoke();
            StartCoroutine(ActivationCooldownRoutine());
        }

        private IEnumerator ActivationCooldownRoutine()
        {
            isCoolingDown = true;
            yield return new WaitForSeconds(activationCooldown);
            isActivated = false;
            isCoolingDown = false;
            ResetLevels();
        }

        // Public setters — called by PerceptionSystem, TrioSystem, etc.
        public void SetAttention(float value) => attentionLevel = Mathf.Clamp01(value);
        public void SetUnderstanding(float value) => understandingLevel = Mathf.Clamp01(value);
        public void SetAlignment(float value) => alignmentLevel = Mathf.Clamp01(value);

        public void AddAttention(float delta) => attentionLevel = Mathf.Clamp01(attentionLevel + delta);
        public void AddUnderstanding(float delta) => understandingLevel = Mathf.Clamp01(understandingLevel + delta);
        public void AddAlignment(float delta) => alignmentLevel = Mathf.Clamp01(alignmentLevel + delta);

        public void ResetLevels()
        {
            attentionLevel = 0f;
            understandingLevel = 0f;
            alignmentLevel = 0f;
            currentLayer = ActivationLayer.None;
        }

        /// <summary>
        /// Force activation check — called externally by TrioSystem
        /// </summary>
        public bool CheckActivation()
        {
            return attentionLevel >= attentionThreshold &&
                   understandingLevel >= understandingThreshold &&
                   alignmentLevel >= alignmentThreshold;
        }

        private void OnDrawGizmosSelected()
        {
            // Visual debug in Scene view
            Gizmos.color = isActivated ? Color.yellow : Color.gray;
            Gizmos.DrawWireSphere(transform.position, 0.5f + activationScore);
        }
    }
}