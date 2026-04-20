using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement")] // Настройки скорости и прыжка
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -15f;

    [Header("Mouse Look")] // Настройки обзора мышью
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("References")] // Ссылка на дочерний объект камеры
    [SerializeField] private Transform playerCamera;

    private CharacterController controller;
    private Vector3 velocity; // Используется для расчета вертикальной скорости 
    private float xRotation = 0f; // Текущий угол наклона головы игрока

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Скрываем курсор 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Вызываем методы каждый кадр
        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        // Получаем ввод мыши по осям X и Y
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Поворачиваем тело игрока влево-вправо вокруг вертикальной оси Y
        transform.Rotate(Vector3.up * mouseX);

        // Рассчитываем наклон головы вверх-вниз
        xRotation -= mouseY;
        // Ограничиваем угол наклона
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        // Применяем вращение к локальной ориентации камеры
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        // Получаем ввод с клавиатуры
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Рассчитываем вектор направления движения относительно поворота персонажа
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Выбираем скорость
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Если персонаж на земле, сбрасываем вертикальную скорость
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Небольшое давление вниз, чтобы персонаж не парил
        }

        // Логика прыжка: проверяем нажатие кнопки и стоит ли игрок на земле
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            // Формула для прыжка на определенную высоту
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Применяем гравитацию
        if (velocity.y < 0)
        {
            // Усиленная гравитация при падении 
            velocity.y += gravity * 1.5f * Time.deltaTime;
        }
        else
        {
            // Обычная гравитация, когда игрок летит вверх
            velocity.y += gravity * Time.deltaTime;
        }

        // Собираем горизонтальное движение и вертикальную скорость воедино
        Vector3 finalMove = (move * currentSpeed) + (Vector3.up * velocity.y);

        // Передвигаем персонажа через CharacterController с учетом времени кадра
        controller.Move(finalMove * Time.deltaTime);
    }

    // Метод для добавления эффекта отдачи 
    public void AddRecoil(float recoilAmount)
    {
        xRotation -= recoilAmount;
    }
}