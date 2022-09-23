using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class HudManager : MonoBehaviour
{
    private VisualElement root;
    private VisualElement escapeMenu;

    private Button resumeButton;
    private Button exitButton;

    private void Awake(){
        this.root = GetComponent<UIDocument>().rootVisualElement;
        GameManager.OnStateChange += GameManagerStateChanged;

        this.escapeMenu = root.Q<VisualElement>("EscMenu");
        this.resumeButton = escapeMenu.Q<Button>("Resume");
        this.exitButton = escapeMenu.Q<Button>("Exit");

        this.escapeMenu.visible = false;

        resumeButton.clicked += ResumeGame;
        exitButton.clicked += ExitGame;

    }
    private void GameManagerStateChanged(GameState state){
        escapeMenu.visible = (state == GameState.Paused);
    }

    private void updateScore(){
        
    }

    private void ResumeGame(){
        GameManager.Instance.SetGameState(GameState.Playing);
    }

    private void ExitGame(){
        resumeButton.clicked -= ResumeGame;
        exitButton.clicked -= ExitGame;
        GameManager.OnStateChange -= GameManagerStateChanged;
        GameManager.Instance.SetGameState(GameState.Quit);
    }
}
