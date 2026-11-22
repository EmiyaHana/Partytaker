using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD (Steps & Level)")]
    public TextMeshProUGUI stepsText;
    public TextMeshProUGUI levelText;
    public string levelDisplayName = "Level 1";

    [Header("Option / Audio Settings")]
    public GameObject optionPanel;
    public Slider volumeSlider;
    public string mainMenuSceneName = "MainMenu";

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    [Header("Victory UI (Final Level)")]
    public GameObject victoryPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (levelText != null) levelText.text = levelDisplayName;
        if (optionPanel != null) optionPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        if (volumeSlider != null)
        {
            float savedVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            AudioListener.volume = savedVol;
            volumeSlider.value = savedVol;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void Update()
    {
        if (stepsText != null && GameManager.Instance != null)
        {
            stepsText.text = $"Turns: {GameManager.Instance.currentTurns} | Steps: {GameManager.Instance.actionLog.stepsTaken}";
        }

        if (GameManager.Instance != null && GameManager.Instance.currentTurns <= 0 
            && !DialogueManager.Instance.isDialogueActive && !GameManager.Instance.isGameOver)
        {
            ShowGameOverPanel();
        }
    }

    public void ToggleOptionPanel()
    {
        if (optionPanel == null) return;
        bool nextState = !optionPanel.activeSelf;
        optionPanel.SetActive(nextState);
        GameManager.Instance.isSoftLocked = nextState;
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void ShowGameOverPanel()
    {
        GameManager.Instance.TriggerGameOver();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void OnRetryButtonClicked()
    {
        GameManager.Instance.RestartLevel();
    }

    public void ShowVictoryPanel()
    {
        GameManager.Instance.isSoftLocked = true;
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}