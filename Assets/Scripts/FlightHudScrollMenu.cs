using UnityEngine;

/// <summary>
/// Hold right-click to open a cursor-driven flight menu near the centre-right
/// of the screen. The wheel and the visible buttons both change selection.
/// </summary>
public class FlightHudScrollMenu : MonoBehaviour
{
    private static readonly string[] Items =
    {
        "Wall", "Cannon", "Archer", "Frost",
        "Bomb", "Laser", "Support", "Cancel"
    };

    private int selectedIndex;
    private bool isOpen;

    public string SelectedItem => Items[selectedIndex];

    private void Update()
    {
        isOpen = Input.GetMouseButton(1);
        if (!isOpen)
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
        if (!isOpen)
        {
            return;
        }

        const float width = 250f;
        const float rowHeight = 34f;
        const float padding = 12f;
        float height = 78f + Items.Length * rowHeight;
        float x = Mathf.Clamp(Screen.width * 0.68f, padding, Screen.width - width - padding);
        float y = (Screen.height - height) * 0.5f;

        GUI.Box(new Rect(x, y, width, height), "Flight Menu");
        GUI.Label(
            new Rect(x + padding, y + 24f, width - padding * 2f, 24f),
            "Scroll or click an item");

        for (int index = 0; index < Items.Length; index++)
        {
            string prefix = index == selectedIndex ? ">  " : "   ";
            Rect buttonRect = new Rect(
                x + padding,
                y + 52f + index * rowHeight,
                width - padding * 2f,
                rowHeight - 4f);

            if (GUI.Button(buttonRect, prefix + Items[index]))
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
        int next = (selectedIndex + direction) % Items.Length;
        if (next < 0)
        {
            next += Items.Length;
        }

        Select(next);
    }

    private void Select(int index)
    {
        selectedIndex = index;
        Debug.Log("Flight menu selected: " + SelectedItem);
    }
}
