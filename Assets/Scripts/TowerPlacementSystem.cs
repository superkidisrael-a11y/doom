using UnityEngine;

/// <summary>
/// Selects T1 towers from the right-click HUD and places them using a ray from
/// the exact centre of the screen. Only the named placement floor is valid.
/// </summary>
public class TowerPlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] towerPrefabs = new GameObject[0];
    [SerializeField] private string placementFloorName = "placement floor plane";
    [SerializeField] private float maximumPlacementDistance = 500f;
    [SerializeField] private float gridSize = 1f;

    private FlightHudScrollMenu hudMenu;
    private Collider placementFloor;
    private GameObject selectedPrefab;
    private GameObject preview;
    private Vector3 placementPosition;
    private float previewBottomOffset;
    private bool hasValidPlacement;

    private void Start()
    {
        hudMenu = GetComponent<FlightHudScrollMenu>();
        if (hudMenu == null)
        {
            hudMenu = gameObject.AddComponent<FlightHudScrollMenu>();
        }

        GameObject floor = GameObject.Find(placementFloorName);
        if (floor != null)
        {
            placementFloor = floor.GetComponent<Collider>();
        }

        if (placementFloor == null)
        {
            Debug.LogError("Tower placement floor was not found: " + placementFloorName);
        }

        string[] towerNames = new string[towerPrefabs.Length];
        for (int index = 0; index < towerPrefabs.Length; index++)
        {
            towerNames[index] = FormatTowerName(towerPrefabs[index]);
        }

        hudMenu.Configure(towerNames, BeginPlacement);
    }

    private void Update()
    {
        if (selectedPrefab == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
            return;
        }

        UpdatePlacementPreview();

        if (hasValidPlacement && !hudMenu.IsOpen && Input.GetMouseButtonDown(0))
        {
            PlaceTower();
        }
    }

    private void BeginPlacement(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPrefabs.Length || towerPrefabs[towerIndex] == null)
        {
            return;
        }

        CancelPlacement();
        selectedPrefab = towerPrefabs[towerIndex];
        preview = Instantiate(selectedPrefab);
        preview.name = selectedPrefab.name + " Placement Preview";

        previewBottomOffset = CalculateBottomOffset(preview);

        foreach (Collider childCollider in preview.GetComponentsInChildren<Collider>(true))
        {
            childCollider.enabled = false;
        }

        preview.SetActive(false);
        hasValidPlacement = false;
    }

    private void UpdatePlacementPreview()
    {
        Camera activeCamera = Camera.main;
        if (activeCamera == null || placementFloor == null)
        {
            SetPreviewVisible(false);
            hasValidPlacement = false;
            return;
        }

        Ray centreRay = activeCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        hasValidPlacement = Physics.Raycast(
            centreRay,
            out RaycastHit hit,
            maximumPlacementDistance) && hit.collider == placementFloor;

        if (!hasValidPlacement)
        {
            SetPreviewVisible(false);
            return;
        }

        placementPosition = SnapToGrid(hit.point);
        placementPosition.y = hit.point.y + previewBottomOffset;
        preview.transform.SetPositionAndRotation(placementPosition, Quaternion.identity);
        SetPreviewVisible(true);
    }

    private void PlaceTower()
    {
        Instantiate(selectedPrefab, placementPosition, Quaternion.identity);
        Debug.Log("Placed tower: " + selectedPrefab.name);
        CancelPlacement();
    }

    private void CancelPlacement()
    {
        if (preview != null)
        {
            Destroy(preview);
        }

        preview = null;
        selectedPrefab = null;
        hasValidPlacement = false;
    }

    private Vector3 SnapToGrid(Vector3 point)
    {
        if (gridSize <= 0f)
        {
            return point;
        }

        point.x = Mathf.Round(point.x / gridSize) * gridSize;
        point.z = Mathf.Round(point.z / gridSize) * gridSize;
        return point;
    }

    private static float CalculateBottomOffset(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            return 0f;
        }

        Bounds bounds = renderers[0].bounds;
        for (int index = 1; index < renderers.Length; index++)
        {
            bounds.Encapsulate(renderers[index].bounds);
        }

        return target.transform.position.y - bounds.min.y;
    }

    private static string FormatTowerName(GameObject towerPrefab)
    {
        if (towerPrefab == null)
        {
            return "Missing Tower";
        }

        string displayName = towerPrefab.name;
        if (displayName.StartsWith("tower "))
        {
            displayName = displayName.Substring(6);
        }

        return displayName.ToUpperInvariant();
    }

    private void SetPreviewVisible(bool visible)
    {
        if (preview != null && preview.activeSelf != visible)
        {
            preview.SetActive(visible);
        }
    }

    private void OnGUI()
    {
        if (selectedPrefab == null)
        {
            return;
        }

        Color oldColor = GUI.color;
        GUI.color = hasValidPlacement ? Color.green : Color.red;
        GUI.Label(
            new Rect(Screen.width * 0.5f - 10f, Screen.height * 0.5f - 14f, 30f, 30f),
            "+");
        GUI.color = oldColor;

        string message = hasValidPlacement
            ? "Left click to place " + FormatTowerName(selectedPrefab)
            : "Aim the screen centre at the placement floor";
        GUI.Box(new Rect(Screen.width * 0.5f - 210f, 18f, 420f, 32f), message);
    }
}
