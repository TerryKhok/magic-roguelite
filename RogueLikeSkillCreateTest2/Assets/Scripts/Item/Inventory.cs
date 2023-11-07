using System.Collections.Generic;
using System;

public class Inventory
{
    public List<Item> itemList = new List<Item>();

    Main main;

    public Inventory(Main _main)
    {
        main = _main;
    }

    public Inventory(Main _main,SaveDataOfInventory saveData,DataBase database)
    {
        main = _main;

        itemList = new List<Item>();
        //セーブデータからインベントリクラスのの復元
        foreach (SaveDataOfItem itemSaveData in saveData.itemArray)
        {
            itemList.Add(new Item(itemSaveData, database));
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
        itemArray = new SaveDataOfItem[inventory.itemList.Count];
        for(int i = 0;i < itemArray.Length;i++)
        {
            //アイテムを一個づつ保存する
            itemArray[i] = new SaveDataOfItem(inventory.itemList[i], database);
        }
    }
}
