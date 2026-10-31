using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SurgeryDrawingController : MonoBehaviour
{
    [Header("Settings")]
    public ToolModeController modeController;
    public RectTransform patientRect;
    public GameObject pointPrefab; // A small UI Image prefab
    public Color lineColor = Color.red;
    public float pointSpacing = 2f; // Pixels distance to spawn new point

    [Header("State")]
    private bool isDrawing = false;
    private List<Vector2> currentContour = new List<Vector2>(); // Normalised coordinates (0 to 1)
    private List<GameObject> visualPoints = new List<GameObject>();
    private GameObject contourContainer;

    void Start()
    {
        contourContainer = new GameObject("ContourContainer");
        contourContainer.transform.SetParent(patientRect);
        contourContainer.SetActive(false);

        if (modeController == null)
            modeController = FindObjectOfType<ToolModeController>();
    }

    void Update()
    {
        if (modeController == null || modeController.currentMode != ToolModeController.ToolMode.Surgery)
        {
            if (isDrawing) StopDrawing();
            return;
        }

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }
        else if (Input.GetMouseButton(0))
        {
            UpdateDrawing();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopDrawing();
        }
    }

    private void StartDrawing()
    {
        isDrawing = true;
        currentContour.Clear();
        ClearVisuals();
        contourContainer.SetActive(true);
        Debug.Log("Surgery: Started drawing contour");
    }

    private void UpdateDrawing()
    {
        Vector2 mousePos = Input.mousePosition;
        Canvas canvas = patientRect.GetComponentInParent<Canvas>();
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(patientRect, mousePos, canvas != null ? canvas.worldCamera : null, out Vector2 localPoint))
        {
            // Convert local coordinates to normalised coordinates (0 to 1)
            // This removes any "magnetism" to center or offsets, as it's relative to the rect size
            Vector2 size = patientRect.rect.size;
            Vector2 normalizedPoint = new Vector2(
                (localPoint.x / size.x) + 0.5f,
                (localPoint.y / size.y) + 0.5f
            );

            if (currentContour.Count == 0 || Vector2.Distance(localPoint, GetLastLocalPoint()) > pointSpacing)
            {
                currentContour.Add(normalizedPoint);
                SpawnPoint(localPoint);
            }
        }
    }

    private Vector2 GetLastLocalPoint()
    {
        if (currentContour.Count == 0) return Vector2.zero;
        Vector2 lastNorm = currentContour[currentContour.Count - 1];
        Vector2 size = patientRect.rect.size;
        return new Vector2(
            (lastNorm.x - 0.5f) * size.x,
            (lastNorm.y - 0.5f) * size.y
        );
    }

    private void StopDrawing()
    {
        isDrawing = false;
        Debug.Log($"Surgery: Finished drawing. Points collected: {currentContour.Count}");
        
        SurgeryEvaluator evaluator = GetComponent<SurgeryEvaluator>();
        if (evaluator != null)
        {
            evaluator.EvaluateContour(currentContour);
        }
    }

    private void SpawnPoint(Vector2 localPos)
    {
        GameObject p = pointPrefab != null ? Instantiate(pointPrefab, contourContainer.transform) : CreateDefaultPoint();
        p.transform.localPosition = localPos;
        if (p.GetComponent<Image>() != null)
            p.GetComponent<Image>().color = lineColor;
        
        visualPoints.Add(p);
    }

    private GameObject CreateDefaultPoint()
    {
        GameObject p = new GameObject("Point");
        p.AddComponent<Image>().color = lineColor;
        RectTransform rt = p.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(5, 5);
        return p;
    }

    private void ClearVisuals()
    {
        foreach (var p in visualPoints) Destroy(p);
        visualPoints.Clear();
    }

    public List<Vector2> GetCurrentContour() => currentContour;
}
