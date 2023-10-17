using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class TestPlayerScript : MonoBehaviour
{
    GameObject Obj;
    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        switch (MapGeneraterScript.Instance.g_playerdir) {//プレイヤーがシーンをまたいだ時の向きによって生成位置を変える
            case 1://上方向の時に
                Obj = GameObject.Find("DownRoadTrigger");//下の道を取得
                transform.position = new Vector3(Obj.transform.position.x,Obj.transform.position.y + 2,Obj.transform.position.z);//対応した位置にプレイヤーを移動
                MapGeneraterScript.Instance.NewRoom();//マップジェネレートスクリプトにある新しい部屋に入った時の処理を追加
                break;//以下ほぼ同文
            case 2:
                Obj = GameObject.Find("LeftRoadTrigger");
                transform.position = new Vector3(Obj.transform.position.x + 2, Obj.transform.position.y, Obj.transform.position.z);
                MapGeneraterScript.Instance.NewRoom();
                break;
            case 3:
                Obj = GameObject.Find("UpRoadTrigger");
                transform.position = new Vector3(Obj.transform.position.x, Obj.transform.position.y - 2, Obj.transform.position.z);
                MapGeneraterScript.Instance.NewRoom();
                break;
            case 4:
                Obj = GameObject.Find("RightRoadTrigger");
                transform.position = new Vector3(Obj.transform.position.x - 2, Obj.transform.position.y, Obj.transform.position.z);
                MapGeneraterScript.Instance.NewRoom();
                break;
        }

    }
    void Update()//仮のプレイヤーの動作
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = new Vector2(0,15);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-15, 0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.velocity = new Vector2(0, -15);
        }
        else if (Input.GetKey(KeyCode.D))
        {

            rb.velocity = new Vector2(15, 0);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }
}
