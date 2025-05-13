using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public enum TipType
    {
        WallCount,
        WallDirection,
        Length,
        Add,
        Accept,
    }

    public class WallInfo
    {
        public int x;
        public int y;
        public ObjectPropertiesEnum.WallDirection wallDirection;
        public int length;
        public ColorType wallColor;
        public WallGimmickType wallGimmickType;
    }

    public class WallInfoWindow : EditorWindow
    {
        private Action<List<WallInfo>> callback;
        private int cellSize = 32;

        private ObjectPropertiesEnum.WallDirection wallDirection;
        private ColorType colorType;
        private int length;
        private WallGimmickType gimmickType;

        private List<WallInfo> wallInfoList;
        private int centerX;
        private int centerY;

        public static void OpenWindow(int x, int y, Action<List<WallInfo>> callback)
        {
            var window = GetWindow<WallInfoWindow>("Wall Info");
            window.length = 1;
            window.wallInfoList = new List<WallInfo>();
            window.callback = callback;

            window.centerX = x;
            window.centerY = y;
        }

        // 벽 정보 설정 GUI
        private void OnGUI()
        {
            GUILayout.BeginVertical();
        
            GUIContent tooltipIcon = EditorGUIUtility.IconContent("_Help");

            #region  Set Direction
            GUILayout.BeginHorizontal();
            wallDirection = (ObjectPropertiesEnum.WallDirection)EditorGUILayout.EnumPopup("Direction", wallDirection);
            if (GUILayout.Button(tooltipIcon, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
            {
                TooltipWindow.OpenWindow(GetTip(TipType.WallDirection));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            #endregion

            #region  Set Color
            GUILayout.BeginHorizontal();
            colorType = (ColorType)EditorGUILayout.EnumPopup("Color", colorType);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            #endregion

            #region  Set Wall Length
            GUILayout.BeginHorizontal();
            length = EditorGUILayout.IntField("Length", length);
            if (GUILayout.Button(tooltipIcon, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
            {
                TooltipWindow.OpenWindow(GetTip(TipType.Length));
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            #endregion

            #region  Set GimmickType
            GUILayout.BeginHorizontal();
            gimmickType = (WallGimmickType)EditorGUILayout.EnumPopup("GimmickType", gimmickType);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            #endregion

            #region  ADD Button -> wallInfoList Add
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("추가"))
            {
                WallInfo info = new WallInfo();
                info.x = centerX;
                info.y = centerY;
                info.wallDirection = wallDirection;
                info.wallColor = colorType;
                info.length = length;
                info.wallGimmickType = gimmickType;

                wallInfoList.Add(info);
            }

            if (GUILayout.Button(tooltipIcon, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
            {
                TooltipWindow.OpenWindow(GetTip(TipType.Add));
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            #endregion

            #region  ACCEPT Button
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("ACCEPT"))
            {
                if (wallInfoList.Count > 0)
                {
                    callback?.Invoke(wallInfoList.ToList());
                }

                Close();
            }

            if (GUILayout.Button(tooltipIcon, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
            {
                TooltipWindow.OpenWindow(GetTip(TipType.Accept));
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(30);
            #endregion

            // 추가된 Wall 정보 보여주기
            DrwaSetInfoLabels();

            GUILayout.EndVertical();
        }

        // Tooltip 데이터
        private string GetTip(TipType type)
        {
            string tip = string.Empty;

            switch (type)
            {
                case TipType.WallCount:
                    tip = "벽의 길이가 아닌 갯수 설정입니다. \n 예를 들어 왼쪽 상단 모서리 같은 경우 왼쪽 벽, 위쪽 벽 \n이렇게 두 개를 설정할 수 있습니다.";
                    break;
                case TipType.WallDirection:
                    tip = "벽이 바라 보는 방향입니다.\n" +
                    "Single_Up = 위쪽 방향" +
                    "\nSingle_Down = 아래 방향" +
                    "\nSingle_Left = 왼쪽 방향" +
                    "\nSingle_Right = 오른쪽 방향" +
                    "\nLeft_Up = 왼쪽위 모서리" +
                    "\nLeft_Down = 왼쪽아래 모서리" +
                    "\nRight_Up = 오른쪽 위 모서리" +
                    "\nRight_Down = 오른쪽 아래 모서리" +
                    "\nOpen_Up = 열려있는 위" +
                    "\nOpen_Down = 열려있는 아래" +
                    "\nOpen_Left = 열려있는 왼쪽" +
                    "\nOpen_Right = 열려있는 오른쪽";
                    break;
                case TipType.Length:
                    tip = "설정한 방향 기준으로 몇 칸 길이로 할 지 설정입니다. \n" +
                    "벽이 바라보는 방향이 위, 왼쪽 이면 오른쪽으로 늘어납니다. \n" +
                    "벽이 바라보는 방향이 아래, 오른쪽 이면 왼쪽으로 늘어납니다." +
                    "";
                    break;
                case TipType.Add:
                    tip = "벽 설정을 추가하는 버튼 입니다. \n더 추가하고 싶을 때는 설정을 바꾼 후 다시 Add를 누르면 됩니다.";
                    break;
                case TipType.Accept:
                    tip = "벽 설정을 확정짓습니다. 이 버튼을 누르지 않으면 적용되지 않습니다.";
                    break;
            }

            return tip;
        }

        private void DrwaSetInfoLabels()
        {
            if (wallInfoList.Count > 0)
            {
                GUILayout.BeginVertical();
                for (int i = 0; i < wallInfoList.Count; i++)
                {
                    GUILayout.Label($"DIRECTION : {wallInfoList[i].wallDirection}");
                    GUILayout.Label($"LENGTH : {wallInfoList[i].length}");
                    GUILayout.Label($"WALL COLOR : {wallInfoList[i].wallColor}");
                    GUILayout.Label($"WALL GIMMICKTYPE : {wallInfoList[i].wallGimmickType}");
                    GUILayout.Space(30);
                }
                GUILayout.EndVertical();
            }
        }
    }
}

