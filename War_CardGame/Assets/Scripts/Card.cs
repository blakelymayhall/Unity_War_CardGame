using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardSuit {
        Clubs,
        Spades,
        Hearts,
        Diamonds
    }
    public Sprite faceDownSprite;

    private bool isFaceUp;
    private int cardRank;
    private CardSuit cardSuit;
    private Sprite faceUpSprite;
   
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // For getting started, randomize which suit and rank
        cardSuit = _Debug_AssignRandomSuit();
        cardRank =  Random.Range(2, 15);

        // Assign the sprite
        faceUpSprite = AssignSprite(cardRank, cardSuit);
        spriteRenderer.sprite = faceUpSprite;
    }

    // Update is called once per frame
    void Update()
    {

    }

    CardSuit _Debug_AssignRandomSuit()
    {
        System.Array values = System.Enum.GetValues(typeof(CardSuit));
        int randomIndex = Random.Range(0, values.Length);
        return (CardSuit)values.GetValue(randomIndex);
    }

    Sprite AssignSprite(int cardRank, CardSuit cardSuit)
    {
        // Build up card file name to load from assets
        string cardFileName = "Assets/Sprites/Cards/card";
        cardFileName += cardSuit.ToString();
        switch (cardRank) {
            case 11:
                cardFileName += "J";
                break;
            case 12:
                cardFileName += "Q";
                break;
            case 13:
                cardFileName += "K";
                break;
            case 14:
                cardFileName += "A";
                break; 
            default:
                cardFileName += cardRank.ToString();
                break;
        }
        Debug.Log(cardFileName + ".png");
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(cardFileName + ".png");
    }
}
