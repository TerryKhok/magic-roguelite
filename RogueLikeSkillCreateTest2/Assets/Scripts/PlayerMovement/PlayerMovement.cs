using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // ���x
    public Vector2 SPEED = new Vector2(0.05f, 0.05f);
    // Update is called once per frame
    void Update()
    {
        // �ړ�����
        Move();
    }

    // �ړ��֐�
    void Move()
    {
        // ���݈ʒu��Position�ɑ��
        float moveX = 0f;
        float moveY = 0f;
        Vector2 Position = transform.position;
        
        if (Input.GetKey("a"))
        { // a�L�[�����������Ă�����
            // �������Position�ɑ΂��ĉ��Z���Z���s��
            moveX -= SPEED.x;
        }
        else if (Input.GetKey("d"))
        { // �E�L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
            moveX += SPEED.x;
        }
        if (Input.GetKey("w"))
        { // ��L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
            moveY += SPEED.y;
        }
        else if (Input.GetKey("s"))
        { // ���L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
            moveY -= SPEED.y;
        }
        if (moveX != 0f && moveY != 0f)
        {
            moveX /= 1.4f;
            moveY /= 1.4f;
        }
        Position.x += moveX;
        Position.y += moveY;
        // ���݂̈ʒu�ɉ��Z���Z���s����Position��������
        transform.position = Position;
    }

    //�_�b�V���e�X�g
    /*void Dash()
    {
        if(Input.GetKey("shuft"))
            GetComponent<Rigidbody>().AddForce(10.0f, 0, 0, ForceMode.Impulse);
    }
    */
}