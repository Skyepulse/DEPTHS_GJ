using System.Collections;
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

    [SerializeField] private AudioSource    mainAudioSource;
    [SerializeField] private AudioSource    damageAudioSource;
    [SerializeField] private AudioSource    attackAudioSource;
    [SerializeField] private AudioSource    doorAudioSource;

    private bool                            isDead = false;

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

        GameManager.Instance.ChangeSpell(0);

        if(PrefabManager.Instance == null){
            Debug.LogError("PrefabManager instance is not present in the scene!");
        }

        if( mainAudioSource != null){
            mainAudioSource.loop = true;
            mainAudioSource.Play();
        }
    }

    //================================//
    private void FixedUpdate()
    {
        if (isDead) return;

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

        if( movementInput != Vector2.zero && !isDead)
        {
            if (GameManager.Instance != null && GameManager.Instance.GetCurrentTutorial() == 1)
            {
                GameManager.Instance.NextTutorial();
            }
        }
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
            if (GameManager.Instance != null && GameManager.Instance.GetCurrentTutorial() == 2)
            {
                GameManager.Instance.NextTutorial();
            }
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
        if(attackAudioSource != null){
            attackAudioSource.Play();
        }
    }

    //================================//
    private void OnSwitchSpell(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            castSpellType = (castSpellType + 1) % System.Enum.GetValues(typeof(Spell.eSpellType)).Length;
            GameManager.Instance.ChangeSpell(castSpellType);

            if (GameManager.Instance != null && GameManager.Instance.GetCurrentTutorial() == 4)
            {
                GameManager.Instance.NextTutorial();
            }
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

        if (GameManager.Instance != null && GameManager.Instance.GetCurrentTutorial() == 3)
        {
            GameManager.Instance.NextTutorial();
        }
    }

    //================================//
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        Instantiate(PrefabManager.Instance.DamageEffect, transform.position, Quaternion.identity);

        if (damageAudioSource != null)
        {
            damageAudioSource.Play();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //================================//
    private void Die()
    {
        if (mainAudioSource != null)
        {
            mainAudioSource.Stop();
        }

        isDead = true;
        VisualNode.SetActive(false);

        healthBar.gameObject.SetActive(false);
        manaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(false);

        rb.linearVelocity = Vector2.zero;

        StartCoroutine(DieAsync());
    }

    //================================//
    private IEnumerator DieAsync()
    {
        // Play death animation or effect here
        yield return new WaitForSeconds(1f); // Wait for the animation to finish
        GameManager.Instance.OnPlayerDeath();
        Destroy(gameObject); // Destroy the player object after the animation
    }

    //================================//
    public void PlayCloseDoor()
    {
        if (doorAudioSource != null)
        {
            doorAudioSource.Play();
        }
    }
}
