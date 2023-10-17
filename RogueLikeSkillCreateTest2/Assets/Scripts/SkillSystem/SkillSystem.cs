using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
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
        int[] _args = new int[3];

        public int GetTier() { return _tier; }
        public void GetTier(int t) { _tier = t; }
        public int[] GetArgs() { return _args; }
        public int GetArgsValue(int i) { return _args[i]; }

        public SkillProgress(int t, int[] a)
        {
            _tier = t;
            _args = a;
        }
    }

    //�m�[�h�S�ĂɎ�������C���^�[�t�F�C�X�i�p�����Ă���C���^�[�t�F�C�X�Ŏ������Ă�����̂�����j
    public interface ISkillProgress
    {
        //���s���ɏ������I���܂Ŏ��̃m�[�h�ɑҋ@���Ă��炤����
        public UniTask<SkillElements> SkillProgress(SkillElements elem, CancellationToken token);

        //���s���ɏ�����ҋ@�����Ɏ��̃m�[�h�ɍs������
        public void SkillProgressNoWait(SkillElements elem, CancellationToken token);
    }

    //���[�v�������܂܂��m�[�h�Ɏ�������C���^�[�t�F�C�X
    public interface ISkillLoopStartProgress : ISkillProgress
    {
        public void AddLoopProgressList(ISkillProgress progress);   //���[�v���Ŏ��s���鏈����ǉ�
        public Type GetLoopEndProgressType();                       //���[�v�����I���m�[�h�̎擾�p�i�v�ݒ�j
    }

    //ID��񋓁B�g���̂�SkillCompile.cs��ConvertIdToISkillProgress�֐�
    public enum progressId
    {
        TargetBall,
        MechanicsDamage,
        MechanicsGenerateCube,
        SystemLoopStart,
        SystemLoopEnd,
        SystemWayStart,
        SystemWayEnd
    }
}
