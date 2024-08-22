using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Vector3 offset; // Offset between the camera and the player
    public float smoothSpeed = 0.125f; // Smoothness of the camera movement

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Calculate the desired position for the camera (only X and Y are adjusted)
        Vector3 desiredPosition = new Vector3(player.position.x, player.position.y, transform.position.z) + offset;

        // Smoothly interpolate between the camera's current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the smoothed position to the camera
        transform.position = smoothedPosition;
    }
}
