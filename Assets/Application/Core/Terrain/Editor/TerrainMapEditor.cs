using UnityEngine;
using UnityEditor;

public class TerrainMapEditor : EditorWindow
{
    [MenuItem("Tools/Terrain Map Editor")]
    public static void ShowWindow()
    {
        TerrainMapEditor window = GetWindow<TerrainMapEditor>("Terrain Editor");
        window.minSize = new Vector2(300, 500);
    }
    
    // Editor Settings
    private enum EditMode
    {
        None,
        RaiseHeight,
        LowerHeight,
        FlattenHeight,
        PaintTileType
    }
    
    private EditMode currentMode = EditMode.None;
    
    private float heightBrushSize = 2f;
    private float heightBrushStrength = 0.5f;
    
    private float tileBrushSize = 0f;
    
    private bool isPainting = false;
    
    private TileType selectedTileType = TileType.Grass;
    
    private TerrainGridManager terrainManager;
    
    private void OnEnable()
    {
        terrainManager = FindFirstObjectByType<TerrainGridManager>();
        SceneView.duringSceneGui += OnSceneGUI;
    }
    
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Terrain Map Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Setup", EditorStyles.boldLabel);
        terrainManager = (TerrainGridManager)EditorGUILayout.ObjectField(
            "Terrain Manager", 
            terrainManager, 
            typeof(TerrainGridManager), 
            true
        );
        
        if (terrainManager == null)
        {
            EditorGUILayout.HelpBox("Bitte TerrainGridManager zuweisen!", MessageType.Warning);
            return;
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Edit Mode", EditorStyles.boldLabel);
        currentMode = (EditMode)EditorGUILayout.EnumPopup("Mode", currentMode);
        
        EditorGUILayout.Space();
        
        if (currentMode == EditMode.PaintTileType)
        {
            // TILE PAINTING SETTINGS
            EditorGUILayout.LabelField("Tile Type Settings", EditorStyles.boldLabel);
            selectedTileType = (TileType)EditorGUILayout.EnumPopup("Tile Type", selectedTileType);
            
            // Visual Feedback
            Color tileColor = GetTileTypeColor(selectedTileType);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preview Color:");
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(50, 20), tileColor);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Tile Brush Settings", EditorStyles.boldLabel);
            tileBrushSize = EditorGUILayout.Slider("Brush Size", tileBrushSize, 0f, 10f);
            
            EditorGUILayout.HelpBox(
                "Brush Size bestimmt den Radius in Tiles.\n" +
                "0 = Einzelnes Tile\n" +
                "1 = 3x3 Tiles\n" +
                "2 = 5x5 Tiles", 
                MessageType.Info
            );
        }
        else if (currentMode != EditMode.None)
        {
            // HEIGHT BRUSH SETTINGS
            EditorGUILayout.LabelField("Height Brush Settings", EditorStyles.boldLabel);
            heightBrushSize = EditorGUILayout.Slider("Brush Size", heightBrushSize, 0.5f, 10f);
            heightBrushStrength = EditorGUILayout.Slider("Brush Strength", heightBrushStrength, 0.1f, 2f);
        }
        
        EditorGUILayout.Space();
        
