using UnityEngine;

public class KeyItem : GridEntity
{
    private void Start()
    {
        SetGridPosition(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
    }

    public void SetCollected(bool collected)
    {
        gameObject.SetActive(!collected);
    }
}