
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
        int _KeyInputdir = 0;
        Vector2 movementDirection = Vector2.zero;

        //Debug.Log(_dash);   //==�f�o�b�O==�_�b�V����Ԃ̕\��
        if (Input.GetKey("a"))
        { // a�L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
            _xSpeed = -_Speed;
            _KeyInputdir = 90;
        }
        else if (Input.GetKey("d"))
        { // �E�L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
            _xSpeed = _Speed;
            _KeyInputdir = -90;
        }
        if (Input.GetKey("w"))
        { // ��L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
            _ySpeed = _Speed;
            _KeyInputdir = 0;
        }
        else if (Input.GetKey("s"))
        { // ���L�[�����������Ă�����
          // �������Position�ɑ΂��ĉ��Z���Z���s��
            _ySpeed = -_Speed;
            _KeyInputdir = 180;
        }
        rb.velocity = new Vector2(_xSpeed, _ySpeed);
        if (Input.GetKey("a") || Input.GetKey("d") || Input.GetKey("w") || Input.GetKey("s"))
        {
            movementDirection = new Vector2(_xSpeed, _ySpeed);
        }
        //�_�b�V��
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dash == true && rb.velocity != Vector2.zero)    //�ushift�����������v�Ɂu�_�b�V����ԁv�ł���A�u�ړ����Ă���v�Ȃ�
        {
            Dash();
        }
        //�L�����̉�]
        if (movementDirection != Vector2.zero)  //�����Ă���ꍇ
                                                // if (transform.rotation.z != _KeyInputdir)
        {
            Quaternion torotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, torotation, _roatationSpeed * Time.deltaTime);
        }
    }

    //�_�b�V���֐�
    void Dash()
    {
        _dash = false;  //�_�b�V���s��(�_�b�V����)
        _SpeedPrev = _Speed; //�����O�̑��x���o�b�t�@�ɏ�������
        _Speed = _Speed * _dashSpeed;  //���݂̑��x�Ƀ_�b�V���X�s�[�h��������

        StartCoroutine(Dashstop());
    }

    IEnumerator Dashstop()
    {
        // �_�b�V�����I��点��  
        yield return new WaitForSeconds(_dashtime);
        _Speed = _SpeedPrev;    //���O�̑��x�ɖ߂�

        // �_�b�V���N�[���^�C�������ҋ@  
        yield return new WaitForSeconds(_dashCT);
        _dash = true;

    }

    //�_���[�W����
    public void PlayerTakeDamage(int dmg)
    {
        hp -= dmg;
    }
}
