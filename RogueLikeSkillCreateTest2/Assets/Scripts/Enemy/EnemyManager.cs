using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
//‚±‚ÌƒXƒNƒŠƒvƒg‚ğ“G‚É‚­‚Á‚Â‚¯‚Ä‚­‚¾‚³‚¢
//

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    int EnemyType;           //“G‚Ìí—Ş
    //“G‚Ìí—Ş
    //‚PF‹ß‚¢
    //‚QF‚»‚ñ‚È‹ß‚­‚È‚¢
    //‚RF‰“‚¢
    [SerializeField]
    int EnemyHealth;         //“G‚ÌHP
    [SerializeField]
    int EnemyDamage;         //“G‚Ì—Í
    [SerializeField]
    float EnemySpeed;        //“G‚Ì“®‚«‚Ì‘¬‚³
    [SerializeField]
    int EnemyAttackSpeed;    //“G‚ÌUŒ‚‚Ì‘¬‚³
    private Transform Target;//’Ç‚¢•t‚­ƒ^[ƒQƒbƒgiƒvƒŒƒCƒ„j

    private float maxRange;     //ƒvƒŒƒCƒ„‚ğ‹C•t‚­”ÍˆÍ
    private float minRange;     //ƒvƒŒƒCƒ„‚É‹ß‚Ã‚­‚Æ~‚Ü‚é‚½‚ß‚Ì”ÍˆÍ

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
        }
    }

    void MediumRange()   //‚»‚ñ‚È‹ß‚­‚È‚¢“G
    {
        //“G‚ÍUŒ‚‚·‚éŠÖ”
        if (Vector3.Distance(Target.position, transform.position) <= maxRange && Vector3.Distance(Target.position, transform.position) >= minRange - 1)
        {
            FollowPlayer();
        }
    }

    void LongRange()     //‰“‚¢“G
    {
        //“G‚ÍUŒ‚‚·‚éŠÖ”
        if (Vector3.Distance(Target.position, transform.position) <= maxRange && Vector3.Distance(Target.position, transform.position) >= minRange - 1)
        {
            FollowPlayer();
        }
    }

    void EnemyTakeDamage()
    {
        if (EnemyHealth > 0)
        {
            //‚à‚µ“G‚ªUŒ‚‚³‚ê‚ÎHP‚ªŒ¸‚éŠÖ”
            Debug.Log("¶‚«‚Ä‚¢‚é");
        }

        if (EnemyHealth <= 0)
        {
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
            transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, EnemySpeed * Time.deltaTime);
        }
    }
}
