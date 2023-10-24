using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Transform target; //�ǂ��t���^�[�Q�b�g�i�v���C���j
    public Transform enemyHome; //�G���߂�ʒu
    [SerializeField]
    private float speed; //�G�̓����̑���
    [SerializeField]
    private float maxRange; //�v���C�����C�t���͈�
    [SerializeField]
    private float minRange; //�v���C���ɋ߂Â��Ǝ~�܂邽�߂͈̔�

    void Start()
    {
        target = FindObjectOfType<Player_Temp>().transform; //Player_Temp(player movement)�̃X�N���v�g���^�[�Q�b�g�Ƃ��Ďg���Ă�B
    }

    void Update()
    {
        if (Vector3.Distance(target.position, transform.position) <= maxRange && Vector3.Distance(target.position, transform.position) >= minRange)
        {
            FollowPlayer();
        }
        else if (Vector3.Distance(target.position, transform.position) > maxRange)
        {
            ReturnHome();
        }
    }

    public void FollowPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    public void ReturnHome()
    {
        transform.position = Vector3.MoveTowards(transform.position, enemyHome.position, speed * Time.deltaTime);
    }
}
