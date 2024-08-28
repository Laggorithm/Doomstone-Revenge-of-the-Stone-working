using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCommonBehaviour : MonoBehaviour
{
    private List<int> AttackList = new List<int>();
    public Player player;
    public int Hp = 5;

    public float moveSpeed = 5f; // Speed at which the boss moves
    public float minTravelDistance = 5f; // Minimum distance to travel before changing direction
    public float directionChangeInterval = 3f; // Time interval to change direction
    public float normalJumpForce = 10f; // Normal jump force
    public float doubleJumpFactor = 2f; // Factor by which to multiply the jump force for value 2

    public GameObject leftProjectilePrefab; // Projectile prefab for the left direction
    public GameObject rightProjectilePrefab; // Projectile prefab for the right direction
    public float projectileSpeed = 10f; // Speed at which projectiles move
    public float projectileLaunchDelay = 0.5f; // Delay before launching projectiles after airborne
    public float projectileLifetime = 3f; // Lifetime of the projectile before it gets destroyed

    private Vector2 moveDirection;
    private float currentDistanceTraveled;
    private Rigidbody2D rb;
    private bool isGrounded = true; // Assume boss is grounded initially
    private bool isHandlingAttack = false; // Track if the boss is currently handling an attack
    private bool isPhaseTwo = false; // Track if the boss has entered phase two
    private bool hasLaunchedProjectiles = false; // Ensure projectiles are launched only once per airborne state

    public GameObject extraLeftProjectilePrefab; // New projectile prefab for the left direction
    public GameObject extraRightProjectilePrefab; // New projectile prefab for the right direction
    public Transform leftShootingPoint; // Shooting point for the left projectile
    public Transform rightShootingPoint; // Shooting point for the right projectile
    public Transform extraLeftShootingPoint; // Additional shooting point for the new left projectile
    public Transform extraRightShootingPoint; // Additional shooting point for the new right projectile

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        InitializePhaseOne();
        PrintAttackList();  // Optional: To verify the list content in the console

        // Initialize the boss movement
        SetRandomDirection();
        StartCoroutine(ChangeDirectionPeriodically());
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        MoveBoss();
        if (!isHandlingAttack && isGrounded)
        {
            StartCoroutine(HandleJump());
        }

        if (Hp <= 3 && !isPhaseTwo)
        {
            EnterPhaseTwo();
        }

        if (Hp <= 0)
        {
            Destroy(gameObject); // Destroy the boss when HP reaches 0
        }

        // Handle projectile launching while grounded
        if (isGrounded && !hasLaunchedProjectiles)
        {
            StartCoroutine(LaunchProjectiles());
        }
    }

    void InitializePhaseOne()
    {
        // Clear any existing attacks in the list
        AttackList.Clear();

        // Add 4 "0"s to the AttackList (no action)
        for (int i = 0; i < 4; i++)
        {
            AttackList.Add(0);
        }

        // Add 6 "1"s to the AttackList (normal jump)
        for (int i = 0; i < 6; i++)
        {
            AttackList.Add(1);
        }

        // Shuffle the list to randomize the order
        ShuffleList(AttackList);
    }

    void EnterPhaseTwo()
    {
        isPhaseTwo = true; // Mark that the boss has entered phase two
        Hp = 10; // Heal the boss to 10 HP

        // Clear the existing AttackList and initialize phase two attacks
        AttackList.Clear();

        
        // Add 6 "0"s to the AttackList (no action)
        for (int i = 0; i < 6; i++)
        {
            AttackList.Add(0);
        }

        // Add 9 "1"s to the AttackList (normal jump)
        for (int i = 0; i < 9; i++)
        {
            AttackList.Add(1);
        }

        // Add 5 "2"s to the AttackList (double jump)
        for (int i = 0; i < 5; i++)
        {
            AttackList.Add(2);
        }

        // Shuffle the list to randomize the order
        ShuffleList(AttackList);

        PrintAttackList();  // Optional: To verify the list content in the console
    }

    void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void PrintAttackList()
    {
        foreach (int attack in AttackList)
        {
            Debug.Log("Attack ID: " + attack);
        }
    }

    void MoveBoss()
    {
        // Move the boss in the current direction
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Update the distance traveled
        currentDistanceTraveled += moveSpeed * Time.deltaTime;

        // Check if the minimum travel distance has been reached
        if (currentDistanceTraveled >= minTravelDistance)
        {
            SetRandomDirection();
            currentDistanceTraveled = 0f;
        }
    }

    void SetRandomDirection()
    {
        // Choose a random direction (left or right)
        float xDirection = Random.value > 0.5f ? 1f : -1f;
        moveDirection = new Vector2(xDirection, 0f);
    }

    IEnumerator ChangeDirectionPeriodically()
    {
        while (true)
        {
            // Wait for a specified interval before changing direction
            yield return new WaitForSeconds(directionChangeInterval);
            SetRandomDirection();
        }
    }

    IEnumerator HandleJump()
    {
        isHandlingAttack = true; // Set the flag to indicate the boss is handling an attack

        while (AttackList.Count > 0 && isGrounded)
        {
            int attackId = AttackList[0]; // Get the first attack ID
            AttackList.RemoveAt(0); // Remove the attack ID from the list

            if (attackId != 0) // Only perform action if attackId is not "0"
            {
                // Determine the jump force based on attack ID
                float jumpForce = attackId == 2 ? normalJumpForce * doubleJumpFactor : normalJumpForce;

                // Apply the jump force
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false; // Set the boss to not grounded until it lands again
            }

            // Wait for 3 seconds before handling the next attack
            yield return new WaitForSeconds(3f);
        }

        isHandlingAttack = false; // Reset the flag after handling all attacks
    }

    IEnumerator LaunchProjectiles()
    {
        hasLaunchedProjectiles = true; // Ensure projectiles are only launched once per grounded state

        // Launch original projectiles if prefabs and shooting points are assigned
        if (leftProjectilePrefab != null && leftShootingPoint != null)
        {
            GameObject leftProjectile = Instantiate(leftProjectilePrefab, leftShootingPoint.position, Quaternion.identity);
            leftProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(-projectileSpeed, 0f);
            Destroy(leftProjectile, projectileLifetime); // Destroy the projectile after its lifetime expires
        }

        if (rightProjectilePrefab != null && rightShootingPoint != null)
        {
            GameObject rightProjectile = Instantiate(rightProjectilePrefab, rightShootingPoint.position, Quaternion.identity);
            rightProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileSpeed, 0f);
            Destroy(rightProjectile, projectileLifetime); // Destroy the projectile after its lifetime expires
        }

        // Launch new projectiles if prefabs and shooting points are assigned
        if (extraLeftProjectilePrefab != null && extraLeftShootingPoint != null)
        {
            GameObject extraLeftProjectile = Instantiate(extraLeftProjectilePrefab, extraLeftShootingPoint.position, Quaternion.identity);
            extraLeftProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(-projectileSpeed, 0f);
            Destroy(extraLeftProjectile, projectileLifetime); // Destroy the projectile after its lifetime expires
        }

        if (extraRightProjectilePrefab != null && extraRightShootingPoint != null)
        {
            GameObject extraRightProjectile = Instantiate(extraRightProjectilePrefab, extraRightShootingPoint.position, Quaternion.identity);
            extraRightProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileSpeed, 0f);
            Destroy(extraRightProjectile, projectileLifetime); // Destroy the projectile after its lifetime expires
        }

        yield return null; // Yielding null to end the coroutine
    }


    // Handle collision with the ground to reset isGrounded
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            hasLaunchedProjectiles = false; // Reset projectile launch flag when grounded

            // Ensure no active projectiles remain
            // This will be managed by projectile lifetime
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
