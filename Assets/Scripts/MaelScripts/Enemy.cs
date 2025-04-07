using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public static UnityAction<Enemy> onEnemyDeath;
    [SerializeField] private float maxHealth = 10;
    [SerializeField] private ValueBar healthBar;

    private float health;

    //================================//
    public float Health => health;

    //================================//
    public void OnEnemyDeath(Enemy enemy)
    {
        onEnemyDeath?.Invoke(enemy);
    }

    //================================//
    protected virtual void Awake()
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

        Instantiate(PrefabManager.Instance.EnemyDamageEffect, transform.position, Quaternion.identity);
        
        AudioSource audio = GetComponentInChildren<AudioSource>();
        if (audio != null)
        {
            audio.Play();
        }
        if (health <= 0)
        {
            Die();
        }
    }

    //================================//
    public virtual void Update()
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
        OnEnemyDeath(this);
        Destroy(gameObject);
    }
}