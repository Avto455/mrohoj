using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PatientSpriteSwitcher : MonoBehaviour
{
    public SurgeryEvaluator evaluator;

    [Header("Sprites")]
    public Sprite normalSprite;   // обычный пациент
    public Sprite suturedSprite;  // со швом (после извлечения)
    public Sprite deadSprite;     // мёртвый

    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        if (normalSprite != null) img.sprite = normalSprite;
    }

    void OnEnable()
    {
        evaluator.OnAllExtracted += Sutured;
        evaluator.OnOvercut += Dead;
    }

    void OnDisable()
    {
        evaluator.OnAllExtracted -= Sutured;
        evaluator.OnOvercut -= Dead;
    }

    void Sutured()
    {
        if (suturedSprite != null) img.sprite = suturedSprite;
    }

    void Dead()
    {
        if (deadSprite != null) img.sprite = deadSprite;
    }
}