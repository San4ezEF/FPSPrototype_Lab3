using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 30;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inv = other.GetComponent<PlayerInventory>();

            // Если боезапас еще не полон, подбираем
            if (inv != null && !inv.IsAmmoFull())
            {
                inv.AddAmmo(ammoAmount);
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }
                Destroy(gameObject);
            }
        }
    }
}