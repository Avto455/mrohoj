using UnityEngine;
using System.Collections.Generic;

public class SurgeryEvaluator : MonoBehaviour 
{ 
    public PatientCase currentPatient; 

    public void EvaluateContour(List<Vector2> points) 
    { 
        if (points == null || points.Count < 3) 
        { 
            Debug.Log("Surgery Evaluation: Contour too small."); 
            return; 
        } 

        if (currentPatient == null) 
        { 
            Debug.Log("No patient case assigned!"); 
            return; 
        } 

        // Convert local points to normalized 0..1 for comparison with PatientCase
        // We need a reference to the drawing area to do this
        RectTransform drawingArea = GetComponent<RectTransform>();
        if (drawingArea == null)
        {
            Debug.LogError("SurgeryEvaluator must be on a RectTransform (Drawing Area)!");
            return;
        }

        Vector2 size = drawingArea.rect.size;
        List<Vector2> normalizedPoints = new List<Vector2>();
        foreach (var p in points)
        {
            normalizedPoints.Add(new Vector2((p.x / size.x) + 0.5f, (p.y / size.y) + 0.5f));
        }

        int found = 0; 
        foreach (var parasite in currentPatient.parasites) 
        { 
            if (IsPointInPolygon(parasite.center, normalizedPoints)) 
            {
                found++; 
            }
        } 
        Debug.Log($"Surgery Result: {found} of {currentPatient.parasites.Count} parasites extracted!"); 
    } 

    private bool IsPointInPolygon(Vector2 point, List<Vector2> polygon) 
    { 
        bool isInside = false; 
        int count = polygon.Count;
        for (int i = 0, j = count - 1; i < count; j = i++) 
        { 
            if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) && 
                (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x)) 
            {
                isInside = !isInside; 
            }
        } 
        return isInside; 
    } 
}
