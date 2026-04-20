using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int sharedAmmo = 90;    // Общий запас патронов в 
    public int maxAmmoLimit = 180; // Максимально возможное количество 

    public Action OnAmmoChanged;

    // Метод для пополнения боезапаса 
    public void AddAmmo(int amount)
    {
        sharedAmmo = Mathf.Min(sharedAmmo + amount, maxAmmoLimit);

        OnAmmoChanged?.Invoke();
    }

    // Проверка: заполнен ли инвентарь патронами до предела
    public bool IsAmmoFull() => sharedAmmo >= maxAmmoLimit;

    // Метод, который вызывается оружием при перезарядке
    public int TakeAmmo(int needed)
    {
        // Выбираем меньшее: либо сколько нужно оружию, либо сколько осталось в инвентаре
        int taken = Mathf.Min(needed, sharedAmmo);

        sharedAmmo -= taken;

        OnAmmoChanged?.Invoke();

        return taken;
    }
}