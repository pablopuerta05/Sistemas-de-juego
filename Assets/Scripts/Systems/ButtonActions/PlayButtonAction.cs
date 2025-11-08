using UnityEngine;

public class PlayButtonAction : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Gameplay);
    }
}
