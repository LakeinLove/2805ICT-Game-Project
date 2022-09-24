using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Gameboard board;
    public Piece trackingPiece;

    public Tilemap tilemap {get; private set;}
    public Vector3Int position {get; private set;}
    public Vector3Int[] cells {get; private set;}

    private void Awake(){
        //load the tilemap
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }
    //called after the other updates in the game, once per frame, and ensures the ghost is draw properly
    private void LateUpdate(){
        Clear();
        Copy();
        Drop();
        Set();
    }
    //removes the ghost
    private void Clear(){
        for (int i = 0; i < this.cells.Length; i++){
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }
    //Makes the ghost a copy of the main piece
    private void Copy(){
        for (int i = 0; i < this.cells.Length; i++){
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }
    //Drops the ghost all the way to the bottom, to show where it would end up with no movement
    private void Drop(){
        Vector3Int position = this.trackingPiece.position;

        int current = position.y;
        int bottom = -this.board.boardSize.y / 2 - 1;

        this.board.Unset(this.trackingPiece);

        for (int row = current; row >= bottom; row--){
            position.y = row;
            if (this.board.IsValidMove(this.trackingPiece, position)){
                this.position = position;
            }
            else{
                break;
            }
        }
        this.board.Set(this.trackingPiece);
    }
    //sets the ghost into the tilemap
    private void Set(){
        for (int i = 0; i < this.cells.Length; i++){
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, this.tile);
        }
    }
}
