using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons; // Массив всех объектов оружия игрока
    private int currentWeaponIndex = 0;             // Индекс оружия, которое выбрано сейчас

    private void Start()
    {
        SelectWeapon(currentWeaponIndex);
    }

    private void Update()
    {
        // Переключение с помощью клавиш 
        for (int i = 0; i < weapons.Length; i++)
        {
            // Проверяем нажатие клавиши
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectWeapon(i);
                break;
            }
        }

        // Переключение с помощью колесика мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // Получаем данные о прокрутке

        if (scroll > 0f) // Прокрутка вверх — выбираем следующее оружие
        {
            // Используем остаток от деления (%), чтобы после последнего оружия вернуться к первому
            SelectWeapon((currentWeaponIndex + 1) % weapons.Length);
        }
        else if (scroll < 0f) // Прокрутка вниз — выбираем предыдущее оружие
        {
            // Добавляем weapons.Length, чтобы при уходе в минус индекс корректно зациклился назад
            SelectWeapon((currentWeaponIndex - 1 + weapons.Length) % weapons.Length);
        }
    }

    // Основной метод для смены активного оружия
    private void SelectWeapon(int index)
    {
        // Проверка на корректность индекса (защита от ошибок)
        if (index < 0 || index >= weapons.Length) return;

        // Проходим по всему массиву и включаем только то оружие, чей индекс совпал
        for (int i = 0; i < weapons.Length; i++)
        {
            // Если i == index, метод SetActive получит true, иначе false
            weapons[i].SetActive(i == index);
        }

        // Запоминаем новый текущий индекс
        currentWeaponIndex = index;
    }
}