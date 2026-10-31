using UnityEngine;
using UnityEngine.UI;

public class GameInitializer : MonoBehaviour
{
    void Awake()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Patient Image
        GameObject patient = new GameObject("PatientImage");
        patient.transform.SetParent(canvasObj.transform);
        Image pImg = patient.AddComponent<Image>();
        pImg.color = Color.gray;
        RectTransform pRect = patient.GetComponent<RectTransform>();
        pRect.sizeDelta = new Vector2(300, 500);
        pRect.anchoredPosition = Vector2.zero;

        // Create XRay Image
        GameObject xray = new GameObject("XRayImage");
        xray.transform.SetParent(canvasObj.transform);
        Image xImg = xray.AddComponent<Image>();
        xImg.color = Color.black;
        xray.SetActive(false);
        RectTransform xRect = xray.GetComponent<RectTransform>();
        xRect.sizeDelta = new Vector2(300, 500);
        xRect.anchoredPosition = Vector2.zero;

        // Create Buttons Panel
        GameObject panel = new GameObject("ButtonsPanel");
        panel.transform.SetParent(canvasObj.transform);
        RectTransform panRect = panel.GetComponent<RectTransform>();
        panRect.anchorMin = new Vector2(1, 0.5f);
        panRect.anchorMax = new Vector2(1, 0.5f);
        panRect.pivot = new Vector2(1, 0.5f);
        panRect.sizeDelta = new Vector2(200, 600);

        // Create XRay Button
        GameObject btnX = CreateButton("Btn_XRay", panel.transform, new Vector2(0, 100));
        // Create Scalpel Button
        GameObject btnS = CreateButton("Btn_Scalpel", panel.transform, new Vector2(0, 0));
        // Create Exit Button
        GameObject btnE = CreateButton("Btn_Exit", panel.transform, new Vector2(0, -100));

        // Setup Controller
        GameObject ctrlObj = new GameObject("ToolModeController");
        ToolModeController ctrl = ctrlObj.AddComponent<ToolModeController>();
        ctrl.patientImage = patient;
        ctrl.xrayImage = xray;
        ctrl.xrayButton = btnX.GetComponent<Button>();
        ctrl.scalpelButton = btnS.GetComponent<Button>();
        ctrl.exitButton = btnE.GetComponent<Button>();
    }

    GameObject CreateButton(string name, Transform parent, Vector2 pos)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent);
        Image img = btnObj.AddComponent<Image>();
        img.color = Color.white;
        Button btn = btnObj.AddComponent<Button>();
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 50);
        rect.anchoredPosition = pos;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform);
        Text txt = textObj.AddComponent<Text>();
        txt.text = name.Replace("Btn_", "");
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.black;
        RectTransform tRect = textObj.GetComponent<RectTransform>();
        tRect.anchorMin = Vector2.zero;
        tRect.anchorMax = Vector2.one;
        tRect.sizeDelta = Vector2.zero;
        
        return btnObj;
    }
}
