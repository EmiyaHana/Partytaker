using UnityEngine;

public class EnemyEntity : GridEntity
{
    public bool isAlive = true;

    private void Start()
    {
        SetGridPosition(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
    }

    public bool TryPush(Vector2Int direction)
    {
        Vector2Int targetPos = gridPosition + direction;

        // เช็กว่าช่องด้านหลังศัตรูว่างและเดินได้ไหม
        if (GameManager.Instance.CanMoveTo(targetPos))
        {
            // ช่องว่าง -> ศัตรูถอยหลัง 1 ช่อง
            SetGridPosition(targetPos);
            return true; 
        }

        // ถ้าช่องด้านหลังศัตรูไม่ว่าง (ติดกำแพง/กล่อง/ประตู) -> ศัตรูถูกดันอัดจนตาย!
        CrushEnemy();
        return true; 
    }

    public void CrushEnemy()
    {
        isAlive = false;
        gameObject.SetActive(false); // ซ่อนศัตรูออกไปก่อน
        GameManager.Instance.actionLog.enemiesKilled++;
        Debug.Log("ศัตรูถูกดันอัดกำแพง/สิ่งของจนขยี้ตายแล้ว!");
    }

    public void SetAliveState(bool aliveState, Vector2Int pos)
    {
        isAlive = aliveState;
        gameObject.SetActive(aliveState);
        if (aliveState)
        {
            SetGridPosition(pos);
        }
    }
}