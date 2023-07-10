using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform CameraTarget;

    public float smoothSpeed = 0.25f;
    public Vector3 offset;

    void FixedUpdate ()
    {
        Vector3 desiredPosition = CameraTarget.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
