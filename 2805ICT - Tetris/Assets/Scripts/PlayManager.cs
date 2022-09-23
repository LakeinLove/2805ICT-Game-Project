using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance;
    public Gameboard board;
    public Piece activePiece {get; set;}

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;
    public float lockTime;
    public float stepTime;



    void Awake(){
        Instance = this;
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            GameManager.Instance.SetGameState(GameState.Paused);
        }
        
        this.board.Unset(activePiece);
        this.lockTime += Time.deltaTime;

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

        this.board.Set(activePiece);
        
    }
}
