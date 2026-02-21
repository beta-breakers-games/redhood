using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Last standing")]
    public Vector2 lastStandingPosition;
    public float minMoveToUpdateLastStanding = 0.05f;
    
    private Rigidbody2D rb;
    private bool isGrounded;

    private Animator animator;
    private SpriteRenderer sr; // ← přidáno
    public FootstepManager footstepManager;
    private string currentSurfaceTag = "Gravel"; //default surface

    public AudioSource audioSource;
    public AudioClip inhaleClip;
    public AudioClip exhaleClip;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>(); // ← inicializace SpriteRenderer
        lastStandingPosition = transform.position; 
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Otočení hráče podle směru
        if (moveInput < 0) sr.flipX = true;  // doleva
        else if (moveInput > 0) sr.flipX = false; // doprava

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        SetAnimation(moveInput);
    }
    
    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Update last standing only when on ground (and not moving upward)
        if (isGrounded && rb.linearVelocity.y <= 0.01f)
        {
            Vector2 p = transform.position;
            if (Vector2.Distance(p, lastStandingPosition) >= minMoveToUpdateLastStanding)
                lastStandingPosition = p;
        }

        RaycastHit2D hit = Physics2D.Raycast(
        groundCheck.position,
        Vector2.down,
        0.5f
        );

        if (hit.collider != null)
        {
        currentSurfaceTag = hit.collider.tag;
        }
    }
    
    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput == 0)
            {
                animator.Play("Player_Idle");
            }
            else
            {
                animator.Play("Player_Run");
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                animator.Play("Player_Jump");
            }
            else
            {
                animator.Play("Player_Fall");
            }
        }
    }

    public void PlayFootstep()
    {
    footstepManager.PlayFootstep(currentSurfaceTag);
    }

    public void PlayInhale() => audioSource.PlayOneShot(inhaleClip);
    public void PlayExhale() => audioSource.PlayOneShot(exhaleClip);
}
