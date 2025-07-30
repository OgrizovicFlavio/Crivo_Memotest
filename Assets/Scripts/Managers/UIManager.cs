using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image progressBarFill;

    [Header("End Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Win Panel Buttons")]
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button returnToMenuButton;

    [Header("Lose Panel Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitGameButton;

    private void Awake()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    public void SetAttempts(int attempts, bool showValue = true)
    {
        if (!showValue)
        {
            attemptsText.text = "INTENTOS:";
        }
        else
        {
            attemptsText.text = attempts < 0 ? "INTENTOS: ∞" : $"INTENTOS: {attempts}";
        }
    }

    public void SetProgress(int currentScore, int totalPairs)
    {
        float fill = (float)currentScore / totalPairs;

        if (progressBarFill != null && totalPairs > 0)
            progressBarFill.DOFillAmount(fill, 0.3f).SetEase(Ease.OutQuad);
    }

    public void SetTime(float time, bool showValue = true)
    {
        if (!showValue)
        {
            timerText.text = "TIEMPO:";
        }
        else
        {
            timerText.text = time < 0f ? "TIEMPO: ∞" : $"TIEMPO: {Mathf.CeilToInt(time)}";
        }
    }

    public void SetLevel(int level)
    {
        levelText.text = $"NIVEL {level}";
    }

    public void InitializeUI(int level, int attempts, float time, int currentScore, int totalPairs)
    {
        SetLevel(level);
        SetAttempts(attempts);
        SetTime(time);
        SetProgress(currentScore, totalPairs);
    }

    public void HideGameplayTexts()
    {
        if (attemptsText != null) attemptsText.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
    }

    public void ShowGameplayTexts()
    {
        if (attemptsText != null) attemptsText.gameObject.SetActive(true);
        if (timerText != null) timerText.gameObject.SetActive(true);
    }

    public void PulseTimer()
    {
        if (timerText == null) return;

        timerText.transform.DOKill();
        timerText.transform.localScale = Vector3.one;

        timerText.transform.DOScale(1.2f, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    public void ShowWinPanel(bool hasNextLevel)
    {
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(hasNextLevel);

        AnimatePanel(winPanel);
    }

    public void ShowLosePanel()
    {
        AnimatePanel(losePanel);
    }

    private void AnimatePanel(GameObject panel)
    {
        if (panel == null) return;

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        RectTransform rect = panel.GetComponent<RectTransform>();

        if (canvasGroup != null && rect != null)
        {
            panel.SetActive(true);

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            rect.localScale = Vector3.zero;

            canvasGroup.DOFade(1f, 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                });

            rect.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack);
        }
    }

    public void HideWinPanel()
    {
        HidePanel(winPanel);
    }

    public void HideLosePanel()
    {
        HidePanel(losePanel);
    }

    private void HidePanel(GameObject panel)
    {
        if (panel == null) return;

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        RectTransform rect = panel.GetComponent<RectTransform>();

        if (canvasGroup != null && rect != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(0f, 0.4f).SetEase(Ease.InQuad);
            rect.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    panel.SetActive(false);
                });
        }
    }

    public void SetupEndPanelButtons(System.Action onNextLevel, System.Action onReturnToMenu, System.Action onRetry, System.Action onQuit)
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(() => onNextLevel?.Invoke());
        }

        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveAllListeners();
            returnToMenuButton.onClick.AddListener(() => onReturnToMenu?.Invoke());
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(() => onRetry?.Invoke());
        }

        if (quitGameButton != null)
        {
            quitGameButton.onClick.RemoveAllListeners();
            quitGameButton.onClick.AddListener(() => onQuit?.Invoke());
        }
    }
}
