using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

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

        public int getTier() { return _tier; }
        public void setTier(int t) { _tier = t; }

        public SkillProgress(int t) { _tier = t; }
    }


    public interface ISkillProgress
    {
        public UniTask<SkillElements> SkillProgress(SkillElements elem, CancellationToken token);

        public void SkillProgressNoWait(SkillElements elem, CancellationToken token);
    }

    public interface ISkillLoopStartProgress : ISkillProgress
    {
        public void AddLoopProgressList(ISkillProgress progress);
        public Type GetLoopEndProgressType();
    }

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
