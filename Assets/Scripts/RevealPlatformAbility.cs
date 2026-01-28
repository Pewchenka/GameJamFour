using UnityEngine;

public class RevealPlatformsAbility : MonoBehaviour
{
    // Whether the ability has been unlocked
    // If not unlocked, ability cannot be equipped
    public bool unlocked = false;

    // Array of targets to reveal platforms, each platform needs a RevealPlatformTarget script on it so it knows to enable/disable renderer and collider (hiding/showing the object and making it solid/passable)
    public RevealPlatformTarget[] targets;

    // On Awake, automatically find all objects that have RevealPlatformTarget script on them and stores them in "targets"
    void Awake()
    {
        if (targets == null || targets.Length == 0)
            targets = FindObjectsByType<RevealPlatformTarget>(FindObjectsSortMode.None);

        // Force the ability off on start
        SetEquipped(false);
    }

    // This function is called by PlayerMovement2D when ability 3 is equipped/unequipped
    public void SetEquipped(bool equipped)
    {
        // If ability not unlocked, force equipped to false
        if (!unlocked) equipped = false;

        foreach (var t  in targets)
        {
            // If individual target is not null, apply the on/off state
            if (t != null)
                t.ApplyState(equipped, equipped);

        }
    }
}