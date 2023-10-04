using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorScript : MonoBehaviour
{
    [SerializeField] GameObject[] MapCell;  //マップセルのプレハブ一覧
    [SerializeField] Vector2 MapCellSize;   //マップセルのサイズ
    [SerializeField] Vector2 FieldSize;     //マップセルを敷き詰める数
    // Start is called before the first frame update
    void Start()
    {
        int r;  //マップセル選択用乱数の定義（下で生成）
        Transform tf = GetComponent<Transform>();    //オブジェクトの座標（この後いっぱい使う
        gameObject.transform.position = Vector2.zero;   //マップジェネレータの位置を(0,0,0)に
        for (int i = 0; i < FieldSize.y; i++) {  //Y方向のループ
            for (int j = 0; j < FieldSize.x; j++)//X方向のループ
            {
                r = Random.Range(0, MapCell.Length);    //マップセル選択用の乱数生成
                Instantiate(MapCell[r], GetComponent<Transform>(), true);                    //プレハブリストからランダムなものを今の場所に生成
                tf.position += new Vector3(MapCellSize.x, 0, 0);                      //マップセルのサイズに対応した分横移動
            }
            tf.position += new Vector3(MapCellSize.x * FieldSize.x * -1, MapCellSize.y, 0);                         //マップセルのサイズに対応した分縦移動
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
