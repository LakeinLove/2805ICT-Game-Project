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
    public int Level;
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
        Level = PrefsHelper.LoadInt("level", 0);
        extrominos = (PrefsHelper.LoadInt("gameType", 0) != 0);
        levelCap = 10;
        levelReq = 10 * (1 + Level);
        Score = 0;
        LinesCleared = 0;
        stepDelay = (2f - Level * 0.18f);
        lockDelay = 0.5f;
            
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
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            activePiece.Rotate(1);
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

        if (Time.time >= this.stepTime){
            this.stepTime = Time.time + this.stepDelay;
            activePiece.Step();
        }
        //sets this piece in place, then updates the hud
        this.board.Set(activePiece);
        HudManager.Instance.updateHUD(Score, LinesCleared, Level);
        
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
        if (Level >= levelCap){
            return;
        }
        levelReq += 10;
        Level++;
        stepDelay -= 0.18f;

    }
}
