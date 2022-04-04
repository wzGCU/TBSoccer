using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandle : MonoBehaviour
{
    public Button restart;
    public Text winner;
    public Text score;
    public MainBehaviour main;

    private void Update()
    {
        score.text = "Won games: " + main.wongames + ", Lost games: " + main.lostgames;
    }
    public void RestartButton()
    {
        main.startedPlayer = !main.startedPlayer;
        PlayerPrefs.SetInt("whoStarted", main.startedPlayer ? 1 : 0);
        SceneManager.LoadScene(0);
    }

    public void DebugWinButton()
    {
        main.SetGameFinished();
        UIGameFinished(true);
    }

    public void DebugLooseButton()
    {
        main.SetGameFinished();
        UIGameFinished(false);
    }

    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        main.wongames = 0;
        main.lostgames = 0;
    }

    public void UIGameFinished(bool victory)
    {
        restart.gameObject.SetActive(true);
        winner.gameObject.SetActive(true);
        if (victory)
        {
            winner.text = "You won the game!";
            main.wongames += 1;
        }
        else
        {
            winner.text = "You lost the game!";
            main.lostgames += 1;
        }
        PlayerPrefs.SetInt("wonGames", main.wongames);
        PlayerPrefs.SetInt("lostGames", main.lostgames);
        PlayerPrefs.Save();
    }

    public void DrawGame()
    {
        restart.gameObject.SetActive(true);
        winner.gameObject.SetActive(true);
        winner.text = "No more moves. Draw.";
    }
}
