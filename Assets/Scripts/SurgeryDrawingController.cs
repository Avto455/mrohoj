using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SurgeryDrawingController : MonoBehaviour
{
    [Header("Settings")]
    public ToolModeController modeController;
    public RectTransform patientRect;
    public GameObject pointPrefab; 
    public Color lineColor = Color.red;
    public float pointSpacing = 2f; 

    [Header("State")]
    private bool isDrawing = false;
    private List<Vector2> currentContour = new List<Vector2>(); // Normalised (0..1)
    private List<GameObject> visualPoints = new List<GameObject>();
    private GameObject contourContainer;

    void Start()
    {
        contourContainer = new GameObject("ContourContainer");
        contourContainer.transform.SetParent(patientRect);
        
        // Ensure the container is centered and fills the parent
        RectTransform containerRect = contourContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = Vector2.zero;
        
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
        if (Input.GetMouseButtonDown(0)) StartDrawing();
        else if (Input.GetMouseButton(0)) UpdateDrawing();
        else if (Input.GetMouseButtonUp(0)) StopDrawing();
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
        Debug.Log($"Surgery: Finished drawing. Points: {currentContour.Count}");
        
        SurgeryEvaluator evaluator = GetComponent<SurgeryEvaluator>();
        if (evaluator != null)
        {
            evaluator.EvaluateContour(currentContour);
        }
    }

    private void SpawnPoint(Vector2 localPos)
    {
        GameObject p;
        if (pointPrefab != null)
        {
            p = Instantiate(pointPrefab, contourContainer.transform);
        }
        else
        {
            p = new GameObject("Point");
            p.transform.SetParent(contourContainer.transform);
            p.AddComponent<Image>().color = lineColor;
            RectTransform rt = p.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(5, 5);
        }
        
        // IMPORTANT: Use anchoredPosition for UI elements
        RectTransform rtPoint = p.GetComponent<RectTransform>();
        if (rtPoint != null)
        {
            rtPoint.anchoredPosition = localPos;
            rtPoint.pivot = new Vector2(0.5f, 0.5f);
        }
        
        visualPoints.Add(p);
    }

    private void ClearVisuals()
    {
        foreach (var p in visualPoints) Destroy(p);
        visualPoints.Clear();
    }

    public List<Vector2> GetCurrentContour() => currentContour;
}
