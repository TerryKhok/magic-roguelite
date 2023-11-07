using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField]
    float bulletForce;    //弾丸の速さ

    private GameObject player;
    private Rigidbody2D rb;
    Vector3 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        direction = player.transform.position - transform.position;
        BulletFire();
    }

    void BulletFire() //弾丸のうつ
    {
        rb.velocity = new Vector2(direction.x, direction.y).normalized * bulletForce;
        Destroy(gameObject, 3);
    }

    void OnCollisionEnter2D(Collision2D collision) //プレイヤーを当たる
    {
        if (collision.gameObject.tag == "Player")
        {
            //プレイヤーをダメージする関数
            Destroy(gameObject);
            Debug.Log("プレイヤーを当たった");
        }
    }
}
