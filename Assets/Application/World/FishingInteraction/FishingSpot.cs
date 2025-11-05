using System.Collections.Generic;
using UnityEngine;

public class FishingSpot : MonoBehaviour, IHasInteractionTiles, IInteractable
{
    private List<Vector3> _interactionTiles;

    [SerializeField]
    private int _requiredLevelToInteract = 1;

    [SerializeField]
    private float _xpPerFishing = 5;

    private void Start()
    {
        _interactionTiles = ObjectHelper.CollectInteractionTilesOfPosition(transform.position);
    }

    public List<Vector3> GetInteractionTiles()
        => _interactionTiles;

    public int GetRequiredLevelToInteract()
        => _requiredLevelToInteract;

    public float GetXPPerFishing()
        => _xpPerFishing;

    public void RecalculateInteractionTiles()
    {
        _interactionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
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

    public List<ContextMenuOption> GetContextMenuOptions(PlayerStateManager player)
    {
        var options = new List<ContextMenuOption>();

        var moveCommand =
            new MoveCommand(PlayerMovementService.Instance.GetNearestInteractionTile(GetInteractionTiles()));

        if (player.IsInIdleState() &&
            player.PlayerInventory.HasValidToolForSkill(SkillType.Fishing, player.PlayerSkills))
        {

            
        }

        return options;
    }
}