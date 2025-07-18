using UnityEngine;
using UnityEngine.UI;

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
    public void Initialize(CardData data, Sprite frontSprite, Sprite backSprite, CardBoardManager manager)
    {
        Data = data;
        frontImage.sprite = frontSprite;
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

        // Acá podés reemplazar por animación si querés
        frontImage.gameObject.SetActive(flipped);
        backImage.gameObject.SetActive(!flipped);
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