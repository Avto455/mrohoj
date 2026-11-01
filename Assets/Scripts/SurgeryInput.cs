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
    public RectTransform patientRect;

    [Header("Settings")]
    public float minDistanceBetweenPoints = 5f;

    private List<Vector2> currentStroke = new List<Vector2>();
    private bool isDrawing = false;

    void Start()
    {
        if (modeController == null) modeController = FindObjectOfType<ToolModeController>();
        if (strokeRenderer == null) strokeRenderer = GetComponentInChildren<SurgeryStrokeRenderer>();
        if (evaluator == null) evaluator = GetComponentInParent<SurgeryEvaluator>();
        if (patientRect == null) patientRect = GetComponent<RectTransform>();

        if (modeController != null) modeController.OnModeChanged += HandleModeChanged;
    }

    private void HandleModeChanged(ToolModeController.ToolMode newMode)
    {
        if (newMode != ToolModeController.ToolMode.Surgery) ClearCurrentWork();
    }

    private void ClearCurrentWork()
    {
        isDrawing = false;
        currentStroke.Clear();
        if (strokeRenderer != null) strokeRenderer.ClearLine();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (modeController == null || modeController.currentMode != ToolModeController.ToolMode.Surgery) return;

        isDrawing = true;
        currentStroke.Clear();
        if (strokeRenderer != null) strokeRenderer.ClearLine();
        
        AddPoint(eventData);
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
            if (strokeRenderer != null) strokeRenderer.ClearLine();
            return;
        }

        // Force closure for mathematical consistency
        currentStroke.Add(currentStroke[0]);
        if (strokeRenderer != null) strokeRenderer.AddPoint(currentStroke[0]);

        if (evaluator != null) evaluator.EvaluateContour(currentStroke, patientRect);
    }

    private void AddPoint(PointerEventData eventData)
    {
        // Use Screen Point to Local’s absolute result
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(patientRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            if (currentStroke.Count > 0)
            {
                float dist = Vector2.Distance(localPoint, currentStroke[currentStroke.Count - 1]);
                if (dist < minDistanceBetweenPoints) return;
            }
            currentStroke.Add(localPoint);
            if (strokeRenderer != null) strokeRenderer.AddPoint(localPoint);
        }
    }

    private void OnDestroy()
    {
        if (modeController != null) modeController.OnModeChanged -= HandleModeChanged;
    }
}
