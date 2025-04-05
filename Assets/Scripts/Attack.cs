using UnityEngine;

public class Attack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 10000.0f;

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
        //log
        Debug.Log("Attack collided");
        // Check if the collided object has the EnemyGeneral component
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            // Deal damage to the enemy
            player.TakeDamage(damage);
            Debug.Log("Attack dealt " + damage + " damage to " + player.gameObject.name);
            // Destroy the attack object after dealing damage
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Attack collided with " + collision.gameObject.name + " but did not deal damage.");
        }
    }

}
