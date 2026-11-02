using UnityEngine;

public class HideForeverOnXRay : MonoBehaviour
{
    public ToolModeController modeController;

    void Start()
    {
        if (modeController == null)
            modeController = FindObjectOfType<ToolModeController>();

        modeController.OnModeChanged += OnModeChanged;
    }

    void OnModeChanged(ToolModeController.ToolMode mode)
    {
        if (mode == ToolModeController.ToolMode.XRay || mode == ToolModeController.ToolMode.Surgery)
            gameObject.SetActive(false); // навсегда
    }

    void OnDestroy()
    {
        if (modeController != null)
            modeController.OnModeChanged -= OnModeChanged;
    }
}