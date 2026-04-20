using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    private int currentWeaponIndex = 0;

    private void Start()
    {
        SelectWeapon(currentWeaponIndex);
    }

    private void Update()
    {
        // Переключение цифрами
        for (int i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectWeapon(i);
                break;
            }
        }

        // Переключение колесиком
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
            SelectWeapon((currentWeaponIndex + 1) % weapons.Length);
        else if (scroll < 0f)
            SelectWeapon((currentWeaponIndex - 1 + weapons.Length) % weapons.Length);
    }

    private void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }
        currentWeaponIndex = index;
    }
}