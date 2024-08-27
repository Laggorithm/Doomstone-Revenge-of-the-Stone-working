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

    private Vector2 moveDirection;
    private float currentDistanceTraveled;
    private Rigidbody2D rb;
    private bool isGrounded = true; // Assume boss is grounded initially
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        InitializeAttackList();
        PrintAttackList();  // Optional: To verify the list content in the console

        SetRandomDirection();
        StartCoroutine(ChangeDirectionPeriodically());
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        MoveBoss();
        HandleJump();

        // Check the boss's HP and update the attack list if needed
        if (Hp <= 3 && AttackList.Count < 10) // Adjust check as needed
        {
            UpdateAttackListForLowHp();
        }
    }

    void InitializeAttackList()
    {
        // Initially, the attack list only contains 1x jumps
        AttackList.Clear();
        for (int i = 0; i < 10; i++) // Assuming you want 10 jumps initially, all of 1x
        {
            AttackList.Add(1);
        }

        // Shuffle the list to randomize the order
        ShuffleList(AttackList);
    }

    void UpdateAttackListForLowHp()
    {
        // Replace the attack list with a new list containing 3x 2x jumps
        AttackList.Clear();
        for (int i = 0; i < 7; i++) // Assuming you want 7 jumps of 1x
        {
            AttackList.Add(1);
        }

        for (int i = 0; i < 3; i++) // 3 jumps of 2x
        {
            AttackList.Add(2);
        }

        // Shuffle the list to randomize the order
        ShuffleList(AttackList);
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

    void HandleJump()
    {
        if (AttackList.Count > 0 && isGrounded)
        {
            int attackId = AttackList[0]; // Get the first attack ID
            AttackList.RemoveAt(0); // Remove the attack ID from the list

            // Determine the jump force based on attack ID
            float jumpForce = attackId == 2 ? normalJumpForce * doubleJumpFactor : normalJumpForce;

            // Apply the jump force
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false; // Set the boss to not grounded until it lands again

            // Trigger the corresponding animation based on attack ID
            TriggerJumpAnimation(attackId);
        }
    }

    void TriggerJumpAnimation(int attackId)
    {
        if (attackId == 1)
        {
            animator.SetTrigger("Jump1"); // Animation for 1x jump
        }
        else if (attackId == 2)
        {
            animator.SetTrigger("Jump2"); // Animation for 2x jump
        }
    }

    // Handle collision with the ground to reset isGrounded
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
