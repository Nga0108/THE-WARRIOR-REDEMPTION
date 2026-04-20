using UnityEngine;
using System.Collections;

public class SpikeDamage : MonoBehaviour
{
    public float damageAmount = 5f;
    public float damageInterval = 1f;
    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
                if (damageCoroutine != null) StopCoroutine(damageCoroutine);
                damageCoroutine = StartCoroutine(ContinuousDamage(health));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (damageCoroutine != null) StopCoroutine(damageCoroutine);
        }
    }

    IEnumerator ContinuousDamage(PlayerHealth health)
    {
        while (health != null && health.currentHealth > 0)
        {
            yield return new WaitForSeconds(damageInterval);
            health.TakeDamage(damageAmount);
        }
    }
}