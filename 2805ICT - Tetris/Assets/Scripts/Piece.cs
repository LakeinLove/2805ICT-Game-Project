using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Piece : MonoBehaviour
{
    public int currentRotation {get; private set;}
    public Gameboard board {get; private set;}
    public TetrominoData data {get; private set;}
    public Vector3Int position {get; private set;}
    public Vector3Int[] cells {get; private set;}

    private VisualElement escapeMenu;
    private VisualElement nextPiecePic;

   public void Initialize(Gameboard board, Vector3Int position, TetrominoData data){
    this.board = board;
    this.position = position;
    this.data = data;
    this.currentRotation = 0;
    PlayManager.Instance.activePiece = this;
    PlayManager.Instance.stepTime = Time.time + PlayManager.Instance.stepDelay;
    PlayManager.Instance.lockTime = 0f;

    if (this.cells == null){
        this.cells = new Vector3Int[data.cells.Length];
    }
    
    for (int i = 0; i < data.cells.Length; i++){
        this.cells[i] = (Vector3Int) data.cells[i];
    }
   }

    public void Step(){
        Move(Vector2Int.down);
        if (PlayManager.Instance.lockTime >= PlayManager.Instance.lockDelay){
            Lock();
        }
    }

    public void InstaDrop(){
        while (Move(Vector2Int.down)){
            continue;
        }
        Lock();
    }

    private void Lock(){
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }

    public void Rotate(int direction){
        int originalRotation = this.currentRotation;
        this.currentRotation = Wrap(this.currentRotation + direction, 0, 4);
        ApplyRotationMatrix(direction);
        
        if (!TestKicks(this.currentRotation, direction)){
            this.currentRotation = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

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

    private int GetKickIndex(int rotationIndex, int rotationDirection){
        int kickIndex = rotationIndex * 2;
        if (rotationDirection < 0){
            kickIndex--;
        }

        return Wrap(kickIndex, 0, this.data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max){
        if (input < min)
            return max - (min - input) % (max - min);
        else
            return min + (input - min) % (max - min);
    }

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
