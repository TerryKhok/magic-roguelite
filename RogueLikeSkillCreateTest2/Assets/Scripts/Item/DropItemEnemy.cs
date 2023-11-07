using System.Collections.Generic;
using UnityEngine;
using static EnemyData;
using System.Linq;

public class DropItemEnemy : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;

    private GameObject main;

    private List<DropTable> droplist;

    private Main maindata;

    void Start()
    {
        main = GameObject.Find("Main");
        droplist = enemyData.getDropList();
        maindata = main.GetComponent<Main>();

        if (droplist != null)
        //listのドロップ割合の降順に並び変える
        droplist = droplist.OrderByDescending(i => i.dropratio).ToList();
    }

    public void ItemDrop()
    {
       foreach(DropTable dropdata in droplist)
        {
            int rand = Random.Range(0, 100);
            if(rand < dropdata.dropratio)
            {
                Item item = new Item(dropdata.itemdata, dropdata.dropamount);
                maindata.GetItem(item);

                Debug.Log(dropdata.itemdata.getItemName()  + "がドロップしました");
                return;
            }
        }
    }
}
