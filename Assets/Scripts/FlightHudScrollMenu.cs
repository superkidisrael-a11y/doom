using System;
using UnityEngine;

/// <summary>
/// Hold right-click to open a cursor-driven flight menu near the centre-right
/// of the screen. The wheel and the visible buttons both change selection.
/// </summary>
public class FlightHudScrollMenu : MonoBehaviour
{
    private static readonly string[] DefaultItems =
    {
        "Wall", "Cannon", "Archer", "Frost",
        "Bomb", "Laser", "Support", "Cancel"
    };

    private string[] items = DefaultItems;
    private Action<int> selectionHandler;
    private int selectedIndex;
    private bool dismissedForCurrentHold;

    public bool IsOpen => Input.GetMouseButton(1) && !dismissedForCurrentHold;
    public string SelectedItem => items.Length > 0 ? items[selectedIndex] : "None";

    public void Configure(string[] itemNames, Action<int> onSelected)
    {
        if (itemNames != null && itemNames.Length > 0)
        {
            items = itemNames;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, items.Length - 1);
        }

        selectionHandler = onSelected;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            dismissedForCurrentHold = false;
        }

        if (!IsOpen)
        {
            return;
        }

        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0.01f)
        {
            ChangeSelection(-1);
        }
        else if (scroll < -0.01f)
        {
            ChangeSelection(1);
        }
    }

    private void OnGUI()
    {
        if (!IsOpen || items.Length == 0)
        {
            return;
        }

        const float width = 250f;
        const float rowHeight = 34f;
        const float padding = 12f;
        float height = 78f + items.Length * rowHeight;
        float x = Mathf.Clamp(Screen.width * 0.68f, padding, Screen.width - width - padding);
        float y = (Screen.height - height) * 0.5f;

        GUI.Box(new Rect(x, y, width, height), "T1 Towers");
        GUI.Label(
            new Rect(x + padding, y + 24f, width - padding * 2f, 24f),
            "Scroll to highlight, click to choose");

        for (int index = 0; index < items.Length; index++)
        {
            string prefix = index == selectedIndex ? ">  " : "   ";
            Rect buttonRect = new Rect(
                x + padding,
                y + 52f + index * rowHeight,
                width - padding * 2f,
                rowHeight - 4f);

            if (GUI.Button(buttonRect, prefix + items[index]))
            {
                Select(index);
            }
        }

        GUI.Label(
            new Rect(x + padding, y + height - 24f, width - padding * 2f, 22f),
            "Selected: " + SelectedItem);
    }

    private void ChangeSelection(int direction)
    {
        int next = (selectedIndex + direction) % items.Length;
        if (next < 0)
        {
            next += items.Length;
        }

        selectedIndex = next;
    }

    private void Select(int index)
    {
        selectedIndex = index;
        Debug.Log("Flight menu selected: " + SelectedItem);
        selectionHandler?.Invoke(index);
        dismissedForCurrentHold = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
