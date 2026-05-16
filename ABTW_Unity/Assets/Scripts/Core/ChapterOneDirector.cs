using UnityEngine;
using System.Collections;
using ABTW.Systems;
using ABTW.UI;

namespace ABTW.Core
{
    /// <summary>
    /// Chapter 1 Director — "The First Glitch"
    /// Orchestrates the narrative flow of Chapter 1 beat by beat.
    ///
    /// BEAT STRUCTURE (per GDD):
    ///   Beat 0 — Opening: no UI, player walks, shadow hint
    ///   Beat 1 — First Notice: shadow delay detected
    ///   Beat 2 — Second + Third glitch found
    ///   Beat 3 — Notebook unlocks (Lumi: "it's repeating")
    ///   Beat 4 — Player decides: Record or Wait
    ///   Beat 5a — Record path: world stabilises, Echo scheduled
    ///   Beat 5b — Wait path: Trio activates, Hidden Layer opens
    ///   Beat 6 — Chapter end: "It's not random."
    /// </summary>
    public class ChapterOneDirector : MonoBehaviour
    {
        [Header("— NARRATIVE BEATS —")]
        [SerializeField] private float openingDuration    = 8f;   // walk before any UI
        [SerializeField] private float beat1Delay         = 3f;   // after first anomaly found
        [SerializeField] private float chapterEndDelay    = 5f;

        [Header("— OPENING DIALOGUE —")]
        [SerializeField] private string[] openingLines = {
            "The Academy feels… different today.",
            "Something in the light.",
            "Or maybe the shadows."
        };

        [Header("— CHAPTER END —")]
        [SerializeField] private string chapterEndLine = "It's not random.";
        [SerializeField] private GameObject nextChapterTrigger;

        // State
        private int   _beat = 0;
        private bool  _chapterComplete;

        // ── Unity ────────────────────────────────────────────────────────────

        private void Start()
        {
            StartCoroutine(RunChapterOne());

            // Subscribe to game events
            var gm = GameManager.Instance;
            if (gm?.notebookSystem != null)
            {
                gm.notebookSystem.OnPatternRecorded += _ => OnPatternResolved(recorded: true);
                gm.notebookSystem.OnPatternWaited   += _ => OnPatternResolved(recorded: false);
            }
        }

        // ── Private ──────────────────────────────────────────────────────────

        private IEnumerator RunChapterOne()
        {
            // ── BEAT 0: Opening ──────────────────────────────────────────────
            _beat = 0;
            Debug.Log("[Ch1] Beat 0 — Opening");

            // No UI for first 8 seconds — player just walks
            yield return new WaitForSeconds(openingDuration * 0.4f);

            // Subtle first line from Astra (internal monologue)
            ShowAstraThought(openingLines[0]);
            yield return new WaitForSeconds(openingDuration * 0.3f);

            ShowAstraThought(openingLines[1]);
            yield return new WaitForSeconds(openingDuration * 0.3f);

            // ── BEAT 1: Hint ─────────────────────────────────────────────────
            _beat = 1;
            Debug.Log("[Ch1] Beat 1 — Shadow hint active");

            // Shadow delay anomaly is already in scene — player must find it
            // Perception Mode hint appears subtly
            yield return new WaitForSeconds(beat1Delay);

            GameManager.Instance?.uiManager?.ShowNotification(
                "Something feels… off.", NotificationType.Perception);

            // ── BEAT 2: Wait for anomalies ───────────────────────────────────
            _beat = 2;
            Debug.Log("[Ch1] Beat 2 — Waiting for player to find anomalies");

            // Director waits — anomalies fire their own events
            // GameManager.OnAnomalyFound handles progression
            yield return new WaitUntil(() =>
                GameManager.Instance != null &&
                GameManager.Instance.AnomaliesFound >= 1);

            // ── BEAT 3: Notebook unlock ──────────────────────────────────────
            _beat = 3;
            Debug.Log("[Ch1] Beat 3 — Notebook unlocked");

            yield return new WaitForSeconds(1.5f);
            ShowAstraThought(openingLines[2]);

            // Lumi's key line fires from LumiCompanion automatically
            // Notebook opens via NotebookSystem event

            // ── BEAT 4: Wait for player decision ────────────────────────────
            _beat = 4;
            Debug.Log("[Ch1] Beat 4 — Awaiting player decision");

            yield return new WaitUntil(() =>
                GameManager.Instance != null &&
                (GameManager.Instance.PatternsRecorded >= 1 ||
                 TrioSystem.WasTrioActiveRecently));

            // ── BEAT 5: Resolution ───────────────────────────────────────────
            _beat = 5;
            Debug.Log("[Ch1] Beat 5 — Pattern resolved");

            yield return new WaitForSeconds(chapterEndDelay);

            // ── BEAT 6: Chapter End ──────────────────────────────────────────
            _beat = 6;
            EndChapter();
        }

        private void OnPatternResolved(bool recorded)
        {
            if (_chapterComplete) return;

            if (recorded)
            {
                Debug.Log("[Ch1] Path A — Record chosen");
                // Echo will be scheduled by EchoSystem automatically
            }
            else
            {
                Debug.Log("[Ch1] Path B — Wait chosen → Trio path");
            }
        }

        private void EndChapter()
        {
            if (_chapterComplete) return;
            _chapterComplete = true;

            Debug.Log("[Ch1] Chapter 1 complete.");

            // Final Astra thought
            ShowAstraThought(chapterEndLine);

            // Unlock next chapter trigger
            if (nextChapterTrigger != null)
                nextChapterTrigger.SetActive(true);

            // Award chapter completion EXP
            GameManager.Instance?.playerStats.AddEXP(500);
            GameManager.Instance?.uiManager?.ShowNotification(
                "Chapter 1 complete.\nThe pattern was real.",
                NotificationType.General);
        }

        private void ShowAstraThought(string thought)
        {
            GameManager.Instance?.uiManager?.ShowCompanionDialogue("Astra", thought);
        }
    }
}