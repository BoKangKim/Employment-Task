using System.Collections.Generic;
using UnityEngine;

public class GimmickContainer
{
    #region Singleton
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
    #endregion
    
    // 기믹 Dictionary 컨테이너 생성될 때 Generate
    private Dictionary<string, GimmickData> gimmicDict = new Dictionary<string, GimmickData>();

    // 구현된 기믹 넣기
    public void Generate()
    {
        // Example
        ExampleGimmick exampleGimmick = new ExampleGimmick();

        gimmicDict.TryAdd(exampleGimmick.gimmickType, exampleGimmick);
    }

    // key 값으로 GimmickData 가져오기
    public GimmickData GetGimmickData(string key)
    {
        GimmickData data = null;

        if(!gimmicDict.TryGetValue(key, out data))
        {
            Debug.LogWarning($"Not Found {key}");
        }

        return data;
    }
    
    // static 클래스 정리
    public static void ClearSingleton()
    {
        instance = null;
    }
}
