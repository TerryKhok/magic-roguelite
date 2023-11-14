using System.Collections.Generic;
using UnityEngine;
using static EnemyData;
using System.Linq;

public class DropItemEnemy : MonoBehaviour
{
    [SerializeField]
    private EnemyData _enemyData;

    private GameObject _main;

    private List<DropTable> _droplist;

    private Main _maindata;

    void Start()
    {
        _main = GameObject.Find("Main");
        _droplist = _enemyData.getDropList();
        _maindata = _main.GetComponent<Main>();

        //�P�̃h���b�v
        /*
        if (droplist != null)
        //list�̃h���b�v�����̍~���ɕ��ѕς���
        droplist = droplist.OrderByDescending(i => i.dropratio).ToList();
        */
        
    }

    public void ItemDrop()
    {
       foreach(DropTable dropdata in _droplist)
        {
            int rand = Random.Range(0, 1000);
            if(rand < dropdata.g_dropratio)
            {
                Item item = new Item(dropdata.g_itemdata, dropdata.g_dropamount);
                _maindata.GetItem(item);

                Debug.Log(dropdata.g_itemdata.getItemName()  + "���h���b�v���܂���");
                //return;
            }
        }
    }

    public EnemyData getEnemydata()
    {
        return _enemyData;
    }
}
