using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockData
{
    public PushBlock block;
    public Vector2Int position;

    public BlockData(PushBlock b, Vector2Int pos)
    {
        block = b;
        position = pos;
    }
}

[System.Serializable]
public class TrapData
{
    public HazardTrap trap;
    public bool isActive;

    public TrapData(HazardTrap t, bool state)
    {
        trap = t;
        isActive = state;
    }
}

[System.Serializable]
public class EnemyData
{
    public EnemyEntity enemy;
    public Vector2Int position;
    public bool isAlive;

    public EnemyData(EnemyEntity e, Vector2Int pos, bool alive)
    {
        enemy = e;
        position = pos;
        isAlive = alive;
    }
}

[System.Serializable]
public class PlayerState
{
    public Vector2Int playerPosition;
    public int remainingTurns;
    public bool isKeyCollected;
    public bool isDoorUnlocked;
    public List<BlockData> blocksState;
    public List<TrapData> trapsState;
    public List<EnemyData> enemiesState;

    public PlayerState(Vector2Int pos, int turns, bool keyState, bool doorState, List<BlockData> blocks, List<TrapData> traps, List<EnemyData> enemies)
    {
        playerPosition = pos;
        remainingTurns = turns;
        isKeyCollected = keyState;
        isDoorUnlocked = doorState;

        blocksState = new List<BlockData>();
        foreach (var b in blocks) blocksState.Add(new BlockData(b.block, b.position));

        trapsState = new List<TrapData>();
        foreach (var t in traps) trapsState.Add(new TrapData(t.trap, t.isActive));

        enemiesState = new List<EnemyData>();
        foreach (var e in enemies) enemiesState.Add(new EnemyData(e.enemy, e.position, e.isAlive));
    }
}