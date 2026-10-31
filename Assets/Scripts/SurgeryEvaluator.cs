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

        // 1. Ensure the polygon is closed for the algorithm
        // The Point-in-Polygon algorithm usually handles the last-to-first edge,
        // but we'll make sure our data is clean.
        
        if (currentPatient == null) 
        { 
            Debug.Log("No patient case assigned!"); 
            return; 
        } 

        int found = 0; 
        foreach (var parasite in currentPatient.parasites) 
        { 
            if (IsPointInPolygon(parasite.center, points)) 
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
