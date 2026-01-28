using UnityEngine;

public class PhaseWallsAbility : MonoBehaviour
{
    // Whether the ability has been unlocked
    public bool unlocked = false;

    // Array of targets to phase through walls, each wall needs a PhaseWallTarget script on it so it knows to enable/disable collider (making it solid/passable)
    public PhaseWallTarget[] targets;

    void Awake()
    {
        // Auto-find all PhaseWallTarget objects in the scene if none assigned
        if (targets == null || targets.Length == 0)
            targets = FindObjectsByType<PhaseWallTarget>(FindObjectsSortMode.None);

        // Force the ability off on start
        SetEquipped(false);
    }

    // This function is called by PlayerMovement2D when ability 4 is equipped/unequipped
    public void SetEquipped(bool equipped)
    {
        // If ability not unlocked, force equipped to false
        if (!unlocked) equipped = false;
        // If no targets, exit
        if (targets == null) return;

        // Loop through each target and set its state
        for (int i = 0; i < targets.Length; i++)
        {
            // Skip null targets
            if (targets[i] == null) continue;

            // Set solid state based on equipped status
            if (equipped)
                targets[i].SetSolid(false);      // trigger ON
            // If unequipped, restore original solidity state
            else
                targets[i].RestoreOriginal();    // back to original
        }
    }

    // Ensure ability is turned off when object is disabled or destroyed
    void OnDisable() => SetEquipped(false);
    void OnDestroy() => SetEquipped(false);
}
