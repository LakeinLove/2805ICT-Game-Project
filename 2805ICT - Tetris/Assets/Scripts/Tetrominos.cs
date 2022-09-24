using UnityEngine;
using UnityEngine.Tilemaps;
//these are the actual tetrominos used for the game
public enum Tetromino{
    I,
    O,
    T,
    J,
    L,
    S,
    Z,
    C,
    SL
}
[System.Serializable]
//stores all the tetromino data taken from the Data.Cs file, to stop having to make all 9 tetrominos
public struct TetrominoData{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] cells {get; private set;}
    public Vector2Int[,] wallKicks {get; private set;}
    //when initialized get the cells of the tetromino and the wallkicks using Data.cs
    public void Initialize(){
        this.cells = Data.Cells[this.tetromino];
        this.wallKicks = Data.WallKicks[this.tetromino];
    }
}