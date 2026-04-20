using TMPro;
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    // Перечисление режимов стрельбы: одиночный или автоматический
    public enum FireMode { SemiAuto, FullAuto }

    [Header("Weapon Settings")]
    [SerializeField] private FireMode fireMode = FireMode.FullAuto;
    [SerializeField] private float damage = 10f;       // Урон за одно попадание
    [SerializeField] private float fireRate = 0.1f;    // Задержка между выстрелами
    [SerializeField] private float range = 100f;       // Максимальная дистанция полета пули
    [SerializeField] private int maxAmmo = 30;         // Вместимость магазина
    [SerializeField] private float reloadTime = 2f;    // Длительность перезарядки
    [SerializeField] private LayerMask enemyLayer;     // Слой, на котором находятся враги

    [Header("Spread Settings")]
    [SerializeField] private float baseSpread = 0.02f; // Базовый разброс пуль (когда стоим)
    [SerializeField] private float movementSpreadMultiplier = 0.05f; // На сколько увеличится разброс при движении

    [Header("Debug")]
    [SerializeField] private bool showHitMarkers = true; // Показывать ли метки в месте попадания
    [SerializeField] private GameObject hitMarkerPrefab; // Префаб визуального эффекта попадания

    [Header("Recoil")]
    [SerializeField] private float recoilAmount = 1.5f; // Сила отдачи (подброс камеры вверх)

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash; // Вспышка у дула при выстреле
    [SerializeField] private AudioClip shootClip;        // Звуковой файл выстрела
    [SerializeField] private AudioSource reloadSound;    // Источник звука для перезарядки

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;   // Ссылка на текст интерфейса с патронами

    private int currentAmmo;      // Патроны в текущем магазине
    private float nextFireTime;   // Время, когда можно будет сделать следующий выстрел
    private bool isReloading;     // Флаг процесса перезарядки

    private Camera playerCamera;
    private FPSController fpsController;
    private PlayerInventory inventory;
    private CharacterController characterController;

    private void Awake()
    {
        // Поиск необходимых компонентов при инициализации
        playerCamera = Camera.main;
        fpsController = GetComponentInParent<FPSController>();
        inventory = GetComponentInParent<PlayerInventory>();
        characterController = GetComponentInParent<CharacterController>();
        currentAmmo = maxAmmo;
    }

    private void OnEnable()
    {
        isReloading = false;
        UpdateAmmoUI();

        // Подписываемся на событие изменения патронов в инвентаре, чтобы обновлять интерфейс
        if (inventory != null)
        {
            inventory.OnAmmoChanged += UpdateAmmoUI;
        }
    }

    private void OnDisable()
    {
        // Отписка от события при деактивации оружия, чтобы не было утечек памяти
        if (inventory != null)
        {
            inventory.OnAmmoChanged -= UpdateAmmoUI;
        }
    }

    private void Update()
    {
        if (isReloading) return; // Блокируем стрельбу во время перезарядки

        // Определяем нажатие кнопки в зависимости от режима стрельбы
        bool canShoot = fireMode == FireMode.FullAuto ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        // Проверка условий для выстрела
        if (canShoot && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0) Shoot();
            else StartCoroutine(Reload()); // Авто-перезарядка при пустом магазине
        }

        // Ручная перезарядка на кнопку R
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && inventory.sharedAmmo > 0)
            StartCoroutine(Reload());
    }

    private void Shoot()
    {
        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        UpdateAmmoUI();

        if (muzzleFlash != null) muzzleFlash.Play();

        // Проигрываем звук выстрела в пространстве
        if (shootClip != null)
            AudioSource.PlayClipAtPoint(shootClip, transform.position);

        // Передаем команду контроллеру игрока на подброс камеры (отдачу)
        if (fpsController != null) fpsController.AddRecoil(recoilAmount);

        // Расчет динамического разброса: чем быстрее бежим, тем больше радиус разброса
        float currentSpeed = characterController.velocity.magnitude;
        float dynamicSpread = baseSpread + (currentSpeed * movementSpreadMultiplier);

        // Формируем направление выстрела с учетом случайного отклонения
        Vector3 direction = playerCamera.transform.forward;
        direction += playerCamera.transform.right * Random.Range(-dynamicSpread, dynamicSpread);
        direction += playerCamera.transform.up * Random.Range(-dynamicSpread, dynamicSpread);

        // Выполняем проверку попадания лучом (Raycast)
        if (Physics.Raycast(playerCamera.transform.position, direction, out RaycastHit hit, range, enemyLayer))
        {
            // Создаем временный маркер в точке попадания
            if (showHitMarkers && hitMarkerPrefab != null)
            {
                GameObject marker = Instantiate(hitMarkerPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(marker, 3f);
            }

            // Пытаемся найти скрипт врага и нанести ему урон
            EnemyTarget target = hit.collider.GetComponent<EnemyTarget>();
            if (target != null) target.TakeDamage(damage);
        }
    }

    // Корутина для имитации процесса перезарядки
    private IEnumerator Reload()
    {
        isReloading = true;
        if (reloadSound != null) reloadSound.Play();

        yield return new WaitForSeconds(reloadTime); // Ожидание завершения таймера

        // Запрашиваем нужное количество патронов из общего инвентаря
        currentAmmo += inventory.TakeAmmo(maxAmmo - currentAmmo);

        isReloading = false;
        UpdateAmmoUI();
    }

    // Метод для обновления текстового интерфейса патронов
    private void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {inventory.sharedAmmo}";
    }
}