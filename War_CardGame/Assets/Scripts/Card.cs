using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Card : MonoBehaviour
{
    //======================================================================
    public Sprite faceDownSprite;
    public CardData cardData = new();
    public bool isCOM;

    //======================================================================
    private Sprite faceUpSprite;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;

    //======================================================================
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        faceUpSprite = AssignFaceUpSprite(cardData);
        spriteRenderer.sprite = faceDownSprite;
        gameManager = GameObject.Find("Player").GetComponent<GameManager>();
    }

    //======================================================================
    void OnMouseDown()
    {
        bool ignore_click =
            isCOM ||
            cardData.isFaceUp ||
            cardData != gameManager.player.deck.playedCards.Last().cardData;
        if (ignore_click)
        {
            return;
        }

        FlipCards();
        gameManager.SetHandWinLoss();
        gameManager.CheckMatchWinLoss();
    }

    //======================================================================
    void FlipCards()
    {
        gameManager.player.deck.playedCards.Last().cardData.isFaceUp = true;
        gameManager.com.deck.playedCards.Last().cardData.isFaceUp = true;
        spriteRenderer.sprite = faceUpSprite;
        gameManager.com.deck.playedCards.Last().GetComponent<SpriteRenderer>().sprite = 
            gameManager.com.deck.playedCards.Last().faceUpSprite;
    }

    //======================================================================
    public static Sprite AssignFaceUpSprite(CardData cardData)
    {
        // Build up card file name to load from assets
        string cardFileName = "Assets/Sprites/Cards/card";
        cardFileName += cardData.cardSuit.ToString();
        cardFileName += cardData.cardRank switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            14 => "A",
            _ => cardData.cardRank.ToString(),
        };
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(
            cardFileName + ".png");
    }
}
