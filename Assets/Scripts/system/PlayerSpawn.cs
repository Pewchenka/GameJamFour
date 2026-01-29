using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        var player = FindObjectOfType<PlayerMovement2D>();
        if (player == null) return;

        // move player to spwanpoint
        player.transform.position = transform.position;

        var player1 = FindObjectOfType<PlayerRespawn>();
        if (player1 == null) return;

        player1.SetCheckpoint(transform.position);
        // aplly masks
        if (GameManager.I != null)
            GameManager.I.ApplySaveToPlayer(player);
    }
}
