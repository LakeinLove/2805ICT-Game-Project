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
    private VisualElement root;

    private void Awake(){
        this.root = GetComponent<UIDocument>().rootVisualElement;
        LoadMainMenu();
       
    }

    private void LoadMainMenu(){
        //root.Clear();
        menuTemplate.CloneTree(root);
        var playButton = root.Q<Button>("Play");
        var settingsButton = root.Q<Button>("Settings");
        var highScoreButton = root.Q<Button>("Score");
        var exitButton = root.Q<Button>("Exit");

        playButton.clicked += PlayButtonClicked;
        settingsButton.clicked += SettingsButtonClicked;
        highScoreButton.clicked += ScoreButtonClicked;
        exitButton.clicked += ExitButtonClicked;
    }

    private void PlayButtonClicked(){
        SceneManager.LoadScene("Tetris");
    }

    private void SettingsButtonClicked(){
        //root.Clear();
        settingsTemplate.CloneTree(root);
        var returnButtone = root.Q<Button>("Return");


    }

    private void ScoreButtonClicked(){

    }

    private void ExitButtonClicked(){
        Application.Quit();
    }

}