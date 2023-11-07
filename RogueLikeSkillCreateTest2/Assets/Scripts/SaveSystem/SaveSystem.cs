using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem 
{
    public static void SaveGame(SaveData data)
    {
        //バイナリーフォーマッターの作成
        BinaryFormatter formatter = new BinaryFormatter();
        //セーブ先のパスを作成
        string path = Application.persistentDataPath + "/SaveData.hsr";
        //ファイルストリームの作成
        FileStream stream = new FileStream(path, FileMode.Create);

        //Serializeでファイルストリームにデータを書き込む
        formatter.Serialize(stream, data);
        //ファイルストリームを閉じる
        stream.Close();

        //Debug.Log("save");

    }

    public static SaveData LoadGame()
    {
        //セーブデータのパスを読み込む
        string path = Application.persistentDataPath + "/SaveData.hsr";
        Debug.Log(path);

        //パスが存在するか
        if (File.Exists(path))
        {
            //バイナリーフォーマッターの作成
            BinaryFormatter formatter = new BinaryFormatter();
            //ファイルストリームの作成
            FileStream stream = new FileStream(path, FileMode.Open);

            //ファイル内のデータをDeserializeして復元
            SaveData data = formatter.Deserialize(stream) as SaveData;
            //ファイルストリームを閉じる
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("notFilePath");
            return null;
        }
    }
}
