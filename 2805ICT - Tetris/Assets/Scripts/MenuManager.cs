using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private UIDocument doc;
    private Button playButton;
    private Button settingsButton;
    private Button highScoreButton;
    private Button exitButton;
    private void Awake(){
        this.doc = GetComponent<UIDocument>();
        this.playButton = doc.rootVisualElement.Q<Button>("Play");
        this.settingsButton = doc.rootVisualElement.Q<Button>("Settings");
        this.highScoreButton = doc.rootVisualElement.Q<Button>("Score");
        this.exitButton = doc.rootVisualElement.Q<Button>("Exit");

        this.playButton.clicked += PlayButtonClicked;
        this.settingsButton.clicked += SettingsButtonClicked;
        this.highScoreButton.clicked += ScoreButtonClicked;
        this.exitButton.clicked += ExitButtonClicked;
    }

    private void PlayButtonClicked(){
        SceneManager.LoadScene("Tetris");
    }

    private void SettingsButtonClicked(){

    }

    private void ScoreButtonClicked(){

    }

    private void ExitButtonClicked(){
        Application.Quit();
    }

}
