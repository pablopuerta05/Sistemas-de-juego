using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultScreen;
    public GameObject levelUpScreen;

    [Header("Current Stats Displays")]
    public TextMeshProUGUI currentHealthDisplay;
    public TextMeshProUGUI currentRecoveryDisplay;
    public TextMeshProUGUI currentMoveSpeedDisplay;
    public TextMeshProUGUI currentMightDisplay;
    public TextMeshProUGUI currentProjectileSpeedDisplay;
    public TextMeshProUGUI currentMagnetDisplay;

    [Header("Results Screen Displays")]
    public Image chosenCharacterImage;
    public TextMeshProUGUI chosenCharacterName;
    public TextMeshProUGUI LevelReachedDisplay;
    public TextMeshProUGUI timeSurvivedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    [Header("Stopwatch")]
    public TMP_Text stopwatchDisplay;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI levelText;
    [SerializeField] private PlayerExperience playerExperience;

    [Header("ASync Loader")]
    [SerializeField] private ASyncLoader asyncLoader;

    public void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void DestroySingleton()
    {
        Instance = null;
        Destroy(gameObject);
    }

    #region OnActionButton

    public void OnPauseButtonClicked()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Paused);
    }

    public void OnResumeGameClicked()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Gameplay);
    }

    public void OnMainMenuButtonClicked()
    {
        DestroySingleton();
        asyncLoader.LoadLevelBtn("MainMenu");
        GameManager.Instance.SetGameState(GameManager.GameState.MainMenu);
    }

    public void OnRestartButtonClicked()
    {
        DestroySingleton();
        asyncLoader.LoadLevelBtn("GameScene");
        GameManager.Instance.SetGameState(GameManager.GameState.Gameplay);
        resultScreen.SetActive(false);
    }

    #endregion

    public void UpdateStopWatchDisplay()
    {
        // calculate the number of minutes and seconds that have elapsed
        int minutes = Mathf.FloorToInt(GameManager.Instance.stopwatch.StopwatchTime / 60);
        int seconds = Mathf.FloorToInt(GameManager.Instance.stopwatch.StopwatchTime % 60);

        // update the stopwatch text to display the elapsed time
        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AssignChosenCharacterUI(CharacterData chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.Name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        LevelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsAndPassiveItemsUI(List<Image> chosenWeaponsData, List<Image> chosenPassiveItemsData)
    {
        if (chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.LogWarning("Chosen weapons and passive items data lists have different lengths");
            return;
        }

        // assign chosen weapons data to chosenWeaponsUI
        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            // check that the sprite of the corresponding element in chosenWeaponsData is not null
            if (chosenWeaponsData[i].sprite)
            {
                // enable the corresponding element in chosenWeaponsUI and set it's sprite to the corresponding sprite in chosenWeaponsData
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
            }
            else
            {
                // if the sprite is null, disable the corresponding element in chosenWeaponsUI
                chosenWeaponsUI[i].enabled = false;
            }
        }

        // assign chosen passive item data to chosenPassiveItemUI
        for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            // check that the sprite of the corresponding element in chosenPassiveItemsData is not null
            if (chosenPassiveItemsData[i].sprite)
            {
                // enable the corresponding element in chosenPassiveItemsUI and set it's sprite to the corresponding sprite in chosenPassiveItemsData
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
            }
            else
            {
                // if the sprite is null, disable the corresponding element in chosenPassiveItemsUI
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }

    public void UpdateExpBar()
    {
        // update exp bal fill amount
        expBar.fillAmount = (float)playerExperience.experience / playerExperience.experienceCap;
    }

    public void UpdateLevelText()
    {
        // update level text
        levelText.text = "LV. " + playerExperience.level.ToString();
    }

    public void InitializeRuntimeUI(CharacterData data, float currentHealth)
    {
        if (data == null) return;

        AssignChosenCharacterUI(data);
        UpdateExpBar();
        UpdateLevelText();
    }
}
