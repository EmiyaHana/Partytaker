using UnityEngine;

public class EndLevelNPC : GridEntity
{
    [Header("NPC Settings")]
    public string npcName = "";
    public string nextSceneName = "";

    private void Awake()
    {
        SetGridPosition(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
    }

    public void TriggerInteraction()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.nextSceneToLoad = nextSceneName;
            DialogueManager.Instance.StartDialogue(npcName);
        }
    }
}