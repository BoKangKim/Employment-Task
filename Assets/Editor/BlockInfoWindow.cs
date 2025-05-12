using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class BlockInfo
    {
        public ColorType colorType;
    }

    public class BlockInfoWindow : EditorWindow
    {
        private static Action<BlockInfo> callback;
        private int cellSize = 32;

        private ColorType selectedColor;

        private static int centerX = 2;
        private static int centerY = 2;

        private static bool[,] blockDotArr = new bool[5, 5];

        public static void OpenWindow(Action<BlockInfo> action)
        {
            GetWindow<BlockInfoWindow>("Block Info");
            callback = action;
            blockDotArr[centerY, centerX] = true;
        }

        private void OnGUI()
        {
            selectedColor = (ColorType)EditorGUILayout.EnumPopup("색상", selectedColor);

            GUILayout.Space(10);

            DrawGrid();
        }

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
                        if(y == centerY && x == centerX)
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