using TNRD.Autohook;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SoundPlayer))]
public class LivingEntity : MonoBehaviour
{
    [Header("References")]

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    public SoundPlayer soundPlayer;

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    public Rigidbody2D rb;

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    public Animator animator;

    [SerializeField, AutoHook(ReadOnlyWhenFound = true)]
    private SpriteRenderer spriteRenderer;

    [Header("Movement")]

    public float jumpHeight = 5;

    public float walkDirection = 0;

    public float speed;

    public LayerMask groundMask;

    private bool jumping = false;

    public bool isGrounded { get { return IsGrounded(); } }

    [SerializeField]
    private float groundedRayLength;

    [SerializeField]
    private Vector2 spawnPosition;

    [Header("Jump Timer")]

    [SerializeField]
    private float maxJumpTimer;

    [SerializeField]
    private float jumpTimer;

    private void Start()
    {
        spawnPosition = transform.position;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(walkDirection * speed, rb.velocity.y);

        if (walkDirection != 0)
        {
            spriteRenderer.flipX = walkDirection < 0;
        }

        if (isGrounded) jumping = false;

        animator.SetBool("Walking", walkDirection != 0);
        animator.SetBool("Jumping", jumping);
        animator.SetFloat("Absolute Speed", speed);
    }

    private void OnDrawGizmos() => Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundedRayLength);

    private void Update()
    {
        if (transform.position.y < GameManager.instance.worldBarrier)
        {
            Die();
        }

        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        } else if (jumpTimer < 0)
        {
            jumpTimer = 0;
        }
    }

    public void Jump(bool onlyIfGrounded = true)
    {
        if (onlyIfGrounded && !isGrounded) return;

        if (jumpTimer > 0) return;

        jumping = true;
        rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Force);
        soundPlayer.PlaySound("jump");

        jumpTimer = maxJumpTimer;
    }

    public void Die()
    {
        transform.position = spawnPosition;
        rb.velocity = Vector2.zero;
        soundPlayer.PlaySound("death");
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundedRayLength, groundMask);

        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }
}