using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class FileManager
{
    public static void SaveText(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }

    public static void AppendText(string filePath, string content)
    {
        //File.AppendAllText(filePath, content);
        List<string> list = new List<string>();
        list.Add(content);
        File.AppendAllLines(filePath, list);
    }

    public static string LoadText(string filePath)
    {
        if (!File.Exists(filePath))
        {
            // 파일이 존재하지 않음
            return string.Empty;
        }
        return File.ReadAllText(filePath);

    }

    public static string[] LoadAllLines(string filePath)
    {
        if (!File.Exists(filePath))
        {
            // 파일이 존재하지 않음
            return null;
        }
        return File.ReadAllLines(filePath);
    }

    static BinaryFormatter bf = null;
    public static void SaveToBinary<T>(string filePath, T data)
    {
        //FileStream fs = File.Create(filePath);
        //BinaryFormatter bf = new BinaryFormatter();
        //bf.Serialize(fs, data);
        //fs.Close();

        using (FileStream fs = File.Create(filePath))
        {
            if (bf == null) bf = new BinaryFormatter();
            bf.Serialize(fs, data);
        }
    }

    public static T LoadFromBinary<T>(string filePath)
    {
        if (!File.Exists(filePath)) return default;
        using (FileStream fs = File.Open(filePath, FileMode.Open))
        {
            if (bf == null) bf = new BinaryFormatter();
            fs.Position = 0;
            return (T)bf.Deserialize(fs);
        }
    }
}
