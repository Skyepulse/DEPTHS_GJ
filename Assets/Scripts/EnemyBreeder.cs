using UnityEngine;

public class EnemyBroodmother : Enemy
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float detectRange = 6.0f;
    [SerializeField] private float spawnCooldown = 3f;
    [SerializeField] private Eggs eggPrefab;
    [SerializeField] private int maxEggsAtOnce = 3;

    [SerializeField] private float eggDistanceSpawn = 3.0f;
    [SerializeField] private CircleCollider2D fieldOfView;
    [SerializeField] private GameObject VisualNode;



    private float spawnTimer = 0f;
    private bool canSpawn = true;

    private Collider2D lastCollision;
    private bool playerVisible = false;

    //================================//
    public float Speed => speed;
    public float EggDistanceSpawn => eggDistanceSpawn;
    public float DetectRange => detectRange;
    public float SpawnCooldown => spawnCooldown;
    public Eggs EggPrefab => eggPrefab;
    public int MaxEggsAtOnce => maxEggsAtOnce;
    public CircleCollider2D FieldOfView => fieldOfView;
    public float SpawnTimer => spawnTimer;
    public bool CanSpawn => canSpawn;
    public Collider2D LastCollision => lastCollision;
    public bool PlayerVisible => playerVisible;

    //================================//

    void Start()
    {
        fieldOfView = GetComponent<CircleCollider2D>();
        fieldOfView.radius = detectRange;
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

        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
            if (lastCollision != null)
            {
                moveAwayFrom(lastCollision.transform.position);
            }
        }
        else
        {
            canSpawn = true;
        }

        if (playerVisible && lastCollision != null && canSpawn)
        {
            DoWhenCollision(lastCollision);
        }
    }

    //================================//
    private void moveTo(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        VisualNode.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    private void moveAwayFrom(Vector2 targetPosition)
    {
        Vector2 direction = ((Vector2)transform.position - targetPosition).normalized;
        VisualNode.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position - targetPosition, speed * Time.deltaTime);
    }

    //================================//

    private void SpawnEgg()
    {
        // limit number of eggs spawned (optional)
        int currentEggs = GameObject.FindGameObjectsWithTag("Eggs").Length;
        if (currentEggs >= maxEggsAtOnce) return;

        Vector2 spawnOffset = Random.insideUnitCircle.normalized * eggDistanceSpawn;
        Instantiate(eggPrefab, (Vector2)transform.position, Quaternion.identity);

        spawnTimer = spawnCooldown;
        canSpawn = false;
    }

    //================================//
    private void DoWhenCollision(Collider2D collision)
    {
        PlayerController player = collision.gameObject.transform.parent.GetComponent<PlayerController>();
        if (player != null)
        {
            Vector2 playerPosition = player.transform.position;

            if (canSpawn)
            {


                float distanceToPlayer = Vector2.Distance(transform.position, playerPosition);

                float minDistance = 2.0f; // buffer space
                if (distanceToPlayer > eggDistanceSpawn - 0.5f)
                {
                    // Move towards the player
                    moveTo(playerPosition);
                }

                else if (distanceToPlayer < eggDistanceSpawn)
                {
                    // Spawn an egg
                    SpawnEgg();
                }



            }
        }
    }

    //================================//
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
