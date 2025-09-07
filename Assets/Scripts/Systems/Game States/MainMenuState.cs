using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : IState
{
    private GameManager gameManager;

    // Constructor de la clase
    public MainMenuState()
    {
        gameManager = GameManager.Instance;
    }

    public void Enter()
    {
        Time.timeScale = 0f; // Pausar el tiempo mientras estamos en el menú
    }

    public void Exit()
    {
        Time.timeScale = 1f;
    }

    public void Update()
    {
        //if (Time.timeScale == 1f)
        //{
        //    gameManager.SetGameState(GameManager.GameState.Gameplay);
        //}
    }
}
