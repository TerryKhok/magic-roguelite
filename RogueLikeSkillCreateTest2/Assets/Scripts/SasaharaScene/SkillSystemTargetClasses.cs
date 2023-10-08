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
        token.ThrowIfCancellationRequested();

        GameObject obj = Object.Instantiate(Resources.Load("ball") as GameObject);
        obj.transform.position = elem.GetLocationData().GetPos();
        obj.transform.rotation = elem.GetLocationData().GetRotate();

        obj.GetComponent<Rigidbody2D>().velocity = obj.transform.up * 3;

        UniTask<Collider2D> task1 = obj.GetAsyncTriggerEnter2DTrigger().OnTriggerEnter2DAsync(token);
        UniTask task2 = UniTask.Delay(3000);

        var awaiter = await UniTask.WhenAny(task1, task2);

        if (awaiter.result != null)
        {
            GameObject hit = awaiter.result.gameObject;

            if (hit.gameObject.CompareTag("Enemy"))
            {
                elem.AddTargets(hit.gameObject);
            }
            elem.SetLocationData(hit.gameObject.transform);
        }
        elem.SetLocationData(obj.gameObject.transform);
        Object.Destroy(obj);

        return elem;
    }

    void ISkillProgress.SkillProgressNoWait(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
    }
}
