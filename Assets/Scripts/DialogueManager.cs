using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Tree Nodes")]
    public DialogueNode goodRouteNode;
    public DialogueNode normalRouteNode;
    public DialogueNode currentNode;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueBodyText;
    public List<Button> optionButtons;
    public List<TextMeshProUGUI> optionButtonTexts;

    [Header("Level Transition")]
    public string nextSceneToLoad;
    public bool isDialogueActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string npcName = "NPC")
    {
        isDialogueActive = true;
        GameManager.Instance.isSoftLocked = true;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (npcNameText != null) npcNameText.text = npcName;

        PlayerActionLog log = GameManager.Instance.actionLog;

        if (log.undoCount <= 2 && log.enemiesKilled >= 1 && goodRouteNode != null)
        {
            currentNode = goodRouteNode;
        }
        else
        {
            currentNode = normalRouteNode;
        }

        DisplayCurrentNode();
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

        if (dialogueBodyText != null)
            dialogueBodyText.text = currentNode.dialogueText;

        for (int i = 0; i < optionButtons.Count; i++)
        {
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].gameObject.SetActive(false);
        }

        if (currentNode.IsEndingNode)
        {
            TriggerEnding(currentNode.endingType);
            return;
        }

        for (int i = 0; i < currentNode.options.Count && i < optionButtons.Count; i++)
        {
            int index = i;
            optionButtons[i].gameObject.SetActive(true);
            optionButtonTexts[i].text = currentNode.options[i].optionText;
            optionButtons[i].onClick.AddListener(() => SelectOption(index));
        }
    }

    private void TriggerEnding(EndingType ending)
    {
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
                    GameManager.Instance.TriggerGameOver();
                    optionButtons[0].onClick.AddListener(() => {
                        dialoguePanel.SetActive(false);
                        isDialogueActive = false;
                        GameManager.Instance.RestartLevel();
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