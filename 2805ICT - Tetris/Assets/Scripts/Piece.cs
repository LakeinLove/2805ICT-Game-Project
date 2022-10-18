using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Piece : MonoBehaviour
{
    [SerializeField] private AudioClip placementNoise;
    public int currentRotation {get; private set;}
    public Gameboard board {get; private set;}
    public TetrominoData data {get; private set;}
    public Vector3Int position {get; private set;}
    public Vector3Int[] cells {get; private set;}

    //when the piece is created on the game board it gains all of this logic
   public void Initialize(Gameboard board, Vector3Int position, TetrominoData data){
    this.board = board;
    this.position = position;
    this.data = data;
    this.currentRotation = 0;
    PlayManager.Instance.activePiece = this;
    PlayManager.Instance.stepTime = Time.time + PlayManager.Instance.stepDelay;
    PlayManager.Instance.lockTime = 0f;
    //loads the cells taken up by the tetromino
    if (this.cells == null){
        this.cells = new Vector3Int[data.cells.Length];
    }
    //gets the data for the cells
    for (int i = 0; i < data.cells.Length; i++){
        this.cells[i] = (Vector3Int) data.cells[i];
    }
   }
    //steps the piece down
    public void Step(){
        Move(Vector2Int.down);
        if (PlayManager.Instance.lockTime >= PlayManager.Instance.lockDelay){
            Lock();
        }
    }
    public void CopyPieceData(Piece first){
        this.position = first.position;
        this.board = first.board;
        this.data = first.data;
        this.currentRotation = first.currentRotation;
        this.cells = new Vector3Int[data.cells.Length];
        for (int i = 0; i < data.cells.Length; i++){
            this.cells[i] = (Vector3Int) data.cells[i];
        }
    }
    //instantly sets location
    public void SetLocation(Vector3Int newPosition){
        this.position = newPosition;
    }
    //drops the piece to the bottom by moving over and over
    public void InstaDrop(){
        while (Move(Vector2Int.down)){
            continue;
        }
        Lock();
    }

    public void AIDrop(){
        while (Move(Vector2Int.down)){
            continue;
        }
    }
    //locks the piece in place, setting it to the board and then checked if the line needs to be cleared
    //spawns the next piece afterwards
    private void Lock(){
        this.board.Set(this);
        SoundManager.Instance.PlaySound(placementNoise);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }
    //rotation of the tetromino done in a direction, checks with kicks to revert it
    public void Rotate(int direction){
        int originalRotation = this.currentRotation;
        this.currentRotation = Wrap(this.currentRotation + direction, 0, 4);
        ApplyRotationMatrix(direction);
        if (!GameManager.Instance.aiGame){
            if (!TestKicks(this.currentRotation, direction)){
            this.currentRotation = originalRotation;
            ApplyRotationMatrix(-direction);
            }
        }
    }
    public void RotateMultiple(int numberRotation){
        for(int i = 0; i < numberRotation; i++){
            Rotate(1);
        }
    }
    //applies the rotation matrix to each square of the tetromino
    private void ApplyRotationMatrix(int direction){
        for (int i = 0; i < this.cells.Length; i++){
            Vector3 cell = this.cells[i];
            int x, y;

            switch(this.data.tetromino){
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y *Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y *Data.RotationMatrix[3] * direction));

                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y *Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y *Data.RotationMatrix[3] * direction));

                    break;
            }
            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }
    //checks if a kick is needed
    private bool TestKicks(int index, int Direction){
        int kickIndex = GetKickIndex(index, Direction);
        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++){
            Vector2Int translation = this.data.wallKicks[kickIndex, i];
            if (Move(translation)){
                return true;
            }
        }
        return false;
    }
    //checks which kick is needed
    private int GetKickIndex(int rotationIndex, int rotationDirection){
        int kickIndex = rotationIndex * 2;
        if (rotationDirection < 0){
            kickIndex--;
        }

        return Wrap(kickIndex, 0, this.data.wallKicks.GetLength(0));
    }
    //wraps around the numbers, basic universal function
    private int Wrap(int input, int min, int max){
        if (input < min)
            return max - (min - input) % (max - min);
        else
            return min + (input - min) % (max - min);
    }
    //moves the piece in a translation
    public bool Move(Vector2Int translation){
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidMove(this, newPosition);
        if (valid && translation == Vector2Int.down){
            this.position = newPosition;
            PlayManager.Instance.lockTime = 0f;
        }
        else if (valid){
            this.position = newPosition;
            PlayManager.Instance.lockTime -= 0.05f;
        }
        return valid;
    }
}
