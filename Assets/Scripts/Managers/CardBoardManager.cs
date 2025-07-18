using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBoardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Transform container; //UI Grid

    [Header("Sprites")]
    [SerializeField] private Sprite[] frontSprites;
    [SerializeField] private Sprite backSprite;

    [Header("Configuration")]
    [SerializeField] private int totalPairs = 6;
    [SerializeField] public int attempts = 10;

    private GameLogic gameLogic;
    private List<CardView> cardViews = new();
    private CardView firstCardFlipped;

    private void Start()
    {
        if (frontSprites.Length < totalPairs)
        {
            Debug.LogError("No hay suficientes sprites frontales para los pares indicados.");
            return;
        }

        gameLogic = new GameLogic(totalPairs, attempts);

        for (int i = 0; i < gameLogic.Cards.Count; i++)
        {
            var data = gameLogic.Cards[i];
            var card = Instantiate(cardPrefab, container);

            var sprite = frontSprites[data.Id];

            card.Initialize(data, sprite, backSprite, this);

            cardViews.Add(card);
        }
    }

    public void OnCardClicked(CardView clicked)
    {
        if (clicked.IsFlipped()) return;

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
            Debug.Log("¡Par encontrado!");

            // Remover listeners: ya no deben reaccionar al click
            firstCardFlipped.DisableInteraction();
            clicked.DisableInteraction();
        }
        else
        {
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
}
