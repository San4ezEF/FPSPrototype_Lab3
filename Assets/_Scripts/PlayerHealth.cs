using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;        // Максимальное количество здоровья
    [SerializeField] private TextMeshProUGUI healthText; // Ссылка на текстовое поле интерфейса
    [SerializeField] private GameObject deathScreen;     // Панель экрана смерти (UI)

    private int currentHealth; // Текущее здоровье игрока

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Скрываем экран смерти в самом начале игры
        if (deathScreen != null) deathScreen.SetActive(false);
    }

    // Публичные методы 
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    // Метод получения урона
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die(); // Если здоровье закончилось — вызываем смерть
        }
        UpdateHealthUI();
    }

    // Метод лечения 
    public void Heal(int amount)
    {
        // Mathf.Min гарантирует, что здоровье не станет больше максимального
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    // Логика при смерти игрока
    private void Die()
    {
        // Показываем экран смерти
        if (deathScreen != null) deathScreen.SetActive(true);

        // Возвращаем курсор, чтобы можно было нажать кнопки в меню
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Отключаем скрипт управления движением
        if (TryGetComponent<FPSController>(out var controller))
        {
            controller.enabled = false;
        }

        // Отключаем возможность переключать оружие
        WeaponSwitcher switcher = GetComponentInChildren<WeaponSwitcher>();
        if (switcher != null)
        {
            switcher.enabled = false;
        }

        // Находим всё оружие и деактивируем скрипты стрельбы
        Gun[] allGuns = GetComponentsInChildren<Gun>(true);
        foreach (Gun gun in allGuns)
        {
            gun.enabled = false;
        }

        // Отключаем сам скрипт здоровья, чтобы больше не обрабатывать урон
        this.enabled = false;
    }

    // Обновление текста HP на экране
    private void UpdateHealthUI()
    {
        if (healthText != null) healthText.text = $"HP: {currentHealth}";
    }
}