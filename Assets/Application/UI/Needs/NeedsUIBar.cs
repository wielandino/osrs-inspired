using UnityEngine;
using UnityEngine.UI;

public class NeedsUIBar : MonoBehaviour
{
    [SerializeField]
    private Image _fillImage;
    
    private float _currentPercentage = 1f;
    
    [Header("Optional: Smooth Animation")]
    [SerializeField]
    private bool _useSmoothTransition = true;
    [SerializeField]
    private float _transitionSpeed = 5f;
    
    private float _targetPercentage = 1f;
    
    private void Start()
    {
        if (_fillImage == null)
        {
            Debug.LogError("Fill Image not assigned in NeedsUIBar!");
            return;
        }
        
        UpdateBarVisual();
    }
    
    private void Update()
    {
        if (_useSmoothTransition && _fillImage != null)
        {
            _currentPercentage = Mathf.Lerp(_currentPercentage, _targetPercentage, Time.deltaTime * _transitionSpeed);
            _fillImage.fillAmount = _currentPercentage;
        }
    }
    
    public void AddValueToBar(float amount)
    {
        if (_useSmoothTransition)
        {
            _targetPercentage = Mathf.Clamp01(_targetPercentage + amount);
        }
        else
        {
            _currentPercentage = Mathf.Clamp01(_currentPercentage + amount);
            UpdateBarVisual();
        }
    }
    
    public void DecreaseValueFromBar(float amount)
    {
        if (_useSmoothTransition)
        {
            _targetPercentage = Mathf.Clamp01(_targetPercentage - amount);
        }
        else
        {
            _currentPercentage = Mathf.Clamp01(_currentPercentage - amount);
            UpdateBarVisual();
        }
    }
    
    private void UpdateBarVisual()
    {
        if (_fillImage != null)
            _fillImage.fillAmount = _currentPercentage;
    }
    
    public float GetCurrentPercentage()
        => _currentPercentage;
}