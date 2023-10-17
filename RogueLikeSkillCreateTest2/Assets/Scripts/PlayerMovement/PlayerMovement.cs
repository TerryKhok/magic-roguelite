using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    GameObject Obj;
    // 速度
    public Vector2 SPEED = new Vector2(0.05f, 0.05f);

    private void Start()
    {
        switch (MapGeneraterScript.Instance.g_playerdir)
        {//プレイヤーがシーンをまたいだ時の向きによって生成位置を変える
            case 1://上方向の時に
                Obj = GameObject.Find("DownRoadTrigger");//下の道を取得
                transform.position = new Vector3(Obj.transform.position.x, Obj.transform.position.y + 2, Obj.transform.position.z);//対応した位置にプレイヤーを移動
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

    // Update is called once per frame
    void Update()
    {
        // 移動処理
        Move();

        //スキル発動
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<SkillUser>().RunSkill(0);
        }
    }

    // 移動関数
    void Move()
    {
        // 現在位置をPositionに代入
        float moveX = 0f;
        float moveY = 0f;
        Vector2 Position = transform.position;
        
        if (Input.GetKey("a"))
        { // aキーを押し続けていたら
            // 代入したPositionに対して加算減算を行う
            moveX -= SPEED.x;
        }
        else if (Input.GetKey("d"))
        { // 右キーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            moveX += SPEED.x;
        }
        if (Input.GetKey("w"))
        { // 上キーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            moveY += SPEED.y;
        }
        else if (Input.GetKey("s"))
        { // 下キーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            moveY -= SPEED.y;
        }
        if (moveX != 0f && moveY != 0f)
        {
            moveX /= 1.4f;
            moveY /= 1.4f;
        }
        Position.x += moveX;
        Position.y += moveY;
        // 現在の位置に加算減算を行ったPositionを代入する
        transform.position = Position;
    }

    //ダッシュテスト
    /*void Dash()
    {
        if(Input.GetKey("shift"))
            GetComponent<Rigidbody>().AddForce(10.0f, 0, 0, ForceMode.Impulse);
    }
    */
}