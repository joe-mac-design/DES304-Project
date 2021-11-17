using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject PauseMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
    }

    public void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
