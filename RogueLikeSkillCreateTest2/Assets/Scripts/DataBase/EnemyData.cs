using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject
{
    [System.Serializable]
    public class DropTable
    {
        [Header("�A�C�e���̃f�[�^")]
        public ItemData itemdata;

        [Header("�h���b�v��")]
        public int dropamount;

        [Header("�m��(0�`100)")]
        public int dropratio;
    }

    [Header("�G�̖��O")]
    [SerializeField]
    private string enemyname;

    [Header("�h���b�v����A�C�e��")]
    [SerializeField] 
    private List<DropTable> droplist = new List<DropTable>();

    [Header("�G�̃A�C�R��")]
    [SerializeField]
    private Sprite Enemyicon; 

  public List<DropTable> getDropList()
    {
        return droplist;
    }
}
