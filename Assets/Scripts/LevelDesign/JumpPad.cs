using UnityEngine;

public class Jumper : MonoBehaviour
{
    [Header("Jump Pad")]
    [SerializeField] private float launchVelocityY = 16f;   // сила подброса
    [SerializeField] private bool resetVerticalVelocity = true;
    [SerializeField] private float cooldown = 0.05f;        // защита от многократных триггеров

    private float nextAllowedTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < nextAllowedTime) return;

        var rb = other.attachedRigidbody;
        if (rb == null) return;

        // Чтобы не подкидывать случайные физ-объекты, проверяем игрока
        if (other.GetComponent<PlayerMovement2D>() == null) return;

        if (resetVerticalVelocity)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, launchVelocityY);

        nextAllowedTime = Time.time + cooldown;
    }
}
