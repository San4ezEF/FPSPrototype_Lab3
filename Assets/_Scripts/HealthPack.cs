using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private int healAmount = 25;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();

            // Если здоровье меньше максимального, лечим и уничтожаем объект
            if (hp != null && hp.GetCurrentHealth() < hp.GetMaxHealth())
            {
                hp.Heal(healAmount);
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }
                Destroy(gameObject);
            }
        }
    }
}