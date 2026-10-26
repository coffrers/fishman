using System.Collections;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    [SerializeField] private GameObject cherryPrefab;
    [SerializeField] private float cherrySpeed = 4f;
    [SerializeField] private float spawnDelay = 5f;
    [SerializeField] private float edgeMargin = 1.5f;
    [SerializeField] private int cherrySortingOrder = 100;

    private GameObject currentCherry;

    private int Rows
    {
        get { return LevelGenerator.FullMap.GetLength(0); }
    }
    private int Cols
    {
        get { return LevelGenerator.FullMap.GetLength(1); }
    }

    private Vector2 LevelSize
    {
        get { return new Vector2(Cols * LevelGenerator.TileSize, Rows * LevelGenerator.TileSize); }
    }

    private Vector2 LevelCenter => new Vector2(LevelGenerator.GridOrigin.x + (Cols - 1) * 0.5f * LevelGenerator.TileSize, LevelGenerator.GridOrigin.y - (Rows - 1) * 0.5f * LevelGenerator.TileSize);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(spawnDelay);

        while(true)
        {
            SpawnCherry();
            while (currentCherry != null)
            {
                yield return null;
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnCherry()
    {
        Vector2 halfSize = LevelSize * 0.5f;
        Vector2 offset;

        switch (Random.Range(0, 4))
        {
            case 0:
                offset = new Vector2(-(halfSize.x + edgeMargin), Random.Range(-halfSize.y, halfSize.y));
                break;
            case 1:
                offset = new Vector2(halfSize.x + edgeMargin, Random.Range(-halfSize.y, halfSize.y));
                break;
            case 2:
                offset = new Vector2(Random.Range(-halfSize.x, halfSize.x), halfSize.y + edgeMargin);
                break;
            default:
                offset = new Vector2(Random.Range(-halfSize.x, halfSize.x), - (halfSize.y + edgeMargin));
                break;
        }

        Vector3 startPos = LevelCenter + offset;
        Vector3 endPos = LevelCenter - offset;
        currentCherry = Instantiate(cherryPrefab, startPos, Quaternion.identity);
        SpriteRenderer sprite = currentCherry.GetComponent<SpriteRenderer>();
        sprite.sortingOrder = cherrySortingOrder;
        sprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        StartCoroutine(MoveCherry(currentCherry, startPos, endPos));
    }

    private IEnumerator MoveCherry(GameObject cherry, Vector3 startPos, Vector3 endPos)
    {
        float duration = Vector3.Distance(startPos, endPos) / (cherrySpeed * LevelGenerator.TileSize);
        float timer = 0f;
        while(timer < duration)
        {
            if (cherry == null) yield break;
            timer += Time.deltaTime;
            cherry.transform.position = Vector3.Lerp(startPos, endPos, timer / duration);
            yield return null;
        }
        if(cherry != null)
        {
            Destroy(cherry);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
