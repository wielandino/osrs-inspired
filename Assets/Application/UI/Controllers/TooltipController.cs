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
    public Vector2 offset = new(10f, -10f);
    public float fadeSpeed = 5f;

    [Header("Auto-Resize Settings")]
    public Vector2 padding = new(20f, 10f);
    public Vector2 minSize = new(100f, 40f);
    public Vector2 maxSize = new(400f, 200f);

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        Instance = null;
    }

    private void Update()
    {
        if (ContextMenuPanel.Instance.MenuPanel.activeSelf)
            HideTooltip();
    }

    private void Start()
    {
        canvasGroup = tooltipPanel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = tooltipPanel.AddComponent<CanvasGroup>();

        HideTooltip();
    }

    public void ShowTooltip(string text, Color color)
    {
        tooltipText.text = text;
        tooltipText.color = color;

        ResizeTooltip();

        tooltipPanel.SetActive(true);
        UpdateTooltipPosition();

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

        Vector2 mousePos = Input.mousePosition;

        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

        Vector2 tooltipPos = mousePos + offset;
        tooltipRect.position = tooltipPos;

        ClampTooltipToScreen();
    }

    private void ClampTooltipToScreen()
    {
        RectTransform rectTransform = tooltipPanel.GetComponent<RectTransform>();

        Vector2 pos = rectTransform.position;

        Vector2 tooltipSize = rectTransform.sizeDelta;

        float minX = tooltipSize.x * 0.5f;
        float maxX = Screen.width - (tooltipSize.x * 0.5f);
        float minY = tooltipSize.y * 0.5f;
        float maxY = Screen.height - (tooltipSize.y * 0.5f);

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

        tooltipText.text = tooltipText.text;

        tooltipText.ForceMeshUpdate();

        Vector2 textSize = tooltipText.GetPreferredValues();

        if (textSize.x > (maxSize.x - padding.x))
            textSize = tooltipText.GetPreferredValues(maxSize.x - padding.x, 0);
        

        Vector2 tooltipSize = textSize + padding;

        tooltipSize.x = Mathf.Clamp(tooltipSize.x, minSize.x, maxSize.x);
        tooltipSize.y = Mathf.Clamp(tooltipSize.y, minSize.y, maxSize.y);

        tooltipRect.sizeDelta = tooltipSize;
    }
}