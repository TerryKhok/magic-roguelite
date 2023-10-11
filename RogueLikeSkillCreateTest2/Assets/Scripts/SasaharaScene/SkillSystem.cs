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

    //ノード全てに実装するインターフェイス（継承しているインターフェイスで実装しているものもある）
    public interface ISkillProgress
    {
        //実行時に処理が終わるまで次のノードに待機してもらう処理
        public UniTask<SkillElements> SkillProgress(SkillElements elem, CancellationToken token);

        //実行時に処理を待機せずに次のノードに行く処理
        public void SkillProgressNoWait(SkillElements elem, CancellationToken token);
    }

    //ループ処理が含まれるノードに実装するインターフェイス
    public interface ISkillLoopStartProgress : ISkillProgress
    {
        public void AddLoopProgressList(ISkillProgress progress);   //ループ内で実行する処理を追加
        public Type GetLoopEndProgressType();                       //ループ処理終了ノードの取得用（要設定）
    }

    //IDを列挙。使うのはSkillCompile.csのConvertIdToISkillProgress関数
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
