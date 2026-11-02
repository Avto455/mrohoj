using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPatient", menuName = "Prosvet/PatientCase")]
public class PatientCase : ScriptableObject
{
    public string patientName;
    public string description;

    [Tooltip("Что рисуется на рентгене")]
    [FormerlySerializedAs("parasites")]
    public List<ParasiteTarget> xrayParasites = new List<ParasiteTarget>();

    [Tooltip("Зона для проверки скальпелем. Индекс = индекс паразита на рентгене")]
    public List<ParasiteTarget> scalpelTargets = new List<ParasiteTarget>();

    void OnValidate()
    {
        if (xrayParasites.Count != scalpelTargets.Count)
            Debug.LogWarning($"{name}: xrayParasites ({xrayParasites.Count}) и scalpelTargets ({scalpelTargets.Count}) разной длины", this);
    }
}