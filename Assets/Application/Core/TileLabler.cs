using TMPro;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TileLabler : MonoBehaviour
{
    private TextMeshPro _label;
    private Vector2Int _cords;
    private GridManager _gridManager;

    private void Awake()
    {
        _label = GetComponentInChildren<TextMeshPro>();  // ← Zuerst initialisieren!
        _gridManager = FindAnyObjectByType<GridManager>();

        DisplayCords();
    }

    private void Update()
    {
        //if (Application.isPlaying) return;  // ← Im Editor nicht updaten
        
        DisplayCords();
        transform.name = _cords.ToString();
    }

    private void DisplayCords()
    {
        if (_label == null || _gridManager == null) return;  // ← Null-Check!
        
        _cords.x = Mathf.RoundToInt(transform.position.x / _gridManager.UnityGridSize);
        _cords.y = Mathf.RoundToInt(transform.position.z / _gridManager.UnityGridSize);
        _label.text = $"{_cords.x}, {_cords.y}";

    }
}