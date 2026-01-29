using UnityEngine;

public class PlayerPersistenceSync : MonoBehaviour
{
    PlayerMovement2D pm;

    void Awake() => pm = GetComponent<PlayerMovement2D>();

    void Start()
    {
        if (GameManager.I == null) return;

        pm.dashUnlocked = GameManager.I.dashUnlocked;
        pm.doubleJumpUnlocked = GameManager.I.doubleJumpUnlocked;
        pm.revealPlatformsUnlocked = GameManager.I.revealPlatformsUnlocked;
        pm.phaseWallsUnlocked = GameManager.I.phaseWallsUnlocked;

        pm.RefreshRevealUnlockedState();
        pm.RefreshPhaseWallsUnlockedState();

        //pm.ForceSetAbility(GameManager.I.currentAbility);
    }
}
