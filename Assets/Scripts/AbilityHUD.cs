using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityHUD : MonoBehaviour
{
    // Reference to the player to check ability states
    [Header("Player")]
    public PlayerMovement2D player;

    // UI Elements for dash ability
    [Header("Dash UI")]
    public Image dashIcon;
    public TMP_Text dashText;
    public CanvasGroup dashGroup; // for transparency control of the whole dash UI (repeated for other abilities)

    // UI Elements for double jump ability
    [Header("Double Jump UI")]
    public Image doubleJumpIcon;
    public TMP_Text doubleJumpText;
    public CanvasGroup doubleJumpGroup;

    // UI Elements for reveal platforms ability
    [Header("Seeing UI")]
    public Image seeingIcon;
    public TMP_Text seeingText;
    public CanvasGroup seeingGroup;

    // UI Elements for phase walls ability
    [Header("Phase UI")]
    public Image phaseIcon;
    public TMP_Text phaseText;
    public CanvasGroup phaseGroup;

    // Visual settings
    [Header("Visuals")]
    // How transparent the ability UI is when locked vs unlocked
    [Range(0f, 1f)] public float lockedAlpha = 0.35f;
    [Range(0f, 1f)] public float unlockedAlpha = 1f;

    // Auto-find player if not assigned
    void Reset()
    {
        player = FindFirstObjectByType<PlayerMovement2D>();
    }

    // Updates each abilities UI and keeps the HUD in sync with player state, if player is missing, it exits
    void Update()
    {
        if (player == null) return;

        UpdateDash();
        UpdateDoubleJump();
        UpdateSeeing();
        UpdatePhaseWalls();
    }

    // This program is repeated for each ability
    void UpdateDash()
    {
        // Check if dash is unlocked and active
        bool unlocked = player.dashUnlocked;
        bool active = player.currentAbility == PlayerMovement2D.AbilityMode.Dash;

        // Update UI transparency based on unlocked/locked state
        if (dashGroup != null) dashGroup.alpha = unlocked ? unlockedAlpha : lockedAlpha;

        if (dashText != null)
        {
            // If not unlocked, show locked text
            if (!unlocked)
                dashText.text = "1: Dash (Shift) — Locked";
            // Else show active/inactive text
            else
                dashText.text = active ? "1: Dash (Shift) - 20 Stam — ACTIVE" : "1: Dash (Shift) - 20 Stam";
        }
    }

    void UpdateDoubleJump()
    {
        bool unlocked = player.doubleJumpUnlocked;
        bool active = player.currentAbility == PlayerMovement2D.AbilityMode.DoubleJump;

        if (doubleJumpGroup != null) doubleJumpGroup.alpha = unlocked ? unlockedAlpha : lockedAlpha;

        if (doubleJumpText != null)
        {
            if (!unlocked)
                doubleJumpText.text = "2: Double Jump — Locked";
            else
                doubleJumpText.text = active ? "2: Double Jump - 40 Stam — ACTIVE" : "2: Double Jump - 40 Stam";
        }
    }

    void UpdateSeeing()
    {
        bool unlocked = player.revealPlatformsUnlocked;
        bool active = player.currentAbility == PlayerMovement2D.AbilityMode.RevealPlatforms;

        if (seeingGroup != null) seeingGroup.alpha = unlocked ? unlockedAlpha : lockedAlpha;

        if (seeingText != null)
        {
            if (!unlocked)
                seeingText.text = "3: Reveal Platforms — Locked";
            else
                seeingText.text = active ? "3: Reveal Platforms - 2 Stam/Sec — ACTIVE" : "3: Reveal Platforms - 2 Stam/Sec";
        }
    }

    void UpdatePhaseWalls()
    {
        bool unlocked = player.phaseWallsUnlocked;
        bool active = player.currentAbility == PlayerMovement2D.AbilityMode.PhaseWalls;

        if (phaseGroup != null) phaseGroup.alpha = unlocked ? unlockedAlpha : lockedAlpha;

        if (phaseText != null)
        {
            if (!unlocked)
                phaseText.text = "4: Phase Walls — Locked";
            else
                phaseText.text = active ? "4: Phase Walls - 2 Stam/Sec — ACTIVE" : "4: Phase Walls - 2 Stam/Sec";
        }
    }
}
