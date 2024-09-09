using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System;

public class RoundOutcomes
{
    GameManager gameManager;

    // Support Functions
    //======================================================================
    void ClearDecks()
    {
        gameManager.player.deck.cards.Clear();
        gameManager.com.deck.cards.Clear();
        Assert.IsTrue(gameManager.player.deck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.deck.cards.Count == 0);
    }

    // Load cards into the provided deck. The cards are loaded 
    // such that the first list item provided is the bottom most card
    void AddCardsToDeck(Deck deck, List<int> cardRanks)
    {
        List<CardData> cards = new();
        cardRanks.ForEach((cardRank) => {
            CardData card = new();
            card.cardRank = cardRank;
            card.isFaceUp = true;
            card.cardSuit = CardSuit.Diamonds;
            cards.Add(card);
        });
        deck.LoadCards(cards);
    }

    void DrawCardAndFlipCard()
    {
        gameManager.drawButton.onClick.Invoke();
        gameManager.SetHandWinLoss();
        gameManager.CheckMatchWinLoss();
    }
    //======================================================================

    // Tests
    //======================================================================
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        SceneManager.LoadScene("War");

        // Wait until the scene is loaded
        yield return new WaitUntil(() => SceneManager.GetSceneByName("War").isLoaded);  
        gameManager = GameObject.Find("Player").GetComponent<GameManager>();  
    }

    // Test that test code for adding cards works
    //======================================================================
    [UnityTest]
    public IEnumerator AddCardsToDeck_Test()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 8, 11, 8, 2, 13, 5 };
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 7, 10, 6, 2, 14, 5 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);

        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==5));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==13));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==2));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==8));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==11));

        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count);
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==5));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==14));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==2));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==6));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==10));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==7));

        yield return null;
    }

    // Test that the higher rank card wins
    //======================================================================
    // Test higher card wins and confirm that correct cards are added to correct burn pile
    [UnityTest]
    public IEnumerator HandOutcome_HigherRankWins_EqualDeck_Test()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 13, 5 };
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 14, 4 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 4);
        Assert.IsTrue(gameManager.player.PlayerWin());
        Assert.IsTrue(gameManager.com.PlayerLose());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-1);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-1);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 13);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 14);
        Assert.IsTrue(gameManager.player.PlayerLose());
        Assert.IsTrue(gameManager.com.PlayerWin());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 2);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Any(card=>card.cardRank==5));
        Assert.IsTrue(gameManager.player.burntDeck.cards.Any(card=>card.cardRank==4));
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-2);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-2);
        Assert.IsFalse(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsTrue(gameManager.reshuffleButton.IsActive());

        // Reshuffle
        gameManager.reshuffleButton.onClick.Invoke();

        Assert.IsTrue(gameManager.player.deck.cards.Count == 2);
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==5));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==4));
        Assert.IsTrue(gameManager.com.deck.cards.Count == 2);
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==13));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==14));     
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);  
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        yield return null;
    }
    
    // Test higher card wins and confirm that correct cards are added to correct burn pile. The decks are unequal 
    // and players have burnt cards. Expect reshuffle button and appropriate reshuffle action
    [UnityTest]
    public IEnumerator HandOutcome_HigherRankWins_UnequalDeck_Test()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 13, 5, 6};
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 14, 4 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);
        List<int> burnRanks = new() { 7, 7, 7 };
        AddCardsToDeck(gameManager.com.burntDeck, burnRanks);
        AddCardsToDeck(gameManager.player.burntDeck, burnRanks);

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 6);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 4);
        Assert.IsTrue(gameManager.player.PlayerWin());
        Assert.IsTrue(gameManager.com.PlayerLose());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-1);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-1);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 14);
        Assert.IsTrue(gameManager.player.PlayerLose());
        Assert.IsTrue(gameManager.com.PlayerWin());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == burnRanks.Count+2);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.player.burntDeck.cards.Any(card=>card.cardRank==6));
        Assert.IsTrue(gameManager.player.burntDeck.cards.Any(card=>card.cardRank==4));
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-2);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-2);
        Assert.IsFalse(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsTrue(gameManager.reshuffleButton.IsActive());

        // Reshuffle
        gameManager.reshuffleButton.onClick.Invoke();

        Assert.IsTrue(gameManager.player.deck.cards.Count == 6);
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==6));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==4));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==13));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.com.deck.cards.Count == 5);
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==14));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==5));     
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);  
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());
        yield return null;
    }
    //======================================================================
    
    // Test that draw logic works appropriately
    //======================================================================
    // This scenario is a draw where both players have equal cards in their deck and at least 3 remaining cards
    // Expect two cards to be drawn from both players decks, and winner determined by last cards drawn
    [UnityTest]
    public IEnumerator DrawLogic_SufficientCards_Test()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 9, 2, 13, 5 };
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 7, 5, 14, 5 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.player.PlayerDraw());
        Assert.IsTrue(gameManager.com.PlayerDraw());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-1);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-1);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 2);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.player.PlayerLose());
        Assert.IsTrue(gameManager.com.PlayerWin());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-3);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-3);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 9);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 7);
        Assert.IsTrue(gameManager.player.PlayerWin());
        Assert.IsTrue(gameManager.com.PlayerLose());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 6);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Any(card=>card.cardRank==5));
        Assert.IsTrue(gameManager.com.burntDeck.cards.Any(card=>card.cardRank==13));
        Assert.IsTrue(gameManager.com.burntDeck.cards.Any(card=>card.cardRank==14));
        Assert.IsTrue(gameManager.com.burntDeck.cards.Any(card=>card.cardRank==2));
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-4);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-4);
        Assert.IsFalse(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsTrue(gameManager.reshuffleButton.IsActive());

        // Reshuffle
        gameManager.reshuffleButton.onClick.Invoke();

        Assert.IsTrue(gameManager.player.deck.cards.Count == 2);
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==9));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.com.deck.cards.Count == 6);
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==13));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==14));     
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==5));     
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==2));     
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);  
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        yield return null;
    }


    // This scenario is a draw where both players have equal cards in their deck and 5 remaining cards. The test will
    // consist of two successive draws
    [UnityTest]
    public IEnumerator DrawLogic_SufficientCards_Successive_Test()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 7, 11, 8, 2, 13, 5 };
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 8, 10, 6, 2, 14, 5 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.player.PlayerDraw());
        Assert.IsTrue(gameManager.com.PlayerDraw());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-1);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-1);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 2);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 2);
        Assert.IsTrue(gameManager.player.PlayerDraw());
        Assert.IsTrue(gameManager.com.PlayerDraw());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-3);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-3);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 11);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 10);
        Assert.IsTrue(gameManager.player.PlayerWin());
        Assert.IsTrue(gameManager.com.PlayerLose());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-5);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-5);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 7);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 8);
        Assert.IsTrue(gameManager.player.PlayerLose());
        Assert.IsTrue(gameManager.com.PlayerWin());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 10);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-6);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-6);
        Assert.IsFalse(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsTrue(gameManager.reshuffleButton.IsActive());

        // Reshuffle
        gameManager.reshuffleButton.onClick.Invoke();

        Assert.IsTrue(gameManager.player.deck.cards.Count == 10);
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==11));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==8));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==2));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==13));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==5));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==10));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==6));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==14));
        Assert.IsTrue(gameManager.com.deck.cards.Count == 2);
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==8));    
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);  
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        yield return null;
    }

    // This scenario is a draw where both players have equal cards in their deck and exactly 2 remaining cards
    // This test will also begin with both players having cars in burn pile to prevent exercising match win/loss 
    // logic, which will be tested elsewhere
    
    // Expect two cards to be drawn from both players decks, and winner determined by last cards drawn. Reshuffle button
    // is shown
    [UnityTest]
    public IEnumerator DrawLogic_SufficientCards_FinalCards_Test()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 2, 13, 5 };
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 5, 14, 5 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);
        List<int> burnRanks = new() { 7, 7, 7 };
        AddCardsToDeck(gameManager.com.burntDeck, burnRanks);
        AddCardsToDeck(gameManager.player.burntDeck, burnRanks);

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.player.PlayerDraw());
        Assert.IsTrue(gameManager.com.PlayerDraw());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == playerCardRanks.Count);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == playerCardRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-1);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-1);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 2);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.player.PlayerLose());
        Assert.IsTrue(gameManager.com.PlayerWin());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == playerCardRanks.Count);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == playerCardRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-3);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-3);
        Assert.IsFalse(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsTrue(gameManager.reshuffleButton.IsActive());

        // Reshuffle
        gameManager.reshuffleButton.onClick.Invoke();

        Assert.IsTrue(gameManager.player.deck.cards.Count == 3);
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.com.deck.cards.Count == 3+6);
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==13));     
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==14));     
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==5));     
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==2));     
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);  
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        yield return null;
    }
    
    // This scenario is a draw where one player has less cards than the other. 
    // This test will also begin with both players having cars in burn pile to prevent 
    // exercising match win/loss logic, which will be tested elsewhere
    [UnityTest]
    public IEnumerator DrawLogic_InsufficientCards_Test()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 13, 5, 6};
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 14, 6 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);
        List<int> burnRanks = new() { 7, 7, 7 };
        AddCardsToDeck(gameManager.com.burntDeck, burnRanks);
        AddCardsToDeck(gameManager.player.burntDeck, burnRanks);

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 6);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 6);
        Assert.IsTrue(gameManager.player.PlayerWin());
        Assert.IsTrue(gameManager.com.PlayerLose());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-1);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-1);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 14);
        Assert.IsTrue(gameManager.player.PlayerLose());
        Assert.IsTrue(gameManager.com.PlayerWin());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == burnRanks.Count+2);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-2);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-2);
        Assert.IsFalse(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsTrue(gameManager.reshuffleButton.IsActive());

        // Reshuffle
        gameManager.reshuffleButton.onClick.Invoke();

        Assert.IsTrue(gameManager.player.deck.cards.Count == 6);
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==6));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==13));   
        Assert.IsTrue(gameManager.com.deck.cards.Count == 5);
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==14));  
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==5));    
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);  
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        yield return null;
    }

    // This scenario is a draw where both players have unequal remaining cards, and have consecutive draws.
    // This test will also begin with both players having cars in burn pile to prevent 
    // exercising match win/loss logic, which will be tested elsewhere
    
    // Expect two cards to be drawn from both players decks. Expect player with remaining cards to win
    // round. Reshuffle button is shown
    [UnityTest]
    public IEnumerator DrawLogic_InsufficientCards_UnequalDeck_FinalCardsDraw_Test()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 3, 2, 13, 5 };
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 2, 14, 5 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);
        List<int> burnRanks = new() { 7, 7, 7 };
        AddCardsToDeck(gameManager.player.burntDeck, burnRanks);
        AddCardsToDeck(gameManager.com.burntDeck, burnRanks);

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.player.PlayerDraw());
        Assert.IsTrue(gameManager.com.PlayerDraw());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-1);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-1);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 2);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 2);
        Assert.IsTrue(gameManager.player.PlayerWin());
        Assert.IsTrue(gameManager.com.PlayerLose());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-3);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-3);
        Assert.IsFalse(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsTrue(gameManager.reshuffleButton.IsActive());

        // Reshuffle
        gameManager.reshuffleButton.onClick.Invoke();

        Assert.IsTrue(gameManager.player.deck.cards.Count == 10);
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==5));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==2));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==13));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==3));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==14));
        Assert.IsTrue(gameManager.com.deck.cards.Count == 3);
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==7));     
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);  
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        yield return null;
    }

    // This scenario is a draw where both players have unequal remaining cards, and have consecutive draws.
    // This test will also begin with both players having cars in burn pile to prevent 
    // exercising match win/loss logic, which will be tested elsewhere
    
    // Expect two cards to be drawn from both players decks. Expect push i.e. cards returned and no round winner
    // Reshuffle button is shown
    [UnityTest]
    public IEnumerator DrawLogic_InsufficientCards_EqualDeck_FinalCardsDraw_Test()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 2, 13, 5 };
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 2, 14, 5 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);
        List<int> burnRanks = new() { 7, 7, 7 };
        AddCardsToDeck(gameManager.player.burntDeck, burnRanks);
        AddCardsToDeck(gameManager.com.burntDeck, burnRanks);

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 5);
        Assert.IsTrue(gameManager.player.PlayerDraw());
        Assert.IsTrue(gameManager.com.PlayerDraw());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-1);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-1);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 2);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 2);
        Assert.IsTrue(gameManager.player.PlayerPush());
        Assert.IsTrue(gameManager.com.PlayerPush());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == burnRanks.Count);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-3);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-3);
        Assert.IsFalse(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsTrue(gameManager.reshuffleButton.IsActive());

        // Reshuffle
        gameManager.reshuffleButton.onClick.Invoke();

        Assert.IsTrue(gameManager.player.deck.cards.Count == 6);
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==5));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==2));
        Assert.IsTrue(gameManager.player.deck.cards.Any(card=>card.cardRank==13));
        Assert.IsTrue(gameManager.com.deck.cards.Count == 6);
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==7));
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==5));     
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==14));     
        Assert.IsTrue(gameManager.com.deck.cards.Any(card=>card.cardRank==2));        
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);  
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        yield return null;
    }
    //======================================================================

    // Win / Loss and Replay Button
    //======================================================================
    [UnityTest]
    public IEnumerator WinLoss_WinByHighRank()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 13, 3 };
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 14, 4 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 3);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 4);
        Assert.IsTrue(gameManager.player.PlayerLose());
        Assert.IsTrue(gameManager.com.PlayerWin());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-1);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-1);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 13);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 14);
        Assert.IsTrue(gameManager.player.PlayerLose());
        Assert.IsTrue(gameManager.com.PlayerWin());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 1);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 2);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-2);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-2);
        Assert.IsFalse(gameManager.drawButton.IsActive());
        Assert.IsTrue(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Reshuffle
        gameManager.replayButton.onClick.Invoke();

        Assert.IsTrue(gameManager.player.deck.cards.Count == gameManager.player.deck.max_cards);
        Assert.IsTrue(gameManager.com.deck.cards.Count == gameManager.com.deck.max_cards);
        Assert.IsTrue(gameManager.player.deck.playedCards.Count == 0);
        Assert.IsTrue(gameManager.com.deck.playedCards.Count == 0);     
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);  
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        yield return null;
    }

    [UnityTest]
    public IEnumerator WinLoss_WinByDraw_InsufficientCards()
    {
        // Clear out the decks 
        ClearDecks();

        // Add cards to the decks 
        List<int> playerCardRanks = new() { 4, 2, 13, 4 };
        AddCardsToDeck(gameManager.player.deck, playerCardRanks);
        List<int> comCardRanks = new() { 2, 14, 4 };
        AddCardsToDeck(gameManager.com.deck, comCardRanks);
        List<int> burnRanks = new() { 7, 7, 7 };
        AddCardsToDeck(gameManager.player.burntDeck, burnRanks);

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 4);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 4);
        Assert.IsTrue(gameManager.player.PlayerDraw());
        Assert.IsTrue(gameManager.com.PlayerDraw());
        Assert.IsTrue(gameManager.playerWinCount == 0);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 3);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-1);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-1);
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Draw and play hand
        DrawCardAndFlipCard();

        Assert.IsTrue(gameManager.player.deck.playedCards.Last().cardData.cardRank == 2);
        Assert.IsTrue(gameManager.com.deck.playedCards.Last().cardData.cardRank == 2);
        Assert.IsTrue(gameManager.player.PlayerWin());
        Assert.IsTrue(gameManager.com.PlayerLose());
        Assert.IsTrue(gameManager.playerWinCount == 1);
        Assert.IsTrue(gameManager.comWinCount == 0);
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 3);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.player.deck.cards.Count == playerCardRanks.Count-3);
        Assert.IsTrue(gameManager.com.deck.cards.Count == comCardRanks.Count-3);
        Assert.IsFalse(gameManager.drawButton.IsActive());
        Assert.IsTrue(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        // Reshuffle
        gameManager.replayButton.onClick.Invoke();

        Assert.IsTrue(gameManager.player.deck.cards.Count == gameManager.player.deck.max_cards);
        Assert.IsTrue(gameManager.com.deck.cards.Count == gameManager.com.deck.max_cards);
        Assert.IsTrue(gameManager.player.deck.playedCards.Count == 0);
        Assert.IsTrue(gameManager.com.deck.playedCards.Count == 0);     
        Assert.IsTrue(gameManager.player.burntDeck.cards.Count == 0);
        Assert.IsTrue(gameManager.com.burntDeck.cards.Count == 0);  
        Assert.IsTrue(gameManager.drawButton.IsActive());
        Assert.IsFalse(gameManager.replayButton.IsActive());
        Assert.IsFalse(gameManager.reshuffleButton.IsActive());

        yield return null;
    }

    //======================================================================
}
