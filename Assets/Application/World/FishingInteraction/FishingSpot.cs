using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FishingSpot : MonoBehaviour, IHasInteractionTiles, IInteractable
{
    private List<Vector3> _interactionTiles;

    [SerializeField]
    private int _requiredLevelToInteract = 1;

    [SerializeField]
    private float _fishingCapacity = 40;

    [SerializeField]
    private List<Fish> _possibleFishesToCatch;

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

    public List<ContextMenuOption> GetContextMenuOptions(PlayerStateManager player)
    {
        var options = new List<ContextMenuOption>();

        var moveCommand =
            new MoveCommand(PlayerMovementService.Instance.GetNearestInteractionTile(GetInteractionTiles()));

        var fishingCommand =
            new FishingCommand(this);

        if (player.IsInIdleState() &&
            player.PlayerInventory.HasValidToolForSkill(SkillType.Fishing, player.PlayerSkills))
        {

            options.Add(
                new(
                    "Fish",
                    () => player.AddCommands(moveCommand, fishingCommand)
                )
            );
        }

        return options;
    }
}