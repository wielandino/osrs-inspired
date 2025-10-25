using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour
{
    public static TooltipController Instance;

    [Header("UI References")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public Image tooltipBackground;

    [Header("Settings")]
    public Vector2 offset = new Vector2(10f, -10f);
    public float fadeSpeed = 5f;

    [Header("Auto-Resize Settings")]
    public Vector2 padding = new Vector2(20f, 10f); // Padding um den Text
    public Vector2 minSize = new Vector2(100f, 40f); // Mindestgr��e
    public Vector2 maxSize = new Vector2(400f, 200f); // Maximalgr��e

    private Camera playerCamera;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (ContextMenuPanel.Instance.MenuPanel.activeSelf)
            HideTooltip();
    }

    void Start()
    {
        playerCamera = Camera.main;
        canvasGroup = tooltipPanel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = tooltipPanel.AddComponent<CanvasGroup>();

        HideTooltip();
    }

    public void ShowTooltip(string text, Color color)
    {
        tooltipText.text = text;
        tooltipText.color = color;

        // Tooltip-Gr��e automatisch anpassen
        ResizeTooltip();

        tooltipPanel.SetActive(true);
        UpdateTooltipPosition();

        // Smooth Fade In
        StopAllCoroutines();
        StartCoroutine(FadeTooltip(1f));
    }

    public void HideTooltip()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTooltip(0f));
    }

    public void UpdateTooltipPosition()
    {
        if (!tooltipPanel.activeInHierarchy) return;

        // Mausposition in Screen Space
        Vector2 mousePos = Input.mousePosition;

        // Tooltip als RectTransform behandeln f�r UI-Positionierung
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

        // Position mit Offset setzen
        Vector2 tooltipPos = mousePos + offset;
        tooltipRect.position = tooltipPos;

        // Stelle sicher, dass Tooltip im Bildschirm bleibt
        ClampTooltipToScreen();
    }

    private void ClampTooltipToScreen()
    {
        RectTransform rectTransform = tooltipPanel.GetComponent<RectTransform>();

        // Aktuelle Position holen
        Vector2 pos = rectTransform.position;

        // Tooltip-Gr��e holen
        Vector2 tooltipSize = rectTransform.sizeDelta;

        // Bildschirmgrenzen berechnen
        float minX = tooltipSize.x * 0.5f;
        float maxX = Screen.width - (tooltipSize.x * 0.5f);
        float minY = tooltipSize.y * 0.5f;
        float maxY = Screen.height - (tooltipSize.y * 0.5f);

        // Position auf Bildschirmgrenzen begrenzen
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rectTransform.position = pos;
    }

    private System.Collections.IEnumerator FadeTooltip(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0f)
            tooltipPanel.SetActive(false);
    }

    private void ResizeTooltip()
    {
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        RectTransform textRect = tooltipText.GetComponent<RectTransform>();

        // TextMeshPro - viel einfacher!
        // Erst den Text setzen, dann Gr��e berechnen
        tooltipText.text = tooltipText.text; // Stelle sicher, dass Text gesetzt ist

        // Force Update f�r sofortige Berechnung
        tooltipText.ForceMeshUpdate();

        // Textgr��e direkt von TextMeshPro holen
        Vector2 textSize = tooltipText.GetPreferredValues();

        // Bei zu breitem Text: Nutze die maximale Breite und lass TextMeshPro wrappen
        if (textSize.x > (maxSize.x - padding.x))
        {
            textSize = tooltipText.GetPreferredValues(maxSize.x - padding.x, 0);
        }

        // Tooltip-Gr��e basierend auf Text + Padding
        Vector2 tooltipSize = textSize + padding;

        // Gr��e begrenzen
        tooltipSize.x = Mathf.Clamp(tooltipSize.x, minSize.x, maxSize.x);
        tooltipSize.y = Mathf.Clamp(tooltipSize.y, minSize.y, maxSize.y);

        tooltipRect.sizeDelta = tooltipSize;
    }

    // Die alte CalculateTextSize Methode wird nicht mehr ben�tigt f�r TextMeshPro
}