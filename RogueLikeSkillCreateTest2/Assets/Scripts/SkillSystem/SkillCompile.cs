using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillCompile : MonoBehaviour
{
    [SerializeField] List<ProgressId> _progressIdList = new List<ProgressId>();

    public List<SkillProgress> GetSkill()
    {
        return Compile();
    }



    List<SkillProgress> Compile()
    {
        List<SkillProgress> list = new List<SkillProgress>(); //�ŏI�I�ɕԂ����X�g
        List<SkillProgress> surplus = new List<SkillProgress>();  //���[�v�����Ŏg�����X�g
        foreach (var id in _progressIdList)
        {       //progressId�^�̃��X�g��SkillProgress�^�̃��X�g��
            surplus.Add(ConvertIdToISkillProgress(id));
        }

        do
        {
            foreach (var progress in surplus.ToArray()) //�r����surplus��ύX�ł��郋�[�v
            {
                surplus.Remove(progress);
                if (progress is SkillLoopStartProgress)
                {
                    var result = CompressProcess((SkillLoopStartProgress)progress, surplus);
                    list.Add(result.r_compress);

                    surplus = new List<SkillProgress>(result.r_surplus);
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

    (SkillProgress r_compress, List<SkillProgress> r_surplus) CompressProcess(SkillLoopStartProgress compress, List<SkillProgress> surplus)
    {   //���[�v�n���������k���鏈��
        (SkillProgress r_compress, List<SkillProgress> r_surplus) result = default;
        Type compressEnd = compress.GetLoopEndProgressType();
        do
        {
            foreach (SkillProgress progress in surplus.ToArray())
            {
                surplus.Remove(progress);
                if (progress is SkillLoopStartProgress)
                {
                    var compResult = CompressProcess((SkillLoopStartProgress)progress, surplus);
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

    public SkillProgress ConvertIdToISkillProgress(ProgressId id) //string�^��id��n����SkillProgress�̃C���X�^���X�ŕԂ��Ă����
    {
        switch (id)
        {
            case ProgressId.TargetBall:
                return new TargetBall(1);
            case ProgressId.MechanicsDamage:
                return new MechanicsDamage(1);
            case ProgressId.MechanicsGenerateCube:
                return new MechanicsGenerateCube(1);
            case ProgressId.SystemLoopStart:
                return new SystemLoopStart(1);
            case ProgressId.SystemLoopEnd:
                return new SystemLoopEnd(1);
            case ProgressId.SystemWayStart:
                return new SystemWayStart(1);
            case ProgressId.SystemWayEnd:
                return new SystemWayEnd(1);
        }
        return null;
    }
}
