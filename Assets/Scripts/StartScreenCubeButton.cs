using System;
using UnityEngine;

/// <summary>Mouse interaction for the physical cube buttons.</summary>
public class StartScreenCubeButton : MonoBehaviour
{
    [SerializeField] private float hoverScale = 0.94f;
    [SerializeField] private Vector3 pressScale = new Vector3(0.9f, 0.78f, 0.9f);

    private Vector3 normalScale;
    private Action clickAction;
    private Collider cubeCollider;
    private bool hovered;
    private bool pressed;

    public void Setup(Action onClick)
    {
        clickAction = onClick;
        normalScale = transform.localScale;
    }

    private void Awake()
    {
        normalScale = transform.localScale;
        cubeCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        Camera activeCamera = Camera.main;
        if (activeCamera == null || cubeCollider == null)
        {
            return;
        }

        Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
        bool isHovered = Physics.Raycast(ray, out RaycastHit hit) && hit.collider == cubeCollider;

        if (isHovered != hovered)
        {
            hovered = isHovered;
            transform.localScale = hovered ? normalScale * hoverScale : normalScale;
        }

        if (hovered && Input.GetMouseButtonDown(0))
        {
            pressed = true;
            transform.localScale = Vector3.Scale(normalScale, pressScale);
        }

        if (pressed && Input.GetMouseButtonUp(0))
        {
            pressed = false;
            transform.localScale = hovered ? normalScale * hoverScale : normalScale;
            if (hovered)
            {
                clickAction?.Invoke();
            }
        }
    }
}
