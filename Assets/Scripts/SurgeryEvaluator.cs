using UnityEngine;
using System.Collections.Generic;

public class SurgeryEvaluator : MonoBehaviour
{
    public void EvaluateContour(List<Vector2> points)
    {
        if (points == null || points.Count < 3)
        {
            Debug.Log("Surgery Evaluation: Contour too small to be a closed area.");
            return;
        }

        float area = CalculateArea(points);
        Debug.Log($"Surgery Evaluation: Calculated Area = {area}");
        
        // Future logic for comparing area with ParasiteTarget will go here
    }

    private float CalculateArea(List<Vector2> points)
    {
        // Shoelace formula (Gauss's area formula)
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
