using UnityEngine;

public static class PlayerSceneBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreatePlayerInScene()
    {
        GameObject existingPlayer = GameObject.Find("Player");
        if (existingPlayer != null)
        {
            EnsurePlayerWeaponHandler(existingPlayer);
            EnsurePlayerHPManager(existingPlayer);
            EnsureAmmoDisplay(existingPlayer);
            return;
        }

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0f, 1f, 0f);

        var controller = player.AddComponent<CharacterController>();
        controller.center = new Vector3(0f, 1f, 0f);
        controller.height = 2f;
        controller.radius = 0.5f;
        controller.enabled = true;

        GameObject cameraObject = new GameObject("PlayerCamera");
        cameraObject.transform.SetParent(player.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 1.6f, 0f);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.fieldOfView = 60f;
        camera.nearClipPlane = 0.1f;
        cameraObject.AddComponent<AudioListener>();

        var movement = player.AddComponent<PlayerMovementController>();
        movement.SetCameraTransform(cameraObject.transform);
        EnsurePlayerWeaponHandler(player);
        EnsurePlayerHPManager(player);
        EnsureAmmoDisplay(player);
    }

    private static void EnsurePlayerWeaponHandler(GameObject player)
    {
        if (player.GetComponent<PlayerWeaponHandler>() == null)
        {
            player.AddComponent<PlayerWeaponHandler>();
        }
    }

    private static void EnsureAmmoDisplay(GameObject player)
    {
        if (Object.FindFirstObjectByType<AmmoDisplayUI>() == null)
        {
            player.AddComponent<AmmoDisplayUI>();
        }
    }

    private static void EnsurePlayerHPManager(GameObject player)
    {
        if (player.GetComponent<PlayerHPManager>() == null)
        {
            player.AddComponent<PlayerHPManager>();
        }
    }
}
