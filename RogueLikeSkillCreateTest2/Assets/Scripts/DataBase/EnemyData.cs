using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject
{
    [System.Serializable]
    public class DropTable
    {
        [Header("アイテムのデータ")]
        public ItemData itemdata;

        [Header("ドロップ量")]
        public int dropamount;

        [Header("確率(0～100)")]
        public int dropratio;
    }

    [Header("敵の名前")]
    [SerializeField]
    private string enemyname;

    [Header("ドロップするアイテム")]
    [SerializeField] 
    private List<DropTable> droplist = new List<DropTable>();

    [Header("敵のアイコン")]
    [SerializeField]
    private Sprite Enemyicon; 

  public List<DropTable> getDropList()
    {
        return droplist;
    }
}
