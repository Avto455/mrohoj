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
    public float closureDistance = 50f; // Increased for easier closing

    private List<Vector2> currentStroke = new List<Vector2>();
    private bool isDrawing = false;
    private Vector2 lastAddedLocalPoint;

    void Start()
    {
        if (modeController == null) modeController = FindObjectOfType<ToolModeController>();
        if (strokeRenderer == null) strokeRenderer = GetComponentInChildren<SurgeryStrokeRenderer>();
        if (evaluator == null) evaluator = GetComponentInParent<SurgeryEvaluator>();
        if (// Ensure drawing area is assigned
            drawingArea == null) drawingArea = GetComponent<RectTransform>();

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

        // Get initial point
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingArea, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            PushPoint(localPoint);
            lastAddedLocalPoint = localPoint;
        }
        Debug.Log("SurgeryInput: Started new stroke");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDrawing) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingArea, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            // Interpolation for smoothness
            float dist = Vector2.Distance(localPoint, lastAddedLocalPoint);
            if (dist > minDistanceBetweenPoints * 2f)
            {
                int steps = Mathf.FloorToInt(dist / minDistanceBetweenPoints);
                for (int i = 1; i < steps; i++)
                {
                    Vector2 interpPoint = Vector2.Lerp(lastAddedLocalPoint, localPoint, (float)i / steps);
                    PushPoint(interpPoint);
                }
            }
            PushPoint(localPoint);
            lastAddedLocalPoint = localPoint;
        }
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

        // FORCED CLOSURE: For the Point-in-Polygon to work, the loop MUST be closed.
        // We add the first point to the end of the list.
        currentStroke.Add(currentStroke[0]);
        if (strokeRenderer != null)
        {
            // Visual closure
            Vector2 size = drawingArea.rect.size;
            Vector2 firstLocal = new Vector2(
                (currentStroke[0].x - 0.5f) * size.x,
                (currentStroke[0].y - 0.5f) * size.y
            );
            strokeRenderer.AddPoint(firstLocal);
        }
        
        Debug.Log("SurgeryInput: Contour closed and sent to evaluator");

        if (evaluator != null)
        {
            evaluator.EvaluateContour(currentStroke);
        }
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
