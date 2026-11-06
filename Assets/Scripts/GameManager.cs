using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Turn Settings")]
    public int maxTurns = 20;
    public int currentTurns;
    public bool isSoftLocked = false;

    [Header("Data Structures")]
    private Stack<PlayerState> undoStack = new Stack<PlayerState>();
    private Stack<PlayerState> redoStack = new Stack<PlayerState>();

    [Header("Action Tracker")]
    public PlayerActionLog actionLog = new PlayerActionLog();

    [Header("Level Objects")]
    public Vector2Int currentPlayerGridPos;
    public bool hasKey = false;
    public bool isDoorOpen = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitLevel(maxTurns);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) Undo();
        if (Input.GetKeyDown(KeyCode.R)) Redo();
    }

    public void InitLevel(int turns)
    {
        maxTurns = turns;
        currentTurns = maxTurns;
        isSoftLocked = false;
        hasKey = false;
        isDoorOpen = false;
        undoStack.Clear();
        redoStack.Clear();
        actionLog.ResetLog();
        
        RecordState();
    }

    public void RecordState()
    {
        PushBlock[] blocks = FindObjectsOfType<PushBlock>(true);
        List<BlockData> blockDataList = new List<BlockData>();
        foreach (var b in blocks) blockDataList.Add(new BlockData(b, b.gridPosition));

        HazardTrap[] traps = FindObjectsOfType<HazardTrap>(true);
        List<TrapData> trapDataList = new List<TrapData>();
        foreach (var t in traps) trapDataList.Add(new TrapData(t, t.isActive));

        EnemyEntity[] enemies = FindObjectsOfType<EnemyEntity>(true);
        List<EnemyData> enemyDataList = new List<EnemyData>();
        foreach (var e in enemies) enemyDataList.Add(new EnemyData(e, e.gridPosition, e.isAlive));

        PlayerState snapshot = new PlayerState(currentPlayerGridPos, currentTurns, hasKey, isDoorOpen, blockDataList, trapDataList, enemyDataList);
        undoStack.Push(snapshot);
        redoStack.Clear();
    }

    public void UseTurn(int cost = 1)
    {
        currentTurns -= cost;

        HazardTrap[] traps = FindObjectsOfType<HazardTrap>();
        foreach (var trap in traps)
        {
            trap.ToggleState();
        }

        if (currentTurns <= 0)
        {
            currentTurns = 0;
            isSoftLocked = true;
            Debug.Log("ก้าวหมดแล้ว! ติด Soft-Lock - กรุณากด Undo เพื่อย้อนเวลา");
        }
    }

    public void Undo()
    {
        if (undoStack.Count <= 1) return;

        PlayerState currentState = undoStack.Pop();
        redoStack.Push(currentState);

        PlayerState previousState = undoStack.Peek();
        ApplyState(previousState);

        actionLog.undoCount++;
    }

    public void Redo()
    {
        if (redoStack.Count == 0) return;

        PlayerState nextState = redoStack.Pop();
        undoStack.Push(nextState);
        ApplyState(nextState);

        actionLog.redoCount++;
    }

    private void ApplyState(PlayerState state)
    {
        currentPlayerGridPos = state.playerPosition;
        currentTurns = state.remainingTurns;
        hasKey = state.isKeyCollected;
        isDoorOpen = state.isDoorUnlocked;

        if (currentTurns > 0) isSoftLocked = false;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) player.SetGridPosition(state.playerPosition);

        foreach (var bData in state.blocksState)
        {
            if (bData.block != null) bData.block.SetGridPosition(bData.position);
        }

        foreach (var tData in state.trapsState)
        {
            if (tData.trap != null) tData.trap.SetState(tData.isActive);
        }

        foreach (var eData in state.enemiesState)
        {
            if (eData.enemy != null) eData.enemy.SetAliveState(eData.isAlive, eData.position);
        }

        KeyItem key = FindObjectOfType<KeyItem>(true);
        if (key != null) key.SetCollected(hasKey);

        DoorBlock door = FindObjectOfType<DoorBlock>(true);
        if (door != null) door.SetUnlocked(isDoorOpen);
    }

    public bool CanMoveTo(Vector2Int targetPos)
    {
        if (GetBlockAt(targetPos) != null) return false;
        if (GetEnemyAt(targetPos) != null) return false;

        DoorBlock door = GetDoorAt(targetPos);
        if (door != null && !isDoorOpen) return false;

        return true;
    }

    public PushBlock GetBlockAt(Vector2Int pos)
    {
        PushBlock[] blocks = FindObjectsOfType<PushBlock>();
        foreach (var b in blocks)
        {
            if (b.gameObject.activeSelf && b.gridPosition == pos) return b;
        }
        return null;
    }

    public DoorBlock GetDoorAt(Vector2Int pos)
    {
        DoorBlock[] doors = FindObjectsOfType<DoorBlock>(true);
        foreach (var d in doors)
        {
            if (d.gridPosition == pos) return d;
        }
        return null;
    }

    public EnemyEntity GetEnemyAt(Vector2Int pos)
    {
        EnemyEntity[] enemies = FindObjectsOfType<EnemyEntity>();
        foreach (var e in enemies)
        {
            if (e.gameObject.activeSelf && e.isAlive && e.gridPosition == pos)
            {
                return e;
            }
        }
        return null;
    }
}