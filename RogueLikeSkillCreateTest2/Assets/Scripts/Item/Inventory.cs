using System.Collections.Generic;
using System;

public class Inventory
{
    public List<Item> g_itemList = new List<Item>();

    Main _main;

    public Inventory(Main _main)
    {
        this._main = _main;
    }

    public Inventory(Main _main,SaveDataOfInventory saveData,DataBase database)
    {
        this._main = _main;

        g_itemList = new List<Item>();
        //セーブデータからインベントリクラスのの復元
        foreach (SaveDataOfItem itemSaveData in saveData.itemArray)
        {
            g_itemList.Add(new Item(itemSaveData, database));
        }
    }
}

//インベントリクラスのセーブクラス
[Serializable]
public class SaveDataOfInventory
{
    //持っているアイテムのデータ配列
    public SaveDataOfItem[] itemArray;

    public SaveDataOfInventory(Inventory inventory,DataBase database)
    {
        //持っているアイテムの数データを配列に保存する
        itemArray = new SaveDataOfItem[inventory.g_itemList.Count];
        for(int i = 0;i < itemArray.Length;i++)
        {
            //アイテムを一個づつ保存する
            itemArray[i] = new SaveDataOfItem(inventory.g_itemList[i], database);
        }
    }
}
