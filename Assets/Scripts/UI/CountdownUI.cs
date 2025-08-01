using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0;
        countdownText.gameObject.SetActive(false);
    }

    public void StartCountdown(System.Action onComplete)
    {
        StartCoroutine(CountdownRoutine(onComplete));
    }

    private IEnumerator CountdownRoutine(System.Action onComplete)
    {
        canvasGroup.alpha = 1;
        countdownText.gameObject.SetActive(true);

        string[] sequence = new string[] { "3", "2", "1", "¡MEMORIZÁ!" };

        foreach (var text in sequence)
        {
            countdownText.text = text;
            countdownText.transform.localScale = Vector3.zero;

            countdownText.transform.DOScale(1.2f, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    countdownText.transform.DOScale(1f, 0.1f);
                });

            yield return new WaitForSeconds(1f);
        }

        countdownText.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        canvasGroup.DOFade(0, 0.3f).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(0.3f);

        countdownText.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}
