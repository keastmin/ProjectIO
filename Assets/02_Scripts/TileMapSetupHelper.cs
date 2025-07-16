using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TileMapSetupHelper : MonoBehaviour
{
    [Header("자동 설정")]
    [SerializeField] bool autoCreateTilePrefabs = true;
    [SerializeField] bool autoSetupTileManager = true;
    
    [Header("타일 설정")]
    [SerializeField] Material tileMaterial;
    [SerializeField] Vector3 tileScale = Vector3.one;
    
    [ContextMenu("타일맵 설정 자동화")]
    public void SetupTileMap()
    {
        #if UNITY_EDITOR
        if (autoCreateTilePrefabs)
        {
            CreateBasicTilePrefabs();
        }
        
        if (autoSetupTileManager)
        {
            SetupTileManagerComponent();
        }
        
        Debug.Log("타일맵 설정이 완료되었습니다!");
        #endif
    }
    
    #if UNITY_EDITOR
    void CreateBasicTilePrefabs()
    {
        // Resources 폴더 확인/생성
        string resourcesPath = "Assets/07_Resources";
        string tilesPath = resourcesPath + "/Tiles";
        
        if (!AssetDatabase.IsValidFolder(resourcesPath))
            AssetDatabase.CreateFolder("Assets", "07_Resources");
            
        if (!AssetDatabase.IsValidFolder(tilesPath))
            AssetDatabase.CreateFolder(resourcesPath, "Tiles");

        // 기본 타일 프리팹들 생성
        CreateTilePrefab("GrassTile", Color.green, tilesPath);
        CreateTilePrefab("StoneTile", Color.gray, tilesPath);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    void CreateTilePrefab(string name, Color color, string path)
    {
        // 이미 존재하는지 확인
        string prefabPath = $"{path}/{name}.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            return;
            
        // 기본 큐브 생성
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.name = name;
        tile.transform.localScale = tileScale;
        
        // Tile 컴포넌트 추가
        tile.AddComponent<Tile>();
        
        // 머터리얼 설정
        Renderer renderer = tile.GetComponent<Renderer>();
        if (tileMaterial != null)
        {
            renderer.material = tileMaterial;
        }
        renderer.material.color = color;
        
        // 콜라이더 설정 (클릭 감지용)
        var collider = tile.GetComponent<Collider>();
        collider.isTrigger = false;
        
        // 프리팹으로 저장
        PrefabUtility.SaveAsPrefabAsset(tile, prefabPath);
        DestroyImmediate(tile);
        
        Debug.Log($"{name} 프리팹이 생성되었습니다: {prefabPath}");
    }
    
    void SetupTileManagerComponent()
    {
        // 현재 오브젝트에 TileManager가 없다면 추가
        TileManager tileManager = GetComponent<TileManager>();
        if (tileManager == null)
        {
            tileManager = gameObject.AddComponent<TileManager>();
        }
        
        // TileContainer 자식 오브젝트 찾기 또는 생성
        TileContainer tileContainer = GetComponentInChildren<TileContainer>();
        if (tileContainer == null)
        {
            GameObject containerObj = new GameObject("TileContainer");
            containerObj.transform.SetParent(transform);
            tileContainer = containerObj.AddComponent<TileContainer>();
        }
        
        // 생성된 타일 프리팹들을 TileContainer에 할당
        AssignTilePrefabsToContainer(tileContainer);
        
        Debug.Log("TileManager 설정이 완료되었습니다!");
    }
    
    void AssignTilePrefabsToContainer(TileContainer container)
    {
        // Resources/Tiles 폴더에서 타일 프리팹들 로드
        GameObject[] tilePrefabs = Resources.LoadAll<GameObject>("Tiles");
        
        if (tilePrefabs.Length > 0)
        {
            // SerializedObject를 사용하여 private 필드에 접근
            SerializedObject serializedContainer = new SerializedObject(container);
            SerializedProperty tilePrefabsProperty = serializedContainer.FindProperty("tilePrefabs");
            
            tilePrefabsProperty.arraySize = tilePrefabs.Length;
            for (int i = 0; i < tilePrefabs.Length; i++)
            {
                tilePrefabsProperty.GetArrayElementAtIndex(i).objectReferenceValue = tilePrefabs[i];
            }
            
            serializedContainer.ApplyModifiedProperties();
            Debug.Log($"{tilePrefabs.Length}개의 타일 프리팹이 할당되었습니다.");
        }
    }
    #endif
}
