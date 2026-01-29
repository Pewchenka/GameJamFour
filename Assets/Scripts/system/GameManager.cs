using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("Prefabs")]
    public GameObject playerPrefab;

    [Header("Unlocked masks")]
    public bool dashUnlocked;
    public bool doubleJumpUnlocked;
    public bool revealPlatformsUnlocked;
    public bool phaseWallsUnlocked;

    [Header("Current ability")]
    public PlayerMovement2D.AbilityMode currentAbility = PlayerMovement2D.AbilityMode.None;

    private GameObject playerInstance;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        EnsurePlayerExists();
        SceneManager.LoadScene("1 level");
    }

    public void EnsurePlayerExists()
    {
        if (playerInstance != null) return;
        if (playerPrefab == null)
        {
            Debug.LogError("GameManager: playerPrefab is not assigned!");
            return;
        }

        playerInstance = Instantiate(playerPrefab);
        DontDestroyOnLoad(playerInstance);
    }

    public void ApplySaveToPlayer(PlayerMovement2D pm)
    {
        pm.dashUnlocked = dashUnlocked;
        pm.doubleJumpUnlocked = doubleJumpUnlocked;
        pm.revealPlatformsUnlocked = revealPlatformsUnlocked;
        pm.phaseWallsUnlocked = phaseWallsUnlocked;

        pm.RefreshRevealUnlockedState();
        pm.RefreshPhaseWallsUnlockedState();

        // Чтобы корректно включались/выключались Reveal/Phase, лучше переключать через SwitchAbility
        pm.SwitchAbility(currentAbility);
    }

    public void SaveFromPlayer(PlayerMovement2D pm)
    {
        dashUnlocked = pm.dashUnlocked;
        doubleJumpUnlocked = pm.doubleJumpUnlocked;
        revealPlatformsUnlocked = pm.revealPlatformsUnlocked;
        phaseWallsUnlocked = pm.phaseWallsUnlocked;
        currentAbility = pm.currentAbility;
    }
}
