using UnityEngine;
using UnityEditor;

namespace Game.Editor
{
    public class TooltipWindow : EditorWindow
    {
        private string tip = string.Empty;
        private TipType type;

        public static void OpenWindow(string tip)
        {
            var window = GetWindow<TooltipWindow>();
            window.tip = tip;
        }

        private void OnGUI()
        {
            GUILayout.Label(tip);
            
        }
    }
}