using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    public int sharedAmmo = 90;
    public int maxAmmoLimit = 180;

    public Action OnAmmoChanged;

    public void AddAmmo(int amount)
    {
        sharedAmmo = Mathf.Min(sharedAmmo + amount, maxAmmoLimit);
        OnAmmoChanged?.Invoke();
    }

    public bool IsAmmoFull() => sharedAmmo >= maxAmmoLimit;

    public int TakeAmmo(int needed)
    {
        int taken = Mathf.Min(needed, sharedAmmo);
        sharedAmmo -= taken;
        OnAmmoChanged?.Invoke();
        return taken;
    }
}