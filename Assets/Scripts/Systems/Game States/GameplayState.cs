using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class GameplayState : IState
{
    private GameManager gameManager;

    // Constructor de la clase
    public GameplayState()
    {
        gameManager = GameManager.Instance;
    }

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
        //CheckForPauseAndResume();
    }

    //// Define the method to check for pause and resume input
    //private void CheckForPauseAndResume()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        if (currentState == GameState.Paused)
    //        {
    //            ResumeGame();
    //        }
    //        else
    //        {
    //            PauseGame();
    //        }
    //    }
    //}

    //private void DisableScreens()
    //{
    //    pauseScreen.SetActive(false);
    //    resultScreen.SetActive(false);
    //}
}
