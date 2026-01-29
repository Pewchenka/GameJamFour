using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TimedDisappearPlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float disappearDelay = 0.6f;   // time before disapear
    [SerializeField] private float respawnDelay = 1.5f;     // time to appear
    [SerializeField] private bool triggerOnlyOnceUntilRespawn = true;

    [Header("Warning Visual")]
    [SerializeField] private float warnDuration = 0.3f;     // warning time
    [SerializeField] private float blinkInterval = 0.06f;   // intreaval of blinckning
    [SerializeField] private bool shake = true;
    [SerializeField] private float shakeAmount = 0.04f;

    private Collider2D col;
    private SpriteRenderer sr;

    private bool isActive = true;
    private bool isRunning;

    private Vector3 startLocalPos;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        startLocalPos = transform.localPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;
        if (isRunning && triggerOnlyOnceUntilRespawn) return;

        
        if (collision.gameObject.GetComponent<PlayerMovement2D>() == null) return;

        StartCoroutine(RunCycle());
    }

    private IEnumerator RunCycle()
    {
        isRunning = true;

        
        float preWarn = Mathf.Max(0f, disappearDelay - warnDuration);
        if (preWarn > 0f)
            yield return new WaitForSeconds(preWarn);

        // warning
        if (warnDuration > 0f)
            yield return StartCoroutine(Warn(warnDuration));

        // disapepparea
        SetPlatformActive(false);

        // respwan tyinme
        yield return new WaitForSeconds(respawnDelay);

        // appear
        SetPlatformActive(true);

        isRunning = false;
    }

    private IEnumerator Warn(float duration)
    {
        float t = 0f;
        float nextBlink = 0f;
        bool visible = true;

        while (t < duration)
        {
            t += Time.deltaTime;

            if (t >= nextBlink)
            {
                visible = !visible;
                if (sr != null) sr.enabled = visible;
                nextBlink += blinkInterval;
            }

            if (shake)
            {
                float x = Random.Range(-shakeAmount, shakeAmount);
                float y = Random.Range(-shakeAmount, shakeAmount);
                transform.localPosition = startLocalPos + new Vector3(x, y, 0f);
            }

            yield return null;
        }

        // retutn normal state
        if (sr != null) sr.enabled = true;
        transform.localPosition = startLocalPos;
    }

    private void SetPlatformActive(bool active)
    {
        isActive = active;

        if (col != null) col.enabled = active;
        if (sr != null) sr.enabled = active;

        if (active)
            transform.localPosition = startLocalPos;
    }
}
