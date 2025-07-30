using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBoardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private UIManager uiManager;

    [Header("Sprites")]
    [SerializeField] private Sprite[] frontSprites;
    [SerializeField] private Sprite backSprite;

    [Header("Debug Options")]
    [SerializeField] private bool useDynamicColors = true;

    private GameLogic gameLogic;
    private List<CardView> cardViews = new();
    private CardView firstCardFlipped;
    private bool interactionLocked = false;

    public void InitializeBoard(int level, int totalPairs, int attempts, float timeLimit, float memorizeTime)
    {
        if (!useDynamicColors && frontSprites.Length < totalPairs)
        {
            Debug.LogError("No hay suficientes sprites frontales para los pares indicados.");
            return;
        }

        ClearBoard();
        gameLogic = new GameLogic(totalPairs, attempts);
        uiManager?.SetLevel(level);

        // Crear cartas
        for (int i = 0; i < gameLogic.Cards.Count; i++)
        {
            var data = gameLogic.Cards[i];
            var card = Instantiate(cardPrefab, container);

            Sprite spriteToUse;
            Color colorToUse;

            if (useDynamicColors)
            {
                spriteToUse = frontSprites.Length > 0 ? frontSprites[0] : null;
                colorToUse = GenerateColorById(data.Id);
            }
            else
            {
                spriteToUse = frontSprites[data.Id];
                colorToUse = Color.white;
            }

            card.Initialize(data, spriteToUse, backSprite, this, colorToUse);
            card.AnimateSpawn();
            cardViews.Add(card);
        }

        // Bloquear input
        interactionLocked = true;

        // Ocultar instantáneamente todas las cartas
        foreach (var card in cardViews)
            card.SetFlipped(false, true); // true = sin animación

        uiManager?.HideGameplayTexts();

        // Comenzar fase de memorización
        StartCoroutine(MemorizeAndStart());
    }

    private IEnumerator MemorizeAndStart()
    {
        LevelData data = GameManager.Instance.GetLevelData();

        yield return new WaitForSeconds(2f);

        foreach (var card in cardViews)
            card.SetFlipped(true, false);

        yield return new WaitForSeconds(data.memorizeDuration);

        GameManager.Instance.InitializeGame(data);

        uiManager?.ShowGameplayTexts();

        foreach (var card in cardViews)
            card.SetFlipped(false, false);

        yield return new WaitForSeconds(0.3f);

        interactionLocked = false;

        GameManager.Instance.StartTimer();
    }

    private void ClearBoard()
    {
        foreach (var card in cardViews)
            Destroy(card.gameObject);

        cardViews.Clear();
    }

    public void OnCardClicked(CardView clicked)
    {
        if (interactionLocked || clicked.IsFlipped() || GameManager.Instance.GameFinished)
            return;

        clicked.SetFlipped(true);

        if (firstCardFlipped == null)
        {
            firstCardFlipped = clicked;
            return;
        }

        bool matched;
        gameLogic.TryMatch(firstCardFlipped.Data, clicked.Data, out matched);

        if (matched)
        {
            firstCardFlipped.DisableInteraction();
            clicked.DisableInteraction();
            GameManager.Instance.RegisterMatch();
        }
        else
        {
            GameManager.Instance.RegisterFailedAttempt();
            StartCoroutine(FlipBackAfterDelay(firstCardFlipped, clicked));
        }

        firstCardFlipped = null;
    }

    private IEnumerator FlipBackAfterDelay(CardView a, CardView b)
    {
        yield return new WaitForSeconds(1f);
        a.SetFlipped(false);
        b.SetFlipped(false);
    }

    private Color GenerateColorById(int id)
    {
        Random.InitState(id * 997);
        return new Color(Random.value, Random.value, Random.value);
    }

    public void LoadLevel()
    {
        LevelData data = GameManager.Instance.GetLevelData();

        InitializeBoard(data.levelNumber, data.totalPairs, data.maxAttempts, data.timeLimit, data.memorizeDuration);
    }
}