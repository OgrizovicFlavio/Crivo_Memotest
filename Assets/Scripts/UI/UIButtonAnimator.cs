using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class UIButtonAnimator : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float punchScale = 0.2f;
    [SerializeField] private float punchDuration = 0.2f;

    [Header("Delay Settings")]
    [SerializeField] private float delayAfterClick = 0.25f;

    [Header("Action to Execute After Delay")]
    public UnityEvent onClickDelayed;

    private Button button;
    private RectTransform rectTransform;
    private bool isClickable = true;

    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();

        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (!isClickable) return;

        isClickable = false;

        rectTransform.DOKill();
        rectTransform.DOPunchScale(Vector3.one * punchScale, punchDuration, 10, 1f);

        Invoke(nameof(ExecuteDelayedAction), delayAfterClick);
    }

    private void ExecuteDelayedAction()
    {
        onClickDelayed?.Invoke();
        isClickable = true;
    }
}
