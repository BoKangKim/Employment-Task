using System.Collections.Generic;
using UnityEngine;

public class GimmickContainer
{
    private static GimmickContainer instance = null;
    public static GimmickContainer Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GimmickContainer();
                instance.Generate();
            }

            return instance;
        }
    }

    private GimmickContainer()
    {

    }
    
    private Dictionary<string, GimmickData> gimmicDict = new Dictionary<string, GimmickData>();

    // 구현된 기믹 넣기
    public void Generate()
    {
        // Example
        ExampleGimmick exampleGimmick = new ExampleGimmick();

        gimmicDict.TryAdd(exampleGimmick.gimmickType, exampleGimmick);
    }

    public GimmickData GetGimmickData(string key)
    {
        GimmickData data = null;

        if(!gimmicDict.TryGetValue(key, out data))
        {
            Debug.LogWarning($"Not Found {key}");
        }

        return data;
    }

    public static void ClearSingleton()
    {
        instance = null;
    }
}
