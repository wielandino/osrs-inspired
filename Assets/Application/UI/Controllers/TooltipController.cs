using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour
{
    public static TooltipController Instance;

    [Header("UI References")]
    public GameObject TooltipPanel;
    public TextMeshProUGUI TooltipText;
    public Image TooltipBackground;

    [Header("Settings")]
    public Vector2 Offset = new(10f, -10f);
    public float FadeSpeed = 5f;

    [Header("Auto-Resize Settings")]
    public Vector2 Padding = new(20f, 10f);
    public Vector2 MinSize = new(100f, 40f);
    public Vector2 MaxSize = new(400f, 200f);

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
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
        _canvasGroup = TooltipPanel.GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
            _canvasGroup = TooltipPanel.AddComponent<CanvasGroup>();

        HideTooltip();
    }

    public void ShowTooltip(string text, Color color)
    {
        if (_canvasGroup == null)
            return;

        TooltipText.text = text;
        TooltipText.color = color;

        ResizeTooltip();

        TooltipPanel.SetActive(true);
        UpdateTooltipPosition();

        StopAllCoroutines();
        StartCoroutine(FadeTooltip(1f));
    }

    public void HideTooltip()
    {
        if (_canvasGroup == null)
            return;
            
        StopAllCoroutines();
        StartCoroutine(FadeTooltip(0f));
    }

    public void UpdateTooltipPosition()
    {
        if (!TooltipPanel.activeInHierarchy) return;

        Vector2 mousePos = Input.mousePosition;

        RectTransform tooltipRect = TooltipPanel.GetComponent<RectTransform>();

        Vector2 tooltipPos = mousePos + Offset;
        tooltipRect.position = tooltipPos;

        ClampTooltipToScreen();
    }

    private void ClampTooltipToScreen()
    {
        RectTransform rectTransform = TooltipPanel.GetComponent<RectTransform>();

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
        float startAlpha = _canvasGroup.alpha;
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime * FadeSpeed;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time);
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0f)
            TooltipPanel.SetActive(false);
    }

    private void ResizeTooltip()
    {
        RectTransform tooltipRect = TooltipPanel.GetComponent<RectTransform>();

        TooltipText.text = TooltipText.text;

        TooltipText.ForceMeshUpdate();

        Vector2 textSize = TooltipText.GetPreferredValues();

        if (textSize.x > (MaxSize.x - Padding.x))
            textSize = TooltipText.GetPreferredValues(MaxSize.x - Padding.x, 0);
        

        Vector2 tooltipSize = textSize + Padding;

        tooltipSize.x = Mathf.Clamp(tooltipSize.x, MinSize.x, MaxSize.x);
        tooltipSize.y = Mathf.Clamp(tooltipSize.y, MinSize.y, MaxSize.y);

        tooltipRect.sizeDelta = tooltipSize;
    }
}