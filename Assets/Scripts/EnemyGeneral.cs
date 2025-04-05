using UnityEngine;

public class EnemyGeneral : MonoBehaviour
{
    [SerializeField] private int health = 10;
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float attackRange = 1.5f;

    [SerializeField] private float detectRange = 6.0f;

    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Attack attackPrefab;

    [SerializeField] private CircleCollider2D FieldOfView;

    //================================//
    public int Health => health;
    public int Damage => damage;
    public float Speed => speed;
    public float AttackRange => attackRange;
    public float DetectRange => detectRange;
    public float AttackCooldown => attackCooldown;
    public Attack AttackPrefab => attackPrefab;

    //================================//
    void Start()
    {
        // Initialize the enemy
        FieldOfView = GetComponent<CircleCollider2D>();
        FieldOfView.radius = detectRange;
    }

    //================================//
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    //================================//
    private void Die()
    {
        // Handle enemy death logic here
        Debug.Log(this.gameObject.name + " has died!");
        Destroy(gameObject);
    }

    // ================================//
    private void Attack(Vector2 targetPosition)
    {
        // Handle attack logic here
        Debug.Log(this.gameObject.name + " attacks!");
        // pose COLLISION object on detected location
        Attack attack = Instantiate(attackPrefab, transform.position, Quaternion.identity);
        attack.transform.position = transform.position;

    }

    private void moveTo(Vector2 targetPosition)
    {
        Debug.Log("moving to player position");
        // rotate towards the target position

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


        //don't move if close enough
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        if (distanceToTarget <= attackRange - 1.0f)
        {
            Debug.Log("Enemy close to player, not moving");
            return;
        }


        // Move forwards
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    //================================//
    //proximity to player
    //=================================//

    private void DoWhenCollision(Collider2D collision)
    {
        // Check if the collided object has the PlayerController component
        PlayerController player = collision.GetComponent<PlayerController>();
        Debug.Log("Enemy spotted with: " + collision.gameObject.name);
        if (player != null)
        {
            Debug.Log("Player spotted");
            //get player position
            Vector2 playerPosition = player.transform.position;
            //check if player is in range
            moveTo(playerPosition);

            float distanceToPlayer = Vector2.Distance(transform.position, playerPosition);

            if (distanceToPlayer <= detectRange)
            {
                // Attack the player
                Debug.Log("Enemy attacks player");
                // Attack(playerPosition);
            }
            else
            {
                Debug.Log("Player out of range");
            }

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        DoWhenCollision(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        DoWhenCollision(collision);
    }

}


