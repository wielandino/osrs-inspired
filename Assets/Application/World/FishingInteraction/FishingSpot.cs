using System.Collections.Generic;
using UnityEngine;

public class FishingSpot : MonoBehaviour, IHasInteractionTiles, ITooltipProvider
{
    private List<Vector3> _interactionTiles;

    [SerializeField]
    private int _requiredLevelToInteract = 1;

    [SerializeField]
    private float _fishingCapacity = 40;

    [SerializeField]
    private List<Fish> _possibleFishesToCatch;

    [Header("Needs Settings")]
    public float EnergyDrain;
    public float HungerDrain;

    private void Start()
    {
        if (_possibleFishesToCatch == null || _possibleFishesToCatch.Count == 0)
            Debug.LogError($"Fishing Spot \"{transform.name}\" contains no fishes to catch!");
        
        _interactionTiles = ObjectHelper.CollectInteractionTilesOfPosition(transform.position);
    }

    public List<Vector3> GetInteractionTiles()
        => _interactionTiles ?? null;

    public int GetRequiredLevelToInteract()
        => _requiredLevelToInteract;

    public float GetFishingCapacity()
        => _fishingCapacity;

    public List<Fish> GetListOfPossibleFishesToCatch()
        => _possibleFishesToCatch;

    public void RecalculateInteractionTiles()
    {
        _interactionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }

    public void ReduceFishingCapacity(float amount)
    {
        _fishingCapacity -= amount;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        var tileSize = GridManager.Instance.UnityGridSize;

        Gizmos.color = Color.red;
        Vector3 debugTileSize = new(tileSize, 0.1f, tileSize);

        foreach (var interactionTile in GetInteractionTiles())
        {
            Gizmos.DrawWireCube(interactionTile, debugTileSize);
        }
    }

    public string GetTooltipText()
    {
        string toolTipText = "<color=white>Examine</color> <color=yellow>fishing place</color>";

        var selectedItem = PlayerInventory.Instance.SelectedItem;

        if ((selectedItem != null && 
            ToolValidator.CanToolBeUsedForSkill(selectedItem, SkillType.Fishing)) ||
            PlayerInventory.Instance.HasValidToolForSkill(SkillType.Fishing, PlayerSkill.Instance))
        {
            if (GetFishingCapacity() > 0)
                toolTipText = "<color=white>Fish on</color> <color=yellow>fishing place</color>";
        }
        
        return toolTipText;
    }
}