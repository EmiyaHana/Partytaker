using UnityEngine;

public class PushBlock : GridEntity
{
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

        return false;
    }
}