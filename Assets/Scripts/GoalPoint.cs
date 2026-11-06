using UnityEngine;

public class GoalPoint : GridEntity
{
    private void Start()
    {
        SetGridPosition(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
    }

    public void CheckGoal(Vector2Int playerPos)
    {
        if (playerPos == gridPosition)
        {
            Debug.Log("ผู้เล่นถึงจุดหมายแล้ว! กำลังเริ่มบทสนทนาประเมินผล...");
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue();
            }
        }
    }
}