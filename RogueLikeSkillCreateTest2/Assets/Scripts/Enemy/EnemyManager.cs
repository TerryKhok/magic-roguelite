using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
//このスクリプトを敵にくっつけてください
//

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    int EnemyType;           //敵の種類
    //敵の種類
    //１：近い
    //２：そんな近くない
    //３：遠い
    [SerializeField]
    int EnemyHealth;         //敵のHP
    [SerializeField]
    int EnemyDamage;         //敵の力
    [SerializeField]
    float EnemySpeed;        //敵の動きの速さ
    [SerializeField]
    int EnemyAttackSpeed;    //敵の攻撃の速さ
    private Transform Target;//追い付くターゲット（プレイヤ）

    private float maxRange;     //プレイヤを気付く範囲
    private float minRange;     //プレイヤに近づくと止まるための範囲

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
                Debug.Log("敵の種類は無効です");
                break;
        }
    }

    //
    //敵の色々な関数
    //
    //ShortRange, MediumRange,
    //LongRange, EnemyTakeDamage, FollowPlayer
    //

    void ShortRange()    //近い敵
    {
        //敵は攻撃する関数
        if (Vector3.Distance(Target.position, transform.position) <= maxRange && Vector3.Distance(Target.position, transform.position) >= minRange - 1)
        {
            FollowPlayer();
        }
    }

    void MediumRange()   //そんな近くない敵
    {
        //敵は攻撃する関数
        if (Vector3.Distance(Target.position, transform.position) <= maxRange && Vector3.Distance(Target.position, transform.position) >= minRange - 1)
        {
            FollowPlayer();
        }
    }

    void LongRange()     //遠い敵
    {
        //敵は攻撃する関数
        if (Vector3.Distance(Target.position, transform.position) <= maxRange && Vector3.Distance(Target.position, transform.position) >= minRange - 1)
        {
            FollowPlayer();
        }
    }

    void EnemyTakeDamage()
    {
        if (EnemyHealth > 0)
        {
            //もし敵が攻撃さればHPが減る関数
            Debug.Log("生きている");
        }

        if (EnemyHealth <= 0)
        {
            //敵が死ぬコード
            Debug.Log("的死んだ");
        }
    }

    void FollowPlayer() //プレイヤを追う
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
