using UnityEngine;

public class DashPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement2D>();
        if (player == null) return;

        player.dashUnlocked = true;

        if (GameManager.I != null)
            GameManager.I.dashUnlocked = true;

        Destroy(gameObject);
    }
}
