using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using Cysharp.Threading.Tasks.Triggers;
using UnityEditor;



public class TargetBall : SkillProgress, ISkillProgress
{
    public TargetBall(int t) : base(t) { Debug.Log("TargetBall Generated"); }

    async UniTask<SkillElements> ISkillProgress.SkillProgress(SkillElements elem, CancellationToken token)
    {
        GameObject obj = new GameObject("ball", typeof(CircleCollider2D), typeof(Rigidbody2D));
        obj.GetComponent<CircleCollider2D>().isTrigger = true;
        obj.GetComponent<Rigidbody2D>().gravityScale = 0.0f;

        obj.transform.position = elem.GetTransform().position;
        obj.transform.rotation = elem.GetTransform().rotation;

        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(1, 1);
        Debug.Log(obj.GetComponent<Rigidbody2D>().velocity + "tsts");

        Collider2D hit;
        hit = await obj.GetAsyncTriggerEnter2DTrigger().OnTriggerEnter2DAsync(token);

        if (hit.gameObject.CompareTag("Enemy"))
        {
            elem.AddTargets(hit.gameObject);
        }
        elem.SetTransform(hit.gameObject.transform);

        Object.Destroy(obj);

        return elem;
    }

    void ISkillProgress.SkillProgressNoWait(SkillElements elem, CancellationToken token)
    {

    }
}
