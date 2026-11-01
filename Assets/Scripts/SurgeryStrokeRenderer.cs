
using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;

public class SurgeryStrokeRenderer : MonoBehaviour
{
    [Header("Settings")]
    public Color lineColor = Color.red;
    public float thickness = 5f;

    private UILineRenderer lineRenderer;
    private readonly List<Vector2> points = new List<Vector2>();

    void Awake()
    {
        lineRenderer = GetComponent<UILineRenderer>();

        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<UILineRenderer>();

        lineRenderer.color = lineColor;
        lineRenderer.LineThickness = thickness;

        ClearLine();
    }

    public void AddPoint(Vector2 point)
    {
        points.Add(point);
        UpdateLine();
    }

    public void ClearLine()
    {
        points.Clear();
        UpdateLine();
    }

    private void UpdateLine()
    {
        lineRenderer.Points = points.ToArray();
        lineRenderer.SetVerticesDirty();
    }
}
