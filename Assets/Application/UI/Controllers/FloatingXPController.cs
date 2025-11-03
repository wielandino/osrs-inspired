using UnityEngine;

public class FloatingXPController : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private GameObject _floatingXPPrefab;
    [SerializeField] private Canvas _uiCanvas;

    [Header("XP Display Settings")]
    [SerializeField] private Color _woodcuttingXPColor = Color.green;
    [SerializeField] private Vector3 _spawnOffset = Vector3.up * 2f;

    public static FloatingXPController Instance { get; private set; }

    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _mainCamera = Camera.main;
        if (_mainCamera == null)
            Debug.LogError($"No Camera found for {this.name}");
        
        if (_uiCanvas == null)
            Debug.LogError($"No UI Canvas for {this.name}");
        
    }

    public void ShowXPGain(SkillType skillType, float xpAmount, Vector3 worldPosition)
    {
        if (_floatingXPPrefab == null ||
            _uiCanvas == null ||
            _mainCamera == null)
            return;
        

        GameObject floatingTextObj = Instantiate(_floatingXPPrefab, _uiCanvas.transform);
        FloatingXPText floatingText = floatingTextObj.GetComponent<FloatingXPText>();

        if (floatingText == null)
        {
            Destroy(floatingTextObj);
            return;
        }

        Color textColor = GetColorForSkill(skillType);
        string displayText = $"+{xpAmount:F1} XP";

        Vector3 spawnWorldPosition = worldPosition + _spawnOffset;
        Vector3 screenPosition = ConvertWorldToUIPosition(spawnWorldPosition);

        floatingText.Initialize(displayText, textColor, screenPosition);
    }

    public void ShowXPGain(SkillType skillType, float xpAmount, Transform targetTransform)
    {
        ShowXPGain(skillType, xpAmount, targetTransform.position);
    }

    private Vector3 ConvertWorldToUIPosition(Vector3 worldPosition)
    {
        if (_uiCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector3 screenPoint = _mainCamera.WorldToScreenPoint(worldPosition);

            RectTransform canvasRect = _uiCanvas.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPoint,
                _uiCanvas.worldCamera,
                out Vector2 localPoint
            );

            return _uiCanvas.transform.TransformPoint(localPoint);
        }
        else if (_uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return _mainCamera.WorldToScreenPoint(worldPosition);
        }

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