using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OneWayPlatformDrop : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private float dropDuration = 0.25f; // time of ignoring
    [SerializeField] private LayerMask oneWayPlatformMask; // layer

    [Header("Input")]
    //[SerializeField] private bool requireJumpToDrop = false; // if true: down + space
    [SerializeField] private Key dropKey = Key.S;

    private Collider2D playerCol;
    private Rigidbody2D rb;

    void Awake()
    {
        playerCol = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool downHeld = Keyboard.current[dropKey].isPressed;

        bool dropPressed;
        dropPressed = Keyboard.current[dropKey].wasPressedThisFrame;
        
        /*/ if (requireJumpToDrop)
        {
            // down + soace
            dropPressed = downHeld && Keyboard.current.spaceKey.wasPressedThisFrame;
        }
        else
        {
            // just down
            dropPressed = Keyboard.current[dropKey].wasPressedThisFrame;
        }
        /*/

        if (!dropPressed) return;

        Collider2D platform = GetPlatformUnderPlayer();
        if (platform == null) return;

        StartCoroutine(TemporarilyIgnore(platform));
    }

    Collider2D GetPlatformUnderPlayer()
    {
        Vector2 size = new Vector2(playerCol.bounds.size.x * 0.9f, 0.1f);
        Vector2 center = new Vector2(playerCol.bounds.center.x, playerCol.bounds.min.y - 0.05f);

        var hit = Physics2D.OverlapBox(center, size, 0f, oneWayPlatformMask);
        return hit;
    }

    IEnumerator TemporarilyIgnore(Collider2D platformCol)
    {
        // ignore collision
        Physics2D.IgnoreCollision(playerCol, platformCol, true);

        // little force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, -1f));

        yield return new WaitForSeconds(dropDuration);

        Physics2D.IgnoreCollision(playerCol, platformCol, false);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (playerCol == null) return;
        Gizmos.color = Color.yellow;
        Vector2 size = new Vector2(playerCol.bounds.size.x * 0.9f, 0.1f);
        Vector2 center = new Vector2(playerCol.bounds.center.x, playerCol.bounds.min.y - 0.05f);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
