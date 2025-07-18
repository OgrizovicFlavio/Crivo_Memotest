using UnityEngine;
using System.Collections.Generic;

// Lógica principal del memotest: mazo, intentos, puntaje y chequeo de match.
public class GameLogic
{
    // Lista de todas las cartas mezcladas (cada par está duplicado)
    public List<CardData> Cards { get; private set; }

    // Intentos restantes del jugador (se resta cuando gira 2 cartas)
    public int AttemptsRemaining { get; private set; }

    // Puntos del jugador: se suma 1 por cada par encontrado
    public int Score { get; private set; }


    // Constructor: recibe la cantidad de pares a generar y la cantidad de intentos
    public GameLogic(int totalPairs, int attempts)
    {
        AttemptsRemaining = attempts;
        Score = 0;
        Cards = GenerateDeck(totalPairs); // Genera el mazo y lo mezcla
    }

    // Crea un mazo con pares de cartas (cada par con el mismo ID)
    private List<CardData> GenerateDeck(int totalPairs)
    {
        List<CardData> deck = new();

        for (int i = 0; i < totalPairs; i++)
        {
            string name = $"Carta {i + 1}";

            // Se agregan dos cartas con el mismo ID y nombre
            deck.Add(new CardData(i, name));
            deck.Add(new CardData(i, name));
        }

        Shuffle(deck); // Se mezclan aleatoriamente
        return deck;
    }

    // Mezcla la lista de cartas usando el algoritmo Fisher-Yates
    private void Shuffle(List<CardData> list)
    {
        var random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]); // Intercambia posiciones
        }    
    }

    // Intenta dar vuelta una carta y verificar si hay match con la anterior
    // Devuelve true si se completó un intento (es decir, se giraron 2 cartas)
    public bool TryMatch(CardData first, CardData second, out bool matched)
    {
        matched = false;
        AttemptsRemaining--;

        if (first.Equals(second))
        {
            Score++;
            matched = true;
        }

        Debug.Log($"Intentos restantes: {AttemptsRemaining}");
        Debug.Log($"Puntaje actual: {Score}");

        return true; // Siempre devuelve true porque se completó un intento
    }
}
