using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType
    {
        WalkableTile,
        WaterTile
    }

    [SerializeField]
    private TileType _tileType;

    public TileType GetTileType()
        => _tileType;
}