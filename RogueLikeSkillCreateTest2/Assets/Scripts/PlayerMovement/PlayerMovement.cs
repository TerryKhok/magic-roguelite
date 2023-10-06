using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // 速度
    public Vector2 SPEED = new Vector2(0.05f, 0.05f);
    // Update is called once per frame
    void Update()
    {
        // 移動処理
        Move();
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
        if(Input.GetKey("shuft"))
            GetComponent<Rigidbody>().AddForce(10.0f, 0, 0, ForceMode.Impulse);
    }
    */
}