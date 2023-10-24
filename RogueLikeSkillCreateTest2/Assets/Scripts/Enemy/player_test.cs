using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一時的なコード、このプログラムのために作られた、メインプロジェクトに入れてこれを消していい

public class player_test : MonoBehaviour
{
    public float moveSpeed;
    Vector2 movement;
    Rigidbody2D rb;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
