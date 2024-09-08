using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //======================================================================
    public Button reshuffleButton;
    public Button drawButton;
    public Button replayButton;
    public int playerWinCount = 0;
    public int comWinCount = 0;

    //======================================================================
    void Start()
    {
        replayButton.gameObject.SetActive(false);
        reshuffleButton.gameObject.SetActive(false);
    }

    //======================================================================
    public void ActivateReshuffleButton()
    {
        reshuffleButton.gameObject.SetActive(true);
        drawButton.gameObject.SetActive(false);
    }

    //======================================================================
    public void ActivateDrawButton()
    {
        reshuffleButton.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(true);
    }
}
