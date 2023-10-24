using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
//���̃X�N���v�g��G�ɂ������Ă�������
//

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    int EnemyType;           //�G�̎��
    //�G�̎��
    //�P�F�߂�
    //�Q�F����ȋ߂��Ȃ�
    //�R�F����
    [SerializeField]
    int EnemyHealth;         //�G��HP
    [SerializeField]
    int EnemyDamage;         //�G�̗�
    [SerializeField]
    float EnemySpeed;        //�G�̓����̑���
    [SerializeField]
    int EnemyAttackSpeed;    //�G�̍U���̑���
    private Transform Target;//�ǂ��t���^�[�Q�b�g�i�v���C���j

    private float maxRange;     //�v���C�����C�t���͈�
    private float minRange;     //�v���C���ɋ߂Â��Ǝ~�܂邽�߂͈̔�

    void Start()
    {
        Target = FindObjectOfType<player_test>().transform;
    }

    void Update()
    {
        EnemySet();
        EnemyTakeDamage();
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
                minRange = 3;
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
        }
    }

    void MediumRange()   //����ȋ߂��Ȃ��G
    {
        //�G�͍U������֐�
        if (Vector3.Distance(Target.position, transform.position) <= maxRange && Vector3.Distance(Target.position, transform.position) >= minRange - 1)
        {
            FollowPlayer();
        }
    }

    void LongRange()     //�����G
    {
        //�G�͍U������֐�
        if (Vector3.Distance(Target.position, transform.position) <= maxRange && Vector3.Distance(Target.position, transform.position) >= minRange - 1)
        {
            FollowPlayer();
        }
    }

    void EnemyTakeDamage()
    {
        if (EnemyHealth > 0)
        {
            //�����G���U�������HP������֐�
            Debug.Log("�����Ă���");
        }

        if (EnemyHealth <= 0)
        {
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
            transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, EnemySpeed * Time.deltaTime);
        }
    }
}
