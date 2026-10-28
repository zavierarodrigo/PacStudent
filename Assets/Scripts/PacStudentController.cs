
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tweener tweener;
    [SerializeField] private LevelGrid grid;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource moveAudio;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip pelletWalkClip;
    [SerializeField] private ParticleSystem dust;

    [Header("Movement")]
    [SerializeField] private Vector2Int startCell = new Vector2Int(1, 1);
    [SerializeField] private float tilesPerSecond = 5f;

    [Header("Tunnel World Positions")]
    [SerializeField] private Vector2 leftTunnel = new Vector2(-8.5f, -7f);
    [SerializeField] private Vector2 rightTunnel = new Vector2(19.5f, -7f);
    [SerializeField] private float tunnelTolerance = 0.4f;

    private static bool movementEnabled = true;

    private static readonly Vector2Int Up = new Vector2Int(0, -1);
    private static readonly Vector2Int Down = new Vector2Int(0, 1);
    private static readonly Vector2Int Left = new Vector2Int(-1, 0);
    private static readonly Vector2Int Right = new Vector2Int(1, 0);

    private Vector2Int gridPos;
    private Vector2Int currentDirection = Vector2Int.zero;
    private Vector2Int requestedDirection = Vector2Int.zero;
    private Vector2Int facing = Right;

    private bool isMoving;
    private float moveEndTime;

    public static void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }

    private void Start()
    {
        gridPos = startCell;

        Vector3 startPosition = grid.CellToWorld(startCell);
        startPosition.z = transform.position.z;
        transform.position = startPosition;

        if (animator != null)
        {
            animator.Play("WalkRight", 0, 0f);
            animator.speed = 0f;
        }
    }

    private void Update()
    {
        if (!movementEnabled)
            return;

        ReadInput();

        if (tweener.TweenExists(transform))
            return;

        if (TryTeleport())
            return;

        if (requestedDirection != Vector2Int.zero)
        {
            if (TryMove(requestedDirection))
            {
                currentDirection = requestedDirection;
                requestedDirection = Vector2Int.zero;
                return;
            }
        }

        if (TryMove(currentDirection))
            return;

        StopMoving();
    }

    private void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            requestedDirection = Up;
        else if (Input.GetKeyDown(KeyCode.S))
            requestedDirection = Down;
        else if (Input.GetKeyDown(KeyCode.A))
            requestedDirection = Left;
        else if (Input.GetKeyDown(KeyCode.D))
            requestedDirection = Right;
    }

    private bool TryMove(Vector2Int direction)
    {
        if (direction == Vector2Int.zero)
            return false;

        Vector2Int targetCell = gridPos + direction;

        if (!grid.IsWalkable(targetCell))
            return false;

        float duration = 1f / tilesPerSecond;
        float startTime = isMoving ? moveEndTime : Time.time;

        Vector3 from = transform.position;
        Vector3 to = grid.CellToWorld(targetCell);
        to.z = from.z;

        tweener.AddTween(transform, from, to, duration, startTime);

        gridPos = targetCell;
        moveEndTime = startTime + duration;
        isMoving = true;

        Face(direction);
        PlayMoveAudio(targetCell);

        if (dust != null && !dust.isPlaying)
            dust.Play();

        return true;
    }

    private bool TryTeleport()
    {
        Vector2 position = transform.position;

        if (currentDirection == Left &&
            IsAtTunnel(position, leftTunnel))
        {
            TeleportTo(rightTunnel);
            return true;
        }

        if (currentDirection == Right &&
            IsAtTunnel(position, rightTunnel))
        {
            TeleportTo(leftTunnel);
            return true;
        }

        return false;
    }

    private bool IsAtTunnel(Vector2 position, Vector2 tunnel)
    {
        return Mathf.Abs(position.x - tunnel.x) <= tunnelTolerance
            && Mathf.Abs(position.y - tunnel.y) <= tunnelTolerance;
    }

    private void TeleportTo(Vector2 destination)
    {
        Vector3 position = new Vector3(
            destination.x,
            destination.y,
            transform.position.z
        );

        transform.position = position;
        gridPos = grid.WorldToCell(position);

        isMoving = false;
        moveEndTime = Time.time;
    }

    private void StopMoving()
    {
        isMoving = false;

        if (animator != null)
            animator.speed = 0f;

        if (moveAudio != null)
            moveAudio.Stop();

        if (dust != null)
            dust.Stop();
    }

    private void Face(Vector2Int direction)
    {
        if (animator == null)
            return;

        animator.speed = 1f;

        if (direction == facing)
            return;

        facing = direction;
        animator.Play(StateFor(direction), 0, 0f);
    }

    private void PlayMoveAudio(Vector2Int targetCell)
    {
        if (moveAudio == null)
            return;

        AudioClip clip = grid.HasPellet(targetCell)
            ? pelletWalkClip
            : walkClip;

        if (moveAudio.clip != clip || !moveAudio.isPlaying)
        {
            moveAudio.clip = clip;
            moveAudio.loop = true;
            moveAudio.Play();
        }
    }

    private string StateFor(Vector2Int direction)
    {
        if (direction == Up)
            return "WalkUp";

        if (direction == Down)
            return "WalkDown";

        if (direction == Left)
            return "WalkLeft";

        return "WalkRight";
    }
}
