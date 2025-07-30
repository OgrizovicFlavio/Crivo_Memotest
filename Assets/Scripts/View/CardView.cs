using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardView : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private Image frontImage; // Sprite visible al dar vuelta
    [SerializeField] private Image backImage; // Sprite de dorso
    [SerializeField] private Button button;
    public CardData Data { get; private set; }

    private bool isFlipped = false;
    private CardBoardManager boardManager;

    // Asignamos la información lógica y el sprite frontal
    public void Initialize(CardData data, Sprite frontSprite, Sprite backSprite, CardBoardManager manager, Color frontColor)
    {
        Data = data;
        frontImage.sprite = frontSprite;
        frontImage.color = frontColor;
        backImage.sprite = backSprite;
        boardManager = manager;

        SetFlipped(false, true);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    // Cambia el estado visual de la carta
    public void SetFlipped(bool flipped, bool instant = false)
    {
        isFlipped = flipped;

        if (instant)
        {
            frontImage.gameObject.SetActive(flipped);
            backImage.gameObject.SetActive(!flipped);
            return;
        }

        // Desactivar interacción durante el flip
        button.interactable = false;

        // Fase 1: escalar a X = 0
        transform.DOScaleX(0f, 0.15f).OnComplete(() =>
        {
            // Intercambiar visibilidad
            frontImage.gameObject.SetActive(flipped);
            backImage.gameObject.SetActive(!flipped);

            // Fase 2: volver a escalar a X = 1
            transform.DOScaleX(1f, 0.15f).OnComplete(() =>
            {
                button.interactable = true;
            });
        });
    }

    public void AnimateSpawn()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;

        transform.DOScale(1f, 1.3f) // un poco más lenta
            .SetEase(Ease.OutElastic, 1f, 0.3f) // <== suaviza el rebote
            .SetDelay(Random.Range(0f, 0.2f));
    }

    public bool IsFlipped() => isFlipped;

    private void OnClick()
    {
        boardManager.OnCardClicked(this);
    }

    public void DisableInteraction()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = false;
        }
    }
}