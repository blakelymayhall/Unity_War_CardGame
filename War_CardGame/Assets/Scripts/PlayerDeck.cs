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
        COM_deck = GameObject.Find("COM").transform.Find("Deck").gameObject.GetComponent<Deck>();
    }

    //======================================================================
    public override void InstantiateCard()
    {
        base.InstantiateCard();
        playedCards.Last().isCOM = false;
    }
}

