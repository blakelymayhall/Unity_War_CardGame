using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //======================================================================
    public enum CardSuit {
        Clubs,
        Spades,
        Hearts,
        Diamonds
    }
    public Sprite faceDownSprite;

    //======================================================================
    private bool isFaceUp;
    private int cardRank;
    private CardSuit cardSuit;
    private Sprite faceUpSprite;
   
    private SpriteRenderer spriteRenderer;

    //======================================================================
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // For getting started, randomize which suit and rank
        cardSuit = _Debug_AssignRandomSuit();
        cardRank =  Random.Range(2, 15);

        // Get the face up sprite
        faceUpSprite = AssignFaceUpSprite(cardRank, cardSuit);
       
        // Init face down
        isFaceUp = false;
        spriteRenderer.sprite = faceDownSprite;
    }

    //======================================================================
    void Update()
    {
        if (isFaceUp && spriteRenderer.sprite != faceUpSprite)
        {
            spriteRenderer.sprite = faceUpSprite;
        }
        else if(!isFaceUp && spriteRenderer.sprite != faceDownSprite)
        {
            spriteRenderer.sprite = faceDownSprite;
        }
    }

    //======================================================================
    void OnMouseDown()
    {
        isFaceUp = !isFaceUp;
    }

    //======================================================================
    CardSuit _Debug_AssignRandomSuit()
    {
        System.Array values = System.Enum.GetValues(typeof(CardSuit));
        int randomIndex = Random.Range(0, values.Length);
        return (CardSuit)values.GetValue(randomIndex);
    }

    //======================================================================
    Sprite AssignFaceUpSprite(int cardRank, CardSuit cardSuit)
    {
        // Build up card file name to load from assets
        string cardFileName = "Assets/Sprites/Cards/card";
        cardFileName += cardSuit.ToString();
        cardFileName += cardRank switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            14 => "A",
            _ => cardRank.ToString(),
        };
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(cardFileName + ".png");
    }
}
