using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static int[,] FullMap;
    public static Vector2 GridOrigin;
    public static float TileSize = 1f;

    [SerializeField] private Vector2 topLeftPos;
    [SerializeField] private float tileSize = 1f;

    int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    private int[,] fullMap;

    private void Awake()
    {
        BuildFullMap();
        FullMap = fullMap;
        TileSize = tileSize;
        GridOrigin = new Vector2(topLeftPos.x - tileSize, topLeftPos.y + tileSize);
    }

    private void BuildFullMap()
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);
        int fullRows = rows * 2 - 1;
        int fullCols = cols * 2;
        fullMap = new int[fullRows, fullCols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int v = levelMap[r, c];
                fullMap[r, c] = v;
                fullMap[r, fullCols - 1 - c] = v;
                fullMap[fullRows - 1 - r, c] = v;
                fullMap[fullRows - 1 - r, fullCols - 1 - c] = v;
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
