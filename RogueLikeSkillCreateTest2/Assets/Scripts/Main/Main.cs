using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //�����Ă���A�C�e��
    public SaveDataOfInventory g_saveDataOfInventory;

    //�R���X�g���N�^
    public SaveData(Main main,DataBase database)
    {
        //�����Ă���A�C�e��
        g_saveDataOfInventory = new SaveDataOfInventory(main.g_inventory, main.g_database);
    }
}

public class Main : MonoBehaviour
{
    public DataBase g_database;
    public Inventory g_inventory { get; private set; }
    private InventoryUI _inventoryUI;

    void Start()
    {
        g_inventory = new Inventory(this);
        _inventoryUI = GetComponent<InventoryUI>();
    }

    public void Save()
    {
        //�V�����Z�[�u�f�[�^�̍쐬
        SaveData newSaveData = new SaveData(this, g_database);

        SaveSystem.SaveGame(newSaveData);

    }
    public void Load(SaveData saveData)
    {
        //Inventory�N���X�̃��[�h
        g_inventory = new Inventory(this, saveData.g_saveDataOfInventory, g_database);
    }

    public void GetItem(Item item)
    {
        string itemname = item.g_data.getItemName();
        List<Item> _itemList = g_inventory.g_itemList;

        for (int i = 0;i < _itemList.Count;i++)
        {
            //�Q�b�g�����A�C�e�����C���x���g���ɂ��邩�`�F�b�N
            if(itemname == _itemList[i].g_data.getItemName())
            {
                g_inventory.g_itemList[i].g_value += item.g_value;
                _inventoryUI.UpdateUI(g_inventory);
                return;
            }
        }

        g_inventory.g_itemList.Add(item);
        _inventoryUI.UpdateUI(g_inventory);
    }

    public void LoadUpdateInventoryUI()
    {
        _inventoryUI.UpdateUI(g_inventory);
    }
}


