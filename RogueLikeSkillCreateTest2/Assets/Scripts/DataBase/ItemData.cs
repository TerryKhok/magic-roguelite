using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    public enum itemtype
    {
        Sword, rod, clothes, armor, recovery, nomal, important
    }

    [Header("�A�C�e���̖��O")]
    [SerializeField]
    private string Itemname;

    [Header("�A�C�e���̃^�C�v")]
    [SerializeField]
    private itemtype Itemtype; 

    [Header("�A�C�e���̃A�C�R��")]
    [SerializeField]
    private Sprite Itemicon; 

    [Header("�A�C�e���̐���")]
    [SerializeField]
    private string Itemexplanation; 

    public string getItemName()
    {
        return Itemname;
    }

}

[Serializable]
public class Item
{
    public ItemData data;
    public int value;

    public Item(ItemData _data,int _value)
    {
        data = _data;
        value = _value;
    }

    public Item(Item _item)
    {
        data = _item.data;
        value = _item.value;
    }

    //�Z�[�u�f�[�^����Item�N���X�𕜌�
    public Item(SaveDataOfItem saveData,DataBase database)
    {
        //�Z�[�u�f�[�^��ID���f�[�^�x�[�X�ŎQ�Ƃ��đ��
        data = database.itemDatabase[saveData.ID];
        value = saveData.value;
    }
}

//Item�N���X���Z�[�u���邽�߂̃N���X
[Serializable]
public class SaveDataOfItem
{
    public int ID;
    public int value;

    public SaveDataOfItem(Item item,DataBase database)
    {
        //�f�[�^�x�[�X���ID(index)�ɕϊ�����
        ID = database.GetItemID(item.data);
        value = item.value;
    }
}

