using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //======================================================================
    public Sprite faceDownSprite;
    public CardData cardData = new();

    //======================================================================
    private Sprite faceUpSprite;
    private SpriteRenderer spriteRenderer;

    //======================================================================
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        faceUpSprite = AssignFaceUpSprite(cardData.cardRank, cardData.cardSuit);
        spriteRenderer.sprite = faceDownSprite;
    }

    //======================================================================
    void Update()
    {
        if (cardData.isFaceUp && spriteRenderer.sprite != faceUpSprite)
        {
            spriteRenderer.sprite = faceUpSprite;
        }
        else if (!cardData.isFaceUp && spriteRenderer.sprite != faceDownSprite)
        {
            spriteRenderer.sprite = faceDownSprite;
        }
    }

    //======================================================================
    void OnMouseDown()
    {
        cardData.isFaceUp = !cardData.isFaceUp;
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
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(
            cardFileName + ".png");
    }
}
