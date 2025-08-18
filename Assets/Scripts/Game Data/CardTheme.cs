using UnityEngine;

[CreateAssetMenu(fileName = "CardTheme", menuName = "Memotest/Card Theme")]
public class CardTheme : ScriptableObject
{
    [Header("Sprites")]
    [Tooltip("Sprite del dorso")]
    [SerializeField] private Sprite backSprite;

    [Tooltip("Lista de sprites frontales")]
    [SerializeField] private Sprite[] pairFronts;

    public Sprite BackSprite => backSprite;
    public Sprite[] PairFronts => pairFronts;
}
