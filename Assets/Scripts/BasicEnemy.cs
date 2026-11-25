using UnityEngine;

public class BasicEnemy : Enemy
{
    private float damageCooldown = 0.5f;
    private float nextDamageTime = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(damageToPlayer);
                nextDamageTime = Time.time + damageCooldown;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null && Time.time >= nextDamageTime)
            {
                player.TakeDamage(staydamge);
                nextDamageTime = Time.time + damageCooldown;
            }
        }
    }
}
