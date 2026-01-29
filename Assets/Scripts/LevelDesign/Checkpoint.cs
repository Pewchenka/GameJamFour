using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool deactivateAfterUse = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var respawn = other.GetComponent<PlayerRespawn>();
        if (respawn == null) return;

        respawn.SetCheckpoint(transform.position);

        if (deactivateAfterUse)
            gameObject.SetActive(false);
    }
}
