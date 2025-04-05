using UnityEngine;
using UnityEngine.Events;

public class Spell: MonoBehaviour
{
    public static UnityAction<Spell> onSpellHit;
    public static UnityAction<Spell> onSpellSpawn;
    public static UnityAction<Spell> onSpellDie;

    //================================//
    public enum eSpellType
    {
        Electric,
        Fire,
        Ice,
    }

    //================================//
    protected eSpellType        spellType;
    protected int               spellDamage;
    protected int               spellCost;
    protected Collider2D        spellCollider;
    protected float             spellDuration;

    //================================//
    protected float             lifeTimer = 0f;
    //================================//
    public eSpellType       SpellType     => spellType;
    public int              SpellDamage   => spellDamage;
    public int              SpellCost     => spellCost;

    //================================//
    protected void Start()
    {
        spellCollider = GetComponent<Collider2D>();
        if (spellCollider == null)
        {
            Debug.LogError("Spell collider is not assigned or missing!");
            Die();
        }

        // deactivate collider at start
        spellCollider.enabled = false;

        OnSpawn();
    }

    //================================//
    protected virtual void Update()
    {
        if (lifeTimer > 0f)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                Die();
                return;
            }
        }
    }

    //================================//
    protected void OnSpellHit(Spell spellHit)
    {
        onSpellHit?.Invoke(spellHit);
    }

    //================================//
    protected void OnSpellSpawn(Spell spellSpawn)
    {
        onSpellSpawn?.Invoke(spellSpawn);
    }

    //================================//
    protected void OnSpellDie(Spell spellDie)
    {
        onSpellDie?.Invoke(spellDie);
    }
    
    //================================//
    public virtual void OnHit()
    {
        // Handle the spell hit logic here
        OnSpellHit(this);
        Die();
    }

    //================================//
    public virtual void OnSpawn()
    {
        // Handle the spell spawn logic here
        OnSpellSpawn(this);
        lifeTimer = spellDuration;
    }

    //================================//
    public void Die()
    {
        // Handle the spell death logic here
        OnSpellDie(this);
        Destroy(this.gameObject);
    }

    //================================//
    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            OnHit();
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(spellDamage);
            }
            else
            {
                Debug.LogError("Enemy component is missing on the collided object!");
            }
        } else if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Die();
        }
    }
}