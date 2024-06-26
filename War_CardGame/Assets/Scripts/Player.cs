using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //======================================================================
    public HandOutcomes handOutcome = HandOutcomes.NoCardsPlayed;
    public string playerName = "Human";

    public bool PlayerWin()
    {
        return handOutcome == HandOutcomes.Win;
    }

    public bool PlayerLose()
    {
        return handOutcome == HandOutcomes.Lose;
    }

    public bool PlayerDraw()
    {
        return handOutcome == HandOutcomes.Draw;
    }

}