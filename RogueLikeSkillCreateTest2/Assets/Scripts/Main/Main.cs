using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //持っているアイテム
    public SaveDataOfInventory saveDataOfInventory;

    //コンストラクタ
    public SaveData(Main main,DataBase database)
    {
        //持っているアイテム
        saveDataOfInventory = new SaveDataOfInventory(main.inventory, main.database);
    }
}

public class Main : MonoBehaviour
{
    public DataBase database;
    public Inventory inventory { get; private set; }
    private InventoryUI inventoryUI;

    void Start()
    {
        inventory = new Inventory(this);
        inventoryUI = GetComponent<InventoryUI>();
    }

    public void Save()
    {
        //新しいセーブデータの作成
        SaveData newSaveData = new SaveData(this, database);

        SaveSystem.SaveGame(newSaveData);

    }
    public void Load(SaveData saveData)
    {
        //Inventoryクラスのロード
        inventory = new Inventory(this, saveData.saveDataOfInventory, database);
    }

    public void GetItem(Item item)
    {
        string itemname = item.data.getItemName();
        List<Item> _itemList = inventory.itemList;

        for (int i = 0;i < _itemList.Count;i++)
        {
            //ゲットしたアイテムがインベントリにあるかチェック
            if(itemname == _itemList[i].data.getItemName())
            {
                inventory.itemList[i].value += item.value;
                inventoryUI.UpdateUI(inventory);
                return;
            }
        }

        inventory.itemList.Add(item);
        inventoryUI.UpdateUI(inventory);
    }

    public void LoadUpdateInventoryUI()
    {
        inventoryUI.UpdateUI(inventory);
    }
}


