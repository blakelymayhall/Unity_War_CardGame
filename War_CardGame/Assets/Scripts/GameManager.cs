using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //======================================================================
    public Player player;
    public COM com;
    public Button reshuffleButton;
    public Button drawButton;
    public Button replayButton;
    public GameObject handOutcomeMessage;
    public GameObject matchOutcomeMessage;
    public int playerWinCount = 0;
    public int comWinCount = 0;
    //======================================================================
    private const string handOutcome_msg_win = "Win";
    private const string handOutcome_msg_loss = "Lose";
    private const string matchOutcome_msg_win = "Game Over!\nYou Win!";
    private const string matchOutcome_msg_loss = "Game Over!\nYou Lose!";
    //======================================================================

    void Start()
    {
        replayButton.gameObject.SetActive(false);
        reshuffleButton.gameObject.SetActive(false);
        handOutcomeMessage.SetActive(false);
        matchOutcomeMessage.SetActive(false);
    }

    //======================================================================
    public void ActivateReshuffleButton()
    {
        reshuffleButton.gameObject.SetActive(true);
        drawButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
    }

    //======================================================================
    public void ActivateReplayButton()
    {
        reshuffleButton.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(true);
        matchOutcomeMessage.SetActive(false);
    }

    //======================================================================
    public void ActivateDrawButton()
    {
        reshuffleButton.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(true);
        replayButton.gameObject.SetActive(false);
    }

    //======================================================================
    public void SetHandWinLoss()
    {
        int playerRank = player.deck.playedCards.Last().cardData.cardRank;
        int comRank = com.deck.playedCards.Last().cardData.cardRank;

        if (playerRank > comRank)
        {
            SetOutcome(HandOutcomes.Win, HandOutcomes.Lose);
        }
        else if (playerRank == comRank)
        {
            SetOutcome(HandOutcomes.Draw, HandOutcomes.Draw);
        }
        else
        {
            SetOutcome(HandOutcomes.Lose, HandOutcomes.Win);
        }

        // If either player has no cards remaining, we need to reshuffle the deck 
        if (!player.deck.cards.Any() || !com.deck.cards.Any())
        {
            ActivateReshuffleButton();
        }

        // A war where one or both players cannot finish the war requires special attention:
        //      if this happens, see if both players have same amount of cards. 
        //      if so, return those cards to their hands. If the do not have the
        //      same amount of cards, the loser is the player with fewest remaining cards
        if (player.deck.deck_owner.PlayerDraw() && (player.deck.cards.Count < 2 || com.deck.cards.Count < 2))
        {
            if (player.deck.cards.Count == com.deck.cards.Count)
            {
                SetOutcome(HandOutcomes.Push, HandOutcomes.Push);
            }
            else
            {
                if (player.deck.cards.Count < com.deck.cards.Count)
                {
                    SetOutcome(HandOutcomes.Lose, HandOutcomes.Win);
                }
                else
                {
                    SetOutcome(HandOutcomes.Win, HandOutcomes.Lose);
                }
            }
        }
    }

    //======================================================================    
    private void SetOutcome(HandOutcomes playerOutcome, HandOutcomes comOutcome)
    {
        player.handOutcome = playerOutcome;
        com.handOutcome = comOutcome;
    }

    //======================================================================
    public void CheckMatchWinLoss()
    {
        bool playerWonGame = !com.deck.burnDeck.cards.Any() && com.handOutcome == HandOutcomes.Lose &&
            !com.deck.cards.Any();
        bool comWonGame = !player.deck.burnDeck.cards.Any() && player.handOutcome == HandOutcomes.Lose &&
            !player.deck.cards.Any();
        if (playerWonGame || comWonGame) 
        { 
            ActivateReplayButton();
            if(playerWonGame) 
            {
                playerWinCount++;
                StartCoroutine(ShowMessage(matchOutcomeMessage, matchOutcome_msg_win));
            }
            else 
            {
                comWinCount++;
                StartCoroutine(ShowMessage(matchOutcomeMessage, matchOutcome_msg_loss));
            }
        }
        else 
        {
            if (player.handOutcome == HandOutcomes.Win)
            {
                StartCoroutine(ShowMessage(handOutcomeMessage, handOutcome_msg_win, 0.7f));
            }
            else if (player.handOutcome == HandOutcomes.Lose)
            {
                StartCoroutine(ShowMessage(handOutcomeMessage, handOutcome_msg_loss, 0.7f));
            }
        }
    }
    
    //======================================================================
    IEnumerator ShowMessage (GameObject go, string message, float delay = -1) 
    {
        var tm = go.GetComponent<TextMeshProUGUI>();
        tm.text = message;
        go.SetActive(true);
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
            go.SetActive(false);
        }
        yield return null;
    }
}
