using UnityEngine;

public class Attack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 1.0f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float size = 0.5f;

    //================================//
    public int Damage => damage;
    public float Lifetime => lifetime;
    public float Size => size;
    public float Speed => speed;
    //================================//

    private Vector2 destinationPosition;
    //================================//
    void Start()
    {
        // Set the size of the attack object
        transform.localScale = new Vector3(size, size, 1f);

    }

    // Update is called once per frame
    void Update()
    {
        // Move the attack object towards the destination position
        transform.position = Vector2.MoveTowards(transform.position, destinationPosition, speed * Time.deltaTime);
        //destroy the attack object after a certain time
        if (Vector2.Distance(transform.position, destinationPosition) < 0.01f)
        {


            lifetime -= Time.deltaTime;
            if (lifetime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    //================================//
    // Set the destination position for the attack object
    public void SetDestination(Vector2 destination)
    {
        destinationPosition = destination;
    }

    //================================//
    //on trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //log
        Debug.Log("Attack collided");
        // Check if the collided object has the EnemyGeneral component
        PlayerController player = collision.gameObject.transform.parent.GetComponent<PlayerController>();
        if (player != null)
        {
            // Deal damage to the enemy
            player.TakeDamage(damage);
            Debug.Log("Attack dealt " + damage + " damage to " + player.gameObject.name);
            // Destroy the attack object after dealing damage
            Destroy(gameObject);
        }

    }

}
