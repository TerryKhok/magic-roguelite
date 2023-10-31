using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //�����Ă���A�C�e��
    public SaveDataOfInventory saveDataOfInventory;

    //�R���X�g���N�^
    public SaveData(Main main,DataBase database)
    {
        //�����Ă���A�C�e��
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
        //�V�����Z�[�u�f�[�^�̍쐬
        SaveData newSaveData = new SaveData(this, database);

        SaveSystem.SaveGame(newSaveData);

    }
    public void Load(SaveData saveData)
    {
        //Inventory�N���X�̃��[�h
        inventory = new Inventory(this, saveData.saveDataOfInventory, database);
    }

    public void GetItem(Item item)
    {
        string itemname = item.data.getItemName();
        List<Item> _itemList = inventory.itemList;

        for (int i = 0;i < _itemList.Count;i++)
        {
            //�Q�b�g�����A�C�e�����C���x���g���ɂ��邩�`�F�b�N
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


