using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ABTW.Core;

namespace ABTW.Companions
{
    /// <summary>
    /// Lumi — AI companion.
    /// Role: ANALYZER — processes patterns, provides logical insight.
    /// She notices structure, repetition, and data in anomalies.
    /// </summary>
    public class LumiAnalyzer : MonoBehaviour
    {
        [Header("Analysis Settings")]
        public float analysisRadius = 8f;
        public float analysisSpeed = 1.5f;       // how fast she processes patterns
        public float patternMemoryDuration = 30f; // how long she remembers a pattern
        public int maxPatternsTracked = 5;

        [Header("Notebook Behavior")]
        public bool autoWritesInNotebook = true;
        public float notebookWriteDelay = 1.2f;

        [Header("Dialogue")]
        [TextArea(2, 3)] public string[] analysisLines = {
            "It's repeating.",
            "The interval is consistent — 0.4 seconds.",
            "Three occurrences. That's not random.",
            "I've seen this pattern before.",
            "The data suggests a deeper layer."
        };
        [TextArea(2, 3)] public string[] waitLines = {
            "Don't write it yet.",
            "Wait — there's more.",
            "The pattern isn't complete.",
            "Something else is coming."
        };

        [Header("References")]
        public ActivationLogic activationLogic;
        public Transform followTarget; // Astra's transform
        public Animator animator;

        [Header("State")]
        [SerializeField] private bool isAnalyzing = false;
        [SerializeField] private float currentAnalysisProgress = 0f;
        [SerializeField] private int patternsAnalyzed = 0;

        // Pattern tracking
        private class PatternRecord
        {
            public string patternID;
            public float firstSeen;
            public float lastSeen;
            public int occurrences;
            public bool isComplete;
        }

        private List<PatternRecord> trackedPatterns = new List<PatternRecord>();
        private Coroutine analysisCoroutine;

        // Animator hashes
        private static readonly int HashIsAnalyzing = Animator.StringToHash("IsAnalyzing");
        private static readonly int HashWrite = Animator.StringToHash("Write");
        private static readonly int HashPoint = Animator.StringToHash("Point");

        // Events
        public System.Action<string> OnPatternAnalyzed;
        public System.Action<string> OnNotebookEntry;
        public System.Action<string> OnDialogueLine;
        public System.Action OnAnalysisComplete;

        private void Start()
        {
            if (followTarget == null)
            {
                var astra = FindObjectOfType<AstraController>();
                if (astra != null) followTarget = astra.transform;
            }
        }

        private void Update()
        {
            FollowAstra();
            UpdateAnalysisProgress();
        }

        // ─── Follow Behavior ─────────────────────────────────────────────────

        private void FollowAstra()
        {
            if (followTarget == null) return;

            // Lumi stays slightly behind and to the left of Astra
            Vector3 targetOffset = followTarget.position
                - followTarget.forward * 1.2f
                + followTarget.right * (-0.8f);

            transform.position = Vector3.Lerp(transform.position, targetOffset,
                analysisSpeed * 0.5f * Time.deltaTime);

            // Face same direction as Astra
            transform.rotation = Quaternion.Slerp(transform.rotation,
                followTarget.rotation, 3f * Time.deltaTime);
        }

        // ─── Analysis System ─────────────────────────────────────────────────

        public void BeginAnalysis(string patternID, GameObject anomalyObject)
        {
            if (isAnalyzing) return;

            isAnalyzing = true;
            currentAnalysisProgress = 0f;

            if (analysisCoroutine != null) StopCoroutine(analysisCoroutine);
            analysisCoroutine = StartCoroutine(AnalysisRoutine(patternID, anomalyObject));
        }

