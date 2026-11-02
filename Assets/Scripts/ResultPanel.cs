using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultPanel : MonoBehaviour
{
    public SurgeryEvaluator evaluator;
    public Button exitButton;
    public GameObject panel;
    public TMP_Text resultText;

    public string aliveText = "Пациент выжил";
    public string missedText = "Пациент умер: паразит остался внутри";
    public string diedOnTableText = "Пациент скончался на операции";
    public float deathDelay = 1f; // пауза, чтобы игрок увидел последний разрез

    private bool shown;

    void Start()
    {
        panel.SetActive(false);
        exitButton.onClick.AddListener(ShowResult);
        evaluator.OnOvercut += OnDiedOnTable;
    }

    void OnDiedOnTable()
    {
        Invoke(nameof(ShowDeath), deathDelay);
    }

    void ShowDeath() => Show(diedOnTableText);

    void ShowResult()
    {
        if (shown) return;
        Show(evaluator.AllParasitesExtracted() ? aliveText : missedText);
    }

    void Show(string text)
    {
        if (shown) return;
        shown = true;
        resultText.text = text;
        panel.SetActive(true);
    }

    void OnDestroy()
    {
        if (evaluator != null) evaluator.OnOvercut -= OnDiedOnTable;
    }
}