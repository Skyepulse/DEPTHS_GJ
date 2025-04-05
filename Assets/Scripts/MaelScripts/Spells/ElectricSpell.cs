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

        OnSpawn();
    }

    //================================//
    public override void OnHit()
    {
        base.OnHit();
        // Handle the electric spell hit logic here
        Debug.Log("Electric spell hit! Damage: " + spellDamage);
    }

    //================================//
    public override void OnSpawn()
    {
        base.OnSpawn();
        // Handle the electric spell spawn logic here
        Debug.Log("Electric spell spawned! Cost: " + spellCost);
    }

    //================================//
    private void InitializePosition()
    {
        // get mouse position and set the position of the spell to the mouse position
    }
}