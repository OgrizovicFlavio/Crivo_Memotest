using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBoardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Transform container;

    [Header("UI")]
    [SerializeField] private UIManager uiManager;

    private GameLogic gameLogic;
    private readonly List<CardView> cardViews = new();
    private CardView firstCardFlipped;
    private bool interactionLocked = false;

    public void InitializeBoard(int level, int totalPairs, int attempts, float timeLimit, float memorizeTime)
    {
        // 1) Datos del nivel y theme
        LevelData levelData = GameManager.Instance.GetLevelData();
        CardTheme theme = levelData.theme;

        if (!theme)
        {
            Debug.LogError($"El Level {level} no tiene CardTheme asignado.");
            return;
        }

        var fronts = theme.PairFronts;
        if (fronts == null || fronts.Length < totalPairs)
        {
            Debug.LogError($"Faltan frentes en el CardTheme ({fronts?.Length ?? 0}) para {totalPairs} pares.");
            return;
        }

        // 2) Grilla
        int rows, cols, padding; Vector2 spacing;
        switch (totalPairs)
        {
            case 4: rows = 2; cols = 4; padding = 36; spacing = new Vector2(24, 28); break; // Nivel 1
            case 6: rows = 2; cols = 6; padding = 32; spacing = new Vector2(20, 26); break; // Nivel 2
            case 8: rows = 2; cols = 8; padding = 28; spacing = new Vector2(10, 18); break; // Nivel 3
            default: rows = 2; cols = totalPairs; padding = 32; spacing = new Vector2(20, 24); break;
        }

        var fit = container.GetComponent<GridFit>();
        if (fit)
        {
            fit.aspect = 200f / 300f;
            fit.padding = padding;
            fit.spacing = spacing;
            fit.SetGrid(rows, cols);
        }

        // 3) Limpiar y preparar lógica/UI
        ClearBoard();

        gameLogic = new GameLogic(totalPairs, attempts);
        uiManager?.SetLevel(level);
        uiManager?.ResetProgressBar();

        // 4) Instanciar cartas
        for (int i = 0; i < gameLogic.Cards.Count; i++)
        {
            var data = gameLogic.Cards[i];
            var card = Instantiate(cardPrefab, container);
            Sprite spriteToUse = fronts[data.Id];
            card.Initialize(data, spriteToUse, theme.BackSprite, this);
            card.AnimateSpawn();
            cardViews.Add(card);
        }

        // 5) Estado inicial
        interactionLocked = true;
        foreach (var card in cardViews) card.SetFlipped(false, true); // dorso sin animación

        // ***** Visibilidad de HUD desde el principio del nivel *****
        bool showAttempts = level >= 2 && attempts >= 0;
        bool showTime = level >= 2 && timeLimit >= 0f;

        uiManager?.SetStatsVisibility(showAttempts, showTime);
        uiManager?.SetAttempts(attempts, showAttempts);   // setea valor inicial
        uiManager?.SetTime(timeLimit, showTime);          // setea valor inicial

        // Si querés que los números estén activos/visibles (TMP) ya mismo:
        if (showAttempts || showTime) uiManager?.ShowGameplayTexts();
        else uiManager?.HideGameplayTexts();

        // 6) Cuenta regresiva -> memorizar -> juego
        uiManager.StartCountdown(() =>
        {
            StartCoroutine(MemorizeAndStart());
        });
    }

    private IEnumerator MemorizeAndStart()
    {
        LevelData data = GameManager.Instance.GetLevelData();

        // Mostrar todas para memorizar
        foreach (var card in cardViews) card.SetFlipped(true, false);
        yield return new WaitForSeconds(data.memorizeDuration);

        // Inicializa game state (no toca visibilidad del HUD)
        GameManager.Instance.InitializeGame(data);

        // Ocultar cartas y habilitar interacción
        foreach (var card in cardViews) card.SetFlipped(false, false);
        yield return new WaitForSeconds(0.3f);

        interactionLocked = false;
        GameManager.Instance.StartTimer();
    }

    private void ClearBoard()
    {
        foreach (var card in cardViews) Destroy(card.gameObject);
        cardViews.Clear();
    }

    public void OnCardClicked(CardView clicked)
    {
        if (interactionLocked || clicked.IsFlipped() || GameManager.Instance.GameFinished) return;

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

    public void LoadLevel()
    {
        LevelData data = GameManager.Instance.GetLevelData();
        InitializeBoard(data.levelNumber, data.totalPairs, data.maxAttempts, data.timeLimit, data.memorizeDuration);
    }
}