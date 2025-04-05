using UnityEngine;

public class EnemyGeneral : MonoBehaviour
{
    [SerializeField] private int health = 10;
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float attackRange = 1.5f;

    [SerializeField] private float detectRange = 6.0f;

    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Attack attackPrefab;

    //================================//
    public int Health => health;
    public int Damage => damage;
    public float Speed => speed;
    public float AttackRange => attackRange;
    public float DetectRange => detectRange;
    public float AttackCooldown => attackCooldown;
    public Attack AttackPrefab => attackPrefab;

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


}