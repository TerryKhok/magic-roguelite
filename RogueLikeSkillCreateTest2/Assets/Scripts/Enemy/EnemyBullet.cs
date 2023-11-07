using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField]
    float bulletForce;    //�e�ۂ̑���

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

    void BulletFire() //�e�ۂ̂���
    {
        rb.velocity = new Vector2(direction.x, direction.y).normalized * bulletForce;
        Destroy(gameObject, 3);
    }

    void OnCollisionEnter2D(Collision2D collision) //�v���C���[�𓖂���
    {
        if (collision.gameObject.tag == "Player")
        {
            //�v���C���[���_���[�W����֐�
            Destroy(gameObject);
            Debug.Log("�v���C���[�𓖂�����");
        }
    }
}
