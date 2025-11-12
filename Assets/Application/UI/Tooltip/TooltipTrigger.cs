using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour
{
    [Header("Tooltip Settings")]
    
    [SerializeField]
    private string _fallbackTooltipText = "<color=white>White</color> <color=yellow>Yellow</color>";
    public Color tooltipColor = Color.yellow;

    private void OnDisable()
    {
        if (TooltipController.Instance != null)
            TooltipController.Instance.HideTooltip();
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;


        string tooltipText = GetTooltipText();

        if (string.IsNullOrEmpty(tooltipText))
            tooltipText = _fallbackTooltipText;

        TooltipController.Instance.ShowTooltip(tooltipText, tooltipColor);
    }
    
    private string GetTooltipText()
    {
        if (TryGetComponent<ITooltipProvider>(out var tooltipProvider))
            return tooltipProvider.GetTooltipText();
        
        return _fallbackTooltipText;
    }

    private void OnMouseExit()
    {
        TooltipController.Instance.HideTooltip();
    }

    private void OnMouseOver()
    {
        TooltipController.Instance.UpdateTooltipPosition();
    }
}