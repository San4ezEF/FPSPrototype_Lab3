using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject deathScreen;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        if (deathScreen != null) deathScreen.SetActive(false);
    }

    // Методы для доступа из других скриптов
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthUI();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    private void Die()
    {

        if (deathScreen != null) deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (TryGetComponent<FPSController>(out var controller))
        {
            controller.enabled = false;
        }

        WeaponSwitcher switcher = GetComponentInChildren<WeaponSwitcher>();
        if (switcher != null)
        {
            switcher.enabled = false;
        }


        Gun[] allGuns = GetComponentsInChildren<Gun>(true);
        foreach (Gun gun in allGuns)
        {
            gun.enabled = false;
        }

        this.enabled = false;
    }

    private void UpdateHealthUI()
    {
        if (healthText != null) healthText.text = $"HP: {currentHealth}";
    }
}