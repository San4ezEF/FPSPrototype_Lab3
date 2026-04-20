using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    [SerializeField] private float health = 50f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0) Destroy(gameObject);
    }
}