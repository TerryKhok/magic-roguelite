using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillCompile : MonoBehaviour
{
    [SerializeField] List<progressId> _progressIdList = new List<progressId>();

    public List<ISkillProgress> GetSkill()
    {
        return Compile();
    }



    List<ISkillProgress> Compile()
    {
        List<ISkillProgress> list = new List<ISkillProgress>(); //�ŏI�I�ɕԂ����X�g
        List<ISkillProgress> surplus = new List<ISkillProgress>();  //���[�v�����Ŏg�����X�g
        foreach (var id in _progressIdList)
        {       //progressId�^�̃��X�g��ISkillProgress�^�̃��X�g��
            surplus.Add(ConvertIdToISkillProgress(id));
        }

        do
        {
            foreach (var progress in surplus.ToArray()) //�r����surplus��ύX�ł��郋�[�v
            {
                surplus.Remove(progress);
                if (progress is ISkillLoopStartProgress)
                {
                    var result = CompressProcess((ISkillLoopStartProgress)progress, surplus);
                    list.Add(result.r_compress);

                    surplus = new List<ISkillProgress>(result.r_surplus);
                    Debug.Log(surplus.Count);
                    break;
                }
                else
                {
                    list.Add(progress);
                }
            }
        } while (surplus.Count > 0);
        return list;
    }

    (ISkillProgress r_compress, List<ISkillProgress> r_surplus) CompressProcess(ISkillLoopStartProgress compress, List<ISkillProgress> surplus)
    {   //���[�v�n���������k���鏈��
        (ISkillProgress r_compress, List<ISkillProgress> r_surplus) result = default;
        Type compressEnd = compress.GetLoopEndProgressType();
        do
        {
            foreach (ISkillProgress progress in surplus.ToArray())
            {
                surplus.Remove(progress);
                if (progress is ISkillLoopStartProgress)
                {
                    var compResult = CompressProcess((ISkillLoopStartProgress)progress, surplus);
                    compress.AddLoopProgressList(compResult.r_compress);
                    surplus = compResult.r_surplus;
                    break;
                }
                else if (progress.GetType().Equals(compressEnd))
                {
                    Debug.Log(progress.GetType());
                    result.r_compress = compress;
                    result.r_surplus = surplus;
                    return result;
                }
                else
                {
                    compress.AddLoopProgressList(progress);
                }
            }
        } while (surplus.Count > 0);

        return result;
    }

    public ISkillProgress ConvertIdToISkillProgress(progressId id) //string�^��id��n����ISkillProgress�̃C���X�^���X�ŕԂ��Ă����
    {
        switch (id)
        {
            case progressId.TargetBall:
                return new TargetBall(1);
            case progressId.MechanicsDamage:
                return new MechanicsDamage(1);
            case progressId.MechanicsGenerateCube:
                return new MechanicsGenerateCube(1);
            case progressId.SystemLoopStart:
                return new SystemLoopStart(1);
            case progressId.SystemLoopEnd:
                return new SystemLoopEnd(1);
            case progressId.SystemWayStart:
                return new SystemWayStart(1);
            case progressId.SystemWayEnd:
                return new SystemWayEnd(1);
        }
        return null;
    }
}
