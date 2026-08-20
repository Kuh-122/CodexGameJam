using System.Collections;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
}

public enum WeaponType
{
    Revolver,
    Shotgun
}

public class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private WeaponType weaponType = WeaponType.Revolver;
    [SerializeField] private float damage = 2f;
    [SerializeField] private float fireRate = 6f;
    [SerializeField] private float reloadDuration = 1.5f;
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private int reserveAmmo = 48;
    [SerializeField] private float fireDistance = 100f;
    [SerializeField] private int shotgunPelletCount = 10;
    [SerializeField, Range(0f, 30f)] private float shotgunSpreadAngle = 6f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Animator animator;
    [SerializeField] private string fireTrigger = "Fire";
    [SerializeField] private string reloadTrigger = "Reload";

    private int currentAmmo;
    private float nextFireTime;
    private Coroutine reloadRoutine;

    public bool IsReloading => reloadRoutine != null;
    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (name.ToLowerInvariant().Contains("shotgun"))
        {
            weaponType = WeaponType.Shotgun;
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

        animator?.SetTrigger(fireTrigger);

        FireHitscan();
        Debug.Log($"{name} fired. Ammo: {currentAmmo}/{reserveAmmo}");
    }

    private void FireHitscan()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning($"{name} cannot fire: no player camera is assigned.");
            return;
        }

        int rayCount = weaponType == WeaponType.Shotgun ? Mathf.Max(shotgunPelletCount, 1) : 1;
        for (int rayIndex = 0; rayIndex < rayCount; rayIndex++)
        {
            Ray ray = CreateAttackRay();
            Debug.DrawRay(ray.origin, ray.direction * fireDistance, Color.yellow, 0.15f);

            if (Physics.Raycast(ray, out RaycastHit hit, fireDistance))
            {
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(damage);
            }
        }
    }

    private Ray CreateAttackRay()
    {
        Ray cameraRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (weaponType != WeaponType.Shotgun || shotgunSpreadAngle <= 0f)
        {
            return cameraRay;
        }

        float spreadRadius = Mathf.Tan(shotgunSpreadAngle * Mathf.Deg2Rad);
        Vector2 spread = Random.insideUnitCircle * spreadRadius;
        Vector3 direction = (playerCamera.transform.forward
            + playerCamera.transform.right * spread.x
            + playerCamera.transform.up * spread.y).normalized;
        return new Ray(cameraRay.origin, direction);
    }

    public void StartReload()
    {
        if (IsReloading || currentAmmo >= magazineSize || reserveAmmo <= 0)
        {
            return;
        }

        animator?.SetTrigger(reloadTrigger);
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
