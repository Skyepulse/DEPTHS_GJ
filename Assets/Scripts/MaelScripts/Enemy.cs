using UnityEngine;

public class Enemy: MonoBehaviour
{
    [SerializeField] private float          maxHealth = 10;
    [SerializeField] private int            damage = 10;
    [SerializeField] private float          speed = 5f;
    [SerializeField] private float          attackRange = 1.5f;
    [SerializeField] private ValueBar       healthBar;

    private float                           health;

    //================================//
    public float                            Health => health;
    public int                              Damage => damage;
    public float                            Speed => speed;
    public float                            AttackRange => attackRange;

    //================================//
    private void Awake()
    {
        health = maxHealth;
        if (healthBar != null)
        {
            healthBar.Value = health / maxHealth;
        }
    }

    //================================//
    public void TakeDamage(int damageAmount)
    {
        health -= (float)damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    //================================//
    protected virtual void Update()
    {
        // Update health bar value
        if (healthBar != null)
        {
            healthBar.Value = health / maxHealth;
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