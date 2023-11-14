using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using Cysharp.Threading.Tasks.Triggers;
using UnityEditor;



public class TargetBall : SkillProgress
{
    public TargetBall(int t) : base(t)
    {
        Debug.Log($"[Generated] TargetBall: {t}");
    }

    public override async UniTask<SkillElements> RunProgress(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        ProgressId id = ProgressId.TargetBall;
        int speed = SkillDB.GetSkillVariableValue(id, 0, GetTier());
        int lifeTime = SkillDB.GetSkillVariableValue(id, 1, GetTier());

        SkillAttributes attr = elem.GetAttr();
        if (attr.IsExist("SpeedInc"))
        {
            speed = (int)(speed * attr.GetCorrection("SpeedInc").GetMulti());
            speed = (int)(speed + attr.GetCorrection("SpeedInc").GetFixedValue() * 2);
        }
        if (attr.IsExist("RangeInc"))
        {
            lifeTime = (int)(lifeTime * attr.GetCorrection("RangeInc").GetMulti());
            lifeTime = (int)(lifeTime + attr.GetCorrection("RangeInc").GetFixedValue() * 200);
        }

        GameObject obj = Object.Instantiate(Resources.Load("SkillSystem/SkillSystem_Target_Ball_Stub") as GameObject);
        Transform objtf = obj.transform;
        objtf.position = elem.GetLocationData().GetPos();
        objtf.rotation = elem.GetLocationData().GetRotate();

        obj.GetComponent<Rigidbody2D>().velocity = objtf.up * speed;

        UniTask<Collider2D> task1 = obj.GetAsyncTriggerEnter2DTrigger().OnTriggerEnter2DAsync(token);
        UniTask task2 = UniTask.Delay(lifeTime);

        var awaiter = await UniTask.WhenAny(task1, task2);

        if (awaiter.result != null)
        {
            GameObject hit = awaiter.result.gameObject;

            if (elem.IsTarget(hit.gameObject))
            {
                Debug.Log(hit.gameObject.name);
                elem.AddTargets(hit.gameObject);
            }
            elem.SetLocationData(hit.gameObject.transform);
        }
        elem.SetLocationData(obj.gameObject.transform);
        Object.Destroy(obj);

        return elem;
    }
}