        // Quick Tile Type Buttons
        if (currentMode == EditMode.PaintTileType)
        {
            EditorGUILayout.LabelField("Quick Select", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Grass"))
                selectedTileType = TileType.Grass;
            if (GUILayout.Button("Stone"))
                selectedTileType = TileType.Stone;
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
               
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        
        // Quick Actions
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Generate All Tiles"))
        {
            terrainManager.GenerateAllTiles();
        }
        
        if (GUILayout.Button("Clear All Tiles"))
        {
            terrainManager.ClearAllTiles();
        }
        
        if (GUILayout.Button("Regenerate Terrain"))
        {
            RegenerateTerrain();
        }
        
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Save/Load", EditorStyles.boldLabel);

        if (GUILayout.Button("💾 Save Terrain Data", GUILayout.Height(30)))
        {
            terrainManager.SaveTerrainData();
            EditorUtility.DisplayDialog("Saved", "Terrain data saved successfully!", "OK");
        }

        if (GUILayout.Button("📂 Load Terrain Data"))
        {
            terrainManager.LoadTerrainData();
        }
        
        // Info
        string infoText = "Halte SHIFT und klicke in die Scene View.\n";
        if (currentMode == EditMode.PaintTileType)
        {
            infoText += $"Painting: {selectedTileType}\n";
            infoText += "Brush-Form: Quadrat";
        }
        else
        {
            infoText += $"Mode: {currentMode}\n";
            infoText += "Brush-Form: Kreis";
        }
        
        EditorGUILayout.HelpBox(infoText, MessageType.Info);
    }
    
    private void OnSceneGUI(SceneView sceneView)
    {
        if (terrainManager == null || currentMode == EditMode.None)
            return;
        
        Event e = Event.current;
        
        // Zeichne Brush Preview
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            Color brushColor = currentMode == EditMode.PaintTileType 
                ? GetTileTypeColor(selectedTileType) 
                : Color.green;
            
            if (currentMode == EditMode.PaintTileType)
            {
                DrawSquareBrush(hit.point, tileBrushSize, brushColor);
            }
            else
            {
                Handles.color = new Color(brushColor.r, brushColor.g, brushColor.b, 0.3f);
                Handles.DrawSolidDisc(hit.point, Vector3.up, heightBrushSize);
                Handles.color = brushColor;
                Handles.DrawWireDisc(hit.point, Vector3.up, heightBrushSize);
            }
            
            if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
            {
                isPainting = true;
                e.Use();
            }
            
            if (e.type == EventType.MouseUp && e.button == 0)
            {
                isPainting = false;
            }
            
            if (e.type == EventType.MouseDrag && isPainting && e.shift)
            {
                PaintTerrain(hit.point);
                e.Use();
            }
            
            if (isPainting && e.type == EventType.MouseDown && e.shift)
            {
                PaintTerrain(hit.point);
                e.Use();
            }
        }
        
        if (currentMode != EditMode.None)
        {
            sceneView.Repaint();
        }
    }

    private void DrawSquareBrush(Vector3 center, float size, Color color)
    {
        Vector2Int gridPos = terrainManager.WorldToGrid(center);
        Vector3 worldCenter = terrainManager.GridToWorld(gridPos, center.y);
        
        float tileSize = 2f;
        
        if (size < 0.1f)
        {
            Vector3[] cellVerts = new Vector3[4];
            cellVerts[0] = worldCenter + new Vector3(-1, 0, -1);
            cellVerts[1] = worldCenter + new Vector3(1, 0, -1);
            cellVerts[2] = worldCenter + new Vector3(1, 0, 1);
            cellVerts[3] = worldCenter + new Vector3(-1, 0, 1);
            
            Handles.DrawSolidRectangleWithOutline(cellVerts, new Color(color.r, color.g, color.b, 0.4f), color);
            return;
        }
        
        float totalSize = (size * 2 + 1) * tileSize;
        Vector3 halfExtents = new(totalSize / 2f, 0.1f, totalSize / 2f);
        
        Handles.color = new Color(color.r, color.g, color.b, 0.3f);
        Vector3[] verts = new Vector3[4];
        verts[0] = worldCenter + new Vector3(-halfExtents.x, 0, -halfExtents.z);
        verts[1] = worldCenter + new Vector3(halfExtents.x, 0, -halfExtents.z);
        verts[2] = worldCenter + new Vector3(halfExtents.x, 0, halfExtents.z);
        verts[3] = worldCenter + new Vector3(-halfExtents.x, 0, halfExtents.z);
        Handles.DrawSolidRectangleWithOutline(verts, new Color(color.r, color.g, color.b, 0.3f), color);
        
        Handles.color = new Color(color.r, color.g, color.b, 0.5f);
        int gridRadius = Mathf.CeilToInt(size);
        
        for (int x = -gridRadius; x <= gridRadius; x++)
        {
            for (int z = -gridRadius; z <= gridRadius; z++)
            {
                Vector2Int cellPos = gridPos + new Vector2Int(x, z);
                Vector3 cellWorldPos = terrainManager.GridToWorld(cellPos, center.y);
                
                // Zeichne Tile-Umriss
                Vector3[] cellVerts = new Vector3[5];
                cellVerts[0] = cellWorldPos + new Vector3(-1, 0, -1);
                cellVerts[1] = cellWorldPos + new Vector3(1, 0, -1);
                cellVerts[2] = cellWorldPos + new Vector3(1, 0, 1);
                cellVerts[3] = cellWorldPos + new Vector3(-1, 0, 1);
                cellVerts[4] = cellVerts[0];
                
                Handles.DrawPolyLine(cellVerts);
            }
        }
    }
    
