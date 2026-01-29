using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    // List of possible abilities
    public enum AbilityMode
    {
        None,
        Dash,           // 1
        DoubleJump,     // 2
        RevealPlatforms,// 3
        PhaseWalls      // 4
    }

    // Configurable parameters/variables
    [Header("Movement")]
    public float moveSpeed = 7f; 
    public float jumpForce = 12f;

    // Makes jump more comfartable
    [Header("Jump Assist")]
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;

    [Header("Double Jump")]
    public float secondJumpStaminaMultiplier = 2f;

    [Header("Mask Modifiers")]
    public float dashMoveSpeedMultiplier = 1.25f;
    public float phaseMoveSpeedMultiplier = 0.75f;
    public float doubleJumpJumpForceMultiplier = 1.10f;


    [Header("Stamina")]
    public float maxStamina = 100f;
    public float dashStaminaCost = 20f;
    public float staminaRegenPerSecond = 10f;
    public float staminaRegenDelay = 0.5f;

    [Header("Ability Unlocks")]
    public bool dashUnlocked = false;
    public bool doubleJumpUnlocked = false;
    public bool revealPlatformsUnlocked = false;
    public bool phaseWallsUnlocked = false;

    [Header("Current Ability")]
    public AbilityMode currentAbility = AbilityMode.None;

    [Header("Restrictions")]
    public bool canMoveLeft = true;

    [Header("Reveal Platforms Drain")]
    public float revealStaminaDrainPerSecond = 2f;
    public bool disableRevealWhenOutOfStamina = true;

    [Header("Phase Walls Drain")]
    public float phaseStaminaDrainPerSecond = 2f;
    public bool disablePhaseWhenOutOfStamina = true;

    [Header("Ability Components")] // Turn Ability on/off
    public RevealPlatformsAbility revealAbility;
    public PhaseWallsAbility phaseWallsAbility;

    public float stamina { get; private set; }
    public float StaminaNormalized => maxStamina <= 0 ? 0 : stamina / maxStamina;

    // Player state variables

    // These track:
    // Is player touching ground?
    // Which direction player is moving?
    // Has double jump been used?
    // Is dash happening?
    // How long dash lasts
    // When stamina regen can resume
    Rigidbody2D rb;
    bool isGrounded;
    float moveInput;
    bool hasUsedDoubleJump;
    bool isDashing;
    float dashTime;
    float lastDashTime;

    float baseMoveSpeed;
    float baseJumpForce;

    float regenBlockedUntilTime;

    float coyoteTimer;
    float jumpBufferTimer;

    bool externalMovementLock;


    // (On Start)
    // - Gets Rigidbody component
    // - Initializes stamina
    // Finds ability components if not set
    // - Ensures both abilities are OFF at start
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stamina = maxStamina;

        if (revealAbility == null) revealAbility = GetComponent<RevealPlatformsAbility>();
        if (phaseWallsAbility == null) phaseWallsAbility = GetComponent<PhaseWallsAbility>();

        // Force both abilities OFF at start
        ApplyRevealEquippedState(false);
        ApplyPhaseEquippedState(false);

        baseMoveSpeed = moveSpeed;
        baseJumpForce = jumpForce;
    }

    float CurrentMoveSpeed
    {
        get
        {
            float speed = baseMoveSpeed;

            // faster with dash mask
            if (currentAbility == AbilityMode.Dash)
                speed *= dashMoveSpeedMultiplier;

            // slower with phase walls
            if (currentAbility == AbilityMode.PhaseWalls)
                speed *= phaseMoveSpeedMultiplier;

            return speed;
        }
    }

    float CurrentJumpForce
    {
        get
        {
            float jf = baseJumpForce;

            // higher jump with jump mask
            if (currentAbility == AbilityMode.DoubleJump)
                jf *= doubleJumpJumpForceMultiplier;

            return jf;
        }
    }


    // (Every Frame)
    // Check if player switched abilities
    // Drain stamina for abilities 3 & 4
    // Regenerate stamina if allowed
    // Handle jump input
    // Handle dash logic
    void Update()
    {
        HandleAbilitySwitch();

        DrainStaminaWhileRevealing();
        DrainStaminaWhilePhasing();

        RegenerateStamina();
        UpdateJumpAssistTimers();
        Jump();
        HandleDash();
    }

    // Moves player based on input unless dashing
    void FixedUpdate()
    {
        if (isDashing) return;
        if (externalMovementLock) return;
        rb.linearVelocity = new Vector2(moveInput * CurrentMoveSpeed, rb.linearVelocity.y);

    }

    // Input Handlers
    public void OnMove(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();

        if (!canMoveLeft && input < 0f)
            input = 0f;

        moveInput = input;
    }

    // Reads A/D input and initiates dash if certain conditions are met
    public void OnDash(InputAction.CallbackContext context)
    {
        if (currentAbility != AbilityMode.Dash) return;
        if (!context.performed) return;
        if (Time.time < lastDashTime + dashCooldown) return;
        if (stamina < dashStaminaCost) return;

        stamina -= dashStaminaCost;
        BlockStaminaRegen();
        StartDash();
    }

    // Ability Switching
    // Checks for which ability key was pressed and switches if unlocked
    void HandleAbilitySwitch()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame && dashUnlocked)
            ToggleAbility(AbilityMode.Dash);

        if (Keyboard.current.digit2Key.wasPressedThisFrame && doubleJumpUnlocked)
            ToggleAbility(AbilityMode.DoubleJump);

        if (Keyboard.current.digit3Key.wasPressedThisFrame && revealPlatformsUnlocked)
            ToggleAbility(AbilityMode.RevealPlatforms);

        if (Keyboard.current.digit4Key.wasPressedThisFrame && phaseWallsUnlocked)
            ToggleAbility(AbilityMode.PhaseWalls);
    }

    void ToggleAbility(AbilityMode ability)
    {
        // press again - remove mask
        if (currentAbility == ability) SwitchAbility(AbilityMode.None);
        else SwitchAbility(ability);
    }


    // Switches current ability and applies/removes equipped ability states as needed
    public void SwitchAbility(AbilityMode newAbility)
    {
        if (currentAbility == newAbility) return;

        // Turn OFF any ability state when leaving
        if (currentAbility == AbilityMode.RevealPlatforms)
            ApplyRevealEquippedState(false);

        if (currentAbility == AbilityMode.PhaseWalls)
            ApplyPhaseEquippedState(false);

        currentAbility = newAbility;

        // Turn ON any ability state when entering
        if (currentAbility == AbilityMode.RevealPlatforms)
            ApplyRevealEquippedState(true);

        if (currentAbility == AbilityMode.PhaseWalls)
            ApplyPhaseEquippedState(true);

        // Dash mask restriction: can't move left while Dash is equipped
        canMoveLeft = (currentAbility != AbilityMode.Dash);
        if (!canMoveLeft && moveInput < 0f)
            moveInput = 0f;
    }

    // Applies the equipped state for *Reveal Platforms* ability
    void ApplyRevealEquippedState(bool equipped)
    {
        if (revealAbility == null) return;

        revealAbility.unlocked = revealPlatformsUnlocked;
        revealAbility.SetEquipped(equipped);
    }

    // Applies the equipped state for *Phase Walls* ability
    void ApplyPhaseEquippedState(bool equipped)
    {
        if (phaseWallsAbility == null) return;

        phaseWallsAbility.unlocked = phaseWallsUnlocked;
        phaseWallsAbility.SetEquipped(equipped); // this should flip isTrigger on the target walls
    }

    // Refreshing unlocked states for abilities since it's referenced elsewhere
    public void RefreshRevealUnlockedState()
    {
        if (revealAbility != null)
            revealAbility.unlocked = revealPlatformsUnlocked;
    }

    public void RefreshPhaseWallsUnlockedState()
    {
        if (phaseWallsAbility != null)
            phaseWallsAbility.unlocked = phaseWallsUnlocked;
    }

    // Dash Handling
    // Initiates dash state and stops normal movement whilst dashing
    void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        // Record time of dash for cooldown tracking
        lastDashTime = Time.time;
        // Decides dash direction based on current input, defaults to right if no input
        float dashDirection = moveInput != 0 ? Mathf.Sign(moveInput) : 1f;
        // Sets velocity directly for dash and stops vertical movement
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
    }

    // Handles dash timing and ends dash when time is up
    void HandleDash()
    {
        // If not dashing, do nothing
        if (!isDashing) return;

        // Decrease dash time based on how long the frame took
        dashTime -= Time.deltaTime;
        // If dash time is up, end dash
        if (dashTime <= 0f)
            isDashing = false;
    }

    // Stamina Management

    // Blocks stamina regeneration for a set delay after stamina is used
    void BlockStaminaRegen()
    {
        regenBlockedUntilTime = Time.time + staminaRegenDelay;
    }

    // Regenerates stamina over time if not blocked
    void RegenerateStamina()
    {
        // If stamina is full or regeneration is blocked, do nothing
        if (stamina >= maxStamina) return;
        // If still whithin blocked time, do nothing
        if (Time.time < regenBlockedUntilTime) return;

        // Smooth regen over time
        stamina += staminaRegenPerSecond * Time.deltaTime;
        // Prevent exceeding max stamina
        if (stamina > maxStamina) stamina = maxStamina;
    }

    // Drains stamina while reveal ability is active
    void DrainStaminaWhileRevealing()
    {
        // Not currently revealing platforms? Return
        if (currentAbility != AbilityMode.RevealPlatforms) return;
        // Not unlocked? Return
        if (!revealPlatformsUnlocked) return;
        // Drain rate is zero? Return
        if (revealStaminaDrainPerSecond <= 0f) return;

        // Drain stamina based on time passed
        stamina -= revealStaminaDrainPerSecond * Time.deltaTime;

        // If empty
        if (stamina <= 0f)
        {

            // Set to zero
            stamina = 0f;
            // Switch ability off
            if (disableRevealWhenOutOfStamina)
                SwitchAbility(AbilityMode.None);
        }

        // Prevent regen while draining
        BlockStaminaRegen();
    }

    // Drains stamina while phase ability is active (Same logic as above)
    void DrainStaminaWhilePhasing()
    {
        if (currentAbility != AbilityMode.PhaseWalls) return;
        if (!phaseWallsUnlocked) return;
        if (phaseStaminaDrainPerSecond <= 0f) return;

        stamina -= phaseStaminaDrainPerSecond * Time.deltaTime;

        if (stamina <= 0f)
        {
            stamina = 0f;
            if (disablePhaseWhenOutOfStamina)
                SwitchAbility(AbilityMode.None);
        }

        // Prevent regen while draining
        BlockStaminaRegen();
    }

    // Jump Handling
    void Jump()
    {
        // Cant jump durinig dash
        if (isDashing) return;

        // If there is no pressed jump in the buffer, we do nothing.
        if (jumpBufferTimer <= 0f) return;

        bool canGroundJump = isGrounded || coyoteTimer > 0f;

        // Ground / Coyote jump
        if (canGroundJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, CurrentJumpForce);

            // Kill the buffer and the coyote to prevent repetitions
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            return;
        }

        // Double jump (if ccannot do basic jump)
        if (currentAbility == AbilityMode.DoubleJump && !hasUsedDoubleJump)
        {
            float secondJumpCost = dashStaminaCost * secondJumpStaminaMultiplier;
            if (stamina < secondJumpCost) return;

            stamina -= secondJumpCost;
            BlockStaminaRegen();
            hasUsedDoubleJump = true;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, CurrentJumpForce);

            // kill buffer
            jumpBufferTimer = 0f;
        }
    }


    // Ground Check
    [SerializeField] private float groundNormalThreshold = 0.6f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        EvaluateGroundedFromCollision(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        EvaluateGroundedFromCollision(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        isGrounded = false;
    }

    void EvaluateGroundedFromCollision(Collision2D collision)
    {
        // We consider it grounded only if there is contact "below"
        for (int i = 0; i < collision.contactCount; i++)
        {
            var n = collision.GetContact(i).normal;
            if (n.y > groundNormalThreshold)
            {
                isGrounded = true;
                hasUsedDoubleJump = false;
                return;
            }
        }

        // If there are contacts, but all the side/top ones are not grounded
        isGrounded = false;
    }


    void UpdateJumpAssistTimers()
    {
        // Jump Buffer: remember pressing
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        // Coyote
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;
    }
    public void SetExternalMovementLock(bool locked)
    {
        externalMovementLock = locked;
    }
    public float GetJumpForceForExternal()
    {
        return CurrentJumpForce;
    }

}
