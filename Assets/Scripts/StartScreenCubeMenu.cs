using System.Collections.Generic;
using UnityEngine;

/// <summary>Shows the 2x2x1 block choices after a large cube is pressed.</summary>
public class StartScreenCubeMenu : MonoBehaviour
{
    private readonly List<GameObject> smallCubes = new List<GameObject>();
    private static readonly string[] BlockNames =
    {
        "Wall", "Cannon", "Archer", "Frost", "Bomb", "Laser",
        "Mortar", "Tesla", "Slow", "Fire", "Air", "Support"
    };

    private void Start()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int smallIndex = 0;

        foreach (GameObject candidate in allObjects)
        {
            if (!candidate.name.StartsWith("Cube") || candidate.GetComponent<Collider>() == null)
            {
                continue;
            }

            float width = candidate.transform.localScale.x;
            bool isSaveOne = HasLabel(candidate, "save 1");

            if (Mathf.Approximately(width, 2f) || isSaveOne)
            {
                ConfigureSmallCube(candidate, smallIndex++, !isSaveOne);
            }
            else if (width > 3f && !IsAlphaCube(candidate))
            {
                StartScreenCubeButton button = candidate.AddComponent<StartScreenCubeButton>();
                button.Setup(OpenBlockMenu);
            }
        }
    }

    private void ConfigureSmallCube(GameObject cube, int index, bool assignBlockName)
    {
        smallCubes.Add(cube);

        TextMesh label = FindLabel(cube);
        if (assignBlockName && label != null)
        {
            label.text = BlockNames[index % BlockNames.Length];
        }

        StartScreenCubeButton button = cube.AddComponent<StartScreenCubeButton>();
        button.Setup(() => { });
        cube.SetActive(false);
    }

    private void OpenBlockMenu()
    {
        foreach (GameObject cube in smallCubes)
        {
            if (cube != null)
            {
                cube.SetActive(true);
            }
        }
    }

    private static bool IsAlphaCube(GameObject cube)
    {
        TextMesh label = FindLabel(cube);
        return label != null && label.text.ToLowerInvariant().Contains("welcome");
    }

    private static bool HasLabel(GameObject cube, string expectedText)
    {
        TextMesh label = FindLabel(cube);
        return label != null &&
               label.text.Trim().Equals(expectedText, System.StringComparison.OrdinalIgnoreCase);
    }

    private static TextMesh FindLabel(GameObject cube)
    {
        TextMesh[] labels = cube.GetComponentsInChildren<TextMesh>(true);
        foreach (TextMesh label in labels)
        {
            if (!string.IsNullOrWhiteSpace(label.text))
            {
                return label;
            }
        }

        return labels.Length > 0 ? labels[0] : null;
    }
}
