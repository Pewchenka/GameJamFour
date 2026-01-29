using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRespawn : MonoBehaviour
{
    [Header("Fallback")]
    [SerializeField] private bool reloadSceneIfNoCheckpoint = true;

    [Header("Respawn Safety")]
    [SerializeField] private float invulnTimeAfterRespawn = 0.2f;

    private Rigidbody2D rb;
    private Vector3 checkpointPos;
    private bool hasCheckpoint;
    private float invulnUntil;

    public static System.Action OnPlayerRespawned;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Respawn();
    }


    public void SetCheckpoint(Vector3 pos)
    {
        checkpointPos = pos;
        hasCheckpoint = true;
    }

    public void Respawn()
    {
        if (Time.time < invulnUntil) return;

        if (!hasCheckpoint)
        {
            if (reloadSceneIfNoCheckpoint)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        // teleport
        transform.position = checkpointPos;

        // physic reset
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        invulnUntil = Time.time + invulnTimeAfterRespawn;

        OnPlayerRespawned?.Invoke();
    }
}
