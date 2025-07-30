using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Memotest/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public int totalPairs;
    public int maxAttempts;
    public float timeLimit;
    public float memorizeDuration;
}
