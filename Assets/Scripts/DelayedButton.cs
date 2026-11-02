using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DelayedButton : MonoBehaviour
{
    public float delay = 3f;

    void OnEnable()
    {
        StartCoroutine(Unlock());
    }

    IEnumerator Unlock()
    {
        var btn = GetComponent<Button>();
        btn.interactable = false;
        yield return new WaitForSeconds(delay);
        btn.interactable = true;
    }
}