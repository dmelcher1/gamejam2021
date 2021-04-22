using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public Collider2D tempCollider;
    [SerializeField] private LayerMask groundMask;

    public PlayerControls controls;
    private Vector2 playerInput;

    [Range(1, 10)]
    public float moveSpeed = 6f;
    [Range(1, 15)]
    public float runSpeed = 8f;
    private float currentSetSpeed = 6f;
    private bool isRunning = false;

    public float jumpStrength = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    private bool isGrounded = false;
    private bool jumpPressed = false;
    private bool movePressed = false;

    float inputAmount;
    Vector3 gravity;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();

        currentSetSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        controls.Player.Move.performed += ctx => playerInput = ctx.ReadValue<Vector2>();
        controls.Player.Jump.performed += ctx => Jump();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput != Vector2.zero) // Do we have an input?
        {
            Move(playerInput);
        }
        else
        {
            Move(Vector2.zero);
        }

        // Are we close enough to the ground to be considered grounded?
        if (!isGrounded)
            isGrounded = IsGrounded();

        if (rb.velocity.y < 0) // Are we falling?
        {
            gravity += Physics.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !jumpPressed) // Are we jumping, but the player is no longer trying to jump?
        {
            gravity += Physics.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Reset the player input after using it
        // ?playerInput = Vector2.zero;
    }

    private bool IsGrounded()
    {
        Collider2D colliderHit = Physics2D.OverlapBox(tempCollider.bounds.center - Vector3.up, tempCollider.bounds.size, 0f, groundMask);
        Debug.Log("Collider Hit: " + colliderHit.name);
        return colliderHit != null;
    }

    private void Move(Vector2 playerInput)
    {
        transform.Translate(playerInput * Vector2.right * currentSetSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        jumpPressed = !jumpPressed;

        if (IsGrounded() && jumpPressed)
        {
            isGrounded = false;
            rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            Debug.Log("Jumping");
        }

    }
}
