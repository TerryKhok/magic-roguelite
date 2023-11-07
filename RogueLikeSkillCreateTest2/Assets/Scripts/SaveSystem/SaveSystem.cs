using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem 
{
    public static void SaveGame(SaveData data)
    {
        //�o�C�i���[�t�H�[�}�b�^�[�̍쐬
        BinaryFormatter formatter = new BinaryFormatter();
        //�Z�[�u��̃p�X���쐬
        string path = Application.persistentDataPath + "/SaveData.hsr";
        //�t�@�C���X�g���[���̍쐬
        FileStream stream = new FileStream(path, FileMode.Create);

        //Serialize�Ńt�@�C���X�g���[���Ƀf�[�^����������
        formatter.Serialize(stream, data);
        //�t�@�C���X�g���[�������
        stream.Close();

        //Debug.Log("save");

    }

    public static SaveData LoadGame()
    {
        //�Z�[�u�f�[�^�̃p�X��ǂݍ���
        string path = Application.persistentDataPath + "/SaveData.hsr";
        Debug.Log(path);

        //�p�X�����݂��邩
        if (File.Exists(path))
        {
            //�o�C�i���[�t�H�[�}�b�^�[�̍쐬
            BinaryFormatter formatter = new BinaryFormatter();
            //�t�@�C���X�g���[���̍쐬
            FileStream stream = new FileStream(path, FileMode.Open);

            //�t�@�C�����̃f�[�^��Deserialize���ĕ���
            SaveData data = formatter.Deserialize(stream) as SaveData;
            //�t�@�C���X�g���[�������
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
