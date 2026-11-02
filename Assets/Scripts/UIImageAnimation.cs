using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimation : MonoBehaviour
{
    public Image image;
    public Sprite[] frames;
    public float fps = 12f;

    private int currentFrame;
    private float timer;

    void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / fps)
        {
            timer = 0;
            currentFrame = (currentFrame + 1) % frames.Length;
            image.sprite = frames[currentFrame];
        }
    }
}