using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public static GridManager Instance;

    [SerializeField] private Vector2Int _gridSize;
    [SerializeField] private int _unityGridSize;

    public int UnityGridSize { get { return _unityGridSize; } }

    private readonly Dictionary<Vector2Int, GridNode> _grid = new();

    private void Awake()
    {
        Instance = this;

        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                Vector2Int cords = new(x, y);
                _grid.Add(cords, new GridNode(cords));
            }
        }
    }

    public void UpdateGraphOfObject(Collider collider)
    {
        var bounds = collider.bounds;
        var guo = new GraphUpdateObject(bounds);
        AstarPath.active.UpdateGraphs(guo, 0.1f);
    }

    public void UpdateGraph()
    {
        var graphToScan = AstarPath.active.data.gridGraph;
        AstarPath.active.Scan(graphToScan);
    }
}