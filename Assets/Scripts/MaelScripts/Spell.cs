using UnityEngine;
using UnityEngine.Events;

public class Spell: MonoBehaviour
{
    public UnityAction<Spell> onSpellHit;
    public UnityAction<Spell> onSpellSpawn;

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
    private float               lifeTimer = 0f;
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
            Destroy(this.gameObject);
        }
    }

    //================================//
    protected void Update()
    {
        if (lifeTimer > 0f)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                Debug.Log("Spell has expired!");
                Destroy(this.gameObject);
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
    public virtual void OnHit()
    {
        // Handle the spell hit logic here
        OnSpellHit(this);
        Destroy(this.gameObject);
    }

    //================================//
    public virtual void OnSpawn()
    {
        // Handle the spell spawn logic here
        OnSpellSpawn(this);
        lifeTimer = spellDuration;
    }

    //================================//
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnHit();
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(spellDamage);
            } else 
            {
                Debug.LogError("Enemy component is missing on the collided object!");
            }
        }
    }
}