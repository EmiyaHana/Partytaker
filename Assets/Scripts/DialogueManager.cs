using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Tree Nodes")]
    public DialogueNode rootNode;
    public DialogueNode currentNode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // เริ่มการพูดคุยและประเมินผลจาก Log การเล่น
    public void StartDialogue()
    {
        PlayerActionLog log = GameManager.Instance.actionLog;

        Debug.Log($"--- ประเมินผลการเล่น ---");
        Debug.Log($"กด Undo ทั้งหมด: {log.undoCount} ครั้ง | ฆ่าศัตรู: {log.enemiesKilled} ตัว");

        // ตัวอย่าง Logic การประเมินผลเข้า Tree Node:
        // ถ้าไม่เคยกด Undo เลย และฆ่าศัตรูครบ -> ได้เปิดบทสนทนาทางเลือก Good Ending
        // ถ้ากด Undo บ่อย หรือไม่ฆ่าศัตรูเลย -> เข้าสู่เงื่อนไขบทสนทนาทั่วไป/Bad Ending
        
        currentNode = rootNode;
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

        Debug.Log($"[Dialogue]: {currentNode.dialogueText}");

        if (currentNode.IsEndingNode)
        {
            TriggerEnding(currentNode.endingType);
        }
    }

    private void TriggerEnding(EndingType ending)
    {
        switch (ending)
        {
            case EndingType.GoodEnding:
                Debug.Log("สรุปผล: ได้รับ Good Ending! ผ่านด่านเรียบร้อย");
                break;
            case EndingType.NeutralEnding:
                Debug.Log("สรุปผล: ได้รับ Normal Ending! ผ่านด่าน");
                break;
            case EndingType.BadEnding:
                Debug.Log("สรุปผล: ได้รับ Bad Ending! GAME OVER - บังคับให้กด Reset Level");
                GameManager.Instance.TriggerGameOver();
                break;
        }
    }
}