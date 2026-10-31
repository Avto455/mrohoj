using UnityEngine;
using System.Collections.Generic;

//[System.Serializable]
[CreateAssetMenu(fileName = "NewPatient", menuName = "Prosvet/PatientCase")]
[System.Serializable]
public class PatientCase : ScriptableObject 
{ 
    public string patientName; 
    public string description; 
    public List<ParasiteTarget> parasites = new List<ParasiteTarget>(); 
}
