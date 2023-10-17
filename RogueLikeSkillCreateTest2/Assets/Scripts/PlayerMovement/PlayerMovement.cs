using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    GameObject Obj;
    // ���x
    public Vector2 SPEED = new Vector2(0.05f, 0.05f);

    private void Start()
    {
        switch (MapGeneraterScript.Instance.g_playerdir)
        {//�v���C���[���V�[�����܂��������̌����ɂ���Đ����ʒu��ς���
            case 1://������̎���
                Obj = GameObject.Find("DownRoadTrigger");//���̓����擾
                transform.position = new Vector3(Obj.transform.position.x, Obj.transform.position.y + 2, Obj.transform.position.z);//�Ή������ʒu�Ƀv���C���[���ړ�
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

    // Update is called once per frame
    void Update()
    {
        // �ړ�����
        Move();

        //�X�L������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<SkillUser>().RunSkill(0);
        }
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
        if(Input.GetKey("shift"))
            GetComponent<Rigidbody>().AddForce(10.0f, 0, 0, ForceMode.Impulse);
    }
    */
}