using Cysharp.Threading.Tasks;
using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SkillSystem
{
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
    }

    public enum SkillPartsId
    {
        AttackInc,
        RangeInc,
        SpeedInc,
        BootInc,
        ThreeWay,
        Loop,
    }


    public struct Skill
    {
        SkillCompileElements elem;
        List<ProgressId> idList;
        List<SkillProgress> progress;
        public Skill(List<ProgressId> ids)
        {
            elem = new SkillCompileElements(0);
            idList = new List<ProgressId>(ids);
            progress = new List<SkillProgress>(SkillCompile.Compile(idList));
        }

        public SkillCompileElements GetElem() { return elem; }
        public List<SkillProgress> GetProgress() { return progress; }

        public void GiveElemEffect() { progress = SkillCompile.ElemWrap(progress, elem); }
    }

    public struct SkillCompileElements
    {
        List<SkillPartsData> partsData;

        public SkillCompileElements(int a)
        {
            partsData = new();
        }
        public List<SkillPartsData> GetPartsData() { return partsData; }

        public void AddPartsData(SkillPartsData d) { partsData.Add(d); }
    }

    public struct SkillElements
    {
        LocationData ld;
        List<GameObject> targets;
        SkillAttributes attr;
        bool casterIsPlayer;    //true=player, false=enemy
        public SkillElements(Transform t)
        {
            ld = new LocationData(t);
            targets = new List<GameObject>();
            attr = new SkillAttributes(0);
            casterIsPlayer = true;
        }
        public LocationData GetLocationData() { return ld; }
        public List<GameObject> GetTargets() { return targets; }
        public SkillAttributes GetAttr() { return attr; }
        public bool IsPlayer() { return casterIsPlayer; }

        public void AddTargets(GameObject obj) { targets.Add(obj); }
        public void SetLocationData(Transform tf) { ld.ResetData(tf); }
        public void SetLocationData(Vector3 pos, Quaternion rotate) { ld.ResetData(pos, rotate); }
        public void SetCasterType(bool c) { casterIsPlayer = c; }

        public bool IsTarget(GameObject obj)
        {
            if (casterIsPlayer)
            {
                return obj.CompareTag("Enemy");
            }
            else
            {
                return obj.CompareTag("Player");
            }
        }
    }

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

    public struct SkillAttributes
    {
        Dictionary<string, SkillCorrection> attr;
        public SkillAttributes(int a)
        {
            attr = new();
        }
        public SkillCorrection GetCorrection(string key) { return attr[key]; }
        public bool IsExist(string key) { return attr.ContainsKey(key); }

        public void AddAttr(string key, SkillCorrection value)
        {
            if (attr.ContainsKey(key))
            {
                SkillCorrection temp;
                attr.TryGetValue(key, out temp);
                temp.AddFixed(value.GetFixedValue());
                temp.AddMulti(value.GetMulti());
            }
            else
            {
                attr.Add(key, value);
            }
        }
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

        public static List<SkillProgress> ElemWrap(List<SkillProgress> progressList, SkillCompileElements elem)
        {
            List<SkillProgress> result = new List<SkillProgress>(progressList);
            SkillLoopStartProgress prog;
            foreach (SkillPartsData part in elem.GetPartsData())
            {
                switch (part.GetId())
                {
                    case SkillPartsId.ThreeWay:
                        prog = new SystemWayStart(0);
                        foreach (var progress in result)
                        {
                            prog.AddLoopProgressList(progress);
                        }
                        result.Clear();
                        result.Add(prog);
                        break;
                    case SkillPartsId.Loop:
                        prog = new SystemLoopStart(0);
                        foreach (var progress in result)
                        {
                            prog.AddLoopProgressList(progress);
                        }
                        result.Clear();
                        result.Add(prog);
                        break;
                }
            }
            return result;
        }

        public static SkillProgress ConvertIdToISkillProgress(ProgressId id) //string�^��id��n����SkillProgress�̃C���X�^���X�ŕԂ��Ă����
        {
            switch (id)
            {
                case ProgressId.TargetBall:
                    return new TargetBall(0);
                case ProgressId.MechanicsDamage:
                    return new MechanicsDamage(0);
                case ProgressId.MechanicsGenerateCube:
                    return new MechanicsGenerateCube(0);
                case ProgressId.SystemLoopStart:
                    return new SystemLoopStart(0);
                case ProgressId.SystemLoopEnd:
                    return new SystemLoopEnd(0);
                case ProgressId.SystemWayStart:
                    return new SystemWayStart(0);
                case ProgressId.SystemWayEnd:
                    return new SystemWayEnd(0);
            }
            return null;
        }
    }

}
