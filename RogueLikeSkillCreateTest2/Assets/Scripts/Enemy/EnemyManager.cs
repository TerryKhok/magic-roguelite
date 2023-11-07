using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
//‚±‚ÌƒXƒNƒŠƒvƒg‚ğ“G‚É‚­‚Á‚Â‚¯‚Ä‚­‚¾‚³‚¢
//

public class EnemyManager : MonoBehaviour
{
    //“G‚ÌŠî–{“I•Ï”
    [SerializeField]
    private int EnemyType;  //“G‚Ìí—Ş
    //“G‚Ìí—Ş
    //‚PF‹ß‚¢
    //‚QF‚»‚ñ‚È‹ß‚­‚È‚¢
    //‚RF‰“‚¢
    [SerializeField]
    int EnemyHealth;        //“G‚ÌHP
    [SerializeField]
    float EnemySpeed;       //“G‚Ì“®‚«‚Ì‘¬‚³

    //UŒ‚•Ï”
    [SerializeField]
    float EnemyAttackSpeed;   //“G‚ÌUŒ‚‚Ì‘¬‚³
    [SerializeField]
    int EnemyDamage;        //“G‚Ì—Í

    //‘¼‚Ì•Ï”
    float fireRateTime = 0.0f;  //’eŠÛ‚Ì”­Ë‚ÌŠÔŠi”[‚½‚ß
    public GameObject bullet;   //’eŠÛ
    private Transform Target;   //’Ç‚¢•t‚­ƒ^[ƒQƒbƒgiƒvƒŒƒCƒ„j
    private float maxRange;     //ƒvƒŒƒCƒ„‚ğ‹C•t‚­”ÍˆÍ
    private float minRange;     //ƒvƒŒƒCƒ„‚É‹ß‚Ã‚­‚Æ~‚Ü‚é‚½‚ß‚Ì”ÍˆÍ
    private bool inRange = false;       //“G‚ÍƒvƒŒƒCƒ„[‚Ì”ÍˆÍ‚É‚¢‚éƒtƒ‰ƒO

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
                Debug.Log("“G‚Ìí—Ş‚Í–³Œø‚Å‚·");
                break;
        }
    }

    //
    //“G‚ÌFX‚ÈŠÖ”
    //
    //ShortRange, MediumRange,
    //LongRange, EnemyTakeDamage, FollowPlayer
    //

    void ShortRange()    //‹ß‚¢“G
    {
        //“G‚ÍUŒ‚‚·‚éŠÖ”
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

    void MediumRange()   //‚»‚ñ‚È‹ß‚­‚È‚¢“G
    {
        //“G‚ÍUŒ‚‚·‚éŠÖ”
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

    void LongRange()     //‰“‚¢“G
    {
        //“G‚ÍUŒ‚‚·‚éŠÖ”
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
            //“G‚ªUŒ‚‚³‚ê‚é‚ÆHP‚ªŒ¸‚é
            Debug.Log("¶‚«‚Ä‚¢‚é");
        }

        if (EnemyHealth <= 0)
        {
            inRange = false; //UŒ‚‚â‚ß‚é
            //“G‚ª€‚ÊƒR[ƒh
            Debug.Log("“I€‚ñ‚¾");
        }
    }

    void FollowPlayer() //ƒvƒŒƒCƒ„‚ğ’Ç‚¤
    {
        if (Vector3.Distance(Target.position, transform.position) <= minRange)
        {
            transform.position = (transform.position - Target.position).normalized * minRange + Target.position;
        }
        else
        {
            //ƒvƒŒƒCƒ„[‚ÉŒü‚©‚¤
            transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, EnemySpeed * Time.deltaTime);
        }
    }

    void EnemyAttack() //UŒ‚ˆ—
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
