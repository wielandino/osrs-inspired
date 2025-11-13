using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNeeds : MonoBehaviour
{
    [SerializeField] 
    private List<NeedConfig> _needConfigs = new();
    
    private readonly Dictionary<NeedType, Need> _needs = new();
    
    public event Action<NeedType, float, float> OnNeedChanged; // type, current, max
    
    private void Awake()
    {
        foreach (var config in _needConfigs)
            _needs[config.Type] = new Need(config.MaxValue);
        
    }
    
    private void Start()
    {
        foreach (var kvp in _needs)
            OnNeedChanged?.Invoke(kvp.Key, kvp.Value.CurrentValue, kvp.Value.MaxValue);
        
    }
    
    public void ModifyNeed(NeedType type, float amount)
    {
        if (!_needs.ContainsKey(type))
        {
            Debug.LogError($"Need type {type} not found!");
            return;
        }
        
        Need need = _needs[type];
        need.CurrentValue = Mathf.Clamp(need.CurrentValue + amount, 0, need.MaxValue);
        
        OnNeedChanged?.Invoke(type, need.CurrentValue, need.MaxValue);
    }
    
    public bool IsNeedDepleted(NeedType type)
        => _needs.ContainsKey(type) && _needs[type].CurrentValue <= 0;
    
    
    public float GetNeedValue(NeedType type)
        => _needs.ContainsKey(type) ? _needs[type].CurrentValue : 0f;
    
    
    public float GetNeedPercentage(NeedType type)
        => _needs.ContainsKey(type) ? _needs[type].GetPercentage() : 0f;
    
}