using UnityEngine;

public class TemporaryEffect : MonoBehaviour
{
    [SerializeField] private float duration = 1.5f; // Duration of the effect in seconds

    private float timer = 0f; // Timer to track the duration

    //================================//
    private void Start()
    {
        // Start the timer
        timer = duration;
    }

    //================================//
    private void Update()
    {
        // Update the timer
        timer -= Time.deltaTime;

        // Check if the effect duration has ended
        if (timer <= 0f)
        {
            Destroy(gameObject); // Destroy the effect object
        }
    }
}
