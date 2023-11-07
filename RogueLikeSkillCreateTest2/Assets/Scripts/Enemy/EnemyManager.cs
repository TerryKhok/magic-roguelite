using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
//���̃X�N���v�g��G�ɂ������Ă�������
//

public class EnemyManager : MonoBehaviour
{
    //�G�̊�{�I�ϐ�
    [SerializeField]
    private int EnemyType;  //�G�̎��
    //�G�̎��
    //�P�F�߂�
    //�Q�F����ȋ߂��Ȃ�
    //�R�F����
    [SerializeField]
    int EnemyHealth;        //�G��HP
    [SerializeField]
    float EnemySpeed;       //�G�̓����̑���

    //�U���ϐ�
    [SerializeField]
    float EnemyAttackSpeed;   //�G�̍U���̑���
    [SerializeField]
    int EnemyDamage;        //�G�̗�

    //���̕ϐ�
    float fireRateTime = 0.0f;  //�e�ۂ̔��˂̎��Ԋi�[����
    public GameObject bullet;   //�e��
    private Transform Target;   //�ǂ��t���^�[�Q�b�g�i�v���C���j
    private float maxRange;     //�v���C�����C�t���͈�
    private float minRange;     //�v���C���ɋ߂Â��Ǝ~�܂邽�߂͈̔�
    private bool inRange = false;       //�G�̓v���C���[�͈̔͂ɂ���t���O

    void Start()
    {
        Target = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
        EnemySet();
    }

    void FixedUpdate()
    {
        EnemyAttack();
    }

    void EnemySet()
    {
        switch (EnemyType)
        {
            case 1:
                maxRange = 12;
                minRange = 0.6f;
                ShortRange();
                break;
            case 2:
                maxRange = 12;
                minRange = 4.5f;
                MediumRange();
                break;
            case 3:
                maxRange = 12;
                minRange = 8f;
                LongRange();
                break;
            default:
                Debug.Log("�G�̎�ނ͖����ł�");
                break;
        }
    }

    //
    //�G�̐F�X�Ȋ֐�
    //
    //ShortRange, MediumRange,
    //LongRange, EnemyTakeDamage, FollowPlayer
    //

    void ShortRange()    //�߂��G
    {
        //�G�͍U������֐�
        if (Vector3.Distance(Target.position, transform.position) <= maxRange && Vector3.Distance(Target.position, transform.position) >= minRange - 1)
        {
            FollowPlayer();
            inRange = true;
        }
        else
        {
            inRange = false;
        }
    }

    void MediumRange()   //����ȋ߂��Ȃ��G
    {
        //�G�͍U������֐�
        if (Vector3.Distance(Target.position, transform.position) <= maxRange && Vector3.Distance(Target.position, transform.position) >= minRange - 1)
        {
            FollowPlayer();
            inRange = true;
        }
        else
        {
            inRange = false;
        }
    }

    void LongRange()     //�����G
    {
        //�G�͍U������֐�
        if (Vector3.Distance(Target.position, transform.position) <= maxRange && Vector3.Distance(Target.position, transform.position) >= minRange - 1)
        {
            FollowPlayer();
            inRange = true;
        }
        else
        {
            inRange = false;
        }
    }

    void EnemyTakeDamage(int dmg)
    {
        EnemyHealth -= dmg;

        if (EnemyHealth > 0)
        {
            //�G���U��������HP������
            Debug.Log("�����Ă���");
        }

        if (EnemyHealth <= 0)
        {
            inRange = false; //�U����߂�
            //�G�����ʃR�[�h
            Debug.Log("�I����");
        }
    }

    void FollowPlayer() //�v���C����ǂ�
    {
        if (Vector3.Distance(Target.position, transform.position) <= minRange)
        {
            transform.position = (transform.position - Target.position).normalized * minRange + Target.position;
        }
        else
        {
            //�v���C���[�Ɍ�����
            transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, EnemySpeed * Time.deltaTime);
        }
    }

    void EnemyAttack() //�U������
    {
        if (inRange)
        {
            if (Time.time >= fireRateTime + EnemyAttackSpeed)
            {
                var bulletInst = Instantiate(bullet, transform.position, transform.rotation);
                fireRateTime = Time.time;
            }
        }
    }
}
