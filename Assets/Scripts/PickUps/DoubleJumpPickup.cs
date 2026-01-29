using UnityEngine;

public class DoubleJumpPickup : MonoBehaviour
{
    // When player collides with the pickup, unlock the double jump ability and destroy the pickup object
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object that picked up the item is the player (has PlayerMovement2D script)
        var player = other.GetComponent<PlayerMovement2D>();
        if (player == null) return;

        player.doubleJumpUnlocked = true;

        if (GameManager.I != null) GameManager.I.doubleJumpUnlocked = true;

        Destroy(gameObject);
    }
}
