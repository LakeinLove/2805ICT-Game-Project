using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AIBot : MonoBehaviour
{
    [SerializeField] Gameboard board;

    private Piece testPiece;
    private Vector3Int originalPiecePosition;
    private List<Gameboard> boardMaps;
    private RectInt bounds;
    private float bestScore;
    private Vector3Int bestPos;
    private int bestRot;


    public void Start(){
        StartCoroutine(AILoop());
    }

    private IEnumerator AILoop(){
        while (GameManager.Instance.State == GameState.Playing){
            MakeAMove();
            yield return new WaitForSeconds(2);
        }
    }

    public void MakeAMove(){
        bestScore = float.MaxValue;
        bounds = board.Bounds;
        testPiece = board.activePiece;
        originalPiecePosition = testPiece.position;
        GenAllPossibleMoves();
        MovePiece(bestPos, bestRot);
    }

    private void MovePiece(Vector3Int newPosition, int rotation){
        PlayManager.Instance.activePiece.RotateMultiple(rotation);
        PlayManager.Instance.activePiece.SetLocation(newPosition);
        PlayManager.Instance.activePiece.InstaDrop();
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
        for(int i = 0; i < rotNum; i++){
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
            testPiece.InstaDrop();
            this.board.Set(testPiece);
            float score = CalculateScore(this.board.tilemap);
            if (score < bestScore){
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
        float height = 0, gaps = 0;
        CountGapsAndHeight(map, ref gaps, ref height);
        float diff = CalcGreatestDifference(map);
        float total = (height * 0.9f)  + (gaps * 0.29f) + (diff * 0.44f);
        return total;
    }


    private void CountGapsAndHeight(Tilemap map, ref float gaps, ref float height){
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
            for(int j = bounds.yMin; j <= bounds.yMax; j++){
                if(!map.HasTile(new Vector3Int(i, j)) && map.HasTile(new Vector3Int(i, j-1))){
                    gaps += 1;
                }
                if(map.HasTile(new Vector3Int(i, j))){
                    height += 1;
                }
            }
        }
        height = height / (bounds.xMax - bounds.xMin);
    }

    private float CalcGreatestDifference(Tilemap map){
        int lowest = int.MaxValue;
        int highest = int.MinValue;
        int column = 0;
        for(int i = bounds.xMin; i <= bounds.xMax; i++){
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
