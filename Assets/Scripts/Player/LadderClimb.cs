using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class LadderClimb : MonoBehaviour
{
    [Header("Climb")]
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float snapToLadderX = 0.15f; // adjust to ladder

    [SerializeField] private float dismountPushX = 3f;

    private Rigidbody2D rb;
    private PlayerMovement2D pm;

    private bool inLadderZone;
    private bool isClimbing;
    private Transform currentLadder;

    private float defaultGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement2D>();
        defaultGravity = rb.gravityScale;
    }

    void Update()
    {
        // way by key
        float v = 0f;
        if (Keyboard.current.wKey.isPressed) v += 1f;
        if (Keyboard.current.sKey.isPressed) v -= 1f;

        // only when we in ladder and pressing
        if (!isClimbing && inLadderZone && Mathf.Abs(v) > 0.01f)
            StartClimb();

        if (isClimbing)
        {
            // 1) vertical input
            rb.linearVelocity = new Vector2(0f, v * climbSpeed);

            // 2) adjust to ladder
            if (currentLadder != null)
            {
                float targetX = currentLadder.position.x;
                float newX = Mathf.Lerp(transform.position.x, targetX, 1f - Mathf.Exp(-snapToLadderX * 60f * Time.deltaTime));
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            }

            //  3) jump has priority (works even if A/D pressed)
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                float ladderH = 0f;
                if (Keyboard.current.aKey.isPressed) ladderH -= 1f;
                if (Keyboard.current.dKey.isPressed) ladderH += 1f;

                JumpOffLadder();

                if (Mathf.Abs(ladderH) > 0.01f)
                    rb.linearVelocity = new Vector2(ladderH * dismountPushX, rb.linearVelocity.y);

                return;
            }

            // 4) dismount on A/D (only if not jumping this frame)
            float ladderH2 = 0f;
            if (Keyboard.current.aKey.isPressed) ladderH2 -= 1f;
            if (Keyboard.current.dKey.isPressed) ladderH2 += 1f;

            if (Mathf.Abs(ladderH2) > 0.01f)
            {
                StopClimb();
                rb.linearVelocity = new Vector2(ladderH2 * dismountPushX, rb.linearVelocity.y);
                return;
            }
        }



        float h = 0f;
        if (Keyboard.current.aKey.isPressed) h -= 1f;
        if (Keyboard.current.dKey.isPressed) h += 1f;

        if (Mathf.Abs(h) > 0.01f)
        {
            StopClimb();

            // дать небольшой толчок в сторону, чтобы реально "сойти"
            rb.linearVelocity = new Vector2(h * 3f, rb.linearVelocity.y);
            return;
        }
    }

    void StartClimb()
    {
        isClimbing = true;

        // gravity off
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        // no hotizontal from FixedUpdate
        if (pm != null) pm.SetExternalMovementLock(true);
    }

    void StopClimb()
    {
        isClimbing = false;

        rb.gravityScale = defaultGravity;

        if (pm != null) pm.SetExternalMovementLock(false);
    }

    void JumpOffLadder()
    {
        StopClimb();

        // basic jump (jumpForce/CurrentJumpForce)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, pm != null ? pm.GetJumpForceForExternal() : 12f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ladder")) return;

        inLadderZone = true;
        currentLadder = other.transform;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Ladder")) return;

        inLadderZone = false;
        currentLadder = null;

        if (isClimbing)
            StopClimb();
    }
}
