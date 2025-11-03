using TMPro;
using UnityEngine;

[ExecuteAlways]
public class TileLabler : MonoBehaviour
{
    private TextMeshPro _label;
    private Vector2Int _cords;
    private GridManager _gridManager;

    private void Awake()
    {
        _label = GetComponentInChildren<TextMeshPro>();
        _gridManager = FindAnyObjectByType<GridManager>();

        DisplayCords();
    }

    private void Update()
    {    
        DisplayCords();
        transform.name = _cords.ToString();
    }

    private void DisplayCords()
    {
        if (_label == null || _gridManager == null) return;
        
        _cords.x = Mathf.RoundToInt(transform.position.x / _gridManager.UnityGridSize);
        _cords.y = Mathf.RoundToInt(transform.position.z / _gridManager.UnityGridSize);
        _label.text = $"{_cords.x}, {_cords.y}";

    }
}