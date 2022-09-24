using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;



public class HudManager : MonoBehaviour
{
    public static HudManager Instance;
    public UIDocument HUD;
    private VisualElement root;
    private VisualElement escapeMenu;

    private TextField score;
    private TextField lines;
    private TextField level;

    private Button resumeButton;
    private Button exitButton;

    private void Awake(){
                Instance = this;
        Debugger.Break();
        this.root = HUD.rootVisualElement;

        GameManager.OnStateChange += GameManagerStateChanged;
        this.escapeMenu = root.Q<VisualElement>("EscMenu");
        this.resumeButton = escapeMenu.Q<Button>("Resume");
        this.exitButton = escapeMenu.Q<Button>("Exit");
        this.score = root.Q<TextField>("Score");
        this.lines = root.Q<TextField>("Lines");
        this.level = root.Q<TextField>("Level");


        this.escapeMenu.visible = false;

        resumeButton.clicked += ResumeGame;
        exitButton.clicked += ExitGame;
    }

    private void GameManagerStateChanged(GameState state){
        escapeMenu.visible = (state == GameState.Paused);
        if (state == GameState.End){
            int currentHighScore = PrefsHelper.LoadInt("High Score");
            int playerScore = int.Parse(score.value);
            if(playerScore > currentHighScore){
                PlayerPrefs.SetInt("High Score", playerScore);
            }
            ExitGame();
        }
    }

    public void updateHUD(int s, int destroyed, int level){
        this.score.value = $"{s}";
        this.lines.value = $"{destroyed}";
        this.level.value = $"{level}";
    }

    private void ResumeGame(){
        GameManager.Instance.SetGameState(GameState.Playing);
    }

    private void ExitGame(){
        resumeButton.clicked -= ResumeGame;
        exitButton.clicked -= ExitGame;
        GameManager.OnStateChange -= GameManagerStateChanged;
        GameManager.Instance.SetGameState(GameState.Quit);
    }
}
