using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ꎞ�I�ȃR�[�h�A���̃v���O�����̂��߂ɍ��ꂽ�A���C���v���W�F�N�g�ɓ���Ă���������Ă���

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