        private IEnumerator AnalysisRoutine(string patternID, GameObject anomalyObject)
        {
            // Phase 1: Lumi notices
            yield return new WaitForSeconds(0.5f);
            SayLine(analysisLines[Random.Range(0, analysisLines.Length)]);

            if (animator != null) animator.SetBool(HashIsAnalyzing, true);

            // Phase 2: Progressive analysis
            float elapsed = 0f;
            float duration = 3f / analysisSpeed;

            while (elapsed < duration)
            {
                currentAnalysisProgress = elapsed / duration;
                elapsed += Time.deltaTime;

                // Feed understanding into ActivationLogic
                if (activationLogic != null)
                    activationLogic.SetUnderstanding(currentAnalysisProgress * 0.8f);

                yield return null;
            }

            currentAnalysisProgress = 1f;

            // Phase 3: Pattern recorded
            RecordPattern(patternID);
            patternsAnalyzed++;

            // Phase 4: Notebook entry
            if (autoWritesInNotebook)
            {
                yield return new WaitForSeconds(notebookWriteDelay);
                if (animator != null) animator.SetTrigger(HashWrite);
                string entry = GenerateNotebookEntry(patternID);
                OnNotebookEntry?.Invoke(entry);
            }

            OnPatternAnalyzed?.Invoke(patternID);
            OnAnalysisComplete?.Invoke();

            if (animator != null) animator.SetBool(HashIsAnalyzing, false);
            isAnalyzing = false;

            Debug.Log($"[LumiAnalyzer] Analysis complete: {patternID}");
        }

        private void UpdateAnalysisProgress()
        {
            // Clean up expired pattern memories
            float now = Time.time;
            trackedPatterns.RemoveAll(p => now - p.lastSeen > patternMemoryDuration);
        }

        // ─── Pattern Memory ──────────────────────────────────────────────────

        private void RecordPattern(string patternID)
        {
            var existing = trackedPatterns.Find(p => p.patternID == patternID);
            if (existing != null)
            {
                existing.occurrences++;
                existing.lastSeen = Time.time;
                existing.isComplete = existing.occurrences >= 3;
            }
            else
            {
                if (trackedPatterns.Count >= maxPatternsTracked)
                    trackedPatterns.RemoveAt(0); // Remove oldest

                trackedPatterns.Add(new PatternRecord
                {
                    patternID = patternID,
                    firstSeen = Time.time,
                    lastSeen = Time.time,
                    occurrences = 1,
                    isComplete = false
                });
            }
        }

        public bool HasAnalyzedPattern(string patternID)
        {
            return trackedPatterns.Exists(p => p.patternID == patternID && p.isComplete);
        }

        public int GetPatternOccurrences(string patternID)
        {
            var record = trackedPatterns.Find(p => p.patternID == patternID);
            return record?.occurrences ?? 0;
        }

        // ─── Notebook Entry Generation ───────────────────────────────────────

        private string GenerateNotebookEntry(string patternID)
        {
            var record = trackedPatterns.Find(p => p.patternID == patternID);
            if (record == null) return $"Pattern {patternID}: observed.";

            return $"PATTERN: {patternID}\n" +
                   $"Occurrences: {record.occurrences}\n" +
                   $"Duration: {Time.time - record.firstSeen:F1}s\n" +
                   $"Status: {(record.isComplete ? "Complete" : "Incomplete")}\n" +
                   $"Lumi: \"{analysisLines[0]}\"";
        }

        // ─── Dialogue ────────────────────────────────────────────────────────

        public void SayLine(string line)
        {
            OnDialogueLine?.Invoke(line);
            Debug.Log($"[Lumi] \"{line}\"");
        }

        public void SayWaitLine()
        {
            SayLine(waitLines[Random.Range(0, waitLines.Length)]);
        }

        // ─── Trio Integration ────────────────────────────────────────────────

        /// <summary>
        /// Called by TrioSystem when synchronization is needed.
        /// Lumi contributes Understanding to the activation.
        /// </summary>
        public float ContributeUnderstanding()
        {
            float contribution = Mathf.Clamp01(currentAnalysisProgress + (patternsAnalyzed * 0.1f));
            if (activationLogic != null)
                activationLogic.SetUnderstanding(contribution);

            if (animator != null) animator.SetTrigger(HashPoint);
            return contribution;
        }

        // ─── Gizmos ──────────────────────────────────────────────────────────

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, analysisRadius);
        }
    }
}