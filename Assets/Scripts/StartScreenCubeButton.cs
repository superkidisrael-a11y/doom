using System;
using UnityEngine;

/// <summary>Mouse interaction for the physical cube buttons.</summary>
public class StartScreenCubeButton : MonoBehaviour
{
    [SerializeField] private float hoverScale = 0.94f;
    [SerializeField] private Vector3 pressScale = new Vector3(0.9f, 0.78f, 0.9f);

    private Vector3 normalScale;
    private Action clickAction;
    private bool hovered;

    public void Setup(Action onClick)
    {
        clickAction = onClick;
        normalScale = transform.localScale;
    }

    private void Awake()
    {
        normalScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        hovered = true;
        transform.localScale = normalScale * hoverScale;
    }

    private void OnMouseExit()
    {
        hovered = false;
        transform.localScale = normalScale;
    }

    private void OnMouseDown()
    {
        transform.localScale = Vector3.Scale(normalScale, pressScale);
    }

    private void OnMouseUp()
    {
        transform.localScale = hovered ? normalScale * hoverScale : normalScale;
        if (hovered)
        {
            clickAction?.Invoke();
        }
    }
}
