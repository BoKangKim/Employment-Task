using UnityEngine;

namespace Game.Factory
{
    public class WallFactory : MonoBehaviour, IFactory
    {
        [SerializeField] private WallObject[] wallPrefabs;
        [SerializeField] private Material[] wallMaterials;

        public int WallPrefabCount => wallPrefabs.Length;

        public WallObject CreateWall(int index, int wallColor, GameObject parent)
        {
            WallObject wallObj = Instantiate(wallPrefabs[index], parent.transform);
            wallObj.SetWall(wallMaterials[wallColor], wallColor != (int)ColorType.None);

            return wallObj;
        }

        public Material GetWallMatrial(int index)
        {
            if(index < 0 || index >= wallMaterials.Length)
            {
                return null;
            }

            return wallMaterials[index];
        }
    }
}
