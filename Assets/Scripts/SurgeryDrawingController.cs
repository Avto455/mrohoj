using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SurgeryDrawingController : MonoBehaviour
{
    [Header("Settings")]
    public ToolModeController modeController;
    public RectTransform patientRect;
    public Color lineColor = Color.red;
    public float minDistanceBetweenPoints = 2f; 

    [Header("State")]
    private bool isDrawing = false;
    private List<Vector2> currentContour = new List<Vector2>(); // Normalised (0..1)
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
            // Interpolation to prevent gaps during fast movement
            if (localPointsCount() > 0)
            {
                Vector2 lastLocal = GetLastLocalPoint();
                float dist = Vector2.Distance(localPoint, lastLocal);
                if (dist > minDistanceBetweenPoints * 2f)
                {
                    int steps = Mathf.FloorToInt(dist / minDistanceBetweenPoints);
                    for (int i = 1; i < steps; i++)
                    {
                        Vector2 interpPoint = Vector2.Lerp(lastLocal, localPoint, (float)i / steps);
                        AddPoint(interpPoint);
                    }
                }
            }
            AddPoint(localPoint);
        }
    }

    private int localPointsCount()
    {
        // Since we store normalized points, we just return the count
        return currentContour.Count;
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

    private void AddPoint(Vector2 localPos)
    {
        Vector2 size = patientRect.rect.size;
        Vector2 normalizedPoint = new Vector2(
            (localPos.x / size.x) + 0.5f,
            (localPos.y / size.y) + 0.5f
        );
        currentContour.Add(normalizedPoint);
        SpawnPoint(localPos);
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
        GameObject p = new GameObject("Point");
        p.transform.SetParent(contourContainer.transform);
        p.AddComponent<Image>().color = lineColor;
        RectTransform rt = p.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(3, 3); // Smaller points for smoother look
        rt.localPosition = localPos;
        
        visualPoints.Add(p);
    }

    private void ClearVisuals()
    {
        foreach (var p in visualPoints) Destroy(p);
        visualPoints.Clear();
    }

    public List<Vector2> GetCurrentContour() => currentContour;
}
