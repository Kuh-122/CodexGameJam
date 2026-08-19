using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("Weapon Sockets")]
    [SerializeField] private Transform leftHandSocket;
    [SerializeField] private Transform rightHandSocket;

    [Header("Equipped Weapons")]
    [SerializeField] private Weapon leftWeapon;
    [SerializeField] private Weapon rightWeapon;

    private InputAction fireLeftAction;
    private InputAction fireRightAction;
    private InputAction reloadAction;

    private void Awake()
    {
        FindMissingReferences();
        InitializeInput();
    }

    private void Update()
    {
        if (fireLeftAction == null || fireRightAction == null || reloadAction == null)
        {
            return;
        }

        if (fireLeftAction.IsPressed() && leftWeapon != null)
        {
            if (leftWeapon.FireMode == WeaponFireMode.Automatic || fireLeftAction.WasPressedThisFrame())
            {
                leftWeapon.TryFire();
            }
        }

        if (fireRightAction.IsPressed() && rightWeapon != null)
        {
            if (rightWeapon.FireMode == WeaponFireMode.Automatic || fireRightAction.WasPressedThisFrame())
            {
                rightWeapon.TryFire();
            }
        }

        if (reloadAction.WasPressedThisFrame())
        {
            leftWeapon?.StartReload();
            rightWeapon?.StartReload();
        }
    }

    private void FindMissingReferences()
    {
        if (leftHandSocket == null)
        {
            leftHandSocket = FindChild("LeftHandSocket");
        }

        if (rightHandSocket == null)
        {
            rightHandSocket = FindChild("RightHandSocket");
        }

        if (leftWeapon == null && leftHandSocket != null)
        {
            leftWeapon = FindOrCreateWeapon(leftHandSocket);
        }

        if (rightWeapon == null && rightHandSocket != null)
        {
            rightWeapon = FindOrCreateWeapon(rightHandSocket);
        }
    }

    private Weapon FindOrCreateWeapon(Transform socket)
    {
        Weapon weapon = socket.GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            return weapon;
        }

        for (int index = 0; index < socket.childCount; index++)
        {
            Transform model = socket.GetChild(index);
            if (model != socket)
            {
                return model.gameObject.AddComponent<Weapon>();
            }
        }

        return null;
    }

    private Transform FindChild(string childName)
    {
        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }

    private void InitializeInput()
    {
        var actions = InputSystem.actions;
        if (actions == null)
        {
            Debug.LogError("Input System actions are not assigned.");
            enabled = false;
            return;
        }

        var playerMap = actions.FindActionMap("Player");
        if (playerMap == null)
        {
            Debug.LogError("Player action map was not found.");
            enabled = false;
            return;
        }

        fireLeftAction = playerMap.FindAction("FireLeft");
        fireRightAction = playerMap.FindAction("FireRight");
        reloadAction = playerMap.FindAction("Reload");

        if (fireLeftAction == null || fireRightAction == null || reloadAction == null)
        {
            Debug.LogError("Weapon actions are missing. Required actions: FireLeft, FireRight, Reload.");
            enabled = false;
            return;
        }

        actions.Enable();
    }
}
