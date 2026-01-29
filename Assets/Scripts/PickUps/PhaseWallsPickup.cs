using UnityEngine;

public class PhaseWallsPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement2D>();
        if (player == null) return;

        player.phaseWallsUnlocked = true;
        player.RefreshPhaseWallsUnlockedState();

        if (GameManager.I != null) GameManager.I.phaseWallsUnlocked = true;

        Destroy(gameObject);
    }
}
