using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

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
                yield return new WaitForSeconds(0.25f);
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
        originalPiecePosition = board.activePiece.position + (Vector3Int.down * 3);
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
        float stdDiv = stdDevi(map);
        float linesClear = LinesCompleted(map);
        float total = (height* 0.3f) + (gaps *0.2f) + (stdDiv * 0.7f) + (diff * 1.0f) + (consec * 0.6f) + (linesClear * -4f);
        return total;
    }


    private float CountGaps(Tilemap map){
        int hole = 0;
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
            bool top = false;
            for(int j = bounds.yMax; j >= bounds.yMin; j--){
                if(map.HasTile(new Vector3Int(i, j))){
                    top = true;
                }
                if((!map.HasTile(new Vector3Int(i, j))) && top){
                    hole += 1;
                }
            }
        }
        return hole;
    }

    private float stdDevi(Tilemap map){
        List<float> heights = new List<float>();
        float top = 0;
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
            top = 0;
            for(int j = bounds.yMin; j <= bounds.yMax; j++){
                if(map.HasTile(new Vector3Int(i, j))){
                    top = (j - bounds.yMin);
                }
            }
            heights.Add(top);
        }

        float average = heights.Average();  
        float sumOfDerivation = 0;  
        foreach (float value in heights)  
        {  
            sumOfDerivation += (value) * (value);  
        }  
        float sumOfDerivationAverage = sumOfDerivation / (heights.Count - 1);  
        return (float) Math.Sqrt(sumOfDerivationAverage - (average*average));
    }
    private float MeanHeight(Tilemap map){
        float height = 0;
        int top = 0;
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
            top = 0;
            for(int j = bounds.yMax; j >= bounds.yMin; j--){
                if(map.HasTile(new Vector3Int(i, j))){
                    top = (j - bounds.yMin);
                    break;
                }
            }
            height += top;
        }
        height = height / (bounds.xMax - bounds.xMin);
        return height;

    }

    private float LinesCompleted(Tilemap map){
        int totalLines = 0;
        for (int i = bounds.yMin; i <= bounds.yMax; i++){
            int line = 1;
            for (int j = bounds.xMin; j <= bounds.xMax; j++){
                if (!map.HasTile(new Vector3Int(j, i))){
                    line = 0;
                }
            }
            totalLines += line;
        }
        return totalLines;
    }

    private float ConsecutiveDiff(Tilemap map){
        int columnOne;
        int columnTwo = 0;
        int biggestDiff = 0;
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
            columnOne = columnTwo;
            columnTwo = 0;
            for(int j = bounds.yMax; j >= bounds.yMin; j--){
                if(map.HasTile(new Vector3Int(i, j))){
                    columnTwo = (j - bounds.yMin);
                    break;
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
            for(int j = bounds.yMax; j >= bounds.yMin; j--){
                if(map.HasTile(new Vector3Int(i, j))){
                    column = (j - bounds.yMin);
                    break;
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
