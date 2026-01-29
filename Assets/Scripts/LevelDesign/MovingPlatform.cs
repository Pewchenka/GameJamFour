using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform2D : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Motion")]
    [SerializeField] private float speed = 2.5f;          // units per second
    [SerializeField] private float stopTime = 0.5f;       // pause at ends
    [SerializeField] private float arriveThreshold = 0.02f;

    private Rigidbody2D rb;
    private Vector2 target;
    private bool goingToB = true;

    private float waitTimer;

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

        // стартуем у A
        rb.position = pointA.position;
        target = pointB.position;
        goingToB = true;
    }

    void FixedUpdate()
    {
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
            // переключаем цель
            goingToB = !goingToB;
            target = goingToB ? (Vector2)pointB.position : (Vector2)pointA.position;

            // пауза
            if (stopTime > 0f)
                waitTimer = stopTime;
        }
    }
}
