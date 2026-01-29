using UnityEngine;
using UnityEngine.SceneManagement;

public class PhaseWallsAbility : MonoBehaviour
{
    [Header("State")]
    public bool unlocked = false;

    [Header("Target Filter")]
    [Tooltip("Only PhaseWallTarget objects on these layers will be affected.")]
    [SerializeField] private LayerMask targetLayers;

    [Tooltip("Re-scan scene on load (useful if targets differ per level).")]
    [SerializeField] private bool rescanOnSceneLoad = true;

    public PhaseWallTarget[] targets;

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

        if (isEquipped)
            SetEquipped(true);
    }

    [ContextMenu("Scan Targets Now")]
    public void ScanTargets()
    {
        // Looking for phase target
        var all = FindObjectsByType<PhaseWallTarget>(FindObjectsSortMode.None);

        if (targetLayers.value == 0)
        {
            targets = all;
            return;
        }

        System.Collections.Generic.List<PhaseWallTarget> filtered = new();
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

    // Called by PlayerMovement2D when ability is equipped/unequipped
    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;

        if (!unlocked) equipped = false;
        if (targets == null) return;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;

            if (equipped)
                targets[i].SetSolid(false);   // passable
            else
                targets[i].RestoreOriginal(); // back to original
        }
    }
}
