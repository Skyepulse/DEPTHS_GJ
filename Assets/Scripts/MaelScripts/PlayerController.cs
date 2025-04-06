using UnityEngine;
using UnityEngine.InputSystem;
// Visual Effect
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    // 2D up view character controller.
    // Has collisions
    [SerializeField] private float          speed = 2f;
    [SerializeField] private float          sprintMult = 2.5f;
    [SerializeField] private float          maxHealth = 100;
    [SerializeField] private float          maxMana = 50;
    [SerializeField] private float          maxStamina = 50;
    private float                           staminaDepleteRate = 10; // per second
    private float                           staminaRefillRate = 5; // per second
    [SerializeField] private float          manaRefillRate = 1; // per second
    [SerializeField] private GameObject     VisualNode;
    // bars
    [SerializeField] private ValueBar       healthBar;
    [SerializeField] private ValueBar       manaBar;
    [SerializeField] private ValueBar       staminaBar;
    [SerializeField] private Transform      attackPoint;
    private Rigidbody2D                     rb;
    private InputSystem_Actions             inputActions;
    private Vector2                         movementInput;
    private bool                            isSprinting = false;
    private bool                            isSprintingPressed = false;    
    private int                             castSpellType = 0;

    //controll the number of electric follow spells
    [SerializeField] private int            maxElectricSpells = 1;
    private int                             currentElectricSpells = 0;
    private float                           currentMana;
    private float                           currentHealth;
    private float                           currentStamina;

    //================================//
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.SwitchSpell.performed += OnSwitchSpell;

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
        inputActions.Player.SwitchSpell.performed -= OnSwitchSpell;
        inputActions.Player.Disable();

        Spell.onSpellSpawn -= OnSpellPerformed;
        Spell.onSpellDie -= OnSpellExpired;
        Spell.onSpellHit -= OnAttackHit;
    }

    //================================//
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        this.currentHealth = maxHealth;
        this.currentMana = maxMana;
        this.currentStamina = maxStamina;
        this.currentElectricSpells = 0;

        if(VisualNode == null){
            VisualNode = GetComponentInChildren<SpriteAnimator>().gameObject;
        }
    }

    //================================//
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if(PrefabManager.Instance == null){
            Debug.LogError("PrefabManager instance is not present in the scene!");
        }
    }

    //================================//
    private void FixedUpdate()
    {
        if (isSprinting)
        {
            if (currentStamina > 0)
            {
                currentStamina -= staminaDepleteRate * Time.fixedDeltaTime;
                if (currentStamina < 0)
                {
                    currentStamina = 0;
                    isSprinting = false;
                }
            }
        } 
        else 
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRefillRate * Time.fixedDeltaTime;
                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
            }

            if (isSprintingPressed && currentStamina > 20f)
            {
                isSprinting = true;
            }
        }

        float thisSpeed = isSprinting ? speed * sprintMult : speed;
        rb.linearVelocity = movementInput.normalized * thisSpeed;

        if (healthBar != null)
            healthBar.Value = (float)currentHealth / maxHealth;
        if (manaBar != null)
            manaBar.Value = (float)currentMana / maxMana;
        if (staminaBar != null)
            staminaBar.Value = (float)currentStamina / maxStamina;

        if (currentMana < maxMana)
        {
            currentMana += manaRefillRate * Time.fixedDeltaTime;
            if (currentMana > maxMana)
            {
                currentMana = maxMana;
            }
        }

        //Update rotation of VisualNode based on velocity
        if (rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg - 90f;
            VisualNode.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        if(isSprinting) VisualNode.GetComponent<SpriteAnimator>().frameRate = 10f;
        else VisualNode.GetComponent<SpriteAnimator>().frameRate = 5f;
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
            isSprintingPressed = true;
        }
        else if (context.canceled)
        {
            isSprinting = false;
            isSprintingPressed = false;
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
    private void OnSwitchSpell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            castSpellType = (castSpellType + 1) % System.Enum.GetValues(typeof(Spell.eSpellType)).Length;
        }
    }

    //================================//
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (! (context.performed && currentElectricSpells < maxElectricSpells)) return;
        switch (castSpellType)
        {
            case (int)Spell.eSpellType.Electric:
                Instantiate(PrefabManager.Instance.ElectricSpell, attackPoint.position, Quaternion.identity);
                currentElectricSpells++;
                VisualNode.GetComponent<SpriteAnimator>().SetAttackFlag();
                break;
            case (int)Spell.eSpellType.Wave:
                Instantiate(PrefabManager.Instance.WaveSpell, attackPoint.position, Quaternion.identity);
                VisualNode.GetComponent<SpriteAnimator>().SetAttackFlag();
                break;
        }
    }

    //================================//
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        Instantiate(PrefabManager.Instance.DamageEffect, transform.position, Quaternion.identity);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //================================//
    private void Die()
    {
        GameManager.Instance.OnPlayerDeath();
        Destroy(gameObject);
    }
}
