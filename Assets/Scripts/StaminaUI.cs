using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaUI : MonoBehaviour
{
    // Links to other objects
    [Header("References")]
    // Reference to the player to get stamina info from
    public PlayerMovement2D player;
    // UI Elements
    public Image staminaFill;
    public TMP_Text staminaText;

    // Console debugging option
    [Header("Options")]
    public bool showConsoleDebug = false;

    void Update()
    {
        if (player == null) return;

        // Update stamina bar
        if (staminaFill != null)
            staminaFill.fillAmount = player.StaminaNormalized;

        // Update text (e.g. "80/100")
        if (staminaText != null)
        {
            int current = Mathf.RoundToInt(player.stamina);
            int max = Mathf.RoundToInt(player.maxStamina);
            staminaText.text = $"{current}/{max}";
        }

        // Optional console debug (spammy, so turned off by default)
        if (showConsoleDebug)
            Debug.Log($"Stamina: {player.stamina}/{player.maxStamina}");
    }
}
