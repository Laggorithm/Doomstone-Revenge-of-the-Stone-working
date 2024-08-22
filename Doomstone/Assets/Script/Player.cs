using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public float jumpForce = 10f; // Fixed jump force
    public int Hp = 5; // Player health
    public float rotationTorque = 5f; // Torque applied when jumping
    public float slideFriction = 0.1f; // Friction factor to simulate sliding (deceleration)
    public float frozenMoveSpeed = 10f; // Speed while moving frozen
    public float frozenMoveDuration = 2f; // Duration for frozen movement

    private Rigidbody2D rb;
    public bool isGrounded = false;
    public bool isFreezed = false;

    private Vector2 lastMovementDirection = Vector2.right; // Default to right
    private float moveInput = 0f; // Current movement input

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }

    // Method to handle player movement
    void Move()
    {
        moveInput = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(moveInput * speed, rb.velocity.y);

        // Update last movement direction if moving horizontally
        if (moveInput != 0)
        {
            lastMovementDirection = new Vector2(moveInput, 0).normalized;
        }

        // Apply movement input to velocity
        rb.velocity = new Vector2(movement.x, rb.velocity.y);
    }

    // Method to handle player jumping
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            // Apply a torque for free rotation
            rb.AddTorque(rotationTorque * (rb.velocity.x > 0 ? 1 : -1), ForceMode2D.Impulse);

            isGrounded = false;
        }
    }

    // Check if player is grounded or collides with a special object
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpForce = 60;
        }
        else if (collider.CompareTag("Enemy"))
        {
            //DealDamageToEnemy(collider.gameObject);
        }
        else if (collider.CompareTag("RedJumpPad"))
        {
            isGrounded = true;
            jumpForce = 100;
        }
        else if (collider.CompareTag("YellowJumpPad"))
        {
            isGrounded = true;
            jumpForce = 80;
        }
        else if (collider.CompareTag("BluePad"))
        {
            isGrounded = true;
            isFreezed = true;
            jumpForce = 40;

            // Use lastMovementDirection to apply force in the direction the player was moving
            rb.AddForce(lastMovementDirection * 100, ForceMode2D.Impulse);

            // Start the coroutine to move in the last direction while frozen
            StartCoroutine(MoveFrozen());
        }
    }

    // Coroutine to move the player in the direction of last movement while frozen
    IEnumerator MoveFrozen()
    {
        float startTime = Time.time;

        while (Time.time - startTime < frozenMoveDuration)
        {
            // Apply movement in the direction of last movement
            rb.velocity = new Vector2(lastMovementDirection.x * frozenMoveSpeed, rb.velocity.y);

            // Optionally, you can add a delay if needed, e.g., to make the movement less smooth
            yield return null; // Wait until the next frame
        }

        // Stop the frozen movement after the duration
        rb.velocity = new Vector2(0, rb.velocity.y);
        isFreezed = false;
        Debug.Log("Player has stopped moving after frozen period.");
    }

    // Gradually reduce horizontal velocity to simulate sliding on ice
    void FixedUpdate()
    {
        if (!isFreezed)
        {
            // Apply sliding effect to horizontal velocity only
            if (Mathf.Abs(moveInput) < 0.01f) // No significant input
            {
                Vector2 velocity = rb.velocity;
                velocity.x *= (1 - slideFriction); // Apply friction to horizontal velocity

                // Set vertical velocity to its current value to prevent it from being affected by friction
                rb.velocity = new Vector2(velocity.x, rb.velocity.y);

                // Stop the object if its horizontal velocity is very low
                if (Mathf.Abs(rb.velocity.x) < 0.01f)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
        }
    }

    // Method to deal damage to the enemy
    /*void DealDamageToEnemy(GameObject enemy)
    {
        // Calculate damage based on jump force
        int damage = Mathf.RoundToInt(jumpForce);

        // Example enemy damage script interaction
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.TakeDamage(damage);
        }
    }*/
}
