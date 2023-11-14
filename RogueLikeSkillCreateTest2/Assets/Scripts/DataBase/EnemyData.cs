using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject
{
    [System.Serializable]
    public class DropTable
    {
        [Header("アイテムのデータ")]
        public ItemData g_itemdata;

        [Header("ドロップ量")]
        public int g_dropamount;

        [Header("確率(0～1000)")]
        public int g_dropratio;
    }

    [Header("敵の名前")]
    [SerializeField]
    private string _enemyname;

    [Header("ドロップするアイテム")]
    [SerializeField] 
    private List<DropTable> _droplist = new List<DropTable>();

    [Header("敵のアイコン")]
    [SerializeField]
    private Sprite _Enemyicon; 

    public string getEnemyName()
    {
        return _enemyname;
    }

  public List<DropTable> getDropList()
    {
        return _droplist;
    }
}
