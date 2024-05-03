using System;
using System.Linq;
using UnityEngine;

public class PlayerDeck : Deck
{
    //======================================================================
    private Deck COM_deck;

    //======================================================================
    public override void Start()
    {
        base.Start();
        COM_deck = GameObject.Find("COM").GetComponentInChildren<Deck>();
    }

    //======================================================================
    void OnMouseDown()
    {
        base.PlayCard();
        COM_deck.PlayCard();

        Debug.Log("Remaining Cards " + cards.Count);
    }

    //======================================================================
    public override void InstantiateCard()
    {
        base.InstantiateCard();
        playedCards.Last().isCOM = false;
    }
}

