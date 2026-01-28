using UnityEngine;

public class RevealPlatformTarget : MonoBehaviour
{
    // Renderer and Collider2D set to public so you can enable/disable visibility and solidity
    public Renderer targetRenderer;
    public Collider2D targetCollider;

    // Setting the start state of the platform to hidden and non-solid by default
    [Header("Start State")]
    public bool startHidden = true;  
    public bool startSolid = false;

    void Awake()
    {
        // Auto-assign renderer and collider if not set in inspector
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetCollider == null)
            targetCollider = GetComponent<Collider2D>();

        // Apply the start state
        ApplyState(!startHidden, startSolid);
    }

    // Function to apply visibility and solidity state
    public void ApplyState(bool visible, bool solid)
    {
        // Set renderer visibility
        if (targetRenderer != null)
            targetRenderer.enabled = visible;

        // Set collider solidity
        if (targetCollider != null)
            targetCollider.isTrigger = !solid;
    }
}
