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

    //�X�L���̎��s
    public async void RunSkill(int num)
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();         //UniTask�p�g�[�N���擾
        List<SkillProgress> code = new List<SkillProgress>(_skillSlot[num]);    //���s����X�L�����X���b�g����I��
        _elem.GetLocationData().ResetData(gameObject.transform);

        foreach (SkillProgress progress in code)    //�������񂵂�
        {
            try
            {
                progress.RunProgressNoWait(_elem, token);       //�����̏I����҂��Ȃ����
                _elem = await progress.RunProgress(_elem, token);   //�����̏I����҂��
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e.ToString() + " / �X�L�����s���ɃL�����Z��");    //�X�L�����s���ɃG���[���o���烁�b�Z�[�W
            }
        }
    }
}
