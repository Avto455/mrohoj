using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultPanel : MonoBehaviour
{
    public SurgeryEvaluator evaluator;
    public Button exitButton;
    public Button nextButton;
    public GameObject panel;
    public TMP_Text resultText;

    public string aliveText = "Пациент жив";
    public string deadText = "Пациент умер";

    void Start()
    {
        panel.SetActive(false);
        exitButton.onClick.AddListener(ShowResult);
        nextButton.onClick.AddListener(LoadNext);
    }

    void ShowResult()
    {
        resultText.text = evaluator.AllParasitesExtracted() ? aliveText : deadText;
        panel.SetActive(true);
    }

    void LoadNext()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next >= SceneManager.sceneCountInBuildSettings) next = 0; // после последней - снова первая
        SceneManager.LoadScene(next);
    }
}