using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausedState : IState
{
    public string Name { get => "MainMenu State"; }
    public GameManager.GameState gameState { get => GameManager.GameState.Paused; }
    public void Enter()
    {
        Time.timeScale = 0f;
    }

    public void Exit()
    {
        Time.timeScale = 1f;
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
