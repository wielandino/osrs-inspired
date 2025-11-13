using System;

[Serializable]
public class Need
{
    public float MaxValue;
    public float CurrentValue;
    
    public Need(float maxValue)
    {
        MaxValue = maxValue;
        CurrentValue = maxValue;
    }
    
    public float GetPercentage() => MaxValue > 0 ? CurrentValue / MaxValue : 0f;
}