using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    //singleton that managed the moment to moment gameplay
    public static PlayManager Instance;
    public Gameboard board;
    public Piece activePiece {get; set;}
    //capitalised to allow use of level later with no issues
    public int level;
    private int levelCap;
    private int levelReq;
    public int Score;
    public int LinesCleared;
    public float stepDelay;
    public float lockDelay;
    public float lockTime;
    public float stepTime;
    public bool extrominos;

    //set the singleton instance, then load all information either from the game preferences or from standard tetris ideals
    void Awake(){
        Instance = this;
        level = PrefsHelper.LoadInt("level", 0);
        extrominos = (PrefsHelper.LoadInt("gameType", 0) != 0);
        levelCap = 10;
        levelReq = 10 * (1 + level);
        Score = 0;
        LinesCleared = 0;
        stepDelay = (2f - level * 0.18f);
        lockDelay = 0.5f;
            
    }

    void Start(){
        //subscribes to the OnStateChange event in game manager
        GameManager.OnStateChange += GameManagerStateChanged;

    }

    // Update is called once per frame, runs the piece movement
    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            GameManager.Instance.SetGameState(GameState.Paused);
        }
        //usets the piece from the board to move it
        this.board.Unset(activePiece);
        this.lockTime += Time.deltaTime;
        //rotates clockwise
        if (!GameManager.Instance.aiGame){
            if (Input.GetKeyDown(KeyCode.UpArrow)){
                activePiece.Rotate(1);
            }
            if (Input.GetKeyDown(KeyCode.M)){
                SoundManager.Instance.MuteUnmute();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow)){
                activePiece.Move(Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)){
                activePiece.Move(Vector2Int.right);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)){
                activePiece.Move(Vector2Int.down);
            }
            if (Input.GetKeyDown(KeyCode.Space)){
                activePiece.InstaDrop();
            }
        }

        if (Time.time >= this.stepTime){
            this.stepTime = Time.time + this.stepDelay;
            activePiece.Step();
        }
        //sets this piece in place, then updates the hud
        this.board.Set(activePiece);
        HudManager.Instance.updateHUD(Score, LinesCleared, level);
        
    }
    //score is updated by the board when it clears any number of lines, will be one lower than number of lines cleared
    public void updateScore(int linesCleared){
        int[] points = {100, 300, 600, 1000};
        Score += points[linesCleared];
        LinesCleared += (linesCleared + 1);
        if (LinesCleared >= levelReq){
            updateLevel();
        }
    }
    //updates the level of the game, increasing the required number of lines and the delay
    public void updateLevel(){
        if (level >= levelCap){
            return;
        }
        levelReq += 10;
        level++;
        stepDelay -= 0.18f;

    }

    private void GameManagerStateChanged(GameState state){
        this.enabled = (state == GameState.Playing);
    }
}
