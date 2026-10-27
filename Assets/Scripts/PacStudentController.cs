using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    private Animator animator;
    private enum Direction {None, Up, Left, Down, Right}

    [SerializeField] private AudioSource movementAudio;
    [SerializeField] private AudioClip movingClip;
    [SerializeField] private AudioClip pelletClip;
    private bool[,] eatenCells;

    [SerializeField] private ParticleSystem bubbles;

    [SerializeField] private float tilesPerSecond = 5f;
    [SerializeField] private Vector2Int startCell = new Vector2Int(1, 1);

    [SerializeField] private ParticleSystem wallParticles;
    [SerializeField] private AudioSource wallAudio;
    [SerializeField] private AudioClip wallClip;
    private bool hasBumped;

    private Direction lastInput = Direction.None;
    private Direction currentInput = Direction.None;

    private Vector2Int currentCell;
    private Vector2Int targetCell;
    private Vector3 startPos;
    private Vector3 endPos;

    private bool isLerping;
    private float lerpTimer;
    private float carryOver;
    private float LerpDuration => 1f / tilesPerSecond;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eatenCells = new bool[LevelGenerator.FullMap.GetLength(0), LevelGenerator.FullMap.GetLength(1)];
        currentCell = startCell;
        transform.position = CellToWorld(currentCell);
        animator = GetComponent<Animator>();
        SetFacing(Direction.Right);
        animator.speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        GatherInput();
        if (isLerping)
        {
            UpdateLerp();
        }

        if(!isLerping)
        {
            NextMove();
        }

        if (isLerping)
        {
            animator.speed = 1f;
            if (!bubbles.isPlaying) bubbles.Play();
        }
        else
        {
            animator.speed = 0f;
            if (bubbles.isPlaying) bubbles.Stop();
            if (movementAudio.isPlaying) movementAudio.Stop();
        }
    }

    private void GatherInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) lastInput = Direction.Up;
        else if (Input.GetKeyDown(KeyCode.A)) lastInput = Direction.Left;
        else if (Input.GetKeyDown(KeyCode.S)) lastInput = Direction.Down;
        else if (Input.GetKeyDown(KeyCode.D)) lastInput = Direction.Right;
    }

    private void NextMove()
    {
        if(StartMove(lastInput))
        {
            currentInput = lastInput;
        }
        else if(!StartMove(currentInput))
        {
            HitWall();
        }
    }

    private void HitWall()
    {
        carryOver = 0f;
        if (hasBumped) return;
        Direction blocked;
        if(currentInput != Direction.None)
        {
            blocked = currentInput;
        } else
        {
            blocked = lastInput;
        }
        if (blocked == Direction.None) return;
        hasBumped = true;
        Vector2Int offset = DirectionToOffset(blocked);
        Vector3 mapDir = new Vector3(offset.y, -offset.x, 0f);
        Vector3 point = CellToWorld(currentCell) + mapDir * (LevelGenerator.TileSize * 0.5f);
        wallParticles.transform.position = point;
        wallParticles.Play();
        wallAudio.PlayOneShot(wallClip);
    }

    private bool StartMove(Direction direction)
    {
        if (direction == Direction.None) return false;
        Vector2Int target = currentCell + DirectionToOffset(direction);
        if (!IsWalkable(target)) return false;
        targetCell = target;
        startPos = CellToWorld(currentCell);
        endPos = CellToWorld(targetCell);
        lerpTimer = carryOver;
        carryOver = 0f;
        isLerping = true;
        hasBumped = false;
        SetFacing(direction);
        PlayMovementAudio(targetCell);
        return true;
    }

    private void UpdateLerp()
    {
        lerpTimer += Time.deltaTime;
        float t = Mathf.Clamp01(lerpTimer / LerpDuration);
        transform.position = Vector3.Lerp(startPos, endPos, t);

        if (lerpTimer >= LerpDuration)
        {
            carryOver = lerpTimer - LerpDuration;
            transform.position = endPos;
            eatenCells[targetCell.x, targetCell.y] = true;
            currentCell = targetCell;
            isLerping = false;
        }
    }
    private Vector2Int DirectionToOffset(Direction direction)
    {
        switch(direction)
        {
            case Direction.Up: return new Vector2Int(-1, 0);
            case Direction.Down: return new Vector2Int(1, 0);
            case Direction.Left: return new Vector2Int(0, -1);
            case Direction.Right: return new Vector2Int(0, 1);
            default: return Vector2Int.zero;
        }
    }
    private Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3(
            LevelGenerator.GridOrigin.x + cell.y * LevelGenerator.TileSize, LevelGenerator.GridOrigin.y - cell.x * LevelGenerator.TileSize, 0f);
    }

    private bool IsWalkable(Vector2Int cell)
    {
        int[,] map = LevelGenerator.FullMap;
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        if (cell.x < 0) return false;
        if (cell.y < 0) return false;
        if (cell.x >= rows) return false;
        if (cell.y >= cols) return false;

        switch (map[cell.x, cell.y])
        {
            case 0:
            case 5:
            case 6:
                return true;
            default:
                return false;
        }
    }

    private void SetFacing(Direction direction)
    {
        animator.SetInteger("Direction", (int)direction);
    }

    private void PlayMovementAudio(Vector2Int nextCell)
    {
        AudioClip selectedClip = movingClip;
        if (IsPellet(nextCell) && !eatenCells[nextCell.x, nextCell.y])
        {
            selectedClip = pelletClip;
        }

        if (movementAudio.clip == selectedClip && movementAudio.isPlaying) return;

        movementAudio.clip = selectedClip;
        movementAudio.Play();
    }

    private bool IsPellet(Vector2Int cell)
    {
        switch(LevelGenerator.FullMap[cell.x, cell.y])
        {
            case 5:
            case 6: 
                return true;
            default:
                return false;
        }
    }
}
