using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Transform target; //追い付くターゲット（プレイヤ）
    public Transform enemyHome; //敵が戻る位置
    [SerializeField]
    private float speed; //敵の動きの速さ
    [SerializeField]
    private float maxRange; //プレイヤを気付く範囲
    [SerializeField]
    private float minRange; //プレイヤに近づくと止まるための範囲

    void Start()
    {
        target = FindObjectOfType<Player_Temp>().transform; //Player_Temp(player movement)のスクリプトをターゲットとして使ってる。
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
