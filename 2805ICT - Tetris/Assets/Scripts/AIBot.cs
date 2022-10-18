using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class AIBot : MonoBehaviour
{
    [SerializeField] Gameboard board;

    private Piece testPiece;
    private Vector3Int originalPiecePosition;
    private RectInt bounds;
    private float bestScore;
    private Vector3Int bestPos;
    private int bestRot;
    private Queue<KeyCode> inputs;
    private KeyCode currentInput;

    private void Awake(){
        inputs = new Queue<KeyCode>();
        testPiece = GetComponent<Piece>();
    }

    private void Start(){
        StartCoroutine(nameof(InputLoop));
    }

    private IEnumerator InputLoop(){
        while (GameManager.Instance.State == GameState.Playing){
            if (inputs.Count != 0){
                currentInput = inputs.Dequeue();
                yield return null;
                currentInput = KeyCode.None;
                yield return new WaitForSeconds(0.5f);
            }
            else{
                yield return null;
            }
        }

    }

    public void CalcMove(){
        bestScore = float.MaxValue;
        bounds = board.Bounds;
        testPiece.CopyPieceData(board.activePiece);
        originalPiecePosition = board.activePiece.position;
        GenAllPossibleMoves();
        WriteInputs();
    }

    private void WriteInputs(){
        int newX = bestPos.x;
        for (int i = PlayManager.Instance.activePiece.currentRotation; i < bestRot; i++){
            inputs.Enqueue(KeyCode.UpArrow);
        }
        for (int i = PlayManager.Instance.activePiece.position.x; i < newX; i++){
            inputs.Enqueue(KeyCode.RightArrow);
        }
        for (int i = PlayManager.Instance.activePiece.position.x; i > newX; i--){
            inputs.Enqueue(KeyCode.LeftArrow);
        }
        inputs.Enqueue(KeyCode.Space);
    }

    public bool GetKeyDown(KeyCode input){
        return currentInput == input;
    }

    private void GenAllPossibleMoves(){
        int rotNum;
        //spawn piece in all 4 rotations (record up presses), 1 at a time
        if (testPiece.data.tetromino == Tetromino.I){
            rotNum = 1; 
        }
        else if (testPiece.data.tetromino == Tetromino.O){
            rotNum = 0;
        }
        else{
            rotNum = 3;
        }
        for(int i = 0; i <= rotNum; i++){
            DropAll(i);
        }
        //record tilemap of all valid moves that are unique
        
    }

    private void DropAll(int rotates){
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
            testPiece.SetLocation(originalPiecePosition);
            testPiece.RotateMultiple(4-rotates);
            testPiece.RotateMultiple(rotates);
            Vector2Int newPos = new Vector2Int(i, 0);
            if(!testPiece.Move(newPos)){
                continue;
            }
            testPiece.AIDrop();
            this.board.Set(testPiece);
            float score = CalculateScore(this.board.tilemap);
            Debug.Log(score);
            Debug.Log(bestScore);
            if (score < bestScore || (score == bestScore && UnityEngine.Random.Range(0, 1) == 0)){
                bestScore = score;
                bestPos = testPiece.position;
                bestRot = testPiece.currentRotation;
            }
            this.board.Unset(testPiece);
        }
        testPiece.SetLocation(originalPiecePosition);
        testPiece.RotateMultiple(4-rotates);
    }


    private float CalculateScore(Tilemap map){
        float height = MeanHeight(map);
        float gaps = CountGaps(map);
        float diff = CalcGreatestDifference(map);
        float consec = ConsecutiveDiff(map);
        float total = (height* 0.9f) + (gaps *0.29f) + (diff * 0.44f) + (consec * 0.16f);
        return total;
    }


    private float CountGaps(Tilemap map){
        int hole = 0;
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
            bool top = false;
            for(int j = bounds.yMax; j <= bounds.yMin; j--){
                if(map.HasTile(new Vector3Int(i, j))){
                    top = true;
                }
                if(!map.HasTile(new Vector3Int(i, j)) && top){
                    hole += 1;
                }
            }
        }
        return hole;
    }
    private float MeanHeight(Tilemap map){
        float height = 0;
        int top = 0;
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
            for(int j = bounds.yMin; j <= bounds.yMax; j++){
                if(map.HasTile(new Vector3Int(i, j))){
                    top = (j - bounds.yMin);
                }
            }
            height += top;
        }
        height = height / (bounds.xMax * 2);
        return height;

    }

    private float ConsecutiveDiff(Tilemap map){
        int columnOne;
        int columnTwo = 0;
        int biggestDiff = 0;
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
            columnOne = columnTwo;
            columnTwo = 0;
            for(int j = bounds.yMin; j <= bounds.yMax; j++){
                if(map.HasTile(new Vector3Int(i, j))){
                    columnTwo += 1;
                }
            }
            if (i == bounds.xMin){
                continue;
            }
            int diff = Math.Abs(columnOne - columnTwo);
            if((diff > biggestDiff)){
                biggestDiff = diff;
            }
        }
        return biggestDiff;
    }

    private float CalcGreatestDifference(Tilemap map){
        int lowest = int.MaxValue;
        int highest = int.MinValue;
        int column = 0;
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
            column = 0;
            for(int j = bounds.yMin; j <= bounds.yMax; j++){
                if(map.HasTile(new Vector3Int(i, j))){
                    column += 1;
                }
            }
            if (column < lowest){
                lowest = column;
            }
            if (column > highest){
                highest = column;
            }
        }
        return (highest - lowest);
    }
}
