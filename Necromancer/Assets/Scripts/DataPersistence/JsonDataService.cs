using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonDataService : IDataPersistence
{
    
    public bool SaveData<T>(string RelativePath, T Data, bool Encrypted)
    {
        string path = Application.persistentDataPath + RelativePath;

        try
        {
            if (File.Exists(path))
            {
                Debug.Log("File exist");
                File.Delete(path);
            }
            else
            {
                Debug.Log("create file");
            }
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }


    public T LoadData<T>(string RelativePath, bool Encrypted)
    {
        string path = Application.persistentDataPath + RelativePath;
        Debug.Log(path);
        if (!File.Exists(path))
        {
            Debug.Log("File not exist");
            throw new FileNotFoundException("no file!");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw e;
        }

    }


   
}
