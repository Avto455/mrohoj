using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ParasiteVisualizer : MonoBehaviour
{
    [Header("Settings")]
    public ToolModeController modeController;
    public RectTransform xrayRect;
    public PatientCase currentPatient;
    public GameObject parasitePrefab; // Small white circle UI Image
    public Color parasiteColor = new Color(1f, 1f, 1f, 0.5f);

    private List<GameObject> visualParasites = new List<GameObject>();
    private GameObject visualContainer;
    private HashSet<int> extractedParasites = new HashSet<int>();

    void Start()
    {
        visualContainer = new GameObject("ParasiteVisuals", typeof(RectTransform));
        var crt = (RectTransform)visualContainer.transform;
        crt.SetParent(xrayRect, false);
        crt.anchorMin = Vector2.zero;
        crt.anchorMax = Vector2.one;
        crt.offsetMin = crt.offsetMax = Vector2.zero;
        crt.localScale = Vector3.one;
        visualContainer.SetActive(false);

        if (modeController == null)
            modeController = FindObjectOfType<ToolModeController>();

        modeController.OnModeChanged += HandleModeChanged;
    }

    private void HandleModeChanged(ToolModeController.ToolMode newMode)
    {
        if (newMode == ToolModeController.ToolMode.XRay)
        {
            ShowParasites();
        }
        else
        {
            HideParasites();
        }
    }

    private void ShowParasites()
    {
        ClearParasites();
        if (currentPatient == null) return;

        visualContainer.SetActive(true);
        for (int i = 0; i < currentPatient.xrayParasites.Count; i++)
        {
            if (extractedParasites.Contains(i))
            {
                visualParasites.Add(null);
                continue;
            }

            // внутри ShowParasites, вместо localPosition / sizeDelta:
            var parasite = currentPatient.xrayParasites[i];

            GameObject p = parasitePrefab != null
                ? Instantiate(parasitePrefab, visualContainer.transform, false)
                : CreateDefaultCircle();

            RectTransform rt = p.GetComponent<RectTransform>();
            rt.anchorMin = parasite.center - Vector2.one * parasite.radius;
            rt.anchorMax = parasite.center + Vector2.one * parasite.radius;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;

            var img = p.GetComponent<Image>();
            if (img != null) img.color = parasiteColor;

            visualParasites.Add(p);
        }
        Debug.Log($"ParasiteVisualizer: Displayed {visualParasites.Count} parasites");
    }

    private void HideParasites()
    {
        visualContainer.SetActive(false);
    }

    private void ClearParasites()
    {
        foreach (var p in visualParasites) Destroy(p);
        visualParasites.Clear();
    }

    private GameObject CreateDefaultCircle()
    {
        var p = new GameObject("ParasiteMarker", typeof(RectTransform), typeof(Image));
        p.transform.SetParent(visualContainer.transform, false);
        return p;
    }

    private void OnDestroy()
    {
        if (modeController != null)
            modeController.OnModeChanged -= HandleModeChanged;
    }

    public void MarkExtracted(int index)
    {
        if (index < 0 || index >= visualParasites.Count)
            return;

        extractedParasites.Add(index);

        if (visualParasites[index] != null)
            visualParasites[index].SetActive(false);

        Debug.Log($"ParasiteVisualizer: Parasite {index} extracted and hidden.");
    }
}
