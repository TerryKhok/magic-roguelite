using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    public Main main;

    public void Save()
    {
        main.Save();
    }

    public void Load()
    {
        main.Load(SaveSystem.LoadGame());
        main.LoadUpdateInventoryUI();
    }
}
