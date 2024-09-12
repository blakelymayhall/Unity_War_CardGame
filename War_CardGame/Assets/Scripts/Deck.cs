using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Deck : MonoBehaviour
{
    //======================================================================
    public int max_cards;
    public GameObject cardPrefab;
    public List<Card> playedCards = new();
    public List<CardData> cards = new();
    public Player deck_owner;
    public Player opponent;
    public BurntDeck burnDeck; 
    public GameObject cardCount;

    //======================================================================
    protected Vector3 playedCardOffset;
    protected SpriteRenderer spriteRenderer;
    protected GameManager gameManager;

    //======================================================================
    public virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("Player").GetComponent<GameManager>();

        //LoadCards();
        _Debug_LoadCards(max_cards);
        Shuffle();

        var tm = cardCount.GetComponent<TextMeshProUGUI>();
        tm.text = "Cards:\n" + cards.Count;
    }

    //======================================================================
    void OnMouseDown()
    {
        Debug.Log(cards.Count());
        _Debug_PrintDeck(cards);
    }

    //======================================================================
    // Loads random cards into the deck
    void _Debug_LoadCards(int max_cards)
    {
        for (int ii = 0; ii < max_cards; ii++)
        {
            CardData card = new();
            while (cards.Any(card_in_deck => card == card_in_deck))
            {
                card = new();
            }
            cards.Add(card);
        }
    }

    //======================================================================
    public void LoadCards(List<CardData> cards_to_load = null)
    {
        // If didn't get cards to load, load standard 52 card deck
        // todo -- is this terrible? This works and its short
        if (cards_to_load == null)
        {
            const int MAX_CARDS = 52;
            for (int ii = 0; ii < MAX_CARDS; ii++)
            {
                CardData card = new();
                while (cards.Any(card_in_deck => card == card_in_deck))
                {
                    card = new();
                }
                cards.Add(card);
            }
            return;
        }
        else 
        {
            foreach (CardData cardData in cards_to_load)
            {
                cards.Add(cardData);
            }
        }
    }

    //======================================================================
    public void _Debug_PrintDeck(List<CardData> cards)
    {
        foreach (CardData card in cards)
        {
            Debug.Log(card.cardRank + " " + card.cardSuit);
        }
        Debug.Log("==");
    }

    //======================================================================
    public void Shuffle()
    {
        System.Random random = new System.Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            CardData value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    //======================================================================
    public void DrawCard()
    {
        if (playedCards.Count() == 0)
        {
            InstantiateCard();
        }
        else if (cards.Count >= 2)
        {
            // Only other way to get here is if there was a draw on the 
            // previous hand, and both players have sufficient cards to war
            InstantiateCard();
            InstantiateCard();
        }

        gameManager.UpdateCardCounter(cardCount, cards.Count);
    }

    //======================================================================
    public void AddCardsToBurnPile(bool finalCards = false)
    {
        if (deck_owner.PlayerWin())
        {
            List<Card> cardsToBurn = playedCards.Concat(
                opponent.GetComponentInChildren<Deck>().playedCards).ToList();
            burnDeck.AddCards(cardsToBurn);
        }

        if (finalCards || deck_owner.PlayerPush()) 
        {
            // If there were not enough cards to war, and both players have equal 
            // remaining cards, this branch returns the played cards to each
            // player's hands
            if (deck_owner.PlayerDraw() || deck_owner.PlayerPush())
            {
                List<Card> cardsToBurn = playedCards;
                burnDeck.AddCards(cardsToBurn);
            }
        }
    }

    //======================================================================
    public void ClearPlayedCards(bool finalCards = false)
    {
        if (!deck_owner.PlayerDraw() || finalCards)
        {
            foreach (Card card in playedCards)
            {
                Destroy(card.gameObject);
            }
            playedCards.Clear();
        }
    }

    //======================================================================
    public void ResetAfterRound()
    {
        foreach (CardData cardData in burnDeck.cards)
        {
            cardData.isFaceUp = false;
            cards.Add(cardData);
        }
        burnDeck.cards.Clear();
        Shuffle();
        spriteRenderer.sprite = cardPrefab.GetComponent<SpriteRenderer>().sprite;
        burnDeck.GetComponent<SpriteRenderer>().sprite = null;
        gameManager.ActivateDrawButton();
        gameManager.UpdateCardCounter(cardCount, cards.Count);
    }

    //======================================================================
    public void ResetAfterMatch()
    {
        burnDeck.cards.Clear();
        burnDeck.GetComponent<SpriteRenderer>().sprite = null;
        spriteRenderer.sprite = cardPrefab.GetComponent<SpriteRenderer>().sprite;
        cards.Clear();
        ClearPlayedCards(true);
        _Debug_LoadCards(max_cards);
        Shuffle();
        gameManager.ActivateDrawButton();
        gameManager.UpdateCardCounter(cardCount, cards.Count);
    }

    //======================================================================
    public void InstantiateCard()
    {
        if (playedCards.Count() == 0)
        {
            playedCardOffset = new Vector3(
                2f * spriteRenderer.sprite.bounds.size.x,
                deck_owner.isCOM ? -1f : 1f,
                0);
        }
        else
        {
            playedCardOffset += new Vector3(
                0.25f * spriteRenderer.sprite.bounds.size.x,
                deck_owner.isCOM ? -1f : 1f,
                -1);
        }

        playedCards.Add(Instantiate(cardPrefab,
            transform.position + playedCardOffset,
            Quaternion.identity,
            GetComponent<Transform>()).GetComponent<Card>());
        playedCards.Last().name = "playedCard";
        playedCards.Last().cardData = cards.Last();
        playedCards.Last().isCOM = deck_owner.isCOM;

        cards.RemoveAt(cards.Count - 1);
        if (!cards.Any())
        {
            spriteRenderer.sprite = null;
        }
    }
}
