using UnityEngine;

/// <summary>
/// Player flight controller for the bird. WASD moves relative to the bird's
/// current aim direction, while Space/Ctrl move vertically.
/// </summary>
public class BirdFlightController : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float verticalSpeed = 9f;
    [SerializeField] private float shiftMoveSpeedBonus = 8f;
    [SerializeField] private float shiftVerticalSpeedBonus = 6f;
    [SerializeField] private float turnSensitivity = 3f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private float rotationSmoothing = 12f;

    private float yaw;
    private float pitch;

    public float Pitch => pitch;

    private void Start()
    {
        Vector3 startAngles = transform.eulerAngles;
        yaw = startAngles.y;
        pitch = NormalizeAngle(startAngles.x);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        AimWithMouse();
        Fly();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void AimWithMouse()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        yaw += Input.GetAxis("Mouse X") * turnSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * turnSensitivity;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothing * Time.deltaTime);
    }

    private void Fly()
    {
        float forward = Input.GetAxisRaw("Vertical");
        float sideways = Input.GetAxisRaw("Horizontal");
        float vertical = 0f;

        if (Input.GetKey(KeyCode.Space))
        {
            vertical += 1f;
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            vertical -= 1f;
        }

        bool isBoosting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentMoveSpeed = moveSpeed + (isBoosting ? shiftMoveSpeedBonus : 0f);
        float currentVerticalSpeed = verticalSpeed + (isBoosting ? shiftVerticalSpeedBonus : 0f);

        // Keep WASD level so looking up/down does not change altitude.
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
        Vector3 direction = flatForward * forward + flatRight * sideways;
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        Vector3 velocity = direction * currentMoveSpeed + Vector3.up * (vertical * currentVerticalSpeed);
        transform.position += velocity * Time.deltaTime;
    }

    private static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        return angle > 180f ? angle - 360f : angle;
    }
}
