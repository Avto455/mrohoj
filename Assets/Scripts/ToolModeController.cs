using UnityEngine;
using UnityEngine.UI;
using System;

public class ToolModeController : MonoBehaviour
{
    public enum ToolMode
    {
        None,
        XRay,
        Surgery
    }

    [Header("Current State")]
    public ToolMode currentMode = ToolMode.None;
    public bool patientExamined = false;

    [Header("UI References")]
    public GameObject patientImage;
    public GameObject xrayImage;
    public Button xrayButton;
    public Button scalpelButton;
    public Button exitButton;

    [Header("Positions")]
    public Vector2 centerPosition = Vector2.zero;
    public Vector2 leftPosition = new Vector2(-300, 0);

    public event Action<ToolMode> OnModeChanged;

    void Start()
    {
        UpdateUI();
        
        if (xrayButton != null) xrayButton.onClick.AddListener(() => SetMode(ToolMode.XRay));
        if (scalpelButton != null) scalpelButton.onClick.AddListener(() => SetMode(ToolMode.Surgery));
        if (exitButton != null) exitButton.onClick.AddListener(() => SetMode(ToolMode.None));
    }

    public void SetMode(ToolMode newMode)
    {
        // Toggle logic: if the same mode is selected, switch to None
        if (currentMode == newMode)
        {
            currentMode = ToolMode.None;
        }
        else
        {
            currentMode = newMode;
        }

        UpdateUI();
        OnModeChanged?.Invoke(currentMode);
    }

    private void UpdateUI()
    {
        // X-RAY logic
        if (currentMode == ToolMode.XRay)
        {
            patientExamined = true;
            if (patientImage != null) patientImage.GetComponent<RectTransform>().anchoredPosition = leftPosition;
            if (xrayImage != null) xrayImage.SetActive(true);
            SetButtonHighlight(xrayButton, true);
            SetButtonHighlight(scalpelButton, false);
        }
        else
        {
            if (xrayImage != null) xrayImage.SetActive(false);
            SetButtonHighlight(xrayButton, false);
        }

        // SURGERY logic
        if (currentMode == ToolMode.Surgery)
        {
            if (patientImage != null) patientImage.GetComponent<RectTransform>().anchoredPosition = centerPosition;
            if (xrayImage != null) xrayImage.SetActive(false);
            SetButtonHighlight(scalpelButton, true);
            SetButtonHighlight(xrayButton, false);
        }
        else
        {
            SetButtonHighlight(scalpelButton, false);
        }

        // NONE logic
        if (currentMode == ToolMode.None)
        {
            if (patientImage != null) patientImage.GetComponent<RectTransform>().anchoredPosition = centerPosition;
            if (xrayImage != null) xrayImage.SetActive(false);
            SetButtonHighlight(xrayButton, false);
            SetButtonHighlight(scalpelButton, false);
        }
    }

    private void SetButtonHighlight(Button btn, bool highlight)
    {
        if (btn == null) return;
        Color defaultColor = Color.white;
        Color highlightColor = Color.yellow;
        btn.GetComponent<Image>().color = highlight ? highlightColor : defaultColor;
    }
}
