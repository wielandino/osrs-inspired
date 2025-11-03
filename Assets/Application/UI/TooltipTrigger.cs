using UnityEngine;
using UnityEngine.UI;

// 1. TooltipTrigger - Komponente f�r Objekte, die Tooltips haben sollen
public class TooltipTrigger : MonoBehaviour
{
    [Header("Tooltip Settings")]
    public string tooltipText = "Beispiel Tooltip";
    public Color tooltipColor = Color.yellow;

    void OnDisable()
    {
        TooltipController.Instance.HideTooltip();
    }

    void OnMouseEnter()
    {
        TooltipController.Instance.ShowTooltip(tooltipText, tooltipColor);
    }

    void OnMouseExit()
    {
        TooltipController.Instance.HideTooltip();
    }

    void OnMouseOver()
    {
        // Tooltip Position kontinuierlich aktualisieren w�hrend Mouse Over
        TooltipController.Instance.UpdateTooltipPosition();
    }
}