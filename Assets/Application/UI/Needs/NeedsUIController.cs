using UnityEngine;

public class NeedsUIController : MonoBehaviour
{
    public static NeedsUIController Instance;
    
    [SerializeField]
    private NeedsUIPanel _energyPanel;
    [SerializeField]
    private NeedsUIPanel _hungerPanel;
    
    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }
    
    private void OnDisable()
    {
        if (Instance != null)
            Instance = null;
    }
    
    private void Start()
    {
        if (_energyPanel == null)
            Debug.LogError("No Energybar attached!");
        if (_hungerPanel == null)
            Debug.LogError("No Hungerbar attached!");
    }
    
    public void AddValueToHungerBar(float amount)
    {
        if (_hungerPanel != null)
            _hungerPanel.GetNeedsUIBar().AddValueToBar(amount);
    }
    
    public void DecreaseValueFromHungerBar(float amount)
    {
        if (_hungerPanel != null)
            _hungerPanel.GetNeedsUIBar().DecreaseValueFromBar(amount);
    }
    
    public void AddValueToEnergyBar(float amount)
    {
        if (_energyPanel != null)
            _energyPanel.GetNeedsUIBar().AddValueToBar(amount);
    }
    
    public void DecreaseValueFromEnergyBar(float amount)
    {
        if (_energyPanel != null)
            _energyPanel.GetNeedsUIBar().DecreaseValueFromBar(amount);
    }
}