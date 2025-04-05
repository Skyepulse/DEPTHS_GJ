using UnityEngine;
using UnityEngine.VFX;

// Electric spell class follows the mouse position
public class WaveSpell: Spell
{
    [SerializeField] private float          spellDurationVal = 20f;
    [SerializeField] private float          spellDamageVal = 5f;
    [SerializeField] private float          spellCostVal = 2f;
    [SerializeField] private float          speed = 5.0f;
    [SerializeField] private VisualEffect   effect;

    private Vector2 direction = new Vector2(0.0f, 0.0f);

    //================================//
    private void Awake()
    {
        spellType = eSpellType.Wave;
        this.spellDamage = (int)spellDamageVal;
        this.spellCost = (int)spellCostVal;
        this.spellDuration = spellDurationVal;
    }

    //================================//
    public override void OnHit()
    {
        base.OnHit();
    }

    //================================//
    protected override void Update()
    {
        base.Update();

        transform.position += new Vector3(direction.x, direction.y, 0.0f) * speed * Time.deltaTime;
    }

    //================================//
    public override void OnSpawn()
    {
        base.OnSpawn();

        // activate collider
        spellCollider.enabled = true;

        // Get the difference between the spawn position and the mouse position as a normalized vector
        this.direction = (GetMousePosition() - new Vector2(transform.position.x, transform.position.y)).normalized;
        if (effect != null) effect.SetVector3("MyDirection", new Vector3(-direction.x, -direction.y, 0.0f));
    }

    //================================//
    private Vector2 GetMousePosition()
    {
        // get mouse position in world space
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePos;
    }
}