using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManagerPARENT : MonoBehaviour
{
    public RectTransform[] allMenus;
    public AudioSource ClickSound;

    public CanvasGroup loadingScreen;
    public Slider LoadingProgress;
    public float minDisplayTime = 5f;         // seconds the loading screen must stay visible
    public float fadeDuration = 0.25f;        // fade-in/out time
    float elapsed = 0f;

    public string MainGameSceneName = "MainGame";
    public string ExploreSceneName = "Explore";
    public string MainMenuSceneName = "MainMenu";
    public string OrangeVillageSceneName = "OrangeVillage";

    public void PlayClickSound() {
        ClickSound.Play();
    }

    public void ExitGame() {
        Debug.Log("QuittingGame");
        Application.Quit();
    }

    public void ResetAllMenus() {
        foreach (var item in allMenus) {
            item.gameObject.SetActive(false);
        }
    }

    public IEnumerator LoadGameAsync(string gameplaySceneName) {
        // 1) Show / fade in the loading screen
        ResetAllMenus();
        loadingScreen.gameObject.SetActive(true);
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime) {
            loadingScreen.alpha = t / fadeDuration;
            yield return null;
        }
        loadingScreen.alpha = 1;

        // 2) Begin async loading in the background
        AsyncOperation op = SceneManager.LoadSceneAsync(gameplaySceneName);
        op.allowSceneActivation = false;         // we’ll activate manually

        // 3) Optional progress bar / spinner update loop
        while (op.progress < 0.9f || elapsed < minDisplayTime)               // 0 – 0.9 is “loading”, then it waits
        {
            elapsed += Time.unscaledDeltaTime;

            float realProgress = Mathf.Clamp01(op.progress / 0.9f);
            float fakeProgress = Mathf.Clamp01(elapsed / minDisplayTime);
            float shownProgress = Mathf.Min(realProgress, fakeProgress);

            LoadingProgress.value = shownProgress;
            yield return null;
        }

        // 4) Small extra delay
        yield return new WaitForSecondsRealtime(0.3f);

        // 5) Let Unity switch scenes (one frame later)
        op.allowSceneActivation = true;
    }


    public void StopTime() {
        Time.timeScale = 0;
    }

    public void ResumeTime() {
        Time.timeScale = 1f;
    }

}
