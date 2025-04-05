using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // 2D up view character controller.
    // Has collisions
    [SerializeField] private float      speed = 2f;
    [SerializeField] private float      sprintMult = 2.5f;
    [SerializeField] private int        maxHealth = 100;
    [SerializeField] private int        maxMana = 100;
    private Rigidbody2D                 rb;
    private InputSystem_Actions         inputActions;
    private Vector2                     movementInput;
    private bool                        isSprinting = false;

    //controll the number of electric follow spells
    [SerializeField] private int        maxElectricSpells = 1;
    private int                         currentElectricSpells = 0;
    private int                         currentMana;
    private int                         currentHealth;

    //================================//
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
        inputActions.Player.Attack.performed += OnAttack;

        Spell.onSpellSpawn += OnSpellPerformed;
        Spell.onSpellDie += OnSpellExpired;
        Spell.onSpellHit += OnAttackHit;
    }

    //================================//
    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Disable();
    }

    //================================//
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        this.currentHealth = maxHealth;
        this.currentMana = maxMana;
        this.currentElectricSpells = 0;
    }

    //================================//
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //================================//
    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput.normalized * speed;
    }

    //================================//
    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    //================================//
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
    }

    //================================//
    private void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
            speed *= sprintMult;
        }
        else if (context.canceled)
        {
            isSprinting = false;
            speed /= sprintMult;
        }
    }

    //================================//
    private void OnSpellPerformed(Spell spell)
    {
        int manaCount = spell.SpellCost;
        if (!(currentMana - manaCount >= 0))
        {
            spell.Die();
            Debug.Log("Not enough mana to cast the spell!");
            return;
        }

        currentMana -= manaCount;
    }

    //================================//
    private void OnSpellExpired(Spell spell)
    {
        if (spell.SpellType == Spell.eSpellType.Electric)
        {
            currentElectricSpells--;
        }
    }

    //================================//
    private void OnAttackHit(Spell spellHit)
    {
    }

    //================================//
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && currentElectricSpells < maxElectricSpells)
        {
            Instantiate(PrefabManager.Instance.ElectricSpell, transform.position, Quaternion.identity);
            currentElectricSpells++;
        }
    }

    //================================//
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //================================//
    private void Die()
    {
        // Handle player death logic here
        Debug.Log("Player has died!");
        Destroy(gameObject);
    }
}
