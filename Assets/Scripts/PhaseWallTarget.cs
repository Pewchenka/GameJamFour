using UnityEngine;

public class PhaseWallTarget : MonoBehaviour
{
    // Array of colliders on this object to enable/disable
    public Collider2D[] colliders;

    // Stores the starting isTrigger states of each collider
    private bool[] originalIsTrigger;

    void Awake()
    {
        // Auto-assign colliders if not set in inspector
        if (colliders == null || colliders.Length == 0)
            colliders = GetComponents<Collider2D>();

        // Store original isTrigger states
        originalIsTrigger = new bool[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            // Safety check for null colliders
            if (colliders[i] != null)
                originalIsTrigger[i] = colliders[i].isTrigger;
        }
    }

    // Function is called when ability is equipped
    public void SetSolid(bool solid)
    {
        // If no colliders, exit
        if (colliders == null) return;

        // Set each collider's isTrigger based on solid parameter
        for (int i = 0; i < colliders.Length; i++)
        {
            // Safety check for null colliders
            if (colliders[i] != null)
                colliders[i].isTrigger = !solid;
        }
    }

    // Restores each collider to how it started
    public void RestoreOriginal()
    {
        // If no colliders or original states, exit
        if (colliders == null || originalIsTrigger == null) return;

        // Restore each collider's isTrigger to original state
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null && i < originalIsTrigger.Length)
                colliders[i].isTrigger = originalIsTrigger[i];
        }
    }
}
