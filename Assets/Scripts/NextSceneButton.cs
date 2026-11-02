using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class NextSceneButton : MonoBehaviour
{
    [Tooltip("Имена сцен (должны быть в Build Settings)")]
    public List<string> scenePool = new List<string>();
    public bool random = true;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(LoadNext);
    }

    void LoadNext()
    {
        if (scenePool.Count == 0) return;

        string current = SceneManager.GetActiveScene().name;
        string next;

        if (random)
        {
            // случайная сцена из пула, кроме текущей
            var options = scenePool.FindAll(s => s != current);
            next = options.Count > 0 ? options[Random.Range(0, options.Count)] : current;
        }
        else
        {
            // по порядку списка
            int i = scenePool.IndexOf(current);
            next = scenePool[(i + 1) % scenePool.Count];
        }

        SceneManager.LoadScene(next);
    }
}