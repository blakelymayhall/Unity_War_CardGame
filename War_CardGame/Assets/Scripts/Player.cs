using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //======================================================================
    public RoundOutcomes roundOutcome = RoundOutcomes.Playing;
    public HandOutcomes handOutcome = HandOutcomes.NoCardsPlayed;
    public string playerName = "Human";
    public int wins = 0;
    public int loses = 0;

    //======================================================================
    private GameOutcomes gameOutcome = GameOutcomes.Playing;

    //======================================================================
    public GameOutcomes GameOutcome
    {
        get { return gameOutcome; }
        set
        {
            gameOutcome = value;
            if (gameOutcome == GameOutcomes.Win)
            {
                Debug.Log(playerName + "Win Game");
                wins += 1;
            }
            else
            {
                Debug.Log(playerName + "Lost Game");
                loses += 1;
            }
        }
    }

    //======================================================================
    public bool PlayerWinHand()
    {
        return handOutcome == HandOutcomes.Win;
    }

    //======================================================================
    public bool PlayerDrawHand()
    {
        return handOutcome == HandOutcomes.Draw;
    }

}