using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteAnimator : MonoBehaviour
{
    public Sprite[]             frames;           // Assign 4 sprites in inspector
    public float                frameRate = 10f;     // Frames per second

    private                     Image spriteRenderer;
    private int                 currentFrame;
    private float               timer;

    //================================//
    private void Awake()
    {
        spriteRenderer = GetComponent<Image>();
    }

    //================================//
    private void Update()
    {
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        float currentFrameRate = frameRate;

        if (timer >= 1f / currentFrameRate)
        {
            
            currentFrame = (currentFrame + 1) % frames.Length;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= 1f / currentFrameRate;
        }
    }
}
