using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Gameboard : MonoBehaviour
{
    public SpriteRenderer Grid;
    public Tilemap tilemap;
    public Piece activePiece {get; private set;}
    public Vector3Int spawnPosition;
    public TetrominoData nextPiece;
    public Vector2Int boardSize;
    int tetrominoNum = 7;
    public RectInt Bounds{
        get{
            Vector2Int position = new Vector2Int(-this.boardSize.x /2, -this.boardSize.y /2);
            return new RectInt(position, this.boardSize);
        }
    }
    public TetrominoData[] tetrominos;
    private void Awake(){
        int boardWidth = PrefsHelper.LoadInt("boardWidth", 10);
        int boardHeight = PrefsHelper.LoadInt("boardHeight", 20);
        boardSize = new Vector2Int(boardWidth, boardHeight);
        Grid.size = boardSize;

        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        if(PlayManager.Instance.extrominos){
            tetrominoNum = 9;
        }
        for (int i = 0; i < tetrominoNum; i++){
            this.tetrominos[i].Initialize();
        }
    }

    private void Start(){
        SpawnPiece();
    }

    public void SpawnPiece(){
        int random = Random.Range(0, tetrominoNum);
        if (this.nextPiece.tile == null){
            this.nextPiece = this.tetrominos[random];
        }
        TetrominoData data = nextPiece;
        this.nextPiece = this.tetrominos[random];
        this.activePiece.Initialize(this, this.spawnPosition, data);
        if (!IsValidMove(this.activePiece, this.spawnPosition)){
            GameOver();
        }

        Set(this.activePiece);
    }

    public void GameOver(){
        this.tilemap.ClearAllTiles();
        GameManager.Instance.SetGameState(GameState.End);
    }

    public void Set(Piece piece){
        for (int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void Unset(Piece piece){
        for (int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }


    public bool IsValidMove(Piece piece, Vector3Int move){
        RectInt bounds = this.Bounds;
        for (int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePosition = piece.cells[i] + move;

            if (this.tilemap.HasTile(tilePosition)){
                return false;
            }
            if (!bounds.Contains((Vector2Int)tilePosition)){
                return false;
            }
        }
        return true;
    }

    public void ClearLines(){
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        int rowCount = -1;
        while (row < bounds.yMax){
            if (IsLineFull(row)){
                LineClear(row);
                rowCount++;
            }
            else{
                row++;
            }
        }
        if(rowCount >= 0){
            PlayManager.Instance.updateScore(rowCount);
        }
        rowCount = -1;
    }

    private bool IsLineFull(int row){
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++){
            Vector3Int position = new Vector3Int(col, row, 0);
            if (!this.tilemap.HasTile(position)){
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row){
        RectInt bounds = this.Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; col++){
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }
        while (row < bounds.yMax){
            for(int col = bounds.xMin; col < bounds.xMax; col++){
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }
            row++;
        }
    }

}
