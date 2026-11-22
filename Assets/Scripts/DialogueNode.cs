using System.Collections.Generic;
using UnityEngine;

public enum EndingType { None, GoodEnding, NeutralEnding, BadEnding, FinalVictory }

public enum StatType { StepsTaken, UndoCount, RedoCount, EnemiesKilled, TrapsTriggered }

public enum ComparisonType { EqualTo, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual }

[System.Serializable]
public class DialogueConditionRule
{
    [Tooltip("Rule")]
    public string ruleName;
    public StatType statToCheck;
    public ComparisonType comparison;
    public int targetValue;
    [Tooltip("Target Node")]
    public DialogueNode targetNode;

    public bool Evaluate(PlayerActionLog log)
    {
        int actualValue = 0;
        switch (statToCheck)
        {
            case StatType.StepsTaken: actualValue = log.stepsTaken; break;
            case StatType.UndoCount: actualValue = log.undoCount; break;
            case StatType.RedoCount: actualValue = log.redoCount; break;
            case StatType.EnemiesKilled: actualValue = log.enemiesKilled; break;
            case StatType.TrapsTriggered: actualValue = log.trapsTriggered; break;
        }

        switch (comparison)
        {
            case ComparisonType.EqualTo: return actualValue == targetValue;
            case ComparisonType.GreaterThan: return actualValue > targetValue;
            case ComparisonType.GreaterThanOrEqual: return actualValue >= targetValue;
            case ComparisonType.LessThan: return actualValue < targetValue;
            case ComparisonType.LessThanOrEqual: return actualValue <= targetValue;
            default: return false;
        }
    }
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueNode nextNode;
}

public class DialogueNode : MonoBehaviour
{
    [Header("Character & Dialogue")]
    [Tooltip("Character Portrait")]
    public Sprite characterPortrait;
    [Tooltip("Name Text")]
    public string overrideSpeakerName;

    [TextArea(3, 5)]
    public string dialogueText;

    [Header("Ending Settings")]
    public EndingType endingType = EndingType.None;

    [Header("Player Choices")]
    public List<DialogueOption> options = new List<DialogueOption>();

    [Header("Auto-Advance / Press Enter")]
    [Tooltip("Condition")]
    public List<DialogueConditionRule> conditionBranches = new List<DialogueConditionRule>();
    [Tooltip("Default Node")]
    public DialogueNode defaultNextNode;

    public bool IsEndingNode => endingType != EndingType.None;
    public bool HasChoices => options != null && options.Count > 0;
}