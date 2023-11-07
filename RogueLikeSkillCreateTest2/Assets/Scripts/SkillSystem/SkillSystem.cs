using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SkillSystem
{
    public struct LocationData
    {
        Vector3 pos;
        Quaternion rotate;
        public LocationData(Transform tf)
        {
            pos = tf.position;
            rotate = tf.rotation;
        }

        public void ResetData(Transform tf)
        {
            pos = tf.position;
            rotate = tf.rotation;
        }
        public void ResetData(Vector3 pos, Quaternion rotate)
        {
            this.pos = pos;
            this.rotate = rotate;
        }
        public Vector3 GetPos() { return pos; }
        public Quaternion GetRotate() { return rotate; }
    }

    //�X�L���̃m�[�h�␳�l
    public struct SkillCorrection
    {
        float fixedValue;
        float multi;

        public SkillCorrection(float val = 0, float mul = 1)
        {
            fixedValue = val;
            multi = mul;
        }

        public void AddFixed(float value) { fixedValue += value; }
        public void AddMulti(float value) { fixedValue *= value; multi *= value; }
        public float GetFixedValue() { return fixedValue; }
        public float GetMulti() { return multi; }
    }

    public struct SkillElements
    {
        LocationData ld;
        List<GameObject> targets;
        SkillCorrection args;
        public SkillElements(Transform t)
        {
            ld = new LocationData(t);
            targets = new List<GameObject>();
            args = new SkillCorrection();
        }
        public LocationData GetLocationData() { return ld; }
        public List<GameObject> GetTargets() { return targets; }
        public SkillCorrection GetArgs() { return args; }

        public void AddTargets(GameObject obj) { targets.Add(obj); }
        public void SetLocationData(Transform tf) { ld.ResetData(tf); }
        public void SetLocationData(Vector3 pos, Quaternion rotate) { ld.ResetData(pos, rotate); }
    }

    public class SkillProgress
    {
        int _tier;

        public int GetTier() { return _tier; }
        public void GetTier(int t) { _tier = t; }

        public SkillProgress(int t)
        {
            _tier = t;
            //_args = new int[5];
        }

        public virtual async UniTask<SkillElements> RunProgress(SkillElements elem, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await UniTask.Delay(0);
            return elem;
        }

        public virtual void RunProgressNoWait(SkillElements elem, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
        }
    }

    //���[�v�������܂܂��m�[�h�Ɏ�������C���^�[�t�F�C�X
    public abstract class SkillLoopStartProgress : SkillProgress
    {
        public SkillLoopStartProgress(int t) : base(t) { }
        public abstract void AddLoopProgressList(SkillProgress progress);   //���[�v���Ŏ��s���鏈����ǉ�
        public abstract Type GetLoopEndProgressType();                       //���[�v�����I���m�[�h�̎擾�p�i�v�ݒ�j
    }

    //ID��񋓁B�g���̂�SkillCompile.cs��ConvertIdToISkillProgress�֐�
    public enum ProgressId
    {
        None,
        TargetBall,
        MechanicsDamage,
        MechanicsGenerateCube,
        SystemLoopStart,
        SystemLoopEnd,
        SystemWayStart,
        SystemWayEnd,

        EnemyTargetBall,
        EnemyMechanicsDamage,
    }

    //�X�L���f�[�^�x�[�X
    //1�̕ϐ��̕ۑ��`���i���O�ƃe�B�A�ɑΉ�����l�̔z��j
    public struct SkillVariable
    {
        public string name;
        public int[] values;
    }

    public static class SkillDB
    {
        public static Dictionary<ProgressId, List<SkillVariable>> g_SkillVariableMap = new Dictionary<ProgressId, List<SkillVariable>>();

        //�f�[�^�̃��[�h
        public static void Initialize()
        {
            TextAsset resource = Resources.Load("SkillSystem/SkillSystem_Variables") as TextAsset;  //csv�����[�h
            StringReader reader = new StringReader(resource.text);  //1�s���ǂݍ���
            while (reader.Peek() != -1) //�܂��s���ǂ߂�Ȃ�
            {
                ProgressId id = default;    //�}�b�v��Key�p
                List<SkillVariable> list = new List<SkillVariable>(); //�}�b�v��Value�p
                SkillVariable v = new SkillVariable();
                foreach (var str in reader.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries))    //�����Ȃ�����X���[����,���Ƃ̋�؂�Ń��[�v
                    if (id == default)
                    {
                        try
                        {
                            id = (ProgressId)Enum.Parse(typeof(ProgressId), str);   //�s�̂͂��߂�ProgressId�ɕϊ�
                        }
                        catch
                        {
                            Debug.Log("���݂��Ȃ������m�[�hID�ł� / " + str);       //�ϊ��ł��Ȃ��Ȃ烍�O�o���Ď��̍s
                            break;
                        }
                    }
                    else
                    {
                        if (v.name == default)
                        {
                            v.name = str;
                        }
                        else
                        {
                            v.values = str.Split("|").Select(int.Parse).ToArray();
                            list.Add(v);
                            v = new SkillVariable();
                        }
                    }
                if (id != default)
                    g_SkillVariableMap.Add(id, list);
            }
        }

        public static string GetSkillVariableName(ProgressId id, int idx)
        {
            string result;
            try
            {
                result = g_SkillVariableMap.GetValueOrDefault(id)[idx].name;
            }
            catch
            {
                Debug.Log("DB�ɒl���ݒ肳��Ă��܂���");
                throw;
            }
            return result;
        }

        public static int GetSkillVariableValue(ProgressId id, int idx, int tier)
        {
            int result;
            try
            {
                result = g_SkillVariableMap.GetValueOrDefault(id)[idx].values[tier];
            }
            catch
            {
                Debug.Log("DB�ɒl���ݒ肳��Ă��܂���");
                throw;
            }
            return result;
        }
    }


    //�X�L���R���p�C��
    public static class SkillCompile
    {
        public static List<SkillProgress> Compile(List<ProgressId> idList)
        {
            List<SkillProgress> list = new List<SkillProgress>(); //�ŏI�I�ɕԂ����X�g
            List<SkillProgress> surplus = new List<SkillProgress>();  //���[�v�����Ŏg�����X�g
            foreach (var id in idList)
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

        static (SkillProgress r_compress, List<SkillProgress> r_surplus) CompressProcess(SkillLoopStartProgress compress, List<SkillProgress> surplus)
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

        public static SkillProgress ConvertIdToISkillProgress(ProgressId id) //string�^��id��n����SkillProgress�̃C���X�^���X�ŕԂ��Ă����
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

}
