using UnityEngine;
using UnityEditor;

public class TerrainMapEditor : EditorWindow
{
    [MenuItem("Tools/Terrain Map Editor")]
    public static void ShowWindow()
    {
        TerrainMapEditor window = GetWindow<TerrainMapEditor>("Terrain Editor");
        window.minSize = new Vector2(300, 400);
    }
    
    // Editor Settings
    private enum EditMode
    {
        None,
        RaiseHeight,
        LowerHeight,
        FlattenHeight
    }
    
    private EditMode currentMode = EditMode.None;
    private float brushSize = 2f;
    private float brushStrength = 0.5f;
    private bool isPainting = false;
    
    // References
    private TerrainGridManager terrainManager;
    
    private void OnEnable()
    {
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
        
        // Terrain Manager Reference
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
        
        // Edit Mode Selection
        EditorGUILayout.LabelField("Edit Mode", EditorStyles.boldLabel);
        currentMode = (EditMode)EditorGUILayout.EnumPopup("Mode", currentMode);
        
        EditorGUILayout.Space();
        
        // Brush Settings
        EditorGUILayout.LabelField("Brush Settings", EditorStyles.boldLabel);
        brushSize = EditorGUILayout.Slider("Brush Size", brushSize, 0.5f, 10f);
        brushStrength = EditorGUILayout.Slider("Brush Strength", brushStrength, 0.1f, 2f);
        
        EditorGUILayout.Space();
        
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
        
        // Info
        EditorGUILayout.HelpBox(
            "Halte SHIFT und klicke in die Scene View, um Höhen zu malen.\n" +
            "Mode: " + currentMode.ToString(), 
            MessageType.Info
        );
    }
    
    private void OnSceneGUI(SceneView sceneView)
    {
        if (terrainManager == null || currentMode == EditMode.None)
            return;
        
        Event e = Event.current;
        
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            Handles.color = new Color(0, 1, 0, 0.3f);
            Handles.DrawSolidDisc(hit.point, Vector3.up, brushSize);
            Handles.color = Color.green;
            Handles.DrawWireDisc(hit.point, Vector3.up, brushSize);

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
            sceneView.Repaint();
        
    }
    
    private void PaintTerrain(Vector3 worldPosition)
    {
        if (terrainManager == null) return;
        
        HeightGrid heightGrid = terrainManager.GetHeightGrid();

        if (heightGrid == null) return;
        
        Vector2Int gridPos = terrainManager.WorldToGrid(worldPosition);
        
        float amount = brushStrength * 0.1f;
        
        switch (currentMode)
        {
            case EditMode.RaiseHeight:
                heightGrid.RaiseArea(gridPos, brushSize, amount);
                break;
                
            case EditMode.LowerHeight:
                heightGrid.RaiseArea(gridPos, brushSize, -amount);
                break;
                
            case EditMode.FlattenHeight:
                // TODO: Flatten
                break;
        }
        
        RegenerateAffectedTiles(gridPos, brushSize);
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
}