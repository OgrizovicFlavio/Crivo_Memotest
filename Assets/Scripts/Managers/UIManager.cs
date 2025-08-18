using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI attemptsText; // número del círculo
    [SerializeField] private TextMeshProUGUI timerText;    // número del círculo
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image progressBarFill;
    [SerializeField] private ProgressBarShine progressBarShine;

    [Header("Countdown Panel")]
    [SerializeField] private CountdownUI countdownUI;

    [Header("HUD Containers")]
    [SerializeField] private GameObject topLeftGroup;   // INTENTOS (label + círculo)
    [SerializeField] private GameObject topRightGroup;  // TIEMPO   (label + círculo)

    [Header("End Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    // Variantes “final de juego”
    [SerializeField] private GameObject winFinalPanel;
    [SerializeField] private GameObject loseFinalPanel;

    private int currentLevel = 1;

    private void Awake()
    {
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
        if (winFinalPanel) winFinalPanel.SetActive(false);
        if (loseFinalPanel) loseFinalPanel.SetActive(false);
    }

    // -------- HUD numbers --------
    public void SetAttempts(int attempts, bool showValue = true)
    {
        if (!attemptsText) return;
        attemptsText.text = showValue ? (attempts < 0 ? "∞" : attempts.ToString()) : "";
    }

    public void SetTime(float time, bool showValue = true)
    {
        if (!timerText) return;
        timerText.text = showValue ? (time < 0f ? "∞" : Mathf.CeilToInt(time).ToString()) : "";
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
        if (levelText) levelText.text = $"NIVEL {level}";
    }

    public void SetStatsVisibility(bool showAttempts, bool showTime)
    {
        if (topLeftGroup) topLeftGroup.SetActive(showAttempts);
        if (topRightGroup) topRightGroup.SetActive(showTime);
    }

    // -------- Barra de progreso --------
    public void SetProgress(int currentScore, int totalPairs)
    {
        if (progressBarFill && totalPairs > 0)
        {
            float fill = (float)currentScore / totalPairs;
            progressBarFill.DOFillAmount(fill, 0.3f).SetEase(Ease.OutQuad);
        }
    }

    public void ResetProgressBar()
    {
        if (!progressBarFill) return;
        progressBarFill.DOKill();
        progressBarFill.DOFillAmount(0f, 0.4f).SetEase(Ease.InOutQuad);
    }

    // -------- Inicialización --------
    public void InitializeUI(int level, int attempts, float time, int currentScore, int totalPairs)
    {
        SetLevel(level);
        progressBarShine?.ResetEffects(0f);
        ResetProgressBar();
        SetProgress(currentScore, totalPairs);
    }

    // -------- Mostrar/ocultar números --------
    public void HideGameplayTexts()
    {
        if (attemptsText) attemptsText.gameObject.SetActive(false);
        if (timerText) timerText.gameObject.SetActive(false);
    }

    public void ShowGameplayTexts()
    {
        if (attemptsText) attemptsText.gameObject.SetActive(true);
        if (timerText) timerText.gameObject.SetActive(true);
    }

    // -------- Efectos --------
    public void PulseTimer()
    {
        if (!timerText) return;
        timerText.transform.DOKill();
        timerText.transform.localScale = Vector3.one;
        timerText.transform
            .DOScale(1.2f, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    public void PulseAttempts()
    {
        if (!attemptsText) return;
        attemptsText.transform.DOKill();
        attemptsText.transform.localScale = Vector3.one;
        attemptsText.transform
            .DOScale(1.2f, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    // -------- Panels --------
    public void ShowWinPanel(bool hasNextLevel)
    {
        if (hasNextLevel || !winFinalPanel)
            AnimatePanel(winPanel);
        else
            AnimatePanel(winFinalPanel);
    }

    public void ShowLosePanel()
    {
        bool isFinalLevel = currentLevel >= 3;
        if (isFinalLevel && loseFinalPanel)
            AnimatePanel(loseFinalPanel);
        else
            AnimatePanel(losePanel);
    }

    private void AnimatePanel(GameObject panel)
    {
        if (!panel) return;

        var cg = panel.GetComponent<CanvasGroup>();
        var rect = panel.GetComponent<RectTransform>();

        if (cg && rect)
        {
            panel.SetActive(true);
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
            rect.localScale = Vector3.zero;

            cg.DOFade(1f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
            });

            rect.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack);
        }
        else
        {
            panel.SetActive(true);
        }
    }

    public void HideWinPanel(System.Action onComplete = null)
    {
        if (winPanel && winPanel.activeSelf) { HidePanel(winPanel, onComplete); return; }
        if (winFinalPanel && winFinalPanel.activeSelf) { HidePanel(winFinalPanel, onComplete); return; }
        onComplete?.Invoke();
    }

    public void HideLosePanel(System.Action onComplete = null)
    {
        if (losePanel && losePanel.activeSelf) { HidePanel(losePanel, onComplete); return; }
        if (loseFinalPanel && loseFinalPanel.activeSelf) { HidePanel(loseFinalPanel, onComplete); return; }
        onComplete?.Invoke();
    }

    private void HidePanel(GameObject panel, System.Action onComplete = null)
    {
        if (!panel) { onComplete?.Invoke(); return; }

        var cg = panel.GetComponent<CanvasGroup>();
        var rect = panel.GetComponent<RectTransform>();

        if (cg && rect)
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;

            cg.DOFade(0f, 0.4f).SetEase(Ease.InQuad);
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

    // -------- Countdown passthrough --------
    public void StartCountdown(System.Action onComplete)
    {
        if (countdownUI) countdownUI.StartCountdown(onComplete);
        else onComplete?.Invoke();
    }
}
