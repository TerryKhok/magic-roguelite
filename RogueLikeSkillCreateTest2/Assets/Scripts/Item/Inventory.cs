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
        //�Z�[�u�f�[�^����C���x���g���N���X�̂̕���
        foreach (SaveDataOfItem itemSaveData in saveData.itemArray)
        {
            itemList.Add(new Item(itemSaveData, database));
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
        itemArray = new SaveDataOfItem[inventory.itemList.Count];
        for(int i = 0;i < itemArray.Length;i++)
        {
            //�A�C�e������Âۑ�����
            itemArray[i] = new SaveDataOfItem(inventory.itemList[i], database);
        }
    }
}
