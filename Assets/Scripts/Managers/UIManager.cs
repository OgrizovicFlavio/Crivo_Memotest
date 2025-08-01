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

    [Header("Countdown Panel")]
    [SerializeField] private CountdownUI countdownUI;

    [Header("End Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Win Panel Sections")]
    [SerializeField] private GameObject winButtons;
    [SerializeField] private GameObject winCreditsPanel;

    [Header("Lose Panel Sections")]
    [SerializeField] private GameObject loseButtons;
    [SerializeField] private GameObject loseCreditsPanel;

    private int currentLevel = 1;

    private void Awake()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (winCreditsPanel != null) winCreditsPanel.SetActive(false);
        if (loseCreditsPanel != null) loseCreditsPanel.SetActive(false);
    }

    public void SetAttempts(int attempts, bool showValue = true)
    {
        attemptsText.text = !showValue ? "INTENTOS:" : (attempts < 0 ? "INTENTOS: ∞" : $"INTENTOS: {attempts}");
    }

    public void SetProgress(int currentScore, int totalPairs)
    {
        float fill = (float)currentScore / totalPairs;
        if (progressBarFill != null && totalPairs > 0)
            progressBarFill.DOFillAmount(fill, 0.3f).SetEase(Ease.OutQuad);
    }

    public void ResetProgressBar()
    {
        if (progressBarFill != null)
        {
            progressBarFill.DOKill();

            progressBarFill.DOFillAmount(0f, 0.4f)
                .SetEase(Ease.InOutQuad);
        }
    }

    public void SetTime(float time, bool showValue = true)
    {
        timerText.text = !showValue ? "TIEMPO:" : (time < 0f ? "TIEMPO: ∞" : $"TIEMPO: {Mathf.CeilToInt(time)}");
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
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

    public void PulseAttempts()
    {
        if (attemptsText == null) return;

        attemptsText.transform.DOKill();
        attemptsText.transform.localScale = Vector3.one;

        attemptsText.transform.DOScale(1.2f, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    public void ShowWinPanel(bool hasNextLevel)
    {
        bool isFinalLevel = currentLevel >= 3;

        if (winButtons != null)
            winButtons.SetActive(!isFinalLevel);

        if (winCreditsPanel != null)
            winCreditsPanel.SetActive(isFinalLevel);

        AnimatePanel(winPanel);
    }

    public void ShowLosePanel()
    {
        bool isFinalLevel = currentLevel >= 3;

        if (loseButtons != null)
            loseButtons.SetActive(!isFinalLevel);

        if (loseCreditsPanel != null)
            loseCreditsPanel.SetActive(isFinalLevel);

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

            canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });

            rect.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack);
        }
    }

    public void HideWinPanel(System.Action onComplete = null)
    {
        HidePanel(winPanel, onComplete);
    }

    public void HideLosePanel(System.Action onComplete = null)
    {
        HidePanel(losePanel, onComplete);
    }

    private void HidePanel(GameObject panel, System.Action onComplete = null)
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
                    onComplete?.Invoke();
                });
        }
        else
        {
            panel.SetActive(false);
            onComplete?.Invoke();
        }
    }

    public void StartCountdown(System.Action onComplete)
    {
        if (countdownUI != null)
        {
            countdownUI.StartCountdown(onComplete);
        }
        else
        {
            onComplete?.Invoke();
        }
    }
}
