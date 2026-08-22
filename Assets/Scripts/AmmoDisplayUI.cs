using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplayUI : MonoBehaviour
{
    private const int FontSize = 18;

    private PlayerWeaponHandler weaponHandler;
    private PlayerHPManager hpManager;
    private Text ammoText;
    private Text hpText;

    private void Awake()
    {
        weaponHandler = GetComponent<PlayerWeaponHandler>();
        hpManager = GetComponent<PlayerHPManager>();
        CreateUI();

        if (hpManager != null)
        {
            hpManager.OnHealthChanged += HandleHealthChanged;
            HandleHealthChanged(hpManager.CurrentHP, hpManager.MaxHP);
        }
    }

    private void OnDestroy()
    {
        if (hpManager != null)
        {
            hpManager.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void Update()
    {
        if (weaponHandler == null || ammoText == null)
        {
            return;
        }

        string leftAmmo = weaponHandler.LeftWeapon != null
            ? weaponHandler.LeftWeapon.CurrentAmmo.ToString()
            : "-";
        string rightAmmo = weaponHandler.RightWeapon != null
            ? weaponHandler.RightWeapon.CurrentAmmo.ToString()
            : "-";
        ammoText.text = $"LEFT WEAPON: {leftAmmo}   /   RIGHT WEAPON: {rightAmmo}";
    }

    private void HandleHealthChanged(float currentHP, float maxHP)
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {Mathf.CeilToInt(currentHP)}";
        }
    }

    private void CreateUI()
    {
        GameObject canvasObject = new GameObject("AmmoCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject panelObject = new GameObject("AmmoPanel");
        panelObject.transform.SetParent(canvasObject.transform, false);

        RectTransform panelTransform = panelObject.AddComponent<RectTransform>();
        panelTransform.anchorMin = new Vector2(1f, 0f);
        panelTransform.anchorMax = new Vector2(1f, 0f);
        panelTransform.pivot = new Vector2(1f, 0f);
        panelTransform.anchoredPosition = new Vector2(-24f, 24f);
        panelTransform.sizeDelta = new Vector2(400f, 48f);

        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject hpPanelObject = new GameObject("HPPanel");
        hpPanelObject.transform.SetParent(canvasObject.transform, false);

        RectTransform hpPanelTransform = hpPanelObject.AddComponent<RectTransform>();
        hpPanelTransform.anchorMin = Vector2.zero;
        hpPanelTransform.anchorMax = Vector2.zero;
        hpPanelTransform.pivot = Vector2.zero;
        hpPanelTransform.anchoredPosition = new Vector2(24f, 24f);
        hpPanelTransform.sizeDelta = new Vector2(180f, 48f);

        Image hpPanelImage = hpPanelObject.AddComponent<Image>();
        hpPanelImage.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject hpTextObject = new GameObject("HPText");
        hpTextObject.transform.SetParent(hpPanelObject.transform, false);

        RectTransform hpTextTransform = hpTextObject.AddComponent<RectTransform>();
        hpTextTransform.anchorMin = Vector2.zero;
        hpTextTransform.anchorMax = Vector2.one;
        hpTextTransform.offsetMin = new Vector2(12f, 0f);
        hpTextTransform.offsetMax = new Vector2(-12f, 0f);

        hpText = hpTextObject.AddComponent<Text>();
        hpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hpText.fontSize = FontSize;
        hpText.alignment = TextAnchor.MiddleCenter;
        hpText.horizontalOverflow = HorizontalWrapMode.Overflow;
        hpText.verticalOverflow = VerticalWrapMode.Overflow;
        hpText.color = Color.white;
        hpText.raycastTarget = false;

        GameObject textObject = new GameObject("AmmoText");
        textObject.transform.SetParent(panelObject.transform, false);

        RectTransform textTransform = textObject.AddComponent<RectTransform>();
        textTransform.anchorMin = Vector2.zero;
        textTransform.anchorMax = Vector2.one;
        textTransform.offsetMin = new Vector2(12f, 0f);
        textTransform.offsetMax = new Vector2(-12f, 0f);

        ammoText = textObject.AddComponent<Text>();
        ammoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ammoText.fontSize = FontSize;
        ammoText.alignment = TextAnchor.MiddleCenter;
        ammoText.horizontalOverflow = HorizontalWrapMode.Overflow;
        ammoText.verticalOverflow = VerticalWrapMode.Overflow;
        ammoText.color = Color.white;
        ammoText.raycastTarget = false;
    }
}