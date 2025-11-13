using UnityEngine;

public class NeedsUIPanel : MonoBehaviour
{
    [SerializeField]
    private NeedsUIBar _needsUIBar;
    
    public NeedsUIBar GetNeedsUIBar()
        => _needsUIBar;
}