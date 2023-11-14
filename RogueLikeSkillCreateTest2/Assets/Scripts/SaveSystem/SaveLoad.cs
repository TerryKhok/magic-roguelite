using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    Main _main;

    public void Save()
    {
        _main = GameObject.Find("Main").GetComponent<Main>();
        _main.Save();
    }

    public void Load()
    {
        _main.Load(SaveSystem.LoadGame());
        _main.LoadUpdateInventoryUI();
    }
}
