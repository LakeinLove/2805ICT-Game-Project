using UnityEngine;
using System.Collections.Generic;

public static class Data
{
    public static readonly float cos = Mathf.Cos(Mathf.PI/2f);
    public static readonly float sin = Mathf.Sin(Mathf.PI/2f);
    //simplifies rotation functions later by using this matrix in Piece class
    public static readonly float[] RotationMatrix = new float[] {cos, sin, -sin, cos};

    //The cells of each tetromino and extromino
    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new Dictionary<Tetromino, Vector2Int[]>(){
        {Tetromino.I, new Vector2Int[] {new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int(2, 1)} },
        {Tetromino.O, new Vector2Int[] {new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int(1, 0)} },
        {Tetromino.T, new Vector2Int[] {new Vector2Int( 0, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int(1, 0)} },
        {Tetromino.J, new Vector2Int[] {new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int(1, 0)} },
        {Tetromino.L, new Vector2Int[] {new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int(1, 0)} },
        {Tetromino.S, new Vector2Int[] {new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0)} },
        {Tetromino.Z, new Vector2Int[] {new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int(1, 0)} },
        {Tetromino.C, new Vector2Int[] {new Vector2Int( 0, 1), new Vector2Int( 1, 0), new Vector2Int( 0, 0), new Vector2Int(0, 0)} },
        {Tetromino.SL, new Vector2Int[] {new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int( 1, 1), new Vector2Int(0, 1)} },
    };
    //The wallkick data for the I tetromino
     private static readonly Vector2Int[,] WallKicksI = new Vector2Int[,] {
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) },
    };
    //The wallkick data for every other tetromino, the extrominos use this as well
    private static readonly Vector2Int[,] WallKicksOTJLSZ = new Vector2Int[,] {
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) },
    };

    //simple dict linking each tetromino to their wallkick data
    public static readonly Dictionary<Tetromino, Vector2Int[,]> WallKicks = new Dictionary<Tetromino, Vector2Int[,]>()
    {
        { Tetromino.I, WallKicksI },
        { Tetromino.J, WallKicksOTJLSZ },
        { Tetromino.L, WallKicksOTJLSZ },
        { Tetromino.O, WallKicksOTJLSZ },
        { Tetromino.S, WallKicksOTJLSZ },
        { Tetromino.T, WallKicksOTJLSZ },
        { Tetromino.Z, WallKicksOTJLSZ },
        { Tetromino.C, WallKicksOTJLSZ },
        {Tetromino.SL, WallKicksI},
    };
}
