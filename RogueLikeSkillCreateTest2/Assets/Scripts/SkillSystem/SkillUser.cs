using Cysharp.Threading.Tasks;
using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillUser : MonoBehaviour
{
    private List<Skill> _skillSlot = new List<Skill>();

    void Start()
    {
        SkillDB.Initialize();
        _skillSlot.Add(GetComponent<SkillCreater>().GetSkill());
    }

    //スキルの実行
    public async void RunSkill(int num)
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();         //UniTask用トークン取得
        SkillElements elem = new SkillElements(gameObject.transform);
        foreach (SkillPartsData part in _skillSlot[num].GetElem().GetPartsData())
        {
            elem.GetAttr().AddAttr(part.GetId().ToString(), part.GetCor());
        }

        foreach (SkillProgress progress in _skillSlot[num].GetProgress())    //処理を回して
        {
            try
            {
                progress.RunProgressNoWait(elem, token);       //処理の終了を待たないやつ
                elem = await progress.RunProgress(elem, token);   //処理の終了を待つやつ
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e.ToString() + " / スキル実行中にキャンセル");    //スキル実行中にエラーが出たらメッセージ
            }
        }
    }
}
