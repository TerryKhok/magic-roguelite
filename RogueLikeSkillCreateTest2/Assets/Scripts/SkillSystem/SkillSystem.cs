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

    //ループ処理が含まれるノードに実装するインターフェイス
    public abstract class SkillLoopStartProgress : SkillProgress
    {
        public SkillLoopStartProgress(int t) : base(t) { }
        public abstract void AddLoopProgressList(SkillProgress progress);   //ループ内で実行する処理を追加
        public abstract Type GetLoopEndProgressType();                       //ループ処理終了ノードの取得用（要設定）
    }

    //IDを列挙。使うのはSkillCompile.csのConvertIdToISkillProgress関数
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
            TextAsset resource = Resources.Load("SkillSystem/SkillSystem_Variables") as TextAsset;  //csvをロード
            StringReader reader = new StringReader(resource.text);  //1行ずつ読み込む
            while (reader.Peek() != -1) //まだ行が読めるなら
            {
                ProgressId id = default;    //マップのKey用
                List<SkillVariable> list = new List<SkillVariable>(); //マップのValue用
                SkillVariable v = new SkillVariable();
                foreach (var str in reader.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries))    //何もないやつをスルーして,ごとの区切りでループ
                    if (id == default)
                    {
                        try
                        {
                            id = (ProgressId)Enum.Parse(typeof(ProgressId), str);   //行のはじめをProgressIdに変換
                        }
                        catch
                        {
                            Debug.Log("存在しない処理ノードIDです / " + str);       //変換できないならログ出して次の行
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
                Debug.Log("DBに値が設定されていません");
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
                Debug.Log("DBに値が設定されていません");
                throw;
            }
            return result;
        }
    }
}
