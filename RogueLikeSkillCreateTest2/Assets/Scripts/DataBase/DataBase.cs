using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBase",menuName = "Data/DataBase")]
public class DataBase : ScriptableObject
{
    [Header("アイテムデータ")]
    public List<ItemData> itemDatabase;

    [Header("敵のデータ")]
    public List<EnemyData> enemyDatabase;

    //アイテムのID取得
    public int GetItemID(ItemData data)
    {
        //引数dataと同じものをデータベースから持ってくる
        int index = itemDatabase.IndexOf(data);

        if(index == -1)
        {
            Debug.LogError("notFoundItem");
        }

        return index;
    }

    public int GetEnemyID(EnemyData data)
    {
        //引数dataと同じものをデータベースから持ってくる
        int index = enemyDatabase.IndexOf(data);

        if (index == -1)
        {
            Debug.LogError("notFoundEnemy");
        }

        return index;
    }
}


