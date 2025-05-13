using System.Collections.Generic;

[System.Serializable]
public class GimmickData
{
    public string gimmickType;

    public virtual void Gimmick()
    {

    }
}

// 예시 기믹
[System.Serializable]
public class ExampleGimmick : GimmickData
{
    public ExampleGimmick()
    {
        this.gimmickType = nameof(ExampleGimmick);
    }

    public override void Gimmick()
    {
        UnityEngine.Debug.Log($"Active Gimmick!");
    }
}