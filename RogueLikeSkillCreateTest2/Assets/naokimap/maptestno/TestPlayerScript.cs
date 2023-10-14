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
        switch (MapGeneraterScript.Instance.g_playerdir) {//�v���C���[���V�[�����܂��������̌����ɂ���Đ����ʒu��ς���
            case 1://������̎���
                Obj = GameObject.Find("DownRoadTrigger");//���̓����擾
                transform.position = new Vector3(Obj.transform.position.x,Obj.transform.position.y + 2,Obj.transform.position.z);//�Ή������ʒu�Ƀv���C���[���ړ�
                MapGeneraterScript.Instance.NewRoom();//�}�b�v�W�F�l���[�g�X�N���v�g�ɂ���V���������ɓ��������̏�����ǉ�
                break;//�ȉ��قړ���
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
    void Update()//���̃v���C���[�̓���
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
