using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class BlockInfo
    {
        public bool isWall;
        public bool isCenter;
        public Vector2Int center;
        public int uniqueIndex;
        public ColorType colorType;
        public List<ShapeData> shapes;
        public List<GimmickData> gimmicks;

        public void ConverOffset()
        {
            foreach (var shape in shapes)
            {
                shape.offset.y = -shape.offset.y;
            }
        }
    }

    public class BlockInfoWindow : EditorWindow
    {
        private Action<BlockInfo> callback;
        private int cellSize = 32;

        private ColorType selectedColor;
        private string gimmickType = string.Empty;
        private List<GimmickData> gimmickDataList;

        public int centerX = 2;
        public int centerY = 2;

        public int selectedX = 0;
        public int selectedY = 0;

        private bool isWall;

        private bool[,] blockDotArr = null;


        public static void OpenWindow(int x, int y, int width, int height, Action<BlockInfo> action)
        {
            var window = GetWindow<BlockInfoWindow>("Block Info");

            window.selectedX = x;
            window.selectedY = y;

            window.callback = action;

            window.blockDotArr = new bool[height, width];
            window.blockDotArr[y, x] = true;

            window.selectedColor = ColorType.Red;
            window.gimmickDataList = new List<GimmickData>();
        }

        private void OnDisable()
        {
            GimmickContainer.ClearSingleton();
        }

        // 정보 설정 GUI
        private void OnGUI()
        {
            selectedColor = (ColorType)EditorGUILayout.EnumPopup("Color", selectedColor);
            gimmickType = EditorGUILayout.TextField("Gimmick Type", gimmickType);

            if (GUILayout.Button("Gimmick Add"))
            {
                var gimmickData = GimmickContainer.Instance.GetGimmickData(gimmickType);
                if(gimmickData != null)
                {
                    gimmickDataList.Add(gimmickData);
                }
            }

            GUILayout.Space(10);

            GUILayout.Label("떨어진 Cell을 선택해도 하나의 그룹으로 간주합니다.");
            if (GUILayout.Button("ACCEPT"))
            {
                if (selectedColor == ColorType.None)
                {
                    Close();
                    return;
                }

                BlockInfo blockInfo = new BlockInfo();
                blockInfo.isCenter = true;
                blockInfo.center = new Vector2Int(selectedX, selectedY);
                blockInfo.colorType = selectedColor;
                blockInfo.shapes = new List<ShapeData>();

                for (int y = 0; y < blockDotArr.GetLength(0); y++)
                {
                    for (int x = 0; x < blockDotArr.GetLength(1); x++)
                    {
                        if (blockDotArr[y, x])
                        {
                            ShapeData data = new ShapeData();
                            data.offset = new Vector2Int(x - selectedX, y - selectedY);
                            blockInfo.shapes.Add(data);
                        }
                    }
                }

                callback?.Invoke(blockInfo);
                Close();
            }

            if (!isWall)
            {
                DrawGrid();
            }
        }

        // 그리드 그리기
        private void DrawGrid()
        {
            for (int y = 0; y < blockDotArr.GetLength(0); y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < blockDotArr.GetLength(1); x++)
                {
                    Color originalColor = GUI.backgroundColor;
                    string dot = blockDotArr[y, x] ? "O" : "X";

                    if (GUILayout.Button(dot, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                    {
                        if (x == selectedX && y == selectedY)
                        {
                            continue;
                        }

                        blockDotArr[y, x] = !blockDotArr[y, x];
                    }

                    GUI.backgroundColor = originalColor;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}