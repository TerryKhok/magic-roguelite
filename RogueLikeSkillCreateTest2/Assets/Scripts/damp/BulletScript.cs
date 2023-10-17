using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private int start;
    private List<GameObject> targets;
    private Vector2 pos;
    private SkillCreateScript scs;
    private int penetrate;

    public BulletScript setStart(int start) { this.start = start; return this; }
    public BulletScript setTargets(in List<GameObject> targets) { this.targets = new List<GameObject>(targets); return this; }
    public BulletScript setPos(in Vector2 pos) { this.pos = pos; return this; }
    public BulletScript setScs(in SkillCreateScript scs) { this.scs = scs; return this; }
    public BulletScript setPenetrate(int pen) { penetrate = pen; return this; }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            string tag = collision.tag;
            if (tag != "Player" && tag != "Bullet")
            {
                Debug.Log(collision.gameObject.name);
                targets.Add(collision.gameObject);
                pos = collision.gameObject.transform.position;
                scs.runSkillCode(start, targets, pos);
                if (penetrate > 0)
                    penetrate--;
                else
                    Destroy(gameObject);
            }
        }
    }
}
