using System.Collections.Generic;
using UnityEngine;

public enum EndingType { None, GoodEnding, NeutralEnding, BadEnding }

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueNode nextNode;
}

public class DialogueNode : MonoBehaviour
{
    [TextArea(3, 5)]
    public string dialogueText;
    public EndingType endingType = EndingType.None;
    public List<DialogueOption> options = new List<DialogueOption>();

    public bool IsEndingNode => endingType != EndingType.None;
}