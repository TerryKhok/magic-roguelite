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

    //スキルのノード補正値
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

    //スキルデータベース
    //1つの変数の保存形式（名前とティアに対応する値の配列）
    public struct SkillVariable
    {
        public string name;
        public int[] values;
    }

    public static class SkillDB
    {
        public static Dictionary<ProgressId, List<SkillVariable>> g_SkillVariableMap = new Dictionary<ProgressId, List<SkillVariable>>();

        //データのロード
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


    //スキルコンパイル
    public static class SkillCompile
    {
        public static List<SkillProgress> Compile(List<ProgressId> idList)
        {
            List<SkillProgress> list = new List<SkillProgress>(); //最終的に返すリスト
            List<SkillProgress> surplus = new List<SkillProgress>();  //ループ処理で使うリスト
            foreach (var id in idList)
            {       //progressId型のリストをSkillProgress型のリストへ
                surplus.Add(ConvertIdToISkillProgress(id));
            }

            do
            {
                foreach (var progress in surplus.ToArray()) //途中でsurplusを変更できるループ
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
        {   //ループ系処理を圧縮する処理
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

        public static SkillProgress ConvertIdToISkillProgress(ProgressId id) //string型のidを渡すとSkillProgressのインスタンスで返してくれる
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
