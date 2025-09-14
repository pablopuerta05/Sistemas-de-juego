using System.Collections;
using System.Collections.Generic;

public class GameOverState : IState
{
    public string Name { get => "MainMenu State"; }
    public GameManager.GameState gameState { get => GameManager.GameState.GameOver; }
    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
