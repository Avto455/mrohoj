using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
// Assuming UILineRenderer is available from the installed UI Extensions package
// If the exact namespace differs in the installed version, it might need adjustment.
// Common namespaces for UI Extensions are 'UnityEngine.UI' or 'UIExtensions'.

public class SurgeryStrokeRenderer : MonoBehaviour
{
    [Header("Settings")]
    public Color lineColor = Color.red;
    public float thickness = 5f;
    
    private UILineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<UILineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<UILineRenderer>();
        }
        
        // Setup line properties
        lineRenderer.color = lineColor;
        lineRenderer.thickness = thickness;
    }

    public void AddPoint(Vector2 point)
    {
        lineRenderer.Points.Add(point);
    }

    public void ClearLine()
    {
        lineRenderer.Points.Clear();
    }
}
