using System.Collections;
using System.Collections.Generic;
using System;

public class CardData
{
    //======================================================================
    public bool isFaceUp;
    public int cardRank;
    public CardSuit cardSuit;

    //======================================================================
    public CardData()
    {
        // For getting started, randomize which suit and rank
        cardSuit = AssignRandomSuit();
        Random rand = new();
        cardRank = rand.Next(2, 15);

        // Init face down
        isFaceUp = false;
    }

    //======================================================================
    CardSuit AssignRandomSuit()
    {
        System.Array values = System.Enum.GetValues(typeof(CardSuit));
        Random rand = new();
        int randomIndex = rand.Next(0, values.Length);
        return (CardSuit)values.GetValue(randomIndex);
    }

    //======================================================================
    public static bool operator ==(CardData card1, CardData card2)
    {
        return card1.cardRank == card2.cardRank &&
            card1.cardSuit == card2.cardSuit;
    }

    //======================================================================
    public static bool operator !=(CardData card1, CardData card2)
    {
        return card1.cardRank != card2.cardRank ||
            card1.cardSuit != card2.cardSuit;
    }

    //======================================================================
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CardData)obj);
    }

    //======================================================================
    public override int GetHashCode()
    {
        unchecked
        {
            return (int)cardSuit ^ cardRank;
        }
    }
}

