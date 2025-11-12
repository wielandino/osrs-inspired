using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IHasInteractionTiles, ITooltipProvider
{
    [Header("Tree Stats")]
    public float MaxHealth;

    public float CurrentHealth;

    [Header("Leveling")]
    [SerializeField]
    private int _requiredLevelToCut = 1;

    public float XPDropPerCut = 5f;

    [Header("Other")]
    public float RespawnTime = 15f;

    [Header("Tree Models")]
    [SerializeField]
    private GameObject _defaultTreeModel;

    [SerializeField]
    private GameObject _treeTrunkModel;

    [SerializeField]
    private GameObject _treeLogPrefab;

    public bool IsDestroyed;

    private int _tileSize;

    public LayerMask ObstacleLayerMask;

    public Vector3 TreePosition { get; private set; }

    public List<Vector3> InteractionTiles = new();

    private TreeStateManager _treeStateManager;

    private void Start()
    {
        _treeStateManager = gameObject.GetComponent<TreeStateManager>();
        ObstacleLayerMask = LayerMask.GetMask("Obstacle");

        _tileSize = GridManager.Instance.UnityGridSize;
        TreePosition = transform.position;
        CurrentHealth = MaxHealth;

        InteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }


    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Vector3 tileSize = new(_tileSize, 1f, _tileSize);

        foreach(var interactionTile in InteractionTiles)
        {
            Gizmos.DrawWireCube(interactionTile, tileSize);
        }
    }

    public int GetRequiredLevelToCut()
        => _requiredLevelToCut;

    public GameObject TreeLogPrefab => _treeLogPrefab;

    public void SwitchToIdleModel()
    {
        if (_defaultTreeModel != null)
            _defaultTreeModel.SetActive(true);
        

        if (_treeTrunkModel != null)
            _treeTrunkModel.SetActive(false);
        
    }

    public void SwitchToDestroyedModel()
    {
        if (_defaultTreeModel != null)
            _defaultTreeModel.SetActive(false);
        

        if (_treeTrunkModel != null)
            _treeTrunkModel.SetActive(true);
    }

    public void RecalculateInteractionTiles()
    {
        InteractionTiles = ObjectHelper.CollectInteractionTilesOfPosition(gameObject.transform.position);
    }

    public TreeStateManager GetStateManager()
        => _treeStateManager;

    public string GetTooltipText()
    {
        string toolTipText = "<color=white>Examine</color> <color=yellow>Tree</color>";
        
        var selectedItem = PlayerInventory.Instance.SelectedItem;

        if ((selectedItem != null && ToolValidator.CanToolBeUsedForSkill(selectedItem, SkillType.Woodcutting)) ||
            PlayerInventory.Instance.HasValidToolForSkill(SkillType.Woodcutting, PlayerSkill.Instance))
                toolTipText = "<color=white>Chop down</color> <color=yellow>Tree</color>";
        

        return toolTipText;
    }
}