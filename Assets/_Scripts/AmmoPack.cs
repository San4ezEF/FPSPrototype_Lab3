using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 30;   // Сколько патронов дает этот ящик
    [SerializeField] private AudioClip pickupSound; // Звук при поднятии

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем тег "Player"
        if (other.CompareTag("Player"))
        {
            PlayerInventory inv = other.GetComponent<PlayerInventory>();

            // Если инвентарь найден и в нем есть место для патронов
            if (inv != null && !inv.IsAmmoFull())
            {
                inv.AddAmmo(ammoAmount);

                // Воспроизводим звук в точке нахождения ящика
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                // Уничтожаем объект ящика со сцены
                Destroy(gameObject);
            }
        }
    }
}