using UnityEngine;
using System.Collections.Generic;

public class NeedsUIController : MonoBehaviour
{
    [SerializeField] 
    private PlayerNeeds _playerNeeds;

    [SerializeField] 
    private List<NeedsUIBar> _needBars = new();
    
    private readonly Dictionary<NeedType, NeedsUIBar> _barLookup = new();
    
    private void Start()
    {
        if (_playerNeeds == null)
        {
            Debug.LogError("PlayerNeeds not assigned!");
            return;
        }
        
        foreach (var bar in _needBars)
            if (bar != null)
                _barLookup[bar.NeedType] = bar;
        
        _playerNeeds.OnNeedChanged += OnNeedValueChanged;
    }
    
    private void OnDestroy()
    {
        if (_playerNeeds != null)
            _playerNeeds.OnNeedChanged -= OnNeedValueChanged;
    }
    
    private void OnNeedValueChanged(NeedType type, float current, float max)
    {
        if (_barLookup.TryGetValue(type, out NeedsUIBar bar))
            bar.UpdateBar(current, max);
        
    }
}