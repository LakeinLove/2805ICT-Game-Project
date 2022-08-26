using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField]
    private VisualTreeAsset menuTemplate;
    [SerializeField]
    private VisualTreeAsset settingsTemplate;
    [SerializeField]
    private VisualTreeAsset scoreTemplate;
    private VisualElement root;
    private Button playButton;
    private Button settingsButton;
    private Button highScoreButton;
    private Button exitButton;

    private void Awake(){
        this.root = GetComponent<UIDocument>().rootVisualElement;
        LoadMainMenu();
       
    }

    private void LoadMainMenu(){
        root.Clear();
        menuTemplate.CloneTree(root);
        this.playButton = root.Q<Button>("Play");
        this.settingsButton = root.Q<Button>("Settings");
        this.highScoreButton = root.Q<Button>("Score");
        this.exitButton = root.Q<Button>("Exit");

        playButton.clicked += PlayButtonClicked;
        settingsButton.clicked += SettingsButtonClicked;
        highScoreButton.clicked += ScoreButtonClicked;
        exitButton.clicked += ExitButtonClicked;
    }

    private void UnloadMainMenu(){
        playButton.clicked -= PlayButtonClicked;
        settingsButton.clicked -= SettingsButtonClicked;
        highScoreButton.clicked -= ScoreButtonClicked;
        exitButton.clicked -= ExitButtonClicked;
        root.Clear();
    }

    private void PlayButtonClicked(){
        UnloadMainMenu();
        SceneManager.LoadScene("Tetris");
    }

    private void SettingsButtonClicked(){
        root.Clear();
        settingsTemplate.CloneTree(root);
        var returnButton = root.Q<Button>("ExitSettings");


        returnButton.clicked += LoadMainMenu;


    }

    private void ScoreButtonClicked(){
        root.Clear();
        scoreTemplate.CloneTree(root);
        var returnButton = root.Q<Button>("ExitScore");


        returnButton.clicked += LoadMainMenu;

    }

    private void ExitButtonClicked(){
        Application.Quit();
    }

}
