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

        if (GameManager.Instance.CanMoveTo(targetPos))
        {
            SetGridPosition(targetPos);
            return true; 
        }

        CrushEnemy();
        return true; 
    }

    public void CrushEnemy()
    {
        isAlive = false;
        gameObject.SetActive(false);
        GameManager.Instance.actionLog.enemiesKilled++;
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