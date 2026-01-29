using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportToNextLevel : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private bool playerInside;

    private PlayerMovement2D cachedPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var pm = other.GetComponent<PlayerMovement2D>();
        if (pm == null) return;

        playerInside = true;
        cachedPlayer = pm;

        GoNext();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement2D>() == null) return;
        playerInside = false;
        cachedPlayer = null;
    }

    private void Update()
    {
        if (!playerInside) return;

    }

    private void GoNext()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("TeleportToNextLevel: nextSceneName is empty!");
            return;
        }

        // сохранить состояние перед переходом
        if (cachedPlayer != null && GameManager.I != null)
        {
            GameManager.I.dashUnlocked = cachedPlayer.dashUnlocked;
            GameManager.I.doubleJumpUnlocked = cachedPlayer.doubleJumpUnlocked;
            GameManager.I.revealPlatformsUnlocked = cachedPlayer.revealPlatformsUnlocked;
            GameManager.I.phaseWallsUnlocked = cachedPlayer.phaseWallsUnlocked;
            GameManager.I.currentAbility = cachedPlayer.currentAbility;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
