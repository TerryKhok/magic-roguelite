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
        _skillSlot.Add(GetComponent<SkillCompile>().GetSkill());
        Debug.Log(_skillSlot.Count);
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
            } catch ( OperationCanceledException e)
            {
                Debug.Log(e.ToString() + " / スキル実行中にキャンセル");
            }
        }
    }
}
