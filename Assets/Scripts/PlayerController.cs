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
    private bool tryToGround = true;
    private bool isGrounded = false;
    private bool jumpPressed = false;

    float inputAmount;
    Vector3 raycastFloorPos;
    Vector3 floorMovement;
    Vector3 gravity;
    Vector3 jump;
    Vector3 CombinedRaycast;

    public Transform groundChecker;
    public Transform collisionChecker;

    private int playerLayerMask;


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
        IsGrounded();

        // actual movement of the rigidbody + extra down force
        rb.velocity = (playerInput * currentSetSpeed * inputAmount) + (gravity.y * Vector2.up);

        // make sure the input doesnt go negative or above 1;
        float inputMagnitude = Mathf.Abs(playerInput.x) + Mathf.Abs(playerInput.y);
        inputAmount = Mathf.Clamp01(inputMagnitude);

        if (rb.velocity.y < 0)
        {
            gravity += Physics.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !jumpPressed)
        {
            gravity += Physics.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private bool IsGrounded()
    {
        Collider2D colliderHit = Physics2D.OverlapBox(tempCollider.bounds.center - Vector3.up, tempCollider.bounds.size, 0f, groundMask);
        return colliderHit != null;
    }

    private void Jump()
    {
        jumpPressed = !jumpPressed;

        if (isGrounded && jumpPressed)
        {
            tryToGround = false;
            isGrounded = false;
            rb.AddForce(Vector3.up * jumpStrength, ForceMode2D.Impulse);
        }
        Debug.Log("Jump called");
    }
}
