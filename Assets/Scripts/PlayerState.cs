using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public Vector2Int playerPosition;
    public int remainingTurns;
    public bool isKeyCollected;
    public bool isDoorUnlocked;

    // Constructor สำหรับสร้าง Snapshot ณ ก้าวนั้นๆ
    public PlayerState(Vector2Int pos, int turns, bool keyState, bool doorState)
    {
        playerPosition = pos;
        remainingTurns = turns;
        isKeyCollected = keyState;
        isDoorUnlocked = doorState;
    }
}