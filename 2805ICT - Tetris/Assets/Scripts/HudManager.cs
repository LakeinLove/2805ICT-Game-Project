using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class HudManager : MonoBehaviour
{
    private VisualElement root;
    private VisualElement escapeMenu;
    
    private Button resumeButton;
    private Button exitButton;

    private void Awake(){
        this.root = GetComponent<UIDocument>().rootVisualElement;

        this.escapeMenu = root.Q<VisualElement>("EscMenu");
        this.resumeButton = escapeMenu.Q<Button>("Resume");
        this.exitButton = escapeMenu.Q<Button>("Exit");

        this.escapeMenu.visible = false;

        resumeButton.clicked += ResumeGame;
        exitButton.clicked += ExitGame;

    }

    private void ResumeGame(){
        escapeMenu.visible = false;
    }

    private void ExitGame(){
        resumeButton.clicked -= ResumeGame;
        exitButton.clicked -= ExitGame;
        SceneManager.LoadScene("Menus");
    }
}
