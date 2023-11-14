using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject
{
    [System.Serializable]
    public class DropTable
    {
        [Header("�A�C�e���̃f�[�^")]
        public ItemData g_itemdata;

        [Header("�h���b�v��")]
        public int g_dropamount;

        [Header("�m��(0�`1000)")]
        public int g_dropratio;
    }

    [Header("�G�̖��O")]
    [SerializeField]
    private string _enemyname;

    [Header("�h���b�v����A�C�e��")]
    [SerializeField] 
    private List<DropTable> _droplist = new List<DropTable>();

    [Header("�G�̃A�C�R��")]
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
