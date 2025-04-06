using UnityEngine;


public class EnemyRanged : Enemy
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float attackRangeMin = 4f;
    [SerializeField] private float attackRangeMax = 6f;

    [SerializeField] private float detectRange = 6.0f;

    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Attack attackPrefab;

    [SerializeField] private CircleCollider2D FieldOfView;
    [SerializeField] private GameObject VisualNode;

    //================================//
    public int Damage => damage;
    public float Speed => speed;
    public float AttackRangeMax => attackRangeMax;
    public float AttackRangeMin => attackRangeMin;
    public float DetectRange => detectRange;
    public float AttackCooldown => attackCooldown;
    public Attack AttackPrefab => attackPrefab;

    //================================//
    private bool coolDownOver = true;
    private float attackCooldownTimer = 0f;

    private Collider2D lastCollision;
    private bool playerVisible = false;
    //================================//
    void Start()
    {
        // Initialize the enemy
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

    //================================//
    public override void Update()
    {
        base.Update();
        //manage cooldown
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        else
        {
            attackCooldownTimer = 0;
            coolDownOver = true;
        }

        // Check if the enemy is within the detection range of the player
        if (playerVisible)
        {
            // Check if the player is within the attack range
            if (lastCollision != null)
            {
                DoWhenCollision(lastCollision);
            }
        }

    }



    // ================================//
    private void Attack(Vector2 targetPosition)
    {

        //Raycast to check if the attack hits an obstacle

        // pose COLLISION object on detected location
        //add child attack object to the enemy
        Attack attack = Instantiate(attackPrefab, this.transform.position, Quaternion.identity);
        attack.SetDestination(targetPosition);

        attackCooldownTimer = attackCooldown;
        coolDownOver = false;

    }

    private void moveTo(Vector2 targetPosition)
    {
        // rotate towards the target position

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        VisualNode.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        //don't move if close enough
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        if (distanceToTarget <= attackRangeMax)
        {
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
        PlayerController player = collision.gameObject.transform.parent.GetComponent<PlayerController>();
        if (player != null)
        {
            //get player position
            Vector2 playerPosition = player.transform.position;
            //check if player is in range
            //raycast to check if the attack hits an obstacle

            RaycastHit2D hit = Physics2D.Raycast(transform.position, playerPosition - (Vector2)transform.position, detectRange, LayerMask.GetMask("Obstacle"));

            if (hit.collider != null)
            {
                // If the raycast hits an obstacle, do not attack
                Debug.Log("Obstacle detected in between");
                //move toward a perpendicular point
                Vector2 perpendicularPoint = hit.point + (Vector2)hit.normal * 0.5f;
                moveTo(perpendicularPoint);


                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, playerPosition);

            if (distanceToPlayer > attackRangeMax - 1.0f)
            {
                // Move towards the player
                moveTo(playerPosition);
            }

            else if (distanceToPlayer < attackRangeMin + 1.0f)
            {
                // Move away from the player
                Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;
                Vector2 newPosition = (Vector2)transform.position - direction * speed * Time.deltaTime;
                transform.position = newPosition;
            }

            if (distanceToPlayer <= attackRangeMax && coolDownOver)
            {
                // Attack the player
                Attack(playerPosition);

            }


        }
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
}



