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
    private string Itemname;

    [Header("アイテムのタイプ")]
    [SerializeField]
    private itemtype Itemtype; 

    [Header("アイテムのアイコン")]
    [SerializeField]
    private Sprite Itemicon; 

    [Header("アイテムの説明")]
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

    //セーブデータからItemクラスを復元
    public Item(SaveDataOfItem saveData,DataBase database)
    {
        //セーブデータのIDをデータベースで参照して代入
        data = database.itemDatabase[saveData.ID];
        value = saveData.value;
    }
}

//Itemクラスをセーブするためのクラス
[Serializable]
public class SaveDataOfItem
{
    public int ID;
    public int value;

    public SaveDataOfItem(Item item,DataBase database)
    {
        //データベース上のID(index)に変換する
        ID = database.GetItemID(item.data);
        value = item.value;
    }
}

