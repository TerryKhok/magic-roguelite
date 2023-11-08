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

    //�X�L���̎��s
    public async void RunSkill(int num)
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();         //UniTask�p�g�[�N���擾
        SkillElements elem = new SkillElements(gameObject.transform);
        foreach (SkillPartsData part in _skillSlot[num].GetElem().GetPartsData())
        {
            elem.GetAttr().AddAttr(part.GetId().ToString(), part.GetCor());
        }

        foreach (SkillProgress progress in _skillSlot[num].GetProgress())    //�������񂵂�
        {
            try
            {
                progress.RunProgressNoWait(elem, token);       //�����̏I����҂��Ȃ����
                elem = await progress.RunProgress(elem, token);   //�����̏I����҂��
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e.ToString() + " / �X�L�����s���ɃL�����Z��");    //�X�L�����s���ɃG���[���o���烁�b�Z�[�W
            }
        }
    }
}
