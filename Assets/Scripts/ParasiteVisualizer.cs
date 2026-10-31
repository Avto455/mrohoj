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

    void Start()
    {
        visualContainer = new GameObject("ParasiteVisuals");
        visualContainer.transform.SetParent(xrayRect);
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
        foreach (var parasite in currentPatient.parasites)
        {
            GameObject p = parasitePrefab != null ? Instantiate(parasitePrefab, visualContainer.transform) : CreateDefaultCircle();
            p.transform.localPosition = parasite.center;
            
            // Scale the circle based on radius
            RectTransform rt = p.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(parasite.radius * 2, parasite.radius * 2);

            if (p.GetComponent<Image>() != null)
                p.GetComponent<Image>().color = parasiteColor;

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
        GameObject p = new GameObject("ParasiteMarker");
        p.AddComponent<Image>().color = parasiteColor;
        RectTransform rt = p.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(10, 10);
        return p;
    }

    private void OnDestroy()
    {
        if (modeController != null)
            modeController.OnModeChanged -= HandleModeChanged;
    }
}
