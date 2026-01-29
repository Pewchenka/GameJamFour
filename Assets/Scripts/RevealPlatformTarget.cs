using UnityEngine;

public class RevealPlatformTarget : MonoBehaviour
{
    public Renderer targetRenderer;
    public Collider2D targetCollider;

    [Header("Start State")]
    public bool startHidden = true;
    public bool startSolid = false;

    [Header("Hidden Visual")]
    [Range(0f, 1f)]
    public float hiddenAlpha = 0.25f; // hiden 
    public bool hiddenIsSolid = false;

    Color originalColor;
    bool hasColor;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetCollider == null)
            targetCollider = GetComponent<Collider2D>();

        if (targetRenderer != null && targetRenderer.material.HasProperty("_Color"))
        {
            originalColor = targetRenderer.material.color;
            hasColor = true;
        }

        ApplyHiddenState(startHidden);
    }

    public void ApplyState(bool revealed, bool solid)
    {
        if (revealed)
            ApplyRevealedState(solid);
        else
            ApplyHiddenState(true);
    }

    void ApplyRevealedState(bool solid)
    {
        if (targetRenderer != null && hasColor)
        {
            Color c = originalColor;
            c.a = 1f;
            targetRenderer.material.color = c;
            targetRenderer.enabled = true;
        }

        if (targetCollider != null)
            targetCollider.isTrigger = !solid;
    }

    void ApplyHiddenState(bool hidden)
    {
        if (targetRenderer != null && hasColor)
        {
            Color c = originalColor;
            c.a = hiddenAlpha;
            targetRenderer.material.color = c;
            targetRenderer.enabled = true; // DONT TURN OFF Renderer
        }

        if (targetCollider != null)
            targetCollider.isTrigger = !hiddenIsSolid;
    }
}
