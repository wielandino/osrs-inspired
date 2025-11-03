using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingXPText : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _floatHeight = 2f;
    [SerializeField] private float _floatDuration = 2f;
    [SerializeField] private AnimationCurve _movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);

    private TextMeshProUGUI _textComponent;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector3 _startPosition;

    private void Awake()
    {
        _textComponent = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
    }

    public void Initialize(string text, Color color, Vector3 screenPosition)
    {
        _textComponent.text = text;
        _textComponent.color = color;

        _rectTransform.position = screenPosition;
        _startPosition = _rectTransform.position;

        StartCoroutine(AnimateFloatingText());
    }

    private IEnumerator AnimateFloatingText()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _floatDuration)
        {
            float progress = elapsedTime / _floatDuration;

            float heightOffset = _movementCurve.Evaluate(progress) * _floatHeight;
            Vector3 newPosition = _startPosition + Vector3.up * heightOffset;
            _rectTransform.position = newPosition;

            float alpha = _fadeCurve.Evaluate(progress);
            _canvasGroup.alpha = alpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}