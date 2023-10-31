using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBase",menuName = "Data/DataBase")]
public class DataBase : ScriptableObject
{
    [Header("�A�C�e���f�[�^")]
    public List<ItemData> itemDatabase;

    [Header("�G�̃f�[�^")]
    public List<EnemyData> enemyDatabase;

    //�A�C�e����ID�擾
    public int GetItemID(ItemData data)
    {
        //����data�Ɠ������̂��f�[�^�x�[�X���玝���Ă���
        int index = itemDatabase.IndexOf(data);

        if(index == -1)
        {
            Debug.LogError("notFoundItem");
        }

        return index;
    }

    public int GetEnemyID(EnemyData data)
    {
        //����data�Ɠ������̂��f�[�^�x�[�X���玝���Ă���
        int index = enemyDatabase.IndexOf(data);

        if (index == -1)
        {
            Debug.LogError("notFoundEnemy");
        }

        return index;
    }
}


