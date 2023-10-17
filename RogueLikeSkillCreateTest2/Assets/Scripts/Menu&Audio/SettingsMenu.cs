using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject SettingsUI;

    public void OpenSettings()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        SettingsUI.SetActive(true);
    }

    public void CloseSettings()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPressed");
        SettingsUI.SetActive(false);
    }
}
