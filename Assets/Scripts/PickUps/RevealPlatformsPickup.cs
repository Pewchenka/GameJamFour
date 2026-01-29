using UnityEngine;

public class RevealPlatformsPickup : MonoBehaviour
{
    // When player collides with the pickup, unlock the reveal platforms ability and destroy the pickup object
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object that picked up the item is the player
        var player = other.GetComponent<PlayerMovement2D>();
        // Double checking the player picked up the item
        if (player == null) return;

        // Unlock the reveal platforms ability on the player
        player.revealPlatformsUnlocked = true;
        player.RefreshRevealUnlockedState();

        if (GameManager.I != null) GameManager.I.revealPlatformsUnlocked = true;

        // Destroy the pickup object
        Destroy(gameObject);
    }
}
