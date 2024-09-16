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
    public Sprite faceUpSprite;
    public CardAnimator cardAnimator;

    //======================================================================
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;

    //======================================================================
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        faceUpSprite = AssignFaceUpSprite(cardData);
        spriteRenderer.sprite = faceDownSprite;
        gameManager = GameObject.Find("Player").GetComponent<GameManager>();
        cardAnimator.GetComponent<CardAnimator>();
    }

    //======================================================================
    void OnMouseDown()
    {
        Debug.Log(isCOM);
        Debug.Log(cardData.isFaceUp);
        Debug.Log(cardData != gameManager.player.deck.playedCards.Last().cardData);
        bool ignore_click =
            isCOM ||
            cardData.isFaceUp ||
            cardData != gameManager.player.deck.playedCards.Last().cardData;
        if (ignore_click)
        {
            return;
        }
        
        cardAnimator.StartFlipCard();
        gameManager.SetHandWinLoss();
        gameManager.CheckMatchWinLoss();
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
