using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public static Vector3 offset; // Offset between the camera and the player
    public float smoothSpeed = 0.125f; // Smoothness of the camera movement
    public static int Closeup = -10;

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Calculate the desired position for the camera (only X and Y are adjusted)
        Vector3 desiredPosition = new Vector3(player.position.x, player.position.y, Closeup);

        // Smoothly interpolate between the camera's current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the smoothed position to the camera
        transform.position = smoothedPosition;
    }
}
