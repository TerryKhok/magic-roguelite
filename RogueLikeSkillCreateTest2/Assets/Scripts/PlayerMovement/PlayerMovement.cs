
using UnityEngine;
using System.Collections;
//using System.Numerics;

public class PlayerMovement : MonoBehaviour
{
    GameObject Obj;
    private Rigidbody2D rb = null;

    [SerializeField] private float _Speed = 1.0f;
    [SerializeField] private float _roatationSpeed = 100.0f;
    [SerializeField] private float _dashSpeed = 2.0f;
    [SerializeField] private float _dashCT = 2.0f;
    [SerializeField] private float _dashtime = 2.0f;

    int hp = 10;
    float _SpeedPrev;
    bool _dash = true;


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
        int _KeyInputdir = 0;
        Vector2 movementDirection = Vector2.zero;

        //Debug.Log(_dash);   //==デバッグ==ダッシュ可状態の表示
        if (Input.GetKey("a"))
        { // aキーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            _xSpeed = -_Speed;
            _KeyInputdir = 90;
        }
        else if (Input.GetKey("d"))
        { // 右キーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            _xSpeed = _Speed;
            _KeyInputdir = -90;
        }
        if (Input.GetKey("w"))
        { // 上キーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            _ySpeed = _Speed;
            _KeyInputdir = 0;
        }
        else if (Input.GetKey("s"))
        { // 下キーを押し続けていたら
          // 代入したPositionに対して加算減算を行う
            _ySpeed = -_Speed;
            _KeyInputdir = 180;
        }
        rb.velocity = new Vector2(_xSpeed, _ySpeed);
        if (Input.GetKey("a") || Input.GetKey("d") || Input.GetKey("w") || Input.GetKey("s"))
        {
            movementDirection = new Vector2(_xSpeed, _ySpeed);
        }
        //ダッシュ
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dash == true && rb.velocity != Vector2.zero)    //「shiftを押した時」に「ダッシュ可状態」であり、「移動している」なら
        {
            Dash();
        }
        //キャラの回転
        if (movementDirection != Vector2.zero)  //動いている場合
                                                // if (transform.rotation.z != _KeyInputdir)
        {
            Quaternion torotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, torotation, _roatationSpeed * Time.deltaTime);
        }
    }

    //ダッシュ関数
    void Dash()
    {
        _dash = false;  //ダッシュ不可(ダッシュ中)
        _SpeedPrev = _Speed; //加速前の速度をバッファに書き込み
        _Speed = _Speed * _dashSpeed;  //現在の速度にダッシュスピードをかける

        StartCoroutine(Dashstop());
    }

    IEnumerator Dashstop()
    {
        // ダッシュを終わらせる  
        yield return new WaitForSeconds(_dashtime);
        _Speed = _SpeedPrev;    //直前の速度に戻す

        // ダッシュクールタイムだけ待機  
        yield return new WaitForSeconds(_dashCT);
        _dash = true;

    }

    //ダメージ処理
    public void PlayerTakeDamage(int dmg)
    {
        hp -= dmg;
    }
}
