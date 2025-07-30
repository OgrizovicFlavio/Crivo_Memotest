using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardBoardManager boardManager;

    [Header("Level Configuration")]
    [SerializeField] private int level = 1;
    [SerializeField] private int totalPairs = 4;
    [SerializeField] private int attempts = -1;
    [SerializeField] private float timeLimit = -1f;
    [SerializeField] private float memorizeDuration = 1f;

    private void Start()
    {
        boardManager.InitializeBoard(level, totalPairs, attempts, timeLimit, memorizeDuration);
    }
}
