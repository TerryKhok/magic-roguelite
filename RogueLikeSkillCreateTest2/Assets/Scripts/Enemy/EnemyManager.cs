using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
//このスクリプトを敵にくっつけてください
//

public class EnemyManager : MonoBehaviour
{
    //敵の基本的変数
    [SerializeField]
    private int EnemyType;  //敵の種類
    //敵の種類
    //１：近い
    //２：そんな近くない
    //３：遠い
    [SerializeField]
    int EnemyHealth;        //敵のHP
    [SerializeField]
    float EnemySpeed;       //敵の動きの速さ

    //攻撃変数
    [SerializeField]
    float EnemyAttackSpeed;   //敵の攻撃の速さ
    [SerializeField]
    int EnemyDamage;        //敵の力

    //他の変数
    float fireRateTime = 0.0f;  //弾丸の発射の時間格納ため
    public GameObject bullet;   //弾丸
    private Transform Target;   //追い付くターゲット（プレイヤ）
    private float maxRange;     //プレイヤを気付く範囲
    private float minRange;     //プレイヤに近づくと止まるための範囲
    private bool inRange = false;       //敵はプレイヤーの範囲にいるフラグ

    //ドロップアイテム
    DropItemEnemy _drop;

    void Start()
    {
        Target = FindObjectOfType<PlayerMovement>().transform;
        _drop = GetComponent<DropItemEnemy>();
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
            inRange = true;
        }
        else
        {
            inRange = false;
        }
    }

    void MediumRange()   //そんな近くない敵
    {
        //敵は攻撃する関数
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

    void LongRange()     //遠い敵
    {
        //敵は攻撃する関数
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

    public void EnemyTakeDamage(int dmg)
    {
        Debug.Log("dmg" + dmg);
        EnemyHealth -= dmg;

        if (EnemyHealth > 0)
        {
            //敵が攻撃されるとHPが減る
            Debug.Log("生きている");
        }

        if (EnemyHealth <= 0)
        {
            inRange = false; //攻撃やめる
            //敵が死ぬコード
            EnemyDeath();
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
            //プレイヤーに向かう
            transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, EnemySpeed * Time.deltaTime);
        }
    }

    void EnemyAttack() //攻撃処理
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

    void EnemyDeath()
    {
        _drop.ItemDrop();
        Destroy(gameObject);
    }
}
