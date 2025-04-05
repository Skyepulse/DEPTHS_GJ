using UnityEngine;

public class Enemy: MonoBehaviour
{
    [SerializeField] private int            health = 10;
    [SerializeField] private int            damage = 10;
    [SerializeField] private float          speed = 5f;
    [SerializeField] private float          attackRange = 1.5f;

    //================================//
    public int                              Health => health;
    public int                              Damage => damage;
    public float                            Speed => speed;
    public float                            AttackRange => attackRange;

    //================================//
    public void TakeDamage(int damageAmount)
    {
        Debug.Log(this.gameObject.name + " took " + damageAmount + " damage!");
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
}