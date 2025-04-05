using UnityEngine;

public class Attack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 0.02f;

    //================================//
    public int Damage => damage;
    //================================//
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //destroy the attack object after a certain time
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }

    }

    //================================//
    //on trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has the EnemyGeneral component
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            // Deal damage to the enemy
            player.TakeDamage(damage);
            // Destroy the attack object after dealing damage
            Destroy(gameObject);
        }
    }

}
