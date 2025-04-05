using UnityEngine;

public class ValueBar : MonoBehaviour
{
    [SerializeField] private float maxSize = 1.5f;
    [SerializeField] private Color fullColor = Color.green;
    [SerializeField] private Color emptyColor = Color.red;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    private float val;

    public float Value
    {
        get => val;
        set
        {
            val = Mathf.Clamp01(value);
            UpdateBar();
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (spriteRenderer == null) return;

        // Scale based on value
        Vector3 scale = transform.localScale;
        scale.x = maxSize * val;
        transform.localScale = scale;

        // Position adjustment to keep left-aligned
        Vector3 position = transform.localPosition;
        position.x = -(maxSize - scale.x) / 2f;
        transform.localPosition = position;

        // Update color
        spriteRenderer.color = Color.Lerp(emptyColor, fullColor, val);
    }
}
