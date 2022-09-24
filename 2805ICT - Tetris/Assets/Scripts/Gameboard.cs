using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Gameboard : MonoBehaviour
{
    //The used grid and tilemap to make the board itself
    public SpriteRenderer grid;
    public Tilemap tilemap;
    //the currently moving piece
    public Piece activePiece {get; private set;}
    public Vector3Int spawnPosition;
    //data for the next piece so it can be displayed
    public TetrominoData nextPiece;
    public Vector2Int boardSize;
    //unless extrominos are used, there are 7 tetrominos
    int tetrominoNum = 7;
    //when this is called, it returns the bounds and is used for movement and wallkicks, hence captial B;
    public RectInt Bounds{
        get{
            Vector2Int position = new Vector2Int(-this.boardSize.x /2, -this.boardSize.y /2);
            return new RectInt(position, this.boardSize);
        }
    }
    //list of all tetrominos
    public TetrominoData[] tetrominos;
    private void Awake(){
        //load presets from settings
        int boardWidth = PrefsHelper.LoadInt("boardWidth", 10);
        int boardHeight = PrefsHelper.LoadInt("boardHeight", 20);
        boardSize = new Vector2Int(boardWidth, boardHeight);
        grid.size = boardSize;
        //set data
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        if(PlayManager.Instance.extrominos){
            //check for extrominos
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
        //if the tetromino when spawned is an illegal move, then enter game over
        if (!IsValidMove(this.activePiece, this.spawnPosition)){
            GameOver();
        }
        //this sets the active piece
        Set(this.activePiece);
    }
    //empties the tilemap/board and then calls the end of game state
    public void GameOver(){
        this.tilemap.ClearAllTiles();
        GameManager.Instance.SetGameState(GameState.End);
    }
    //essentially locks the piece in place
    public void Set(Piece piece){
        for (int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    //unlocks the piece from that location
    public void Unset(Piece piece){
        for (int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    //checks if move is legal
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

    //erases line
    public void ClearLines(){
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        int rowCount = -1;
        //for each row erased, add the linecount and then send it to the playmanager to update the score and lines destroyed
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
    //checks the row is full
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
    //wipes out a row then moves all rows down so there is no gap
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