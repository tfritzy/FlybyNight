using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    public string DefaultSceneToLoad;
    public Image LoadingBar;
    [SerializeField]
    [Range(0, 1)]
    private float progressAnimationMultiplier = .25f;
    [SerializeField]
    [Range(0, 1)]
    private float startLoadDelay = .25f;
    private AsyncOperation loadingOperation;
    private string sceneName;
    private bool hasSetup = false;
    private float birthTime;

    void Start()
    {
        birthTime = Time.time;
    }

    private void Setup()
    {
        if (!string.IsNullOrEmpty(DefaultSceneToLoad))
        {
            LoadScene(DefaultSceneToLoad);
        }

        hasSetup = true;
    }

    public void LoadScene(string sceneName)
    {
        this.gameObject.SetActive(true);
        this.sceneName = sceneName;
        loadingOperation = SceneManager.LoadSceneAsync(this.sceneName);
        loadingOperation.allowSceneActivation = false;
        loadingOperation.priority = 1000;
    }

    float lerpedLoadingProgress = 0;
    void Update()
    {
        if (!hasSetup && Time.time > birthTime + startLoadDelay)
        {
            Setup();
            return;
        }

        if (loadingOperation != null)
        {
            lerpedLoadingProgress = Mathf.MoveTowards(lerpedLoadingProgress, loadingOperation.progress / .9f, progressAnimationMultiplier * Time.deltaTime);
            this.LoadingBar.fillAmount = lerpedLoadingProgress;

            if (Mathf.Approximately(lerpedLoadingProgress, 1))
            {
                loadingOperation.allowSceneActivation = true;
            }
        }
    }
}