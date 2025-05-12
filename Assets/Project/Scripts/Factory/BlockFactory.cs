using UnityEngine;

namespace Game.Factory
{
    public class BlockFactory : MonoBehaviour, IFactory
    {
        [Header("Board Block")]
        [SerializeField] private BoardBlockObject boardBlockPrefab;
        
        [Header("Block")]
        [SerializeField] private BlockObject blockPrefab;
        [SerializeField] private Material[] testBlockMaterials;

        [Header("Block Group")]
        [SerializeField] private BlockDragHandler blockGroupPrefab;

        public BoardBlockObject CreateBoardBlock(Vector3 createPos, GameObject parent)
        {
            BoardBlockObject blockObj = Instantiate(boardBlockPrefab, parent.transform);

            return blockObj;
        }

        public BlockObject CreateBlock(int materialIndex, GameObject parent)
        {
            BlockObject block = Instantiate(blockPrefab, parent.transform);

            if(materialIndex >= 0)
            {
                block.UpdateView(testBlockMaterials[materialIndex]);
            }

            return block;
        }

        public BlockDragHandler CreateBlockGroup(GameObject parent)
        {
            BlockDragHandler blockGroup = Instantiate(blockGroupPrefab, parent.transform);

            return blockGroup;
        }
    }
}
