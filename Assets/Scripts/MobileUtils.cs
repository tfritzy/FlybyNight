using UnityEngine;
using System.Collections;

public class MobileUtils : MonoBehaviour
{
    private int FramesPerSec;
    private float frequency = 1.0f;
    private string fps;
    private GUIStyle style = new GUIStyle();

    void Start()
    {
        style.fontSize = 50;
        style.normal.textColor = Color.white;
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        for (; ; )
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it
            fps = string.Format("FPS: {0}", Mathf.RoundToInt(frameCount / timeSpan));
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 300, 10, 250, 40), fps, style);
    }
}