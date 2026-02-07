using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MenuManagerPARENT
{

    public RectTransform MainMenu;
    public RectTransform SettingsMenu;
    public RectTransform CreditsMenu;

    public RectTransform previousMenu;
    public RectTransform activeMenu;

    private void Start() {
        Initalize();
    }

    public void StartGame() {
        StartCoroutine(LoadGameAsync(MainGameSceneName));
    }

    public void StartExplorer() {
        StartCoroutine(LoadGameAsync(ExploreSceneName));
    }

    public void OpenSettingsMenu() {
        previousMenu = activeMenu;
        previousMenu.gameObject.SetActive(false);
        activeMenu = SettingsMenu;
        SettingsMenu.gameObject.SetActive(true);
    }

    public void OpenCreditsMenu() {
        previousMenu = activeMenu;
        previousMenu.gameObject.SetActive(false);
        activeMenu = CreditsMenu;
        CreditsMenu.gameObject.SetActive(true);
    }

    public void GoBack() {
        previousMenu.gameObject.SetActive(true);
        activeMenu.gameObject.SetActive(false);
        activeMenu = MainMenu;
    }

    public void Initalize() {
        MainMenu = allMenus[0];
        SettingsMenu = allMenus[1];
        CreditsMenu = allMenus[2];
        ResetAllMenus();
        activeMenu = MainMenu;
        MainMenu.gameObject.SetActive(true);
    }

   
}

