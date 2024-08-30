using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public float speed = 5f;            // Speed of the projectile
    public float rotationSpeed = 200f;  // Speed at which the projectile rotates towards the player
    public Transform target;            // Target to follow (typically the player)
     
    private Rigidbody2D rb;

    void Start()
    {
        // Get the Rigidbody2D component attached to this projectile
        rb = GetComponent<Rigidbody2D>();

        // Optionally, you can automatically find the player by tag (if the player has the "Player" tag)
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned for the homing projectile.");
            return;
        }

        // Calculate the direction to the target
        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();

        // Calculate the amount to rotate this frame
        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        // Apply the rotation
        rb.angularVelocity = -rotateAmount * rotationSpeed;

        // Move the projectile forward
        rb.velocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Add logic for what happens when the projectile hits something (e.g., damage the player, destroy the projectile, etc.)
        if (other.CompareTag("Player"))
        {
            // Handle collision with the player (e.g., deal damage)
            Debug.Log("Projectile hit the player!");
             
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            // Handle collision with other objects, like obstacles
            Destroy(gameObject);
        }

    }
}
