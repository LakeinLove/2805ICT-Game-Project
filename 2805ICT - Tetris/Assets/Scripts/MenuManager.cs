using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //these are all UI elements
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

    //first thing done is to load the main menu
    private void Awake(){
        this.root = GetComponent<UIDocument>().rootVisualElement;
        LoadMainMenu();
       
    }
    private void LoadMainMenu(){
        //clears and loads ui elements from menuTemplate
        root.Clear();
        menuTemplate.CloneTree(root);
        //creates and subscribes to buttons
        this.playButton = root.Q<Button>("Play");
        this.settingsButton = root.Q<Button>("Settings");
        this.highScoreButton = root.Q<Button>("Score");
        this.exitButton = root.Q<Button>("Exit");

        playButton.clicked += PlayButtonClicked;
        settingsButton.clicked += SettingsButtonClicked;
        highScoreButton.clicked += ScoreButtonClicked;
        exitButton.clicked += ExitButtonClicked;
    }
    //unsubscribes the mainmenu buttons and the clears the root directory
    private void UnloadMainMenu(){
        playButton.clicked -= PlayButtonClicked;
        settingsButton.clicked -= SettingsButtonClicked;
        highScoreButton.clicked -= ScoreButtonClicked;
        exitButton.clicked -= ExitButtonClicked;
        root.Clear();
    }
    //unloads the main menu then loads the next scene with a Unity Class
    private void PlayButtonClicked(){
        UnloadMainMenu();
        SceneManager.LoadScene("Tetris");
    }
    //clears the root directory again, then copies over the settingsTemplate, and creates all the buttons and values
    private void SettingsButtonClicked(){
        //Reload UI from SettingsTemplate
        root.Clear();
        settingsTemplate.CloneTree(root);
        //query UI elements by name
        var returnButton = root.Q<Button>("ExitSettings");
        var boardSettings = root.Q<GroupBox>("GameBoardSize");
        var boardWidth = root.Q<SliderInt>("Width");
        var boardHeight = root.Q<SliderInt>("Height");
        var levelSelect = root.Q<DropdownField>("LevelSelect");
        var gameType = root.Q<RadioButtonGroup>("GameType");
        var playerSelect = root.Q<RadioButtonGroup>("AISelect");

        loadSettings();
        returnButton.clicked += ExitSettings;
        //loads all previously saved data from the PlayerPrefs file to the settings menu
        void loadSettings(){
            boardWidth.value = PrefsHelper.LoadInt("boardWidth", 10);
            boardHeight.value = PrefsHelper.LoadInt("boardHeight", 20);
            levelSelect.index = PrefsHelper.LoadInt("level");
            gameType.value = PrefsHelper.LoadInt("gameType");
            playerSelect.value = PrefsHelper.LoadInt("playerSelect");

        }
        //saves any modified settings to the playerprefs file
        void saveSettings(){
            PlayerPrefs.SetInt("boardWidth", boardWidth.value);
            PlayerPrefs.SetInt("boardHeight", boardHeight.value);
            PlayerPrefs.SetInt("level", levelSelect.index);
            PlayerPrefs.SetInt("gameType", gameType.value);
            PlayerPrefs.SetInt("playerSelect", playerSelect.value);
            PlayerPrefs.Save();
        }
        void ExitSettings(){
            saveSettings();
            LoadMainMenu();
        }
    }
    //doesn't do much at the moment, but will load the score template and then copy the top 10 highest scores from PlayerPrefs and display them
    private void ScoreButtonClicked(){
        root.Clear();
        scoreTemplate.CloneTree(root);
        var returnButton = root.Q<Button>("ExitScore");
        returnButton.clicked += LoadMainMenu;
    }
    //quits the entire program
    private void ExitButtonClicked(){
        Application.Quit();
    }
}
