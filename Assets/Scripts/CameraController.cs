using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform followTarget;

    [Header("Camera Position")]
    [SerializeField] float cameraOffset = 8;
    [SerializeField] Vector2 framingOffset;
    [SerializeField] float minVerticalAngle = 5;
    [SerializeField] float maxVerticalAngle = 60;

    [Header("Camera Rotation")]
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] bool invertRotation = false;

    float rotationX;
    float rotationY;
    int invert;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 1. camera rotate around player, offset at a certain distance
        // 2. camera look at player when rotating around it
        // 3. vertical rotation of camera

        // horizontal rotate around y axis, vertical rotate around x axis
        invert = (invertRotation) ? 1 : -1;
        rotationY += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationX += Input.GetAxis("Mouse Y") * rotationSpeed * invert;

        // clamp vertical camera rotation
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        // camera focus position on player
        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        // apply camera offset and rotation
        transform.position = focusPosition - targetRotation * new Vector3(0, 0, cameraOffset);
        transform.rotation = targetRotation;
    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}
