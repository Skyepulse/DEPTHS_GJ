using UnityEngine;

// Electric spell class follows the mouse position
public class ElectricSpell: Spell
{
    [SerializeField] private float spellDurationVal = 10f;
    [SerializeField] private float spellDamageVal = 10f;
    [SerializeField] private float spellCostVal = 5f;

    //================================//
    private void Awake()
    {
        spellType = eSpellType.Electric;
        this.spellDamage = (int)spellDamageVal;
        this.spellCost = (int)spellCostVal;
        this.spellDuration = spellDurationVal;
    }

    //================================//
    public override void OnHit()
    {
        base.OnHit();
        // Handle the electric spell hit logic here
        Debug.Log("Electric spell hit! Damage: " + spellDamage);
    }

    //================================//
    protected override void Update()
    {
        base.Update();

        // Update the position of the spell to follow the mouse cursor with a small delay
        if (lifeTimer > 0f && spellCollider.enabled)
        {
            Vector2 mousePos = GetMousePosition();
            transform.position = Vector2.Lerp(transform.position, mousePos, Time.deltaTime * 10f);
        }
    }

    //================================//
    public override void OnSpawn()
    {
        base.OnSpawn();
        // Handle the electric spell spawn logic here
        Debug.Log("Electric spell spawned! Cost: " + spellCost);

        // activate collider
        spellCollider.enabled = true;
    }

    //================================//
    private Vector2 GetMousePosition()
    {
        // get mouse position in world space
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePos;
    }
}