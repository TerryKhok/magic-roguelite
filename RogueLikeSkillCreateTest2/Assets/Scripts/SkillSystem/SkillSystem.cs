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
        public LocationData(Transform tf)
        {
            pos = tf.position;
            rotate = tf.rotation;
        }
        public Vector3 GetPos() { return pos; }
        public Quaternion GetRotate() { return rotate; }
    }


    public struct SkillElements
    {
        LocationData ld;
        List<GameObject> targets;
        public SkillElements(Transform t)
        {
            ld = new LocationData(t);
            targets = new List<GameObject>();
        }
        public LocationData GetLocationData() { return ld; }
        public List<GameObject> GetTargets() { return targets; }

        public void AddTargets(GameObject obj) { targets.Add(obj); }
        public void SetLocationData(Transform tf) { ld.ResetData(tf); }
        public void SetLocationData(Vector3 pos, Quaternion rotate) { ld.ResetData(pos, rotate); }
    }

    public class SkillProgress
    {
        int _tier;
        //int[] _args;

        public int GetTier() { return _tier; }
        public void GetTier(int t) { _tier = t; }
        //public int[] GetArgs() { return _args; }
        //public int GetArgsValue(int i) { return _args[i]; }

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

    public struct SkillVariable
    {
        public string name;
        public int[] values;
    }


    public static class SkillDB
    {
        public static Dictionary<ProgressId, List<SkillVariable>> g_SkillVariableMap = new Dictionary<ProgressId, List<SkillVariable>>();

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
}
