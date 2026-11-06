using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Turn Settings")]
    public int maxTurns = 20;
    public int currentTurns;
    public bool isSoftLocked = false;

    [Header("Data Structures (3 Points)")]
    private Stack<PlayerState> undoStack = new Stack<PlayerState>();
    private Stack<PlayerState> redoStack = new Stack<PlayerState>();

    [Header("Action Tracker")]
    public PlayerActionLog actionLog = new PlayerActionLog();

    // References สำหรับระบบในเกม (จะเชื่อมต่อใน Step ถัดๆ ไป)
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

    public void InitLevel(int turns)
    {
        maxTurns = turns;
        currentTurns = maxTurns;
        isSoftLocked = false;
        undoStack.Clear();
        redoStack.Clear();
        actionLog.ResetLog();
        
        // บันทึก State แรกสุดของด่านลง Stack
        RecordState();
    }

    // เรียกฟังก์ชันนี้ทุกครั้งก่อนผู้เล่นขยับตัว หรือทำ Action บน Grid
    public void RecordState()
    {
        PlayerState snapshot = new PlayerState(currentPlayerGridPos, currentTurns, hasKey, isDoorOpen);
        undoStack.Push(snapshot);
        
        // เมื่อมีการเดินใหม่ ต้องล้าง Redo Stack ทิ้ง
        redoStack.Clear();
    }

    public void UseTurn(int cost = 1)
    {
        currentTurns -= cost;
        if (currentTurns <= 0)
        {
            currentTurns = 0;
            isSoftLocked = true;
            Debug.Log("ก้าวหมดแล้ว! ติด Soft-Lock - กรุณากด Undo เพื่อย้อนเวลา");
        }
    }

    // ฟังก์ชัน Undo (ย้อนเวลากลับ 1 ก้าว)
    public void Undo()
    {
        if (undoStack.Count <= 1)
        {
            Debug.Log("ไม่สามารถ Undo ต่อได้แล้ว อยู่ก้าวแรกสุด");
            return;
        }

        // Pop State ปัจจุบันออกไปเก็บใน Redo Stack
        PlayerState currentState = undoStack.Pop();
        redoStack.Push(currentState);

        // ดึง State ก่อนหน้าขึ้นมาใช้งาน
        PlayerState previousState = undoStack.Peek();
        ApplyState(previousState);

        actionLog.undoCount++;
        Debug.Log($"Undo สำเร็จ! ก้าวที่เหลือ: {currentTurns}");
    }

    // ฟังก์ชัน Redo (ทำซ้ำก้าวที่เคย Undo)
    public void Redo()
    {
        if (redoStack.Count == 0)
        {
            Debug.Log("ไม่มีประวัติให้ Redo");
            return;
        }

        PlayerState nextState = redoStack.Pop();
        undoStack.Push(nextState);
        ApplyState(nextState);

        actionLog.redoCount++;
        Debug.Log($"Redo สำเร็จ! ก้าวที่เหลือ: {currentTurns}");
    }

    private void ApplyState(PlayerState state)
    {
        currentPlayerGridPos = state.playerPosition;
        currentTurns = state.remainingTurns;
        hasKey = state.isKeyCollected;
        isDoorOpen = state.isDoorUnlocked;

        // ปลด Soft-Lock หากย้อนกลับมาจนมีก้าวเหลือมากกว่า 0
        if (currentTurns > 0)
        {
            isSoftLocked = false;
        }

        // TODO: สั่งให้ Player GameObject และวัตถุบน Grid ขยับตามพิกัดใน State นี้ (จะทำใน Step 2)
    }
}