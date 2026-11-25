using System.Collections.Generic;
using UnityEngine;

public class InfiniteMapManager : MonoBehaviour
{
    [SerializeField] private Sprite grassSprite;
    [SerializeField] private Material grassMaterial;
    [SerializeField] private float chunkSize = 24f;

    [SerializeField] private GameObject[] propPrefabs;
    [SerializeField] private int minPropsPerChunk = 4;
    [SerializeField] private int maxPropsPerChunk = 7;

    [SerializeField] private bool disableOldGrid = true;

    private Transform player;
    private Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();
    private List<Chunk> chunkPool = new List<Chunk>();
    private Vector2Int currentPlayerChunkCoord;

    private class Chunk
    {
        public GameObject gameObject;
        public Vector2Int coord;
        public Transform propsContainer;
        public List<GameObject> activeProps = new List<GameObject>();
    }

    void Awake()
    {
        if (disableOldGrid)
        {
            GameObject oldGrid = GameObject.Find("Grid");
            if (oldGrid != null)
            {
                oldGrid.SetActive(false);
            }
        }

        if (propPrefabs == null || propPrefabs.Length == 0 || System.Array.TrueForAll(propPrefabs, p => p == null))
        {
            propPrefabs = Resources.LoadAll<GameObject>("Props");
        }
    }

    void Start()
    {
        Player playerObj = FindAnyObjectByType<Player>();
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (player == null)
        {
            GameObject pGo = GameObject.FindGameObjectWithTag("Player");
            if (pGo != null) player = pGo.transform;
        }

        Vector3 startPos = player != null ? player.position : Vector3.zero;
        currentPlayerChunkCoord = GetChunkCoord(startPos);

        InitializeChunks();
    }

    void Update()
    {
        if (player == null) return;

        Vector2Int playerCoord = GetChunkCoord(player.position);
        if (playerCoord != currentPlayerChunkCoord)
        {
            currentPlayerChunkCoord = playerCoord;
            UpdateChunks();
        }
    }

    private Vector2Int GetChunkCoord(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x + chunkSize * 0.5f) / chunkSize);
        int y = Mathf.FloorToInt((worldPos.y + chunkSize * 0.5f) / chunkSize);
        return new Vector2Int(x, y);
    }

    private void InitializeChunks()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int coord = new Vector2Int(currentPlayerChunkCoord.x + x, currentPlayerChunkCoord.y + y);
                Chunk chunk = CreateChunkObject(coord);
                activeChunks[coord] = chunk;
                chunkPool.Add(chunk);
                GeneratePropsForChunk(chunk);
            }
        }
    }

    private Chunk CreateChunkObject(Vector2Int coord)
    {
        GameObject chunkGo = new GameObject($"Chunk_{coord.x}_{coord.y}");
        chunkGo.transform.SetParent(transform);
        chunkGo.transform.position = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0f);

        GameObject groundGo = new GameObject("Ground");
        groundGo.transform.SetParent(chunkGo.transform, false);
        groundGo.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = groundGo.AddComponent<SpriteRenderer>();
        sr.sprite = grassSprite;
        if (grassMaterial != null)
        {
            sr.material = grassMaterial;
        }
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(chunkSize, chunkSize);
        sr.sortingLayerName = "Grass";
        sr.sortingOrder = 0;

        GameObject propsGo = new GameObject("Props");
        propsGo.transform.SetParent(chunkGo.transform, false);
        propsGo.transform.localPosition = Vector3.zero;

        Chunk chunk = new Chunk
        {
            gameObject = chunkGo,
            coord = coord,
            propsContainer = propsGo.transform
        };

        return chunk;
    }

    private void UpdateChunks()
    {
        HashSet<Vector2Int> requiredCoords = new HashSet<Vector2Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                requiredCoords.Add(new Vector2Int(currentPlayerChunkCoord.x + x, currentPlayerChunkCoord.y + y));
            }
        }

        List<Chunk> reusableChunks = new List<Chunk>();
        List<Vector2Int> coordsToRemove = new List<Vector2Int>();

        foreach (var kvp in activeChunks)
        {
            if (!requiredCoords.Contains(kvp.Key))
            {
                reusableChunks.Add(kvp.Value);
                coordsToRemove.Add(kvp.Key);
            }
        }

        foreach (var coord in coordsToRemove)
        {
            activeChunks.Remove(coord);
        }

        int reuseIdx = 0;
        foreach (var targetCoord in requiredCoords)
        {
            if (!activeChunks.ContainsKey(targetCoord))
            {
                if (reuseIdx < reusableChunks.Count)
                {
                    Chunk chunk = reusableChunks[reuseIdx++];
                    chunk.coord = targetCoord;
                    chunk.gameObject.name = $"Chunk_{targetCoord.x}_{targetCoord.y}";
                    chunk.gameObject.transform.position = new Vector3(targetCoord.x * chunkSize, targetCoord.y * chunkSize, 0f);
                    activeChunks[targetCoord] = chunk;
                    GeneratePropsForChunk(chunk);
                }
            }
        }
    }

    private void GeneratePropsForChunk(Chunk chunk)
    {
        foreach (var p in chunk.activeProps)
        {
            if (p != null) Destroy(p);
        }
        chunk.activeProps.Clear();

        if (propPrefabs == null || propPrefabs.Length == 0 || System.Array.TrueForAll(propPrefabs, p => p == null))
        {
            propPrefabs = Resources.LoadAll<GameObject>("Props");
        }
        if (propPrefabs == null || propPrefabs.Length == 0) return;

        List<GameObject> validPrefabs = new List<GameObject>();
        for (int i = 0; i < propPrefabs.Length; i++)
        {
            if (propPrefabs[i] != null) validPrefabs.Add(propPrefabs[i]);
        }
        if (validPrefabs.Count == 0) return;

        int seed = (chunk.coord.x * 73856093) ^ (chunk.coord.y * 19349663);
        System.Random rnd = new System.Random(seed);

        int count = rnd.Next(minPropsPerChunk, maxPropsPerChunk + 1);
        float half = chunkSize * 0.5f - 1.5f;

        for (int i = 0; i < count; i++)
        {
            float localX = (float)(rnd.NextDouble() * 2.0 - 1.0) * half;
            float localY = (float)(rnd.NextDouble() * 2.0 - 1.0) * half;
            Vector3 worldPos = chunk.gameObject.transform.position + new Vector3(localX, localY, 0f);

            if (worldPos.magnitude < 3.5f) continue;

            GameObject prefab = validPrefabs[rnd.Next(validPrefabs.Count)];
            GameObject propInstance = Instantiate(prefab, worldPos, Quaternion.identity, chunk.propsContainer);
            chunk.activeProps.Add(propInstance);

            YSort ySort = propInstance.GetComponent<YSort>();
            if (ySort != null)
            {
                ySort.UpdateSorting();
            }
            else
            {
                SpriteRenderer propSr = propInstance.GetComponent<SpriteRenderer>();
                if (propSr != null)
                {
                    propSr.sortingOrder = -(int)(worldPos.y * 100);
                }
            }
        }
    }
}
