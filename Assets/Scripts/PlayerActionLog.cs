using UnityEngine;

[System.Serializable]
public class PlayerActionLog
{
    public int undoCount = 0;
    public int redoCount = 0;
    public int enemiesKilled = 0;
    public int trapsTriggered = 0;
    public int stepsTaken = 0;

    public void ResetLog()
    {
        undoCount = 0;
        redoCount = 0;
        enemiesKilled = 0;
        trapsTriggered = 0;
        stepsTaken = 0;
    }
}