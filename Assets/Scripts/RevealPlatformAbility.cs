using UnityEngine;
using UnityEngine.SceneManagement;

public class RevealPlatformsAbility : MonoBehaviour
{
    [Header("State")]
    public bool unlocked = false;

    [Header("Target Filter")]
    [Tooltip("Only RevealPlatformTarget objects on these layers will be affected.")]
    [SerializeField] private LayerMask targetLayers;

    [Tooltip("Re-scan on scene load (useful because player is persistent).")]
    [SerializeField] private bool rescanOnSceneLoad = true;

    public RevealPlatformTarget[] targets;

    private bool isEquipped;

    void Awake()
    {
        ScanTargets();
        SetEquipped(false);

        if (rescanOnSceneLoad)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SetEquipped(false);
    }

    void OnDisable() => SetEquipped(false);

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ScanTargets();

        // if mask veared use to this scene
        if (isEquipped)
            SetEquipped(true);
    }

    [ContextMenu("Scan Targets Now")]
    public void ScanTargets()
    {
        var all = FindObjectsByType<RevealPlatformTarget>(FindObjectsSortMode.None);

        if (targetLayers.value == 0)
        {
            targets = all;
            return;
        }

        System.Collections.Generic.List<RevealPlatformTarget> filtered = new();
        for (int i = 0; i < all.Length; i++)
        {
            var t = all[i];
            if (t == null) continue;

            int layerBit = 1 << t.gameObject.layer;
            if ((targetLayers.value & layerBit) != 0)
                filtered.Add(t);
        }

        targets = filtered.ToArray();
    }

    // Called by PlayerMovement2D when ability 3 is equipped/unequipped
    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;

        if (!unlocked) equipped = false;
        if (targets == null) return;

        for (int i = 0; i < targets.Length; i++)
        {
            var t = targets[i];
            if (t == null) continue;

            t.ApplyState(equipped, equipped); // visible + solid
        }
    }
}
