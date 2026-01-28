using UnityEngine;

public class PhaseWallsPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement2D>();
        if (player == null) return;

        player.phaseWallsUnlocked = true;
        player.RefreshPhaseWallsUnlockedState();

        Destroy(gameObject);
    }
}
