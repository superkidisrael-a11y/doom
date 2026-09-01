using UnityEngine;

/// <summary>
/// Keeps the main camera behind and above the bird while looking toward it.
/// The bird controller owns mouse aiming; this camera simply follows that aim.
/// </summary>
public class ThirdPersonFlightCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 2.5f, -7f);
    [SerializeField] private float followSmoothness = 10f;
    [SerializeField] private float lookHeight = 0.8f;

    private void Start()
    {
        if (target == null)
        {
            BirdFlightController player = FindObjectOfType<BirdFlightController>();
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.TransformPoint(followOffset);
        float blend = 1f - Mathf.Exp(-followSmoothness * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, blend);

        Vector3 lookDirection = (target.position + Vector3.up * lookHeight) - transform.position;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, blend);
        }
    }
}
