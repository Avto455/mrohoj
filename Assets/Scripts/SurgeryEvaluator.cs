using UnityEngine;
using System.Collections.Generic;

public class SurgeryEvaluator : MonoBehaviour
{
    public PatientCase currentPatient;
    public ParasiteVisualizer parasiteVisualizer;
    private readonly HashSet<int> extracted = new HashSet<int>();

    public void EvaluateContour(List<Vector2> localPoints, RectTransform drawingArea)
    {
        if (localPoints == null || localPoints.Count < 3)
        {
            Debug.Log("Surgery Evaluation: Contour too small.");
            return;
        }

        if (currentPatient == null)
        {
            Debug.Log("No patient case assigned!");
            return;
        }

        // 1. Convert the local points of the contour to normalized 0..1 coordinates
        Vector2 size = drawingArea.rect.size;
        //List<Vector2> normalizedContour = new List<Vector2>();
        //foreach (var p in localPoints)
        //{
        //    normalizedContour.Add(new Vector2((p.x / size.x) + 0.5f, (p.y / size.y) + 0.5f));
        //}
        List<Vector2> normalizedContour = new List<Vector2>();
        foreach (var p in localPoints)
            normalizedContour.Add(Rect.PointToNormalized(drawingArea.rect, p));

        int found = 0;

        for (int i = 0; i < currentPatient.scalpelTargets.Count; i++)
        {
            var parasite = currentPatient.scalpelTargets[i];

            if (IsParasiteCaptured(parasite, normalizedContour))
            {
                extracted.Add(i);   // <-- новая строка
                found++;

                if (parasiteVisualizer != null)
                    parasiteVisualizer.MarkExtracted(i);
            }
        }
        Debug.Log($"Surgery Result: {found} of {currentPatient.scalpelTargets.Count} parasites extracted!");
    }

    private bool IsParasiteCaptured(ParasiteTarget parasite, List<Vector2> polygon)
    {
        // Check if center is inside
        if (IsPointInPolygon(parasite.center, polygon)) return true;

        // Check if any edge of the contour is within the parasite's radius
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            if (DistancePointToSegment(parasite.center, polygon[j], polygon[i]) <= parasite.radius)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPointInPolygon(Vector2 point, List<Vector2> polygon)
    {
        bool isInside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
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
        return Vector2.Distance(p, a + t * ab);
    }

    public bool AllParasitesExtracted()
    {
        // пациент без паразитов считается живым
        return currentPatient == null || extracted.Count >= currentPatient.scalpelTargets.Count;
    }
}
