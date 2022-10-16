using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public  class GameManager : MonoBehaviour
{
    //operates as a singleton with an Instance that is globally accessible
    public static GameManager Instance;
    //the currently selected state of the game
    public GameState State;
    //an event that other objects and singletons can subscribe to 
    public static event Action<GameState> OnStateChange;
    void Awake(){
        //set the singleton
        Instance = this;
    }
    //begin the game when game opens
    void Start(){
        SetGameState(GameState.Playing);
    }
    //simple switch to filter game state, like an FSM
    public void SetGameState(GameState newState){
        State = newState;
        switch(newState){
            case GameState.Playing:
            case GameState.Paused:
            case GameState.End:
                break;
            case GameState.Quit:
                SceneManager.LoadScene("Menus");
                break;
        }
        //ensures that it only calls if it is not NULL
        OnStateChange?.Invoke(newState);
    }

    void updateHighScores(){
        var currentScore = PlayManager.Instance.Score;
        if (currentScore <= PrefsHelper.scoreList[9].score){
            return;
        }
        
    }
}
//Enum of all possible states
public enum GameState{
    Playing,
    Paused,
    Quit,
    End
}