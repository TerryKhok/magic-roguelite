using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor;

public class MechanicsDamage : SkillProgress, ISkillProgress
{
    public MechanicsDamage(int t) : base(t)
    {
        Debug.Log($"[Generated] MechanicsDamage: {t}");
    }
    async UniTask<SkillElements> ISkillProgress.SkillProgress(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await UniTask.Delay(0);
        return elem;
    }

    void ISkillProgress.SkillProgressNoWait(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        elem.GetTargets().ForEach(t => { Object.Destroy(t); });
    }
}

public class MechanicsGenerateCube : SkillProgress, ISkillProgress
{
    public MechanicsGenerateCube(int t) : base(t)
    {
        Debug.Log($"[Generated] MechanicsGenerateCube: {t}");
    }

    async UniTask<SkillElements> ISkillProgress.SkillProgress(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await UniTask.Delay(0);
        return elem;
    }

    async void ISkillProgress.SkillProgressNoWait(SkillElements elem, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        GameObject resource = Resources.Load("OhItsCube") as GameObject;
        GameObject cube = Object.Instantiate(resource, elem.GetLocationData().GetPos(), elem.GetLocationData().GetRotate());
        await UniTask.Delay(2000);
        Object.Destroy(cube);
    }
}

