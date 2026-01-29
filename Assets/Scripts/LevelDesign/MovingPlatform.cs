using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform2D : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Motion")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float stopTime = 0.5f;        // pause
    [SerializeField] private float arriveThreshold = 0.02f;

    [Header("Start Condition")]
    [Tooltip("If true, platform waits for player to step on it before starting.")]
    [SerializeField] private bool waitForPlayer = true;

    [Tooltip("Delay after player steps on before platform starts moving.")]
    [SerializeField] private float startDelay = 0.4f;

    private Rigidbody2D rb;
    private Vector2 target;
    private bool goingToB = true;

    private float waitTimer;
    private bool activated;
    private float startDelayTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("MovingPlatform2D: assign pointA and pointB");
            enabled = false;
            return;
        }

        rb.position = pointA.position;
        target = pointB.position;
        goingToB = true;

        activated = !waitForPlayer;
    }

    void FixedUpdate()
    {
        if (!activated)
        {
            if (startDelayTimer > 0f)
            {
                startDelayTimer -= Time.fixedDeltaTime;
                if (startDelayTimer <= 0f)
                    activated = true;
            }
            return;
        }

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 pos = rb.position;
        Vector2 newPos = Vector2.MoveTowards(pos, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector2.Distance(newPos, target) <= arriveThreshold)
        {
            goingToB = !goingToB;
            target = goingToB ? (Vector2)pointB.position : (Vector2)pointA.position;

            if (stopTime > 0f)
                waitTimer = stopTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!waitForPlayer || activated) return;

        if (collision.gameObject.GetComponent<PlayerMovement2D>() == null) return;

        // timer
        startDelayTimer = startDelay;
    }

    void OnEnable()
    {
        PlayerRespawn.OnPlayerRespawned += ResetPlatform;
    }

    void OnDisable()
    {
        PlayerRespawn.OnPlayerRespawned -= ResetPlatform;
    }

    public void ResetPlatform()
    {
        rb.position = pointA.position;

        goingToB = true;
        target = pointB.position;

        waitTimer = 0f;
        startDelayTimer = 0f;

        // wait again
        activated = !waitForPlayer;
    }

}
