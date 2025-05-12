using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public partial class BoardController : MonoBehaviour
{
    public static BoardController Instance;
    [SerializeField] private StageData[] stageDatas;
    [SerializeField] private BoardInitializer initializer;

    [SerializeField] private Material[] blockMaterials;
    [SerializeField] private Transform spawnerTr;
    [SerializeField] private Transform quadTr;

    public float BoardHeight => initializer.boardHeight;
    public float BoardWidth => initializer.boardWidth;

    private readonly float blockDistance = 0.79f;

    private int nowStageIndex = 0;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Init();
    }

    private async void Init(int stageIdx = 0)
    {
        if (stageDatas == null)
        {
            Debug.LogError("StageData가 할당되지 않았습니다!");
            return;
        }

        await initializer.BoardInit(stageDatas[stageIdx], this);

        CreateMaskingTemp();
    }

    public void GoToPreviousLevel()
    {
        if (nowStageIndex == 0) return;

        Destroy(initializer.BoardParent);
        Destroy(initializer.PlayingBlockParent);
        Init(--nowStageIndex);
        
        StartCoroutine(Wait());
    }

    public void GotoNextLevel()
    {
        if (nowStageIndex == stageDatas.Length - 1) return;
        
        Destroy(initializer.BoardParent);
        Destroy(initializer.PlayingBlockParent);
        Init(++nowStageIndex);
        
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return null;
        
        Vector3 camTr = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(1.5f + 0.5f * (initializer.boardWidth - 4),camTr.y,camTr.z);
    } 
    
}