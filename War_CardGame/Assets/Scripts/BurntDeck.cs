using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class BurntDeck : Deck
{
    public Sprite faceDownSprite;

    //======================================================================
    public override void Start()
    {
        deck_owner = GetComponentInParent<Player>();
        var players = FindObjectsOfType<Player>();
        opponent = players.FirstOrDefault(p => p != deck_owner);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = null;
    }

    //======================================================================
    void OnMouseDown()
    {
        Debug.Log("Burnt Cards " + cards.Count);
    }
    
    //======================================================================
    public void AddCards(List<Card> cardsToBurn)
    {
        foreach (Card card in cardsToBurn)
        {
            cards.Add(card.cardData);
        }

        if (cards.Count > 1 && spriteRenderer.sprite == null) 
        {
            spriteRenderer.sprite = faceDownSprite;
        }
    }
}

