using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor;

public class MechanicsDamage : SkillProgress, ISkillProgress
{
    public MechanicsDamage(int t) : base(t, new int[] { 10 })
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
        int dmg = GetArgsValue(0);
        token.ThrowIfCancellationRequested();
        elem.GetTargets().ForEach(t => { Object.Destroy(t); });
    }
}

public class MechanicsGenerateCube : SkillProgress, ISkillProgress
{
    public MechanicsGenerateCube(int t) : base(t, new int[] { 2000 })
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

        int lifeTime = GetArgsValue(0);

        GameObject resource = Resources.Load("SkillSystem/SkillSystem_Mechanics_GenerateCube_Stub") as GameObject;
        GameObject cube = Object.Instantiate(resource, elem.GetLocationData().GetPos(), elem.GetLocationData().GetRotate());
        await UniTask.Delay(lifeTime);
        Object.Destroy(cube);
    }
}

