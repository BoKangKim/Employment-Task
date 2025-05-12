using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class StageCreator : EditorWindow
    {
        private BlockInfo[,] blockInfoArr = null;

        private int cellSize = 32;
        private int gridWidth = 10;
        private int gridHeight = 10;
        private Vector2 scrollPosition;

        private bool isStart = false;

        [MenuItem("Tools/Stage Editor")]
        public static void OpenWindow()
        {
            GetWindow<StageCreator>("Stage Creator");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            gridWidth = isStart ? gridWidth : EditorGUILayout.IntField("Width", gridWidth);
            gridHeight = isStart ? gridHeight : EditorGUILayout.IntField("Height", gridHeight);

            if (!isStart && GUILayout.Button("ACCEPT"))
            {
                blockInfoArr = new BlockInfo[gridHeight, gridWidth];
                isStart = true;
            }

            if (isStart && GUILayout.Button("RESET"))
            {
                blockInfoArr = null;
                isStart = false;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            if (isStart)
            {
                DrawGrid();
            }
        }


        private void DrawGrid()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int y = 0; y < gridHeight; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < gridWidth; x++)
                {
                    Color originalColor = GUI.backgroundColor;

                    if (blockInfoArr != null && blockInfoArr[y, x] != null)
                    {
                        switch (blockInfoArr[y, x].colorType)
                        {
                            case ColorType.Blue:
                                GUI.backgroundColor = Color.blue;
                                break;
                            case ColorType.Beige:
                                GUI.backgroundColor = Color.cyan;
                                break;
                        }
                    }

                    if (GUILayout.Button("", GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                    {
                        int height = y;
                        int width = x;
                        BlockInfoWindow.OpenWindow((blockInfo) => blockInfoArr[height, width] = blockInfo);
                    }

                    GUI.backgroundColor = originalColor;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
