using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DownRoadScript : MonoBehaviour
{
    private SpriteRenderer spriterenderer;
    private BoxCollider2D boxcollider2d;

    private int _roomnumberint;//部屋番号
    private string _roomnumberstr;//部屋番号（文字列）
    void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        boxcollider2d = GetComponent<BoxCollider2D>();
    }

    public void DownRoadGenerate(int downroad)//道を生成するかしないか
    {
        if (downroad != 1)
        {
            spriterenderer.enabled = true;
            boxcollider2d.isTrigger = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _roomnumberint = MapGeneraterScript.Instance.g_field[MapGeneraterScript.Instance.g_nowpositiony - 2, MapGeneraterScript.Instance.g_nowpositionx];//部屋番号獲得
            _roomnumberstr = _roomnumberint.ToString();//文字列に変更
            MapGeneraterScript.Instance.RoomMet(3);//マップジェネレートに引数を返す
        }
    }
    public void DownSceneload()
    {
        SceneManager.LoadScene("Stage" + _roomnumberstr);//シーンをロードする
    }
}
