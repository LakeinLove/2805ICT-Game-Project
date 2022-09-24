using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public  class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    
    public static event Action<GameState> OnStateChange;
    void Awake(){
        Instance = this;
    }

    void Start(){
        SetGameState(GameState.Playing);
    }


    public void SetGameState(GameState newState){
        State = newState;
        switch(newState){
            case GameState.Playing:
                PlayManager.Instance.enabled = true;
                break;
            case GameState.Paused:
                PlayManager.Instance.enabled = false;
                break;
            case GameState.Quit:
                SceneManager.LoadScene("Menus");
                break;
            case GameState.End:
                PlayManager.Instance.enabled = false;
                break;
        }
        OnStateChange?.Invoke(newState);
    }
}



public enum GameState{
    Playing,
    Paused,
    Quit,
    End
}