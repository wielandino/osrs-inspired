using UnityEngine;

public class FloatingXPController : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private GameObject _floatingXPPrefab;
    [SerializeField] private Canvas _uiCanvas; // Das UI Canvas

    [Header("XP Display Settings")]
    [SerializeField] private Color _woodcuttingXPColor = Color.green;
    [SerializeField] private Vector3 _spawnOffset = Vector3.up * 2f; // Offset �ber dem Spieler

    public static FloatingXPController Instance { get; private set; }

    private Camera _mainCamera;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Finde Main Camera
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            _mainCamera = FindObjectOfType<Camera>();
        }

        // Finde Canvas falls nicht zugewiesen
        if (_uiCanvas == null)
        {
            _uiCanvas = FindObjectOfType<Canvas>();
        }
    }

    public void ShowXPGain(SkillType skillType, float xpAmount, Vector3 worldPosition)
    {
        if (_floatingXPPrefab == null || _uiCanvas == null || _mainCamera == null)
        {
            return;
        }

        // Erstelle schwebenden Text
        GameObject floatingTextObj = Instantiate(_floatingXPPrefab, _uiCanvas.transform);
        FloatingXPText floatingText = floatingTextObj.GetComponent<FloatingXPText>();

        if (floatingText == null)
        {
            Destroy(floatingTextObj);
            return;
        }

        // Bestimme Farbe basierend auf Skill-Typ
        Color textColor = GetColorForSkill(skillType);

        // Formatiere Text
        string displayText = $"+{xpAmount:F1} XP";

        // WICHTIG: Konvertiere Weltposition zu Screen Position
        Vector3 spawnWorldPosition = worldPosition + _spawnOffset;
        Vector3 screenPosition = ConvertWorldToUIPosition(spawnWorldPosition);

        // Initialisiere und starte Animation
        floatingText.Initialize(displayText, textColor, screenPosition);
    }

    public void ShowXPGain(SkillType skillType, float xpAmount, Transform targetTransform)
    {
        ShowXPGain(skillType, xpAmount, targetTransform.position);
    }

    private Vector3 ConvertWorldToUIPosition(Vector3 worldPosition)
    {
        // Screen Space - Camera Canvas
        if (_uiCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // Konvertiere zu Screen Point
            Vector3 screenPoint = _mainCamera.WorldToScreenPoint(worldPosition);

            // F�r Screen Space Camera m�ssen wir die Position anpassen
            RectTransform canvasRect = _uiCanvas.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPoint,
                _uiCanvas.worldCamera,
                out localPoint
            );

            // Konvertiere zur�ck zu World Position f�r das Canvas
            return _uiCanvas.transform.TransformPoint(localPoint);
        }
        // Screen Space - Overlay Canvas
        else if (_uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return _mainCamera.WorldToScreenPoint(worldPosition);
        }

        // Fallback
        return _mainCamera.WorldToScreenPoint(worldPosition);
    }

    private Color GetColorForSkill(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.Woodcutting => _woodcuttingXPColor,
            _ => Color.white
        };
    }

}