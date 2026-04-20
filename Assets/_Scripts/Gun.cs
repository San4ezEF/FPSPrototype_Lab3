using TMPro;
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public enum FireMode { SemiAuto, FullAuto }

    [Header("Weapon Settings")]
    [SerializeField] private FireMode fireMode = FireMode.FullAuto;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float range = 100f;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Spread Settings")]
    [SerializeField] private float baseSpread = 0.02f; // Разброс стоя
    [SerializeField] private float movementSpreadMultiplier = 0.05f; // Насколько увеличится при беге

    [Header("Debug")]
    [SerializeField] private bool showHitMarkers = true;
    [SerializeField] private GameObject hitMarkerPrefab; // Простая сфера-префаб

    [Header("Recoil")]
    [SerializeField] private float recoilAmount = 1.5f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip shootClip; // Используем AudioClip для фикса звука
    [SerializeField] private AudioSource reloadSound;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;

    private Camera playerCamera;
    private FPSController fpsController;
    private PlayerInventory inventory;
    private CharacterController characterController;

    private void Awake()
    {
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

        // Подписываемся на событие изменения патронов в инвентаре
        if (inventory != null)
        {
            inventory.OnAmmoChanged += UpdateAmmoUI;
        }
    }

    private void OnDisable()
    {
        // Обязательно отписываемся, чтобы не возникло ошибок при переключении оружия
        if (inventory != null)
        {
            inventory.OnAmmoChanged -= UpdateAmmoUI;
        }
    }

    private void Update()
    {
        if (isReloading) return;

        bool canShoot = fireMode == FireMode.FullAuto ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        if (canShoot && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0) Shoot();
            else StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && inventory.sharedAmmo > 0)
            StartCoroutine(Reload());

   
    }

    private void Shoot()
    {
        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        UpdateAmmoUI();

        if (muzzleFlash != null) muzzleFlash.Play();

        // ФИКС ЗВУКА: Играем звук в точке, чтобы он не обрывался при смене оружия
        if (shootClip != null)
            AudioSource.PlayClipAtPoint(shootClip, transform.position);

        if (fpsController != null) fpsController.AddRecoil(recoilAmount);

        // Расчет динамического разброса
        float currentSpeed = characterController.velocity.magnitude;
        float dynamicSpread = baseSpread + (currentSpeed * movementSpreadMultiplier);

        Vector3 direction = playerCamera.transform.forward;
        direction += playerCamera.transform.right * Random.Range(-dynamicSpread, dynamicSpread);
        direction += playerCamera.transform.up * Random.Range(-dynamicSpread, dynamicSpread);

        if (Physics.Raycast(playerCamera.transform.position, direction, out RaycastHit hit, range, enemyLayer))
        {
            // Маркер попадания
            if (showHitMarkers && hitMarkerPrefab != null)
            {
                GameObject marker = Instantiate(hitMarkerPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(marker, 3f);
            }

            EnemyTarget target = hit.collider.GetComponent<EnemyTarget>();
            if (target != null) target.TakeDamage(damage);
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        if (reloadSound != null) reloadSound.Play();
        yield return new WaitForSeconds(reloadTime);
        currentAmmo += inventory.TakeAmmo(maxAmmo - currentAmmo);
        isReloading = false;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI() { if (ammoText != null) ammoText.text = $"{currentAmmo} / {inventory.sharedAmmo}"; }
}