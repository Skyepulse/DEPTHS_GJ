using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[]             frames;           // Assign 4 sprites in inspector
    public Sprite[]             attackFrames;     // Assign 4 sprites in inspector
    public float                frameRate = 10f;     // Frames per second
    public float                attackFrameRate = 5f;
    public float                attackDuration = 1.0f; // Duration of attack animation in seconds
    private float               attackTimer = 0f; // Timer for attack animation

    private                     SpriteRenderer spriteRenderer;
    private int                 currentFrame;
    private float               timer;
    private bool                attackFlag = false;

    //================================//
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    //================================//
    public void SetAttackFlag()
    {
        attackFlag = true;
        spriteRenderer.sprite = attackFrames[0];
        timer = 0f;
        currentFrame = 0;
        attackTimer = 0f; // Reset attack timer
    }

    //================================//
    private void Update()
    {
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        if (attackFlag) attackTimer += Time.deltaTime;

        // Use different frame rates for attack and normal animations
        float currentFrameRate = attackFlag ? attackFrameRate : frameRate;

        if (timer >= 1f / currentFrameRate)
        {
            if (attackFlag && attackFrames != null && attackFrames.Length > 0)
            {
                currentFrame = (currentFrame + 1);
                if (currentFrame >= attackFrames.Length)
                {
                    currentFrame = 0;
                }
                spriteRenderer.sprite = attackFrames[currentFrame];
            }
            else
            {
                currentFrame = (currentFrame + 1) % frames.Length;
                spriteRenderer.sprite = frames[currentFrame];
            }
            timer -= 1f / currentFrameRate;
        }

        // Check if attack animation is done
        if (attackFlag && attackTimer >= attackDuration)
        {
            attackFlag = false; // Reset attack flag
            currentFrame = 0; // Reset frame index for normal animation
            spriteRenderer.sprite = frames[currentFrame]; // Reset to first frame of normal animation
        }
    }
}
