using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Text hpText;
    [SerializeField] private Text statusText;
    [SerializeField] private Image crosshair;

    private static Font uiFont;
    private Image damageFlash;
    private PlayerHealth playerHealth;
    private WaveManager waveManager;
    private Vector2 hpTextBasePosition;
    private Coroutine hpShakeRoutine;
    private Coroutine flashRoutine;

    private void Awake()
    {
        EnsureCanvas();
    }

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        playerHealth.OnHealthChanged += UpdateHP;
        playerHealth.OnDamaged += OnPlayerDamaged;
        UpdateHP(playerHealth.CurrentHP, playerHealth.MaxHP);

        waveManager = FindFirstObjectByType<WaveManager>();
    }

    private void Update()
    {
        if (waveManager == null || !waveManager.IsGameEnded) return;
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth == null) return;
        playerHealth.OnHealthChanged -= UpdateHP;
        playerHealth.OnDamaged -= OnPlayerDamaged;
    }

    private void EnsureCanvas()
    {
        uiFont ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = gameObject.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        if (GetComponent<CanvasScaler>() == null)
        {
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        if (hpText == null)
            hpText = CreateText("HP_Text", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(320f, 48f), 32, TextAnchor.UpperLeft, "HP: 100 / 100");

        if (statusText == null)
            statusText = CreateText("Status_Text", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800f, 120f), 56, TextAnchor.MiddleCenter, "");

        if (crosshair == null)
            crosshair = CreateCrosshair();

        if (damageFlash == null)
            damageFlash = CreateDamageFlash();
    }

    private Text CreateText(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 size, int fontSize, TextAnchor align, string defaultText)
    {
        var existing = transform.Find(name);
        GameObject go;
        if (existing != null)
        {
            go = existing.gameObject;
        }
        else
        {
            go = new GameObject(name, typeof(Text));
            go.transform.SetParent(transform, false);
        }

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;

        var text = go.GetComponent<Text>();
        text.font = uiFont;
        text.fontSize = fontSize;
        text.alignment = align;
        text.color = Color.white;
        text.text = defaultText;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private Image CreateCrosshair()
    {
        var root = new GameObject("Crosshair", typeof(RectTransform));
        root.transform.SetParent(transform, false);
        var rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = Vector2.zero;
        rootRect.sizeDelta = new Vector2(24f, 24f);

        var center = CreateCrosshairPart(root.transform, "Center", Vector2.zero, new Vector2(6f, 6f));
        CreateCrosshairPart(root.transform, "Top", new Vector2(0f, 10f), new Vector2(4f, 12f));
        CreateCrosshairPart(root.transform, "Bottom", new Vector2(0f, -10f), new Vector2(4f, 12f));
        CreateCrosshairPart(root.transform, "Left", new Vector2(-10f, 0f), new Vector2(12f, 4f));
        CreateCrosshairPart(root.transform, "Right", new Vector2(10f, 0f), new Vector2(12f, 4f));

        center.color = new Color(1f, 0.9f, 0.2f, 1f);
        return center;
    }

    private static Image CreateCrosshairPart(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        var img = go.GetComponent<Image>();
        img.color = Color.white;
        return img;
    }

    private Image CreateDamageFlash()
    {
        var go = new GameObject("DamageFlash", typeof(Image));
        go.transform.SetParent(transform, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        var img = go.GetComponent<Image>();
        img.color = new Color(1f, 0f, 0f, 0f);
        img.raycastTarget = false;
        return img;
    }

    private void UpdateHP(float currentHP, float maxHP)
    {
        if (hpText != null)
            hpText.text = $"HP: {Mathf.CeilToInt(currentHP)} / {Mathf.CeilToInt(maxHP)}";
    }

    private void OnPlayerDamaged()
    {
        ShowDamageFlash();
        ShakeHPText();
    }

    private void ShowDamageFlash()
    {
        if (damageFlash == null) return;
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        float duration = 0.15f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = t < 0.5f ? Mathf.Lerp(0f, 0.35f, t * 2f) : Mathf.Lerp(0.35f, 0f, (t - 0.5f) * 2f);
            damageFlash.color = new Color(1f, 0f, 0f, alpha);
            yield return null;
        }
        damageFlash.color = new Color(1f, 0f, 0f, 0f);
        flashRoutine = null;
    }

    private void ShakeHPText()
    {
        if (hpText == null) return;
        if (hpShakeRoutine != null)
            StopCoroutine(hpShakeRoutine);
        hpTextBasePosition = hpText.rectTransform.anchoredPosition;
        hpShakeRoutine = StartCoroutine(ShakeHPRoutine());
    }

    private IEnumerator ShakeHPRoutine()
    {
        float duration = 0.1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offset = Mathf.Sin(elapsed * 80f) * 8f;
            hpText.rectTransform.anchoredPosition = hpTextBasePosition + new Vector2(offset, 0f);
            yield return null;
        }
        hpText.rectTransform.anchoredPosition = hpTextBasePosition;
        hpShakeRoutine = null;
    }

    public void ShowStageClear()
    {
        if (statusText != null)
            statusText.text = "Stage 1 Clear!\nPress R to Restart";
    }

    public void ShowGameOver()
    {
        if (statusText != null)
            statusText.text = "Game Over\nPress R to Restart";
    }
}