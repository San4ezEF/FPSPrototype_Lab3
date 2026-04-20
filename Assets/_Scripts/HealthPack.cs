using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private int healAmount = 25;   // Количество восстанавливаемых единиц здоровья
    [SerializeField] private AudioClip pickupSound; // Звук при подборе 

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем тег "Player"
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();

            // лечим только если у игрока меньше 100% здоровья
            if (hp != null && hp.GetCurrentHealth() < hp.GetMaxHealth())
            {
                hp.Heal(healAmount);

                // Если звук назначен, проигрываем его в точке нахождения аптечки
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                // Удаляем объект аптечки со сцены после использования
                Destroy(gameObject);
            }
        }
    }
}