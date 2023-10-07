using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillSystem;
using Cysharp.Threading.Tasks;
using System.Threading;

public class MechanicsDamage : SkillProgress, ISkillProgress
{
    public MechanicsDamage(int t) : base(t) { Debug.Log("MechanicsDamage Generated"); }
    async UniTask<SkillElements> ISkillProgress.SkillProgress(SkillElements elem, CancellationToken token)
    {
        await UniTask.Delay(0);
        return elem;
    }

    void ISkillProgress.SkillProgressNoWait(SkillElements elem, CancellationToken token)
    {
        elem.GetTargets().ForEach(t => { Object.Destroy(t); });
    }
}

