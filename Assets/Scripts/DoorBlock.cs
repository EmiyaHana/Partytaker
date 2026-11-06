using UnityEngine;

public class DoorBlock : GridEntity
{
    private void Start()
    {
        SetGridPosition(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
    }

    public void SetUnlocked(bool unlocked)
    {
        gameObject.SetActive(!unlocked);
    }
}