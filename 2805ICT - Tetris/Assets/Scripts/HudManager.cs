using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;



public class HudManager : MonoBehaviour
{
    //singleton instance for managing the in-game HUD
    public static HudManager Instance;
    //all of these are UI elements
    public UIDocument HUD;
    private VisualElement root;
    private VisualElement escapeMenu;
    private TextField score;
    private TextField lines;
    private TextField level;
    private Button resumeButton;
    private Button exitButton;

    private void Awake(){
        //sets singleton instance
        Instance = this;
        this.root = HUD.rootVisualElement;
        //subscribes to the onStateChange action from Game Manager
        GameManager.OnStateChange += GameManagerStateChanged;
        //loads all buttons
        this.escapeMenu = root.Q<VisualElement>("EscMenu");
        this.resumeButton = escapeMenu.Q<Button>("Resume");
        this.exitButton = escapeMenu.Q<Button>("Exit");
        this.score = root.Q<TextField>("Score");
        this.lines = root.Q<TextField>("Lines");
        this.level = root.Q<TextField>("Level");

        //sets the escmenu invisible
        this.escapeMenu.visible = false;
        //subscribes to the resume and exit button clicks
        resumeButton.clicked += ResumeGame;
        exitButton.clicked += ExitGame;
    }
    //checks the gamestate, if it is paused then escape menu is visible, if not it is invisible
    //also checks for game end and saves the high score for the score menu, then exits the game
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
    //updates the HUD every frame as called by PlayManager, which has the information required
    public void updateHUD(int s, int destroyed, int level){
        this.score.value = $"{s}";
        this.lines.value = $"{destroyed}";
        this.level.value = $"{level}";
    }

    private void ResumeGame(){
        GameManager.Instance.SetGameState(GameState.Playing);
    }
    //exits the game by unsubscribing all buttons then changing the gamestate
    private void ExitGame(){
        resumeButton.clicked -= ResumeGame;
        exitButton.clicked -= ExitGame;
        GameManager.OnStateChange -= GameManagerStateChanged;
        GameManager.Instance.SetGameState(GameState.Quit);
    }
}
