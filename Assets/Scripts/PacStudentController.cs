using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    [SerializeField] private Tweener tweener;
    [SerializeField] private LevelGrid grid;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource moveAudio;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip pelletWalkClip;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private Vector2Int startCell = new Vector2Int(1, 1);
    [SerializeField] private float tilesPerSecond = 5f;

    private static readonly Vector2Int Up = new Vector2Int(0, -1);
    private static readonly Vector2Int Down = new Vector2Int(0, 1);
    private static readonly Vector2Int Left = new Vector2Int(-1, 0);
    private static readonly Vector2Int Right = new Vector2Int(1, 0);

    private Vector2Int gridPos;
    private Vector2Int lastInput = Vector2Int.zero;
    private Vector2Int currentInput = Vector2Int.zero;
    private Vector2Int facing;
    private bool isMoving;
    private float moveEndTime;

    void Start()
    {
        gridPos = startCell;
        Vector3 pos = grid.CellToWorld(startCell);
        pos.z = transform.position.z;
        transform.position = pos;

        facing = Right;
        animator.Play(StateFor(Right), 0, 0f);
        animator.speed = 0f;
    }

    void Update()
    {
        ReadInput();

        if (tweener.TweenExists(transform)) return;

        if (TryMove(lastInput))
        {
            currentInput = lastInput;
            return;
        }

        if (TryMove(currentInput)) return;

        StopMoving();
    }

    private void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) lastInput = Up;
        else if (Input.GetKeyDown(KeyCode.A)) lastInput = Left;
        else if (Input.GetKeyDown(KeyCode.S)) lastInput = Down;
        else if (Input.GetKeyDown(KeyCode.D)) lastInput = Right;
    }

    private bool TryMove(Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return false;

        Vector2Int target = gridPos + dir;
        if (!grid.IsWalkable(target)) return false;

        float stepDuration = 1f / tilesPerSecond;
        float startTime = isMoving ? moveEndTime : Time.time;

        Vector3 from = transform.position;
        Vector3 to = grid.CellToWorld(target);
        to.z = from.z;

        tweener.AddTween(transform, from, to, stepDuration, startTime);
        moveEndTime = startTime + stepDuration;
        isMoving = true;
        gridPos = target;

        Face(dir);
        PlayMoveAudio(target);
if (dust != null && !dust.isPlaying)
{
    dust.Play();
}
        return true;
    }

    private void StopMoving()
    {
        isMoving = false;
        animator.speed = 0f;
        moveAudio.Stop();
        dust.Stop();
    }

    private void Face(Vector2Int dir)
    {
        animator.speed = 1f;
        if (dir == facing) return;
        facing = dir;
        animator.Play(StateFor(dir), 0, 0f);
    }

    private void PlayMoveAudio(Vector2Int target)
    {
        AudioClip clip = grid.HasPellet(target) ? pelletWalkClip : walkClip;
        if (moveAudio.clip != clip || !moveAudio.isPlaying)
        {
            moveAudio.clip = clip;
            moveAudio.loop = true;
            moveAudio.Play();
        }
    }

    private string StateFor(Vector2Int dir)
    {
        if (dir == Up) return "WalkUp";
        if (dir == Down) return "WalkDown";
        if (dir == Left) return "WalkLeft";
        return "WalkRight";
    }
}