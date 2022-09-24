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
        var boardSettings = root.Q<GroupBox>("GameBoardSize");
        var boardWidth = root.Q<SliderInt>("Width");
        var boardHeight = root.Q<SliderInt>("Height");
        var levelSelect = root.Q<DropdownField>("LevelSelect");
        var gameType = root.Q<RadioButtonGroup>("GameType");
        var playerSelect = root.Q<RadioButtonGroup>("AISelect");

        loadSettings();
        returnButton.clicked += ExitSettings;

        void loadSettings(){
            boardWidth.value = loadInt("boardWidth", 10);
            boardHeight.value = loadInt("boardHeight", 20);
            levelSelect.index = loadInt("level");
            gameType.value = loadInt("gameType");
            playerSelect.value = loadInt("playerSelect");

        }
        int loadInt(string key, int @default = 0) {
            if (PlayerPrefs.HasKey(key)){
                return PlayerPrefs.GetInt(key);
            }
            return @default;
        }
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
