using Cysharp.Threading.Tasks;
using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillUser : MonoBehaviour
{
    private List<List<SkillProgress>> _skillSlot = new List<List<SkillProgress>>();
    [SerializeField] SkillElements _elem = new();

    void Start()
    {
        SkillDB.Initialize();
        _skillSlot.Add(GetComponent<SkillCreater>().GetSkill());
    }

    //スキルの実行
    public async void RunSkill(int num)
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();         //UniTask用トークン取得
        List<SkillProgress> code = new List<SkillProgress>(_skillSlot[num]);    //実行するスキルをスロットから選択
        _elem.GetLocationData().ResetData(gameObject.transform);

        foreach (SkillProgress progress in code)    //処理を回して
        {
            try
            {
                progress.RunProgressNoWait(_elem, token);       //処理の終了を待たないやつ
                _elem = await progress.RunProgress(_elem, token);   //処理の終了を待つやつ
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e.ToString() + " / スキル実行中にキャンセル");    //スキル実行中にエラーが出たらメッセージ
            }
        }
    }
}
