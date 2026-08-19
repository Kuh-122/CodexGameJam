using System.Collections;
using UnityEngine;

public enum WeaponFireMode
{
    SemiAutomatic,
    Automatic
}

public class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private WeaponFireMode fireMode = WeaponFireMode.SemiAutomatic;
    [SerializeField] private float fireRate = 6f;
    [SerializeField] private float reloadDuration = 1.5f;
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private int reserveAmmo = 48;
    [SerializeField] private float fireDistance = 100f;
    [SerializeField] private Transform muzzle;

    private int currentAmmo;
    private float nextFireTime;
    private Coroutine reloadRoutine;

    public WeaponFireMode FireMode => fireMode;
    public bool IsReloading => reloadRoutine != null;
    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;

    private void Awake()
    {
        string weaponName = name.ToLowerInvariant();
        if (weaponName.Contains("smg"))
        {
            fireMode = WeaponFireMode.Automatic;
        }

        currentAmmo = magazineSize;
    }

    public void TryFire()
    {
        if (IsReloading || Time.time < nextFireTime || currentAmmo <= 0)
        {
            return;
        }

        currentAmmo--;
        nextFireTime = Time.time + 1f / Mathf.Max(fireRate, 0.01f);

        Transform firingPoint = muzzle != null ? muzzle : transform;
        Debug.DrawRay(firingPoint.position, firingPoint.forward * fireDistance, Color.yellow, 0.15f);
        Debug.Log($"{name} fired. Ammo: {currentAmmo}/{reserveAmmo}");
    }

    public void StartReload()
    {
        if (IsReloading || currentAmmo >= magazineSize || reserveAmmo <= 0)
        {
            return;
        }

        reloadRoutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(reloadDuration);

        int neededAmmo = magazineSize - currentAmmo;
        int loadedAmmo = Mathf.Min(neededAmmo, reserveAmmo);
        currentAmmo += loadedAmmo;
        reserveAmmo -= loadedAmmo;
        reloadRoutine = null;

        Debug.Log($"{name} reloaded. Ammo: {currentAmmo}/{reserveAmmo}");
    }
}
