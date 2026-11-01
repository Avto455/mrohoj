using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SurgeryInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    public ToolModeController modeController;
    public SurgeryStrokeRenderer strokeRenderer;
    public SurgeryEvaluator evaluator;
    public RectTransform drawingArea;

    [Header("Settings")]
    public float minDistanceBetweenPoints = 2f;
    public float closureDistance = 30f;

    private List<Vector2> currentStroke = new List<Vector2>();
    private bool isDrawing = false;

    void Start()
    {
        if (modeController == null) modeController = FindObjectOfType<ToolModeController>();
        if (strokeRenderer == null) strokeRenderer = GetComponentInChildren<SurgeryStrokeRenderer>();
        if (evaluator == null) evaluator = GetComponentInParent<SurgeryEvaluator>();
        if (drawingArea == null) drawingArea = GetComponent<RectTransform>();

        // Subscribe to mode changes to clear line when leaving Surgery mode
        if (modeController != null)
        {
            modeController.OnModeChanged += HandleModeChanged;
        }
    }

    private void HandleModeChanged(ToolModeController.ToolMode newMode)
    {
        if (newMode != ToolModeController.ToolMode.Surgery)
        {
            ClearCurrentWork();
        }
    }

    private void ClearCurrentWork()
    {
        isDrawing = false;
        currentStroke.Clear();
        if (strokeRenderer != null)
        {
            strokeRenderer.ClearLine();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (modeController == null || modeController.currentMode != ToolModeController.ToolMode.Surgery) return;

        isDrawing = true;
        currentStroke.Clear();
        if (strokeRenderer != null) strokeRenderer.ClearLine();

        AddPoint(eventData);
        Debug.Log("SurgeryInput: Started new stroke");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDrawing) return;

        AddPoint(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDrawing) return;
        isDrawing = false;

        if (currentStroke.Count < 3)
        {
            Debug.Log("SurgeryInput: Stroke too short to be a contour");
            if (strokeRenderer != null) strokeRenderer.ClearLine();
            return;
        }

        // Check for closure: if end is close to start, close it automatically
        Vector2 startPoint = currentStroke[0];
        Vector2 endPoint = currentStroke[currentStroke.Count - 1];
        if (Vector2.Distance(startPoint, endPoint) < closureDistance)
        {
            currentStroke.Add(startPoint);
            if (strokeRenderer != null) strokeRenderer.AddPoint(startPoint);
            Debug.Log("SurgeryInput: Contour automatically closed");
        }

        if (evaluator != null)
        {
            evaluator.EvaluateContour(currentStroke);
        }
    }

    private void AddPoint(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingArea, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            // Interpolation for smoothness
            if (currentStroke.Count > 0)
            {
                Vector2 lastLocal = GetLastLocalPoint();
                float dist = Vector2.Distance(localPoint, lastLocal);
                if (dist > minDistanceBetweenPoints * 2f)
                {
                    int steps = Mathf.FloorToInt(dist / minDistanceBetweenPoints);
                    for (int i = 1; i < steps; i++)
                    {
                        Vector2 interpPoint = Vector2.Lerp(lastLocal, localPoint, (float)i / steps);
                        PushPoint(interpPoint);
                    }
                }
            }
            PushPoint(localPoint);
        }
    }

    private Vector2 GetLastLocalPoint()
    {
        if (currentStroke.Count == 0) return Vector2.zero;
        Vector2 lastNorm = currentStroke[currentStroke.Count - 1];
        Vector2 size = drawingArea.rect.size;
        return new Vector2(
            (lastNorm.x - 0.5f) * size.x,
            (lastNorm.y - 0.5f) * size.y
        );
    }

    private void PushPoint(Vector2 point)
    {
        Vector2 size = drawingArea.rect.size;
        Vector2 normalizedPoint = new Vector2(
            (point.x / size.x) + 0.5f,
            (point.y / size.y) + 0.5f
        );
        currentStroke.Add(normalizedPoint);
        if (strokeRenderer != null)
        {
            strokeRenderer.AddPoint(point);
        }
    }

    private void OnDestroy()
    {
        if (modeController != null)
            modeController.OnModeChanged -= HandleModeChanged;
    }
}
