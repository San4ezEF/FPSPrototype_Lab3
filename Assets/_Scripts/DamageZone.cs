using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int damageAmount = 10;     // Сколько урона наносим
    [SerializeField] private float damageInterval = 1f; // Как часто (раз в секунду)
    [SerializeField] private bool isInstantDamage = false; // Если true, ударит один раз при входе

    private float nextDamageTime;

    // Метод для урона при входе (например, для шипов)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isInstantDamage)
        {
            DealDamage(other);
        }
    }

    // Метод для периодического урона (например, для лавы)
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isInstantDamage)
        {
            if (Time.time >= nextDamageTime)
            {
                DealDamage(other);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    private void DealDamage(Collider player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
            Debug.Log($"Игрок получил урон: {damageAmount}. Осталось HP: {health.GetCurrentHealth()}");
        }
    }
}