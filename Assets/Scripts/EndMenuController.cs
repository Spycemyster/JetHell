using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class EndMenuController : MonoBehaviour
{
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI enemiesKilled;
    [SerializeField] private TextMeshProUGUI accuracy;

    public static bool gameIsPaused = false;
    private float previousTimescale = 1f;

    public void RevealScreen(string descriptionStr, int enemiesNum, float accuracyNum)
    {
        description.text = descriptionStr;
        enemiesKilled.text = "Enemies killed: " + enemiesNum;
        if (accuracyNum == float.NaN)
        {
            accuracy.text = "Accuracy: 00.00%";
        }
        else
        {
            accuracyNum *= 100;
            Debug.Log("Accuracy: " + accuracyNum);
            accuracy.text = "Accuracy: " + accuracyNum.ToString(".00") + "%";
        }

        endGameUI.SetActive(true);

        PauseGame();
    }

    public void RestartGame()
    {
        endGameUI.SetActive(false);

        gameIsPaused = false;
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseGame()
    {
        endGameUI.SetActive(true);

        previousTimescale = Time.timeScale;
        gameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
