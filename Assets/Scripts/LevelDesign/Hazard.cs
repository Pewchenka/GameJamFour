using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var respawn = other.GetComponent<PlayerRespawn>();
        if (respawn == null) return;

        respawn.Respawn();
    }
}
