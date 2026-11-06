using UnityEngine;

public class GridEntity : MonoBehaviour
{
    [Header("Grid Position")]
    public Vector2Int gridPosition;

    public virtual void SetGridPosition(Vector2Int newPos)
    {
        gridPosition = newPos;
        transform.position = new Vector3(newPos.x, newPos.y, 0);
    }
}