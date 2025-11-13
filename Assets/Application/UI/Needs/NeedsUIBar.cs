using UnityEngine;
using UnityEngine.UI;

public class NeedsUIBar : MonoBehaviour
{
    [SerializeField] 
    private NeedType _needType;

    [SerializeField] 
    private Image _fillImage;
    
    [Header("Smooth Animation")]

    [SerializeField] 
    private bool _useSmoothTransition = true;
    
    [SerializeField] 
    private float _transitionSpeed = 5f;
    
    private float _currentFillAmount = 1f;
    private float _targetFillAmount = 1f;
    
    public NeedType NeedType => _needType;
    
    private void Start()
    {
        if (_fillImage == null)
            Debug.LogError("Fill Image not assigned!");
    }
    
    private void Update()
    {
        if (_useSmoothTransition && _fillImage != null)
        {
            _currentFillAmount = Mathf.Lerp(_currentFillAmount, _targetFillAmount, Time.deltaTime * _transitionSpeed);
            _fillImage.fillAmount = _currentFillAmount;
        }
    }
    
    public void UpdateBar(float currentValue, float maxValue)
    {
        float targetPercentage = maxValue > 0 ? currentValue / maxValue : 0f;
        
        if (_useSmoothTransition)
        {
            _targetFillAmount = Mathf.Clamp01(targetPercentage);
        }
        else
        {
            _currentFillAmount = Mathf.Clamp01(targetPercentage);
            if (_fillImage != null)
                _fillImage.fillAmount = _currentFillAmount;
        }
    }
}