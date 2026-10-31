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
    public float pointSpacing = 0.1f; // Distance between points to spawn
    public Vector2 drawingOffset = Vector2.zero; // Offset to align drawing with image

    [Header("State")]
    private bool isDrawing = false;
    private List<Vector2> currentContour = new List<Vector2>();
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(patientRect, mousePos, canvas != null ? canvas.worldCamera : null, out Vector2 localPoint);

        // Apply offset for visual and logical alignment
        Vector2 adjustedPoint = localPoint + drawingOffset;

        if (currentContour.Count == 0 || Vector2.Distance(adjustedPoint, currentContour[currentContour.Count - 1]) > pointSpacing * 10f)
        {
            currentContour.Add(adjustedPoint);
            SpawnPoint(adjustedPoint);
        }
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
