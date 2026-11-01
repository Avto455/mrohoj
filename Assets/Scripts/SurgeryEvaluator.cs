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

        float area = CalculateNormalisedArea(points); 
        Debug.Log("Surgery Evaluation: Normalised Area = " + area); 

        if (currentPatient == null) 
        { 
            Debug.Log("No patient case assigned!"); 
            return; 
        } 

        int found = 0; 
        foreach (var parasite in currentPatient.parasites) 
        { 
            if (IsParasiteCaptured(parasite, points)) 
            {
                found++; 
            }
        } 
        Debug.Log($"Surgery Result: {found} of {currentPatient.parasites.Count} parasites extracted!"); 
    } 

    private bool IsParasiteCaptured(ParasiteTarget parasite, List<Vector2> polygon)
    {
        // 1. Check if the center is inside the polygon (Standard Point-in-Polygon)
        if (IsPointInPolygon(parasite.center, polygon))
        {
            return true;
        }

        // 2. "Smart Capture": Check if any part of the parasite's radius intersects the contour
        // We check if the distance from the center to any segment of the polygon is less than the radius.
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            if (DistancePointToSegment(parasite.center, polygon[j], polygon[i]) <= parasite.radius)
            {
                return true; // The contour line actually cuts through or touches the parasite
            }
        }

        return false;
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

    private float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        Vector2 ap = p - a;
        float lengthSq = ab.sqrMagnitude;
        if (lengthSq == 0) return Vector2.Distance(p, a);
        
        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / lengthSq);
        Vector2 projection = a + t * ab;
        return Vector2.Distance(p, projection);
    }

    private float CalculateNormalisedArea(List<Vector2> points) 
    { 
        float area = 0; 
        int j = points.Count - 1; 
        for (int i = 0; i < points.Count; i++) 
        { 
            area += (points[j].x + points[i].x) * (points[j].y - points[i].y); 
            j = i; 
        } 
        return Mathf.Abs(area / 2f); 
    } 
}