    private void PaintTerrain(Vector3 worldPosition)
    {
        if (terrainManager == null) return;
        
        Vector2Int gridPos = terrainManager.WorldToGrid(worldPosition);
        
        if (currentMode == EditMode.PaintTileType)
        {
            PaintTileTypeInArea(gridPos, tileBrushSize);
        }
        else
        {
            HeightGrid heightGrid = terrainManager.GetHeightGrid();
            if (heightGrid == null) return;
            
            float amount = heightBrushStrength * 0.1f;
            
            switch (currentMode)
            {
                case EditMode.RaiseHeight:
                    heightGrid.RaiseArea(gridPos, heightBrushSize, amount);  // Verwende heightBrushSize!
                    break;
                    
                case EditMode.LowerHeight:
                    heightGrid.RaiseArea(gridPos, heightBrushSize, -amount);
                    break;
                    
                case EditMode.FlattenHeight:
                    // TODO: Flatten implementieren
                    break;
            }
            
            RegenerateAffectedTiles(gridPos, heightBrushSize);
        }
    }
    
    private void PaintTileTypeInArea(Vector2Int center, float radius)
    {
        if (radius < 0.1f)
        {
            TerrainCell cell = terrainManager.GetCell(center);
            if (cell != null)
            {
                cell.tileType = selectedTileType;
                terrainManager.PlaceTile(center);
            }
            return;
        }
        
        int radiusInt = Mathf.CeilToInt(radius);
        
        for (int x = -radiusInt; x <= radiusInt; x++)
        {
            for (int z = -radiusInt; z <= radiusInt; z++)
            {
                Vector2Int gridPos = center + new Vector2Int(x, z);
                
                TerrainCell cell = terrainManager.GetCell(gridPos);
                if (cell != null)
                {
                    cell.tileType = selectedTileType;
                    
                    terrainManager.PlaceTile(gridPos);
                }
            }
        }
    }
    
    private void RegenerateAffectedTiles(Vector2Int center, float radius)
    {
        int radiusInt = Mathf.CeilToInt(radius) + 1;
        HeightGrid heightGrid = terrainManager.GetHeightGrid();
        
        for (int x = -radiusInt; x <= radiusInt; x++)
        {
            for (int z = -radiusInt; z <= radiusInt; z++)
            {
                Vector2Int gridPos = center + new Vector2Int(x, z);
                TerrainCell cell = terrainManager.GetCell(gridPos);
                
                if (cell != null)
                {
                    heightGrid.UpdateCellHeights(cell);
                    
                    terrainManager.PlaceTile(gridPos);
                }
            }
        }
    }
    
    private void RegenerateTerrain()
    {
        terrainManager.ClearAllTiles();
        
        HeightGrid heightGrid = terrainManager.GetHeightGrid();
        
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                Vector2Int gridPos = new(x, z);
                TerrainCell cell = terrainManager.GetCell(gridPos);
                if (cell != null)
                {
                    heightGrid.UpdateCellHeights(cell);
                }
            }
        }
        
        terrainManager.GenerateAllTiles();
    }
    
    private Color GetTileTypeColor(TileType type)
    {
        return type switch
        {
            TileType.Grass => new Color(0.2f, 0.8f, 0.2f),
            TileType.Stone => new Color(0.5f, 0.5f, 0.5f),
            _ => Color.white,
        };
    }
}