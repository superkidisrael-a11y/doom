using System.Collections.Generic;
using UnityEngine;

/// <summary>Shows the 2x2x1 block choices after a large cube is pressed.</summary>
public class StartScreenCubeMenu : MonoBehaviour
{
    private readonly List<GameObject> smallCubes = new List<GameObject>();

    private void Start()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

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
                ConfigureSmallCube(candidate);
            }
            else if (width > 3f && !IsAlphaCube(candidate))
            {
                StartScreenCubeButton button = candidate.AddComponent<StartScreenCubeButton>();
                button.Setup(() => OpenMenuFor(candidate));
            }
        }
    }

    private void ConfigureSmallCube(GameObject cube)
    {
        smallCubes.Add(cube);

        StartScreenCubeButton button = cube.AddComponent<StartScreenCubeButton>();
        button.Setup(() => { });
        cube.SetActive(false);
    }

    private void OpenMenuFor(GameObject sourceCube)
    {
        if (HasLabel(sourceCube, "new game"))
        {
            ShowOptions(
                new MenuOption("Start Game", label => SetMessage(label, "Starting new game")),
                new MenuOption("Choose Map", label => SetMessage(label, "Map: Doom Fields")),
                new MenuOption("Difficulty", label => SetMessage(label, "Difficulty: Normal")),
                new MenuOption("Back", label => HideMenu()));
            return;
        }

        if (HasLabel(sourceCube, "hello gang"))
        {
            ShowOptions(
                new MenuOption("Say Hello", label => SetMessage(label, "Hello, gang!")),
                new MenuOption("About", label => SetMessage(label, "Tower Defense of Doom")),
                new MenuOption("Credits", label => SetMessage(label, "Created by Superkidisrael")),
                new MenuOption("Back", label => HideMenu()));
            return;
        }

        ShowOptions(
            new MenuOption("Wall", label => SetMessage(label, "Wall selected")),
            new MenuOption("Cannon", label => SetMessage(label, "Cannon selected")),
            new MenuOption("Archer", label => SetMessage(label, "Archer selected")),
            new MenuOption("Back", label => HideMenu()));
    }

    private void ShowOptions(params MenuOption[] options)
    {
        for (int index = 0; index < smallCubes.Count; index++)
        {
            GameObject cube = smallCubes[index];
            cube.SetActive(true);

            // Keep every original menu cube available; repeat this menu's
            // option set across any additional cubes instead of hiding them.
            MenuOption option = options[index % options.Length];
            TextMesh label = FindLabel(cube);
            if (label != null)
            {
                label.text = option.text;
            }

            StartScreenCubeButton button = cube.GetComponent<StartScreenCubeButton>();
            button.Setup(() => option.action(label));
        }
    }

    private void HideMenu()
    {
        foreach (GameObject cube in smallCubes)
        {
            cube.SetActive(false);
        }
    }

    private static void SetMessage(TextMesh label, string message)
    {
        if (label != null)
        {
            label.text = message;
        }

        Debug.Log(message);
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

    private class MenuOption
    {
        public readonly string text;
        public readonly System.Action<TextMesh> action;

        public MenuOption(string text, System.Action<TextMesh> action)
        {
            this.text = text;
            this.action = action;
        }
    }
}
