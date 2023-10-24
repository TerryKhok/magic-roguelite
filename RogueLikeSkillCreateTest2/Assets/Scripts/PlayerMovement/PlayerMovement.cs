using UnityEngine;
using System.Collections;
//using System.Numerics;

public class PlayerMovement : MonoBehaviour
{
    GameObject Obj;
    // ���x
    private Rigidbody2D rb = null;
    [SerializeField] private float _Speed = 1.0f;
    [SerializeField] private float _roatation_speed = 100.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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
        float _xSpeed = 0.0f;
        float _ySpeed = 0.0f;

        if (Input.GetKey("a"))
        { // a�L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
            _xSpeed = -_Speed;
        }
        else if (Input.GetKey("d"))
        { // �E�L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
            _xSpeed = _Speed;
        }
        if (Input.GetKey("w"))
        { // ��L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
            _ySpeed = _Speed;
        }
        else if (Input.GetKey("s"))
        { // ���L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
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
