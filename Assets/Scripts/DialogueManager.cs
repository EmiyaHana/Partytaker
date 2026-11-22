using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Start")]
    [Tooltip("Start Node")]
    public DialogueNode startNode;
    public DialogueNode currentNode;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public Image characterPortraitImage;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueBodyText;
    public GameObject pressEnterHintText;
    public List<Button> optionButtons;
    public List<TextMeshProUGUI> optionButtonTexts;

    [Header("Level Transition")]
    public string nextSceneToLoad;
    public bool isDialogueActive = false;

    private string baseNpcName = "NPC";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isDialogueActive || currentNode == null) return;

        if (!currentNode.HasChoices && !currentNode.IsEndingNode)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
            {
                AdvanceDialogue();
            }
        }
    }

    public void StartDialogue(string npcName = "NPC")
    {
        isDialogueActive = true;
        baseNpcName = npcName;
        GameManager.Instance.isSoftLocked = true;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (npcNameText != null) npcNameText.text = baseNpcName;

        PlayerActionLog log = GameManager.Instance.actionLog;

        currentNode = startNode;
        DisplayCurrentNode();
    }

    public void AdvanceDialogue()
    {
        if (currentNode == null) return;

        PlayerActionLog log = GameManager.Instance.actionLog;

        foreach (var rule in currentNode.conditionBranches)
        {
            if (rule.Evaluate(log) && rule.targetNode != null)
            {
                currentNode = rule.targetNode;
                DisplayCurrentNode();
                return;
            }
        }

        if (currentNode.defaultNextNode != null)
        {
            currentNode = currentNode.defaultNextNode;
            DisplayCurrentNode();
        }
    }

    public void SelectOption(int optionIndex)
    {
        if (currentNode == null || optionIndex >= currentNode.options.Count) return;

        currentNode = currentNode.options[optionIndex].nextNode;
        DisplayCurrentNode();
    }

    private void DisplayCurrentNode()
    {
       if (currentNode == null) return;

       if (npcNameText != null)
       {
           npcNameText.text = string.IsNullOrEmpty(currentNode.overrideSpeakerName) 
               ? baseNpcName 
               : currentNode.overrideSpeakerName;
       }

       if (characterPortraitImage != null && currentNode.characterPortrait != null)
       {
           characterPortraitImage.sprite = currentNode.characterPortrait;
           characterPortraitImage.gameObject.SetActive(true);
       }

       if (dialogueBodyText != null)
           dialogueBodyText.text = currentNode.dialogueText;

       for (int i = 0; i < optionButtons.Count; i++)
       {
           optionButtons[i].onClick.RemoveAllListeners();
           optionButtons[i].gameObject.SetActive(false);
       }

       if (currentNode.IsEndingNode)
       {
           if (pressEnterHintText != null) pressEnterHintText.SetActive(false);
           TriggerEnding(currentNode.endingType);
           return;
       }

       if (currentNode.HasChoices)
       {
           if (pressEnterHintText != null) pressEnterHintText.SetActive(false);

           for (int i = 0; i < currentNode.options.Count && i < optionButtons.Count; i++)
           {
               int index = i;
               optionButtons[i].gameObject.SetActive(true);
               optionButtonTexts[i].text = currentNode.options[i].optionText;
               optionButtons[i].onClick.AddListener(() => SelectOption(index));
           }
       }

       else
       {
           if (pressEnterHintText != null) pressEnterHintText.SetActive(true);
       }
    }

    private void TriggerEnding(EndingType ending)
    {
        if (ending == EndingType.FinalVictory)
        {
            if (optionButtons.Count > 0 && optionButtonTexts.Count > 0)
            {
                optionButtons[0].gameObject.SetActive(true);
                optionButtons[0].onClick.RemoveAllListeners();
                optionButtonTexts[0].text = "Finish";
                optionButtons[0].onClick.AddListener(() => {
                    dialoguePanel.SetActive(false);
                    isDialogueActive = false;
                    if (UIManager.Instance != null) UIManager.Instance.ShowVictoryPanel();
                });
            }
            return;
        }

        if (optionButtons.Count > 0 && optionButtonTexts.Count > 0)
        {
            optionButtons[0].gameObject.SetActive(true);
            optionButtons[0].onClick.RemoveAllListeners();

            switch (ending)
            {
                case EndingType.GoodEnding:
                case EndingType.NeutralEnding:
                    optionButtonTexts[0].text = "Go to next level.";
                    optionButtons[0].onClick.AddListener(LoadNextLevel);
                    break;

                case EndingType.BadEnding:
                    optionButtonTexts[0].text = "Retry.";
                    optionButtons[0].onClick.AddListener(() => {
                        dialoguePanel.SetActive(false);
                        isDialogueActive = false;
                        if (UIManager.Instance != null) UIManager.Instance.ShowGameOverPanel();
                        else GameManager.Instance.RestartLevel();
                    });
                    break;
            }
        }
    }

    public void LoadNextLevel()
    {
        isDialogueActive = false;
        if (!string.IsNullOrEmpty(nextSceneToLoad))
        {
            SceneManager.LoadScene(nextSceneToLoad);
        }
        else
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }
    }
}