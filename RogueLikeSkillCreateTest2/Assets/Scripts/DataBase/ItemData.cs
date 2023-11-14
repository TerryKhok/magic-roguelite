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
    private string _Itemname;

    [Header("�A�C�e���̃^�C�v")]
    [SerializeField]
    private itemtype _Itemtype; 

    [Header("�A�C�e���̃A�C�R��")]
    [SerializeField]
    private Sprite _Itemicon; 

    [Header("�A�C�e���̐���")]
    [SerializeField]
    private string _Itemexplanation; 

    public string getItemName()
    {
        return _Itemname;
    }

}

[Serializable]
public class Item
{
    public ItemData g_data;
    public int g_value;

    public Item(ItemData _data,int _value)
    {
        g_data = _data;
        g_value = _value;
    }

    public Item(Item _item)
    {
        g_data = _item.g_data;
        g_value = _item.g_value;
    }

    //�Z�[�u�f�[�^����Item�N���X�𕜌�
    public Item(SaveDataOfItem saveData,DataBase database)
    {
        //�Z�[�u�f�[�^��ID���f�[�^�x�[�X�ŎQ�Ƃ��đ��
        g_data = database.g_itemDatabase[saveData.g_ID];
        g_value = saveData.g_value;
    }
}

//Item�N���X���Z�[�u���邽�߂̃N���X
[Serializable]
public class SaveDataOfItem
{
    public int g_ID;
    public int g_value;

    public SaveDataOfItem(Item item,DataBase database)
    {
        //�f�[�^�x�[�X���ID(index)�ɕϊ�����
        g_ID = database.GetItemID(item.g_data);
        g_value = item.g_value;
    }
}

