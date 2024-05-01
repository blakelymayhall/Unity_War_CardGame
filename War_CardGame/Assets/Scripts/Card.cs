using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //======================================================================
    public Sprite faceDownSprite;
    public CardData cardData = new();
    public bool isCOM;

    //======================================================================
    private Sprite faceUpSprite;
    private SpriteRenderer spriteRenderer;
    private Deck COM_deck;

    //======================================================================
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        faceUpSprite = AssignFaceUpSprite(cardData.cardRank, cardData.cardSuit);
        spriteRenderer.sprite = faceDownSprite;

        COM_deck = GameObject.Find("COM").GetComponentInChildren<Deck>();
    }

    //======================================================================
    void Update()
    {
        if (cardData.isFaceUp && spriteRenderer.sprite != faceUpSprite)
        {
            spriteRenderer.sprite = faceUpSprite;
        }
    }

    //======================================================================
    void OnMouseDown()
    {
        if (isCOM)
        {
            // Do this so they can share a script
            // Don't want to be able to click on COM's cards
            return;
        }

        cardData.isFaceUp = true;
        COM_deck.GetComponentInChildren<Card>().cardData.isFaceUp = true;
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
