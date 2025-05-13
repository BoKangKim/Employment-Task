using System.Collections.Generic;
using System.IO;
using System.Linq;
using Project.Scripts.Data_Script;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor
{
    // 에디터에서 사용하는 벽 정보 클래스
    public class WallInfoBlock
    {
        public List<WallInfo> wallInfoList;
        public ColorType colorType;

        public List<WallData> ConvertToWallDataList(int gridWidth, int gridHeight)
        {
            List<WallData> walldataList = new List<WallData>();
            for (int i = 0; i < wallInfoList.Count; i++)
            {
                WallData wallData = new WallData();
                Vector2Int convertedVec2 = ConvertCoordinate(wallInfoList[i].x, wallInfoList[i].y, gridWidth, gridHeight);
                wallData.x = convertedVec2.x;
                wallData.y = convertedVec2.y;
                wallData.WallDirection = wallInfoList[i].wallDirection;
                wallData.wallColor = wallInfoList[i].wallColor;
                wallData.length = wallInfoList[i].length;
                wallData.wallGimmickType = wallInfoList[i].wallGimmickType;

                walldataList.Add(wallData);
            }

            return walldataList;
        }

        private Vector2Int ConvertCoordinate(int x, int y, int gridWidth, int gridHeight)
        {
            return new Vector2Int(x, gridHeight - 1 - y);
        }
    }

    public class StageEditor : EditorWindow
    {
        // ScriptableObject를 저장하는 경로
        private readonly string savePath = "Assets/Project/Resource/Data/StageData SO/";

        // 블록배치 2차원 배열
        private BlockInfo[,] blockInfoArr = null;
        // 벽 배치 2차원 배열
        private WallInfoBlock[,] wallInfoArr = null;

        // GridCell의 사이즈
        private int cellSize = 32;

        // 전체 판의 너비
        private int gridWidth = 5;
        // 전체 판의 높이
        private int gridHeight = 5;

        // 판의 크기를 설정하고 Accept 버튼을 눌렀는지
        private bool isStart = false;
        private int stageIndex = 0;

        [MenuItem("Tools/Stage Editor")]
        public static void OpenWindow()
        {
            GetWindow<StageEditor>("Stage Editor");

            // 실수 방지용으로 InGame과 환경이 똑같은 Test 씬으로 이동하여 데이터 만들기
            string targetScenePath = "Assets/Project/Scenes/StageTestScene.unity";

            if (SceneManager.GetActiveScene().path == targetScenePath)
            {
                return;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Single);
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            gridWidth = isStart ? gridWidth : EditorGUILayout.IntField("Width (벽 포함 X)", gridWidth);
            gridHeight = isStart ? gridHeight : EditorGUILayout.IntField("Height (벽 포함 X)", gridHeight);
            stageIndex = isStart ? stageIndex : EditorGUILayout.IntField("Stage Index", stageIndex);

            // 확인 버튼 전체 판의 크기 확정
            if (!isStart && GUILayout.Button("ACCEPT"))
            {
                blockInfoArr = new BlockInfo[gridHeight, gridWidth];
                wallInfoArr = new WallInfoBlock[gridHeight, gridWidth];
                isStart = true;
            }

            // 판의 크기 재설정
            if (isStart && GUILayout.Button("RESET"))
            {
                blockInfoArr = null;
                isStart = false;
            }

            // Stage 데이터로 변환
            if (isStart && GUILayout.Button("CONVERT TO STAGE DATA"))
            {
                ConvertToStageData();
                Close();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical();
            if (isStart)
            {
                // 그리드 그리기
                GUILayout.Label("블록 설정");
                DrawBlockGrid();
                GUILayout.Space(30);

                GUILayout.Label("벽 설정");
                DrawWallGrid();
            }

            EditorGUILayout.EndVertical();
        }

        // 판의 크기대로 블록 그리드 그리기
        private void DrawBlockGrid()
        {
            for (int y = 0; y < gridHeight; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < gridWidth; x++)
                {
                    Color originalColor = GUI.backgroundColor;

                    if (blockInfoArr != null && blockInfoArr[y, x] != null)
                    {
                        Color color = GetColor(blockInfoArr[y, x].colorType);
                        GUI.backgroundColor = color;
                    }

                    if (GUILayout.Button("", GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                    {
                        int centerY = y;
                        int centerX = x;

                        if (blockInfoArr[y, x] == null)
                        {
                            // 블록 설정하는 Window 열기
                            BlockInfoWindow.OpenWindow(centerX, centerY, gridWidth, gridHeight, (blockInfo) =>
                            {
                                // 블록 세팅 완료 시 콜백 함수
                                blockInfoArr[centerY, centerX] = blockInfo;

                                foreach (var shape in blockInfo.shapes)
                                {
                                    int targetX = shape.offset.x + centerX;
                                    int targetY = shape.offset.y + centerY;
                                    if (targetX < 0 || targetX >= blockInfoArr.GetLength(1)
                                    || targetY < 0 || targetY >= blockInfoArr.GetLength(0))
                                    {
                                        continue;
                                    }

                                    if (blockInfoArr[targetY, targetX] != null)
                                    {
                                        continue;
                                    }

                                    blockInfoArr[targetY, targetX] = new BlockInfo();
                                    blockInfoArr[targetY, targetX].isCenter = false;
                                    blockInfoArr[targetY, targetX].colorType = blockInfo.colorType;
                                }
                            });
                        }
                    }

                    GUI.backgroundColor = originalColor;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        // 벽 그리드 그리기
        private void DrawWallGrid()
        {
            for (int y = 0; y < wallInfoArr.GetLength(0); y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < wallInfoArr.GetLength(1); x++)
                {
                    Color originalColor = GUI.backgroundColor;

                    if (wallInfoArr != null && wallInfoArr[y, x] != null)
                    {
                        Color color = GetColor(wallInfoArr[y, x].colorType);
                        GUI.backgroundColor = color;
                    }

                    if (x == 0 || x == wallInfoArr.GetLength(1) - 1 || y == 0 || y == wallInfoArr.GetLength(0) - 1)
                    {
                        // 벽 정보 설정 Window 열기
                        if (GUILayout.Button("W", GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                        {
                            int centerX = x;
                            int centerY = y;

                            WallInfoWindow.OpenWindow(centerX, centerY, (value) =>
                            {
                                // 벽 정보 설정 완료 시 콜백
                                if (value == null || value.Count == 0)
                                {
                                    return;
                                }

                                if (wallInfoArr[centerY, centerX] == null)
                                {
                                    wallInfoArr[centerY, centerX] = new WallInfoBlock();
                                    wallInfoArr[centerY, centerX].wallInfoList = value;
                                    wallInfoArr[centerY, centerX].colorType = value[0].wallColor;
                                }
                                else
                                {
                                    if (wallInfoArr[centerY, centerX].wallInfoList == null)
                                    {
                                        wallInfoArr[centerY, centerX].wallInfoList = new List<WallInfo>();
                                    }

                                    wallInfoArr[centerY, centerX].wallInfoList.AddRange(value);
                                }

                                for (int i = 0; i < value.Count; i++)
                                {
                                    int x = centerX;
                                    int y = centerY;
                                    for (int j = 0; j < value[i].length - 1; j++)
                                    {
                                        switch (value[i].wallDirection)
                                        {
                                            case ObjectPropertiesEnum.WallDirection.Single_Left:
                                            case ObjectPropertiesEnum.WallDirection.Open_Left:
                                                y--;
                                                break;
                                            case ObjectPropertiesEnum.WallDirection.Single_Right:
                                            case ObjectPropertiesEnum.WallDirection.Open_Right:
                                                y--;
                                                break;
                                            case ObjectPropertiesEnum.WallDirection.Single_Down:
                                            case ObjectPropertiesEnum.WallDirection.Open_Down:
                                                x++;
                                                break;
                                            case ObjectPropertiesEnum.WallDirection.Single_Up:
                                            case ObjectPropertiesEnum.WallDirection.Open_Up:
                                                x++;
                                                break;
                                        }

                                        if (x < 0 || x >= wallInfoArr.GetLength(1) || y < 0 || y >= wallInfoArr.GetLength(0))
                                        {
                                            continue;
                                        }

                                        wallInfoArr[y, x] = new WallInfoBlock();
                                        wallInfoArr[y, x].colorType = wallInfoArr[centerY, centerX].colorType;
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        GUILayout.Label("B", GUILayout.Width(cellSize), GUILayout.Height(cellSize));
                    }

                    GUI.backgroundColor = originalColor;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        // 현재 Window에서 가지고 있는 정보를 StageData로 변환하여 저장
        private void ConvertToStageData()
        {
            var wrapper = ParseToStageJsonData();

            string directoryPath = Path.GetDirectoryName(savePath);
            string fileName = $"StageData_{wrapper.Stage.stageIndex}.asset";
            string fullPath = Path.Combine(directoryPath, fileName);

            if (File.Exists(fullPath))
            {
                AssetDatabase.DeleteAsset(fullPath);
            }

            StageData stageData = CreateInstance<StageData>();
            stageData.stageIndex = wrapper.Stage.stageIndex;
            stageData.boardBlocks = wrapper.Stage.boardBlocks;
            stageData.playingBlocks = wrapper.Stage.playingBlocks;
            stageData.Walls = wrapper.Stage.Walls;

            // ScriptableObject 저장
            AssetDatabase.CreateAsset(stageData, fullPath);
            AssetDatabase.SaveAssets();

            BoardController boardController = FindAnyObjectByType<BoardController>();

            if(boardController != null)
            {
                boardController.AddStageData(stageData);
            }
            else
            {
                Debug.LogWarning("Not Found BoardController");
            }

            Debug.Log($"StageData ScriptableObject 생성 완료: {savePath}");
        }

        // 현재 정보를 StageJson으로 변환
        public StageJsonWrapper ParseToStageJsonData()
        {
            StageJsonData jsonData = new StageJsonData();
            jsonData.stageIndex = stageIndex;
            jsonData.boardBlocks = new List<BoardBlockData>();
            jsonData.playingBlocks = new List<PlayingBlockData>();
            jsonData.Walls = new List<Project.Scripts.Data_Script.WallData>();

            int uniqueIndexCount = 0;

            for (int y = 0; y < blockInfoArr.GetLength(0); y++)
            {
                for (int x = 0; x < blockInfoArr.GetLength(1); x++)
                {
                    if (blockInfoArr[y, x] != null && blockInfoArr[y, x].isWall)
                    {
                        continue;
                    }

                    BoardBlockData blockData = new BoardBlockData();
                    blockData.colorType = new List<ColorType>();
                    blockData.dataType = new List<int>();
                    blockData.x = x;
                    blockData.y = y;
                    jsonData.boardBlocks.Add(blockData);

                    if (blockInfoArr[y, x] != null && blockInfoArr[y, x].isCenter)
                    {
                        PlayingBlockData playingBlockData = new PlayingBlockData();
                        Vector2Int converted = new Vector2Int(blockInfoArr[y, x].center.x, gridHeight - 1 - blockInfoArr[y, x].center.y);
                        blockInfoArr[y, x].ConverOffset();
                        playingBlockData.center = converted;
                        playingBlockData.uniqueIndex = uniqueIndexCount;
                        playingBlockData.colorType = blockInfoArr[y, x].colorType;
                        playingBlockData.shapes = blockInfoArr[y, x].shapes.ToList();
                        playingBlockData.gimmicks = blockInfoArr[y, x].gimmicks;

                        jsonData.playingBlocks.Add(playingBlockData);
                        uniqueIndexCount++;
                    }
                }
            }

            for (int y = 0; y < wallInfoArr.GetLength(0); y++)
            {
                for (int x = 0; x < wallInfoArr.GetLength(1); x++)
                {
                    if (wallInfoArr[y, x] != null && wallInfoArr[y, x].wallInfoList != null && wallInfoArr[y, x].wallInfoList.Count > 0)
                    {
                        jsonData.Walls.AddRange(wallInfoArr[y, x].ConvertToWallDataList(gridWidth, gridHeight));
                    }
                }
            }

            StageJsonWrapper wrapper = new StageJsonWrapper();
            wrapper.Stage = jsonData;

            return wrapper;
        }

        private Color GetColor(ColorType type)
        {
            switch (type)
            {
                case ColorType.Blue:
                    return Color.blue;
                case ColorType.Beige:
                    return new Color(249f / 255f, 228f / 255f, 183f / 255f, 1f);
                case ColorType.Gray:
                    return Color.gray;
                case ColorType.Green:
                    return Color.green;
                case ColorType.Orange:
                    return new Color(1f, 127f / 255f, 0f, 1f);
                case ColorType.Purple:
                    return new Color(137f / 255f, 119f / 255f, 173f / 255f, 1f);
                case ColorType.Red:
                    return Color.red;
                case ColorType.Yellow:
                    return Color.yellow;
            }

            return Color.gray;
        }
    }
}
