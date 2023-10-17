using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManu : MonoBehaviour
{
    [SerializeField]public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public GameObject GameoverUI;
    public GameObject Win;
    GameObject cam;
    bool death;
    bool doneWait = false;
    bool hasPlayed = false;


    private void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        if (death)
        {
            if (!hasPlayed)
            {
                FindObjectOfType<AudioManager>().Play("Victory");
                hasPlayed = true;
            }
            gameover();

        }
    }

    public void Resume()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    void gameover()
    {
        StartCoroutine(waitWIN());
        if (doneWait)
        {
            GameoverUI.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            Win.SetActive(false);
        }

    }

    public void Playagain()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        Time.timeScale = 1f;
        death = false;
        isPaused = false;
        Resume();
        SceneManager.LoadScene("Gameplay");
    }
    public void WeaponSelect()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        Time.timeScale = 1f;
        SceneManager.LoadScene("WeaponSelectScene");
        isPaused = false;
    }

    public void loadMenu()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        FindObjectOfType<AudioManager>().Stop("BGM2");
        FindObjectOfType<AudioManager>().Play("BGM1");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
        isPaused = false;

    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        Application.Quit();
    }

    private IEnumerator waitWIN()
    {
        Win.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        doneWait = true;
    }
}
