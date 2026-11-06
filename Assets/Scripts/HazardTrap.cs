using UnityEngine;

public class HazardTrap : GridEntity
{
    [Header("Trap Settings")]
    public bool isActive = false;
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        gridPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        UpdateVisual();
    }

    public void ToggleState()
    {
        isActive = !isActive;
        UpdateVisual();
    }

    public void SetState(bool activeState)
    {
        isActive = activeState;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            if (isActive)
            {
                if (activeSprite != null) spriteRenderer.sprite = activeSprite;
                else spriteRenderer.color = Color.red;
            }
            else
            {
                if (inactiveSprite != null) spriteRenderer.sprite = inactiveSprite;
                else spriteRenderer.color = Color.gray;
            }
        }
    }
}