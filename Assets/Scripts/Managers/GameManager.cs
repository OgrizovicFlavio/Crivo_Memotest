using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private List<LevelData> levelConfigs;

    [Header("UI References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CardBoardManager boardManager;

    private int totalPairs;
    private int currentScore;
    private int attemptsRemaining;
    private float currentTime;
    private float lastPulseTime = -1f;
    private int currentLevel = 1;
    private bool timerStarted = false;
    private float originalTimeLimit;
    public bool GameFinished { get; private set; }

    protected override void OnAwaken()
    {
        base.OnAwaken();
        GameFinished = false;
    }

    private void Update()
    {
        if (GameFinished || !timerStarted)
            return;

        if (originalTimeLimit < 0f)
        {
            uiManager?.SetTime(-1f);
            return;
        }

        currentTime -= Time.deltaTime;

        if (originalTimeLimit > 0f && currentTime <= 10f)
        {
            int currentSecond = Mathf.CeilToInt(currentTime);

            if (currentSecond != Mathf.CeilToInt(lastPulseTime))
            {
                lastPulseTime = currentTime;
                uiManager?.PulseTimer();
            }
        }

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            HandleLose();
        }

        uiManager?.SetTime(currentTime);
    }

    public void InitializeGame(LevelData data)
    {
        this.currentLevel = data.levelNumber;
        this.totalPairs = data.totalPairs;
        this.attemptsRemaining = data.maxAttempts;
        this.originalTimeLimit = data.timeLimit;
        this.currentTime = data.timeLimit;
        this.currentScore = 0;
        this.timerStarted = false;
        GameFinished = false;

        uiManager?.InitializeUI(currentLevel, attemptsRemaining, currentTime, currentScore, totalPairs);
    }

    public LevelData GetLevelData()
    {
        int index = Mathf.Clamp(currentLevel - 1, 0, levelConfigs.Count - 1);
        return levelConfigs[index];
    }

    public void RegisterMatch()
    {
        if (GameFinished) return;

        currentScore++;

        UpdateUI();

        if (currentScore >= totalPairs)
        {
            HandleWin();
        }
    }

    public void RegisterFailedAttempt()
    {
        if (GameFinished) return;

        if (attemptsRemaining < 0) return;

        attemptsRemaining--;
        uiManager?.PulseAttempts();

        UpdateUI();

        if (attemptsRemaining <= 0)
        {
            HandleLose();
        }
    }

    private void UpdateUI()
    {
        uiManager?.SetAttempts(attemptsRemaining);
        uiManager?.SetProgress(currentScore, totalPairs);

        float displayTime = originalTimeLimit < 0f ? -1f : currentTime;
        uiManager?.SetTime(displayTime);
    }

    public void StartTimer()
    {
        timerStarted = true;
    }

    private void HandleWin()
    {
        GameFinished = true;
        uiManager.ShowWinPanel(HasNextLevel());
    }

    private void HandleLose()
    {
        GameFinished = true;
        uiManager?.ShowLosePanel();
    }

    public void OnNextLevel()
    {
        currentLevel++;
        uiManager.HideWinPanel();
        boardManager.LoadLevel();
    }

    public void OnReturnToMenu()
    {
        uiManager.HideWinPanel(() =>
        {
            uiManager.HideLosePanel(() =>
            {
                SceneManager.LoadScene("MainMenu");
            });
        });
    }

    public void OnRetry()
    {
        uiManager.HideLosePanel(() =>
        {
            currentLevel = 1;
            boardManager.LoadLevel();
        });
    }

    public bool HasNextLevel()
    {
        return currentLevel < levelConfigs.Count;
    }
}
