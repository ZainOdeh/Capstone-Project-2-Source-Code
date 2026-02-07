using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameMenuManager : MenuManagerPARENT {
    public RectTransform MainMenu;
    public RectTransform SettingsMenu;
    public RectTransform BackGround;
    public RectTransform WinMenu;
    public RectTransform LoseMenu;

    public TextMeshProUGUI GPA;
    public Image gradeImg;  // sprite that shows D / M / P
    public Sprite spriteD, spriteM, spriteP;

    private float[] gpaSteps = { 4.0f, 3.2f, 2.4f };
    private int currentStep = 0;


    public TextMeshProUGUI Boost;
    public TextMeshProUGUI boostLeft;

    public Slider sensSlider;
    public Slider volSlider;

    public bool canPause = true;
    public bool isPaused = false;

    public PlayerInput player;
    public AudioMixer mixer;
    private CameraControl camCtrl;

    public bool gameEnded;


    public static GameMenuManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start() {
        camCtrl = player.GetComponent<CameraControl>();
        ResetAllMenus();
        sensSlider.SetValueWithoutNotify(camCtrl.RotationSpeed);

        UpdateGPAUI();
    }


    public bool PlayerDead { get; private set; }
    public void ReduceGPA() {
        if (currentStep < gpaSteps.Length - 1) {
            currentStep++;
            UpdateGPAUI();
        } else {
            // reached 2.4 and got hit again > lose screen
            LoseGameVisual();
            PlayerDead = true;
        }
    }

    private void UpdateGPAUI() {
        GPA.text = gpaSteps[currentStep].ToString("0.0");

        gradeImg.sprite = currentStep switch {
            0 => spriteD,
            1 => spriteM,
            _ => spriteP,
        };
    }

    public void UpdateSensitivity() {
        camCtrl.RotationSpeed = sensSlider.value;
    }


    private Coroutine boosterRoutine;
    public void StartBooster(float durationSeconds) {
        // if a booster is already running, restart the timer
        if (boosterRoutine != null) StopCoroutine(boosterRoutine);
        boosterRoutine = StartCoroutine(BoosterCountdown(durationSeconds));
    }

    private IEnumerator BoosterCountdown(float duration) {
        Boost.gameObject.SetActive(true);

        float remaining = duration;
        while (remaining > 0f) {
            boostLeft.text = remaining.ToString("0.0");   // one decimal place
            yield return null;                            // wait 1 frame
            remaining -= Time.deltaTime;
        }

        // final update so we don't leave "0.1" showing for a frame
        boostLeft.text = "0.0";
        Boost.gameObject.SetActive(false);
        boosterRoutine = null;
    }

    public void UpdateVolume() {
        mixer.SetFloat("Volume", volSlider.value);
    }

    public void Pause() {
        ResetAllMenus();
        MainMenu.gameObject.SetActive(true);
        BackGround.gameObject.SetActive(true);
        PlayClickSound();
        StopTime();
    }

    public void Resume() {
        ResetAllMenus();
        PlayClickSound();
        ResumeTime();
    }

    public void WinGameVisuals() {
        ResetAllMenus();
        StopTime();
        WinMenu.gameObject.SetActive(true);
        player.UnlockCursor();
        gameEnded = true;
    }

    public void LoseGameVisual() {
        ResetAllMenus();
        StopTime();
        LoseMenu.gameObject.SetActive(true);
        player.UnlockCursor();

        gameEnded = true;
    }

    public void ResetGame() {
        ResumeTime();
        StartCoroutine(LoadGameAsync(MainGameSceneName));
    }

    public void GoToMainMenu() {
        ResumeTime();
        ResumeTime();
        ResumeTime();

        StartCoroutine(LoadGameAsync(MainMenuSceneName));
    }

    public void OpenMainMenu() {
        ResetAllMenus();
        MainMenu.gameObject.SetActive(true);
        BackGround.gameObject.SetActive(true);
    }
    public void GoBack() {
        MainMenu.gameObject.SetActive(true);
        SettingsMenu.gameObject.SetActive(false);
    }

    public void OpenSettingsMenu() {
        MainMenu.gameObject.SetActive(false);
        SettingsMenu.gameObject.SetActive(true);
    }

}
