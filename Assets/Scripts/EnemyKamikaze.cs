using UnityEngine;
using System.Collections;



public class EnemyKamikaze : Enemy
{
    [SerializeField] private int damage = 30;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float detectRange = 6.0f;

    [SerializeField] private float explodeDelay = 0.1f;
    [SerializeField] private float explodeCountdown = 0.5f;
    [SerializeField] private GameObject explosionEffect;

    [SerializeField] private CircleCollider2D FieldOfView;
    [SerializeField] private GameObject VisualNode;

    private Collider2D lastCollision;
    private bool playerVisible = false;
    private bool hasExploded = false;
    private bool countingDown = false;
    private bool moving = true;
    void Start()
    {
        FieldOfView = GetComponent<CircleCollider2D>();
        FieldOfView.radius = detectRange;
    }

    private void Awake()
    {
        if (VisualNode == null)
        {
            VisualNode = transform.GetChild(0).gameObject;
        }
    }

    public override void Update()
    {
        base.Update();

        if (playerVisible && lastCollision != null && !hasExploded)
        {
            DoWhenCollision(lastCollision);
        }
        if (countingDown)
        {
            explodeCountdown -= Time.deltaTime;
            if (explodeCountdown <= 0)
            {
                PlayerController player = lastCollision.gameObject.transform.parent.GetComponent<PlayerController>();
                if (player != null)
                {
                    Explode(player);
                }
            }
        }


    }

    private void moveTo(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        VisualNode.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    private void DoWhenCollision(Collider2D collision)
    {
        PlayerController player = collision.gameObject.transform.parent.GetComponent<PlayerController>();
        if (player != null && moving)
        {

            Vector2 playerPosition = player.transform.position;


            moveTo(playerPosition);

            float distanceToPlayer = Vector2.Distance(transform.position, playerPosition);

            if (distanceToPlayer <= attackRange)
            {
                countingDown = true;
                StartCoroutine(BlinkRed());
                moving = false; // Stop moving when in attack range
            }
        }
    }

    private void Explode(PlayerController player)
    {
        if (hasExploded) return;
        hasExploded = true;

        // Deal damage
        player.TakeDamage(damage); // Assuming your PlayerController has this

        // Play explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Optional: delay before destruction to show explosion
        Destroy(gameObject, explodeDelay);


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        lastCollision = collision;
        playerVisible = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        lastCollision = collision;
        playerVisible = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerVisible = false;
        lastCollision = null;
    }
    private IEnumerator BlinkRed()
    {
        SpriteRenderer sr = VisualNode.GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        for (float t = 0; t < explodeCountdown; t += 0.1f)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            sr.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

}

