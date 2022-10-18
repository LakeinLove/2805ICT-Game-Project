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
    private string previousImage;
    private VisualElement scoreMenu;
    private Button saveScore;
    private Button exitScore;
    private int currentScore = 0;

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
        var player = root.Q<RadioButtonGroup>("Player");
        player.value = PrefsHelper.LoadInt("playerSelect");

        //sets the escmenu invisible
        this.escapeMenu.visible = false;
        scoreMenu = root.Q<VisualElement>("HighScoreMenu");
        scoreMenu.visible = false;

        PrefsHelper.refreshList();
        //subscribes to the resume and exit button clicks
        resumeButton.clicked += ResumeGame;
        exitButton.clicked += ExitGame;
    }
    //checks the gamestate, if it is paused then escape menu is visible, if not it is invisible
    //also checks for game end and saves the high score for the score menu, then exits the game
    private void GameManagerStateChanged(GameState state){
        escapeMenu.visible = (state == GameState.Paused);
        if (state == GameState.End){
            if (currentScore > PrefsHelper.scoreList[9].score){
                scoreMenu.visible = true;
                saveScore = scoreMenu.Q<Button>("SaveButton");
                exitScore = scoreMenu.Q<Button>("ExitButton");
                saveScore.clicked += SaveGame;
                exitScore.clicked += ExitGame;
            }
            else{
                ExitGame();
            }
        }
    }
    //updates the HUD every frame as called by PlayManager, which has the information required
    public void updateHUD(int s, int destroyed, int level){
        this.currentScore = s;
        this.score.value = $"{s}";
        this.lines.value = $"{destroyed}";
        this.level.value = $"{level}";
    }

    private void ResumeGame(){
        GameManager.Instance.SetGameState(GameState.Playing);
    }

    private void SaveGame(){
        var field = scoreMenu.Q<TextField>("EnterName");
        var playerName = field.value;
        PrefsHelper.updateScores(currentScore, playerName);
        ExitGame();
    }
    //exits the game by unsubscribing all buttons then changing the gamestate
    private void ExitGame(){
        resumeButton.clicked -= ResumeGame;
        exitButton.clicked -= ExitGame;
        GameManager.OnStateChange -= GameManagerStateChanged;
        GameManager.Instance.SetGameState(GameState.Quit);
    }

    public void setNextPiece(Tetromino tet){
        
        var element = root.Q<MyImage>("nextImage");
        element.RemoveFromClassList(previousImage);
        previousImage = imageUSS[tet];
        element.AddToClassList(previousImage);
        
    }

    private static readonly Dictionary<Tetromino, string>imageUSS = new Dictionary<Tetromino, string>()
    {
        { Tetromino.I, "image-i" },
        { Tetromino.J, "image-j" },
        { Tetromino.L, "image-l" },
        { Tetromino.O, "image-o" },
        { Tetromino.S, "image-s" },
        { Tetromino.T, "image-t" },
        { Tetromino.Z, "image-z" },
        { Tetromino.C, "image-c" },
        {Tetromino.SL, "image-sl"},
    };
}