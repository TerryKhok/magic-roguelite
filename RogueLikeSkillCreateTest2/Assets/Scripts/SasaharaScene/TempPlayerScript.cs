using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerScript : MonoBehaviour
{

    Rigidbody2D rb2;
    // Start is called before the first frame update
    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb2.velocity = transform.up;
        }
        if (Input.GetKey(KeyCode.A)) {
            rb2.angularVelocity = 30.0f;
        }
        if(Input.GetKey(KeyCode.D)) {
            rb2.angularVelocity = -30.0f;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            rb2.velocity = Vector2.zero;
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            rb2.angularVelocity = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<SkillUser>().RunSkill(0);
        }
    }
}
