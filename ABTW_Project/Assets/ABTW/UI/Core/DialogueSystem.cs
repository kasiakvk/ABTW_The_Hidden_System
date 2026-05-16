using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ABTW.UI
{
    /// <summary>
    /// Dialogue System — handles all character conversations and world responses.
    /// Minimal, cinematic style. No dialogue boxes — subtle text overlays.
    /// Decisions made through dialogue affect the world state.
    /// </summary>
    public class DialogueSystem : MonoBehaviour
    {
        public enum Speaker { Astra, Lumi, Nela, World, System }

        [System.Serializable]
        public class DialogueLine
        {
            public Speaker speaker;
            [TextArea(1, 3)] public string text;
            public float displayDuration = 3f;
            public float pauseAfter = 0.5f;
            public bool waitForInput = false;
            public AudioClip voiceClip;
        }

        [System.Serializable]
        public class DialogueChoice
        {
            [TextArea(1, 2)] public string choiceText;
            public string choiceID;
            public UnityEngine.Events.UnityEvent onSelected;
        }

        [System.Serializable]
        public class DialogueSequence
        {
            public string sequenceID;
            public List<DialogueLine> lines = new List<DialogueLine>();
            public List<DialogueChoice> choices = new List<DialogueChoice>();
            public bool hasChoices = false;
            public string nextSequenceID = "";
        }

        [Header("UI References")]
        public CanvasGroup dialogueGroup;
        public Text speakerNameText;
        public Text dialogueText;
        public Image speakerIndicator;
        public CanvasGroup choicePanel;
        public Transform choiceContainer;
        public GameObject choiceButtonPrefab;

        [Header("Typewriter Settings")]
        public float typewriterSpeed = 0.04f;   // seconds per character
        public float fastTypewriterSpeed = 0.01f;
        public bool useTypewriter = true;

        [Header("Speaker Colors")]
        public Color astraColor   = new Color(0.85f, 0.85f, 1f);
        public Color lumiColor    = new Color(0.6f, 0.85f, 1f);
        public Color nelaColor    = new Color(1f, 0.85f, 0.6f);
        public Color worldColor   = new Color(0.8f, 0.9f, 0.8f);
        public Color systemColor  = new Color(0.7f, 0.7f, 0.7f);

        [Header("Speaker Names")]
        public string astraName  = "Astra";
        public string lumiName   = "Lumi";
        public string nelaName   = "Nela";
        public string worldName  = "…";
        public string systemName = "";

        [Header("Position")]
        public bool positionBySpeaker = true;
        // Astra = bottom center, Lumi = bottom left, Nela = bottom right
        public Vector2 astraPosition  = new Vector2(0f, -280f);
        public Vector2 lumiPosition   = new Vector2(-320f, -280f);
        public Vector2 nelaPosition   = new Vector2(320f, -280f);
        public Vector2 worldPosition  = new Vector2(0f, -200f);

        [Header("Audio")]
        public AudioSource audioSource;

        // State
        private bool isPlaying = false;
        private bool skipRequested = false;
        private bool choiceActive = false;
        private string lastChoiceSelected = "";
        private Queue<DialogueLine> lineQueue = new Queue<DialogueLine>();
        private List<GameObject> activeChoiceButtons = new List<GameObject>();
        private Coroutine playCoroutine;

        // Sequence library
        private Dictionary<string, DialogueSequence> sequenceLibrary
            = new Dictionary<string, DialogueSequence>();

        // Events
        public System.Action<string> OnSequenceComplete;
        public System.Action<string> OnChoiceSelected;
        public System.Action<Speaker, string> OnLineDisplayed;

        private void Start()
        {
            if (dialogueGroup != null) dialogueGroup.alpha = 0f;
            if (choicePanel != null) choicePanel.alpha = 0f;
        }

        private void Update()
        {
            // Skip on Space or mouse click
            if (isPlaying && Input.GetKeyDown(KeyCode.Space))
                skipRequested = true;
        }

        // ─── Sequence Library ────────────────────────────────────────────────

        public void RegisterSequence(DialogueSequence sequence)
        {
            if (sequence != null)
                sequenceLibrary[sequence.sequenceID] = sequence;
        }

        public void PlaySequence(string sequenceID)
        {
            if (!sequenceLibrary.ContainsKey(sequenceID))
            {
                Debug.LogWarning($"[DialogueSystem] Sequence not found: {sequenceID}");
                return;
            }
            PlaySequence(sequenceLibrary[sequenceID]);
        }

        public void PlaySequence(DialogueSequence sequence)
        {
            if (isPlaying) StopDialogue();

            if (playCoroutine != null) StopCoroutine(playCoroutine);
            playCoroutine = StartCoroutine(PlaySequenceRoutine(sequence));
        }

        // ─── Quick Play ──────────────────────────────────────────────────────

        public void SayLine(Speaker speaker, string text, float duration = 3f)
        {
            var line = new DialogueLine
            {
                speaker = speaker,
                text = text,
                displayDuration = duration,
                waitForInput = false
            };

            if (isPlaying)
            {
                lineQueue.Enqueue(line);
            }
            else
            {
                if (playCoroutine != null) StopCoroutine(playCoroutine);
                playCoroutine = StartCoroutine(PlaySingleLine(line));
            }
        }

        public void SayAstra(string text, float duration = 3f) => SayLine(Speaker.Astra, text, duration);
        public void SayLumi(string text, float duration = 3f)  => SayLine(Speaker.Lumi, text, duration);
        public void SayNela(string text, float duration = 3f)  => SayLine(Speaker.Nela, text, duration);
        public void SayWorld(string text, float duration = 4f) => SayLine(Speaker.World, text, duration);

        // ─── Coroutines ──────────────────────────────────────────────────────

        private IEnumerator PlaySequenceRoutine(DialogueSequence sequence)
        {
            isPlaying = true;
            yield return StartCoroutine(FadeDialogue(1f, 0.3f));

            foreach (var line in sequence.lines)
            {
                yield return StartCoroutine(DisplayLine(line));

                // Process queued lines
                while (lineQueue.Count > 0)
                    yield return StartCoroutine(DisplayLine(lineQueue.Dequeue()));
            }

            // Show choices if any
            if (sequence.hasChoices && sequence.choices.Count > 0)
            {
                yield return StartCoroutine(ShowChoices(sequence.choices));
            }

            yield return StartCoroutine(FadeDialogue(0f, 0.5f));
            isPlaying = false;

            OnSequenceComplete?.Invoke(sequence.sequenceID);

            // Chain to next sequence
            if (!string.IsNullOrEmpty(sequence.nextSequenceID))
                PlaySequence(sequence.nextSequenceID);
        }

        private IEnumerator PlaySingleLine(DialogueLine line)
        {
            isPlaying = true;
            yield return StartCoroutine(FadeDialogue(1f, 0.2f));
            yield return StartCoroutine(DisplayLine(line));

            // Process any queued lines
            while (lineQueue.Count > 0)
                yield return StartCoroutine(DisplayLine(lineQueue.Dequeue()));

            yield return StartCoroutine(FadeDialogue(0f, 0.5f));
            isPlaying = false;
        }

        private IEnumerator DisplayLine(DialogueLine line)
        {
            skipRequested = false;

            // Set speaker
            ApplySpeakerStyle(line.speaker);
            OnLineDisplayed?.Invoke(line.speaker, line.text);

            // Play voice
            if (audioSource != null && line.voiceClip != null)
                audioSource.PlayOneShot(line.voiceClip);

            // Typewriter effect
            if (useTypewriter)
                yield return StartCoroutine(TypewriterEffect(line.text));
            else if (dialogueText != null)
                dialogueText.text = line.text;

            // Wait for duration or input
            if (line.waitForInput)
            {
                yield return new WaitUntil(() => skipRequested || Input.GetKeyDown(KeyCode.Space));
            }
            else
            {
                float elapsed = 0f;
                while (elapsed < line.displayDuration && !skipRequested)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }

            yield return new WaitForSeconds(line.pauseAfter);
        }

        private IEnumerator TypewriterEffect(string text)
        {
            if (dialogueText == null) yield break;

            dialogueText.text = "";
            foreach (char c in text)
            {
                if (skipRequested)
                {
                    dialogueText.text = text;
                    yield break;
                }

                dialogueText.text += c;
                float delay = char.IsPunctuation(c) ? typewriterSpeed * 4f : typewriterSpeed;
                yield return new WaitForSeconds(delay);
            }
        }

        // ─── Choices ─────────────────────────────────────────────────────────

        private IEnumerator ShowChoices(List<DialogueChoice> choices)
        {
            choiceActive = true;
            lastChoiceSelected = "";

            // Clear old buttons
            ClearChoiceButtons();

            // Create choice buttons
            foreach (var choice in choices)
            {
                if (choiceButtonPrefab == null || choiceContainer == null) break;

                var btnObj = Instantiate(choiceButtonPrefab, choiceContainer);
                activeChoiceButtons.Add(btnObj);

                var btnText = btnObj.GetComponentInChildren<Text>();
                if (btnText != null) btnText.text = choice.choiceText;

                var btn = btnObj.GetComponent<Button>();
                if (btn != null)
                {
                    string choiceID = choice.choiceID;
                    UnityEngine.Events.UnityEvent onSelected = choice.onSelected;
                    btn.onClick.AddListener(() =>
                    {
                        lastChoiceSelected = choiceID;
                        onSelected?.Invoke();
                        OnChoiceSelected?.Invoke(choiceID);
                        choiceActive = false;
                    });
                }
            }

            // Show choice panel
            if (choicePanel != null)
                yield return StartCoroutine(FadeCanvasGroup(choicePanel, 1f, 0.3f));

            // Wait for selection
            yield return new WaitUntil(() => !choiceActive);

            // Hide choice panel
            if (choicePanel != null)
                yield return StartCoroutine(FadeCanvasGroup(choicePanel, 0f, 0.3f));

            ClearChoiceButtons();
        }

        private void ClearChoiceButtons()
        {
            foreach (var btn in activeChoiceButtons)
                if (btn != null) Destroy(btn);
            activeChoiceButtons.Clear();
        }

        // ─── Speaker Styling ─────────────────────────────────────────────────

        private void ApplySpeakerStyle(Speaker speaker)
        {
            string name = GetSpeakerName(speaker);
            Color color = GetSpeakerColor(speaker);

            if (speakerNameText != null)
            {
                speakerNameText.text = name;
                speakerNameText.color = color;
            }

            if (dialogueText != null)
                dialogueText.color = color;

            if (speakerIndicator != null)
                speakerIndicator.color = color;

            // Reposition dialogue box
            if (positionBySpeaker && dialogueGroup != null)
            {
                var rect = dialogueGroup.GetComponent<RectTransform>();
                if (rect != null)
                    rect.anchoredPosition = GetSpeakerPosition(speaker);
            }
        }

        private string GetSpeakerName(Speaker speaker) => speaker switch
        {
            Speaker.Astra  => astraName,
            Speaker.Lumi   => lumiName,
            Speaker.Nela   => nelaName,
            Speaker.World  => worldName,
            Speaker.System => systemName,
            _ => ""
        };

        private Color GetSpeakerColor(Speaker speaker) => speaker switch
        {
            Speaker.Astra  => astraColor,
            Speaker.Lumi   => lumiColor,
            Speaker.Nela   => nelaColor,
            Speaker.World  => worldColor,
            Speaker.System => systemColor,
            _ => Color.white
        };

        private Vector2 GetSpeakerPosition(Speaker speaker) => speaker switch
        {
            Speaker.Astra  => astraPosition,
            Speaker.Lumi   => lumiPosition,
            Speaker.Nela   => nelaPosition,
            Speaker.World  => worldPosition,
            _ => astraPosition
        };

        // ─── Control ─────────────────────────────────────────────────────────

        public void StopDialogue()
        {
            if (playCoroutine != null) StopCoroutine(playCoroutine);
            isPlaying = false;
            choiceActive = false;
            lineQueue.Clear();
            ClearChoiceButtons();
            if (dialogueGroup != null) dialogueGroup.alpha = 0f;
        }

        public bool IsPlaying => isPlaying;
        public string LastChoiceSelected => lastChoiceSelected;

        // ─── Fade Utilities ──────────────────────────────────────────────────

        private IEnumerator FadeDialogue(float target, float duration)
            => FadeCanvasGroup(dialogueGroup, target, duration);

        private IEnumerator FadeCanvasGroup(CanvasGroup group, float target, float duration)
        {
            if (group == null) yield break;
            float start = group.alpha;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                group.alpha = Mathf.Lerp(start, target, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            group.alpha = target;
            group.interactable = target > 0.5f;
            group.blocksRaycasts = target > 0.5f;
        }
    }
}