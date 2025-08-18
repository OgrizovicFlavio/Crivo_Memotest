using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private Image frontImage;
    [SerializeField] private Image backImage;
    [SerializeField] private Button button;

    [Header("FX")]
    [SerializeField] private CardFXRouter fxRouter;

    public CardData Data { get; private set; }
    private bool isFlipped = false;
    private CardBoardManager boardManager;

    void Awake()
    {
        if (!fxRouter) fxRouter = GetComponent<CardFXRouter>();
    }

    public void Initialize(CardData data, Sprite frontSprite, Sprite backSprite, CardBoardManager manager)
    {
        Data = data;
        frontImage.sprite = frontSprite;
        backImage.sprite = backSprite;
        boardManager = manager;

        if (fxRouter)
        {
            fxRouter.frontImage = frontImage; // por si no estaba seteado
            fxRouter.RefreshFxForFront();
            fxRouter.SetFxVisible(false);
        }

        SetFlipped(false, true);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    public void SetFlipped(bool flipped, bool instant = false)
    {
        isFlipped = flipped;

        if (instant)
        {
            frontImage.gameObject.SetActive(flipped);
            backImage.gameObject.SetActive(!flipped);

            if (fxRouter)
            {
                fxRouter.RefreshFxForFront();
                fxRouter.SetFxVisible(isFlipped);
                if (isFlipped) fxRouter.PlayRevealBurstIfAny();
            }
            return;
        }

        button.interactable = false;

        transform.DOScaleX(0f, 0.15f).OnComplete(() =>
        {
            frontImage.gameObject.SetActive(flipped);
            backImage.gameObject.SetActive(!flipped);

            transform.DOScaleX(1f, 0.15f).OnComplete(() =>
            {
                button.interactable = true;

                if (fxRouter)
                {
                    fxRouter.RefreshFxForFront();
                    fxRouter.SetFxVisible(isFlipped);
                    if (isFlipped) fxRouter.PlayRevealBurstIfAny();
                }
            });
        });
    }

    public void AnimateSpawn()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 1.3f)
            .SetEase(Ease.OutElastic, 1f, 0.3f)
            .SetDelay(Random.Range(0f, 0.2f));
    }

    public bool IsFlipped() => isFlipped;
    private void OnClick() => boardManager.OnCardClicked(this);

    public void DisableInteraction()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = false;
        }
    }
}