using UnityEngine;
using System.Collections;
//using System.Numerics;

public class PlayerMovement : MonoBehaviour
{
    GameObject Obj;
    // 速度
    private Rigidbody2D rb = null;
    [SerializeField] private float _Speed = 1.0f;
    [SerializeField] private float _roatation_speed = 100.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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
        float _xSpeed = 0.0f;
        float _ySpeed = 0.0f;

        if (Input.GetKey("a"))
        { // aキーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            _xSpeed = -_Speed;
        }
        else if (Input.GetKey("d"))
        { // 右キーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            _xSpeed = _Speed;
        }
        if (Input.GetKey("w"))
        { // 上キーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            _ySpeed = _Speed;
        }
        else if (Input.GetKey("s"))
        { // 下キーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            _ySpeed = -_Speed;
        }
        rb.velocity = new Vector2(_xSpeed, _ySpeed);


        Vector2 movementDirection = new Vector2(_xSpeed, _ySpeed);
        if (movementDirection != Vector2.zero)
        {
            Quaternion torotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
            Debug.Log(torotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, torotation, _roatation_speed * Time.deltaTime);
        }

    }
}
