using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public float jumpForce = 10f; // Fixed jump force
    public double Hp = 5; // Player health
    public float rotationTorque = 5f; // Torque applied when jumping
    public float slideFriction = 0.1f; // Friction factor to simulate sliding (deceleration)
    public float frozenMoveSpeed = 10f; // Speed while moving frozen
    public float frozenMoveDuration = 2f; // Duration for frozen movement
    public BossCommonBehaviour bossCommonBehaviour;
    private Rigidbody2D rb;
    public Transform spawnPointBoss;
    public Transform spawnPointLevelOne;
    public bool isGrounded = false;
    public bool isFreezed = false;
    public CameraShake cameraShake; // Reference to CameraShake script
    public int Dmg = 1;

    private Vector2 lastMovementDirection = Vector2.right; // Default to right
    private float moveInput = 0f; // Current movement input

    public bool IsPlayerRight = false;
    public bool IsPlayerLeft = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cameraShake = GetComponent<CameraShake>(); // Assuming the camera has the CameraShake script
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();

        if (Hp <= 0)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

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

            // Calculate desired rotation torque based on the current velocity direction
            float desiredTorque = rotationTorque * (rb.velocity.x > 0 ? 1 : -1);

            // Get the current rotation in degrees
            float currentRotation = rb.rotation % 360f;

            // Convert current rotation to a range between -180 and 180 degrees
            if (currentRotation > 180f) currentRotation -= 360f;
            else if (currentRotation < -180f) currentRotation += 360f;

            // Define the maximum rotation angle limit (in degrees)
            float maxRotationLimit = 45f;  // Adjust this value as needed

            // Check if applying the torque would exceed the rotation limit
            if ((desiredTorque > 0 && currentRotation < maxRotationLimit) ||
                (desiredTorque < 0 && currentRotation > -maxRotationLimit))
            {
                rb.AddTorque(desiredTorque, ForceMode2D.Impulse);
            }

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
            isFreezed = false;
            speed = 20;
        }
        else if (collider.CompareTag("Enemy"))
        {
            bossCommonBehaviour.Hp -= Dmg;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            rb.velocity = new Vector2(rb.velocity.y, jumpForce);
        }
        else if (collider.CompareTag("RedJumpPad"))
        {
            isGrounded = true;
            rb.velocity = new Vector2(rb.velocity.x, 100);
            isFreezed = false;
            speed = 20;
        }
        else if (collider.CompareTag("YellowJumpPad"))
        {
            isGrounded = true;
            rb.velocity = new Vector2(rb.velocity.x, 80);
            isFreezed = false;
            speed = 20;
        }
        else if (collider.CompareTag("BluePad"))
        {
            isGrounded = true;
            isFreezed = true;
            jumpForce = 40;
            speed = 10;
            // Use lastMovementDirection to apply force in the direction the player was moving
            rb.AddForce(lastMovementDirection * 100, ForceMode2D.Impulse);

            // Start the coroutine to move in the last direction while frozen
            StartCoroutine(MoveFrozen());
        }
        else if (collider.CompareTag("Traps"))
        {
            Hp -= 1;
            // Check if HP is 3 or below and trigger camera shake
            if (Hp <= 0)
            {
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.name);
            }
            
            Debug.Log(Hp);
        }
        else if (collider.CompareTag("Exit"))
        {
            SceneManager.UnloadSceneAsync("LevelOne");
            SceneManager.LoadScene("BossRoomOne");
           
        }
        else if (collider.CompareTag("IKZ"))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        else if (collider.CompareTag("Projectile"))
        {
            Hp -= 0.3;
        }
        
    }

    public void SceneManagment()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string currentSceneName = currentScene.name;
        switch (currentSceneName)
        {
            case ("BossRoomOne"):transform.position = spawnPointBoss.position; break;
            case ("LevelOne"):transform.position = spawnPointLevelOne.position; break;
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

    public void UnloadSceneByName(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
