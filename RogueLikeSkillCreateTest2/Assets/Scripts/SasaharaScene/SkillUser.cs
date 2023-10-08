using Cysharp.Threading.Tasks;
using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillUser : MonoBehaviour
{
    private List<List<ISkillProgress>> _skillSlot = new List<List<ISkillProgress>>();

    void Start()
    {
        List<ISkillProgress> list = new List<ISkillProgress>();
        list.Add(new TargetBall(1));
        list.Add(new MechanicsGenerateCube(1));
        _skillSlot.Add(new List<ISkillProgress>(list));
    }


    public async void RunSkill(int num)
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        List<ISkillProgress> code = new List<ISkillProgress>(_skillSlot[num]);
        SkillElements elem = new SkillElements(gameObject.transform);

        foreach (ISkillProgress progress in code)
        {
            try {
                progress.SkillProgressNoWait(elem, token);
                elem = await progress.SkillProgress(elem, token);
                Debug.Log(elem.GetLocationData().GetPos());
            } catch ( OperationCanceledException e)
            {
                Debug.Log(e.ToString() + " / スキル実行中にキャンセル");
            }
        }
    }
}
