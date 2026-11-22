using UnityEngine;

public class PlayerController : GridEntity
{
    private void Start()
    {
        SetGridPosition(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
        GameManager.Instance.currentPlayerGridPos = gridPosition;
    }

    private void Update()
    {
        if (GameManager.Instance.isSoftLocked) return;

        Vector2Int moveDir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) moveDir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) moveDir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) moveDir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) moveDir = Vector2Int.right;

        if (moveDir != Vector2Int.zero)
        {
            AttemptMove(moveDir);
        }
    }

    private void AttemptMove(Vector2Int direction)
    {
        Vector2Int targetPos = gridPosition + direction;

        EndLevelNPC npc = GameManager.Instance.GetNPCAt(targetPos);
        if (npc != null)
        {
            npc.TriggerInteraction();
            return;
        }

        PushBlock block = GameManager.Instance.GetBlockAt(targetPos);
        if (block != null)
        {
            if (block.TryPush(direction))
            {
                MovePlayer(targetPos);
            }
            return;
        }

        EnemyEntity enemy = GameManager.Instance.GetEnemyAt(targetPos);
        if (enemy != null)
        {
            if (enemy.TryPush(direction))
            {
                MovePlayer(targetPos);
            }
            return;
        }

        if (GameManager.Instance.CanMoveTo(targetPos))
        {
            MovePlayer(targetPos);
        }
    }

    private void MovePlayer(Vector2Int targetPos)
    {
        SetGridPosition(targetPos);
        GameManager.Instance.currentPlayerGridPos = targetPos;

        GameManager.Instance.actionLog.stepsTaken++;
        GameManager.Instance.UseTurn(1);

        HazardTrap trap = GetTrapAt(targetPos);
        if (trap != null && trap.isActive)
        {
            GameManager.Instance.actionLog.trapsTriggered++;
            GameManager.Instance.UseTurn(1);
        }

        KeyItem key = FindObjectOfType<KeyItem>();
        if (key != null && key.gameObject.activeSelf && key.gridPosition == targetPos)
        {
            GameManager.Instance.hasKey = true;
            GameManager.Instance.isDoorOpen = true;
            key.SetCollected(true);
            
            DoorBlock door = FindObjectOfType<DoorBlock>();
            if (door != null) door.SetUnlocked(true);
        }

        GameManager.Instance.RecordState();
    }

    private HazardTrap GetTrapAt(Vector2Int targetPos)
    {
        HazardTrap[] traps = FindObjectsOfType<HazardTrap>();
        Vector3 targetWorldPos = new Vector3(targetPos.x, targetPos.y, 0);

        foreach (var t in traps)
        {
            if (Vector3.Distance(t.transform.position, targetWorldPos) < 0.5f)
            {
                return t;
            }
        }
        return null;
    }
}