using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //======================================================================
    public Button reshuffleButton;
    public Button drawButton;

    //======================================================================
    void Start()
    {
        reshuffleButton = GameObject.Find("ReshuffleButton").GetComponent<Button>();
        reshuffleButton.gameObject.SetActive(false);
        drawButton = GameObject.Find("DrawButton").GetComponent<Button>();
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
