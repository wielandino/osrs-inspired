using UnityEngine;

public class TooltipTrigger : MonoBehaviour
{
    [Header("Tooltip Settings")]
    public string tooltipText = "Beispiel Tooltip";
    public Color tooltipColor = Color.yellow;

    private void OnDisable()
    {
        if (TooltipController.Instance != null)
            TooltipController.Instance.HideTooltip();
    }

    private void OnMouseEnter()
    {
        TooltipController.Instance.ShowTooltip(tooltipText, tooltipColor);
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