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
        //�Z�[�u�f�[�^����C���x���g���N���X�̂̕���
        foreach (SaveDataOfItem itemSaveData in saveData.itemArray)
        {
            g_itemList.Add(new Item(itemSaveData, database));
        }
    }
}

//�C���x���g���N���X�̃Z�[�u�N���X
[Serializable]
public class SaveDataOfInventory
{
    //�����Ă���A�C�e���̃f�[�^�z��
    public SaveDataOfItem[] itemArray;

    public SaveDataOfInventory(Inventory inventory,DataBase database)
    {
        //�����Ă���A�C�e���̐��f�[�^��z��ɕۑ�����
        itemArray = new SaveDataOfItem[inventory.g_itemList.Count];
        for(int i = 0;i < itemArray.Length;i++)
        {
            //�A�C�e������Âۑ�����
            itemArray[i] = new SaveDataOfItem(inventory.g_itemList[i], database);
        }
    }
}
