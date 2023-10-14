using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1f;
    }
    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        FindObjectOfType<AudioManager>().Stop("BGM1");
        FindObjectOfType<AudioManager>().Play("BGM2");
        SceneManager.LoadScene("SkillCreate");
    }
    public void Option()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
    }
    public void Credits()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        // SceneManager.LoadScene("CreditsScene");
    }
    public void ExitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        Application.Quit();
    }
}
