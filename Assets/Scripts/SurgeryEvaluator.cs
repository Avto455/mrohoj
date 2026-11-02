using UnityEngine;
using System.Collections.Generic;

public class SurgeryEvaluator : MonoBehaviour
{
    public PatientCase currentPatient;
    public ParasiteVisualizer parasiteVisualizer;

    [Header("Balance")]
    [Tooltip("¬о сколько раз вырезанна€ площадь может превышать площадь паразитов")]
    public float maxAreaRatio = 4f;
    [Tooltip("ћинимально допустима€ вырезанна€ площадь (дол€ картинки)")]
    public float minAllowedArea = 0.03f;

    public bool Overcut { get; private set; }
    public event System.Action OnOvercut;
    public event System.Action OnAllExtracted;

    private readonly HashSet<int> extracted = new HashSet<int>();
    private float totalCutArea;

    public bool AllParasitesExtracted()
    {
        // пациент без паразитов считаетс€ живым
        return currentPatient == null || extracted.Count >= currentPatient.scalpelTargets.Count;
    }

    public void EvaluateContour(List<Vector2> localPoints, RectTransform drawingArea)
    {
        if (Overcut) return; // пациент уже мЄртв

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

        //  онтур в нормализованных координатах 0..1
        List<Vector2> normalizedContour = new List<Vector2>();
        foreach (var p in localPoints)
            normalizedContour.Add(Rect.PointToNormalized(drawingArea.rect, p));

        // Ѕаланс: суммарна€ вырезанна€ площадь
        totalCutArea += PolygonArea(normalizedContour);
        Overcut = totalCutArea > AllowedArea();

        if (Overcut)
        {
            Debug.Log($"ѕациент скончалс€ на операции (area {totalCutArea:F3}/{AllowedArea():F3})");
            OnOvercut?.Invoke();
            return; // паразитов на смертельном разрезе не засчитываем
        }

        int found = 0;

        for (int i = 0; i < currentPatient.scalpelTargets.Count; i++)
        {
            if (extracted.Contains(i)) continue;

            if (IsParasiteCaptured(currentPatient.scalpelTargets[i], normalizedContour))
            {
                extracted.Add(i);
                found++;

                if (parasiteVisualizer != null)
                    parasiteVisualizer.MarkExtracted(i);
            }
        }

        if (found > 0 && AllParasitesExtracted())
            OnAllExtracted?.Invoke();

        Debug.Log($"Surgery Result: {found} captured, total extracted {extracted.Count}/{currentPatient.scalpelTargets.Count}, " +
                  $"area {totalCutArea:F3}/{AllowedArea():F3}");
    }

    private float AllowedArea()
    {
        float parasiteArea = 0f;
        foreach (var t in currentPatient.scalpelTargets)
            parasiteArea += Mathf.PI * t.radius * t.radius;
        return Mathf.Max(parasiteArea * maxAreaRatio, minAllowedArea);
    }

    private float PolygonArea(List<Vector2> poly)
    {
        float a = 0f;
        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            a += poly[j].x * poly[i].y - poly[i].x * poly[j].y;
        return Mathf.Abs(a) * 0.5f;
    }

    private bool IsParasiteCaptured(ParasiteTarget parasite, List<Vector2> polygon)
    {
        return IsPointInPolygon(parasite.center, polygon);
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
}