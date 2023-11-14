using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    public enum itemtype
    {
        Sword, rod, clothes, armor, recovery, nomal, important
    }

    [Header("アイテムの名前")]
    [SerializeField]
    private string _Itemname;

    [Header("アイテムのタイプ")]
    [SerializeField]
    private itemtype _Itemtype; 

    [Header("アイテムのアイコン")]
    [SerializeField]
    private Sprite _Itemicon; 

    [Header("アイテムの説明")]
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

    //セーブデータからItemクラスを復元
    public Item(SaveDataOfItem saveData,DataBase database)
    {
        //セーブデータのIDをデータベースで参照して代入
        g_data = database.g_itemDatabase[saveData.g_ID];
        g_value = saveData.g_value;
    }
}

//Itemクラスをセーブするためのクラス
[Serializable]
public class SaveDataOfItem
{
    public int g_ID;
    public int g_value;

    public SaveDataOfItem(Item item,DataBase database)
    {
        //データベース上のID(index)に変換する
        g_ID = database.GetItemID(item.g_data);
        g_value = item.g_value;
    }
}

