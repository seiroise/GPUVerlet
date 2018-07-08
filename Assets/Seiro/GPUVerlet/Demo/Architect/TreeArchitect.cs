using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// 二分木の建築士
    /// </summary>
	// [CreateAssetMenu(fileName = "TreeArchitect", menuName = "Seiro/GPUVerlet/TreeArchitect")]
    public class TreeArchitect : BaseArchitect
    {

        public static float angleOffsetMin = 0.1f, angleOffsetMax = 0.5f;
        public static float lengthMin1 = 0.6f, lengthMax1 = 0.9f;
        public static float lengthMin2 = 0.5f, lengthMax2 = 0.7f;
        public static float lengthBranchAThreshold = 1f, lengthBranchBThreshold = 1f;

        /// <summary>
        /// 枝の最大深度
        /// </summary>	
        [SerializeField, Range(2, 10)]
        int _maxDepth = 4;

        /// <summary>
        /// 一番最初の枝の長さ
        /// </summary>
        [SerializeField, Range(1f, 10f)]
        float _initialLength = 8f;

        /// <summary>
        /// 枝の太さ
        /// </summary>
        [SerializeField, Range(0.01f, 5f)]
        float _branchWidth = 0.1f;

        /// <summary>
        /// 葉っぱ部分の色
        /// </summary>
        [SerializeField]
        Gradient _leafColor;

        /// <summary>
        /// パーティクルの最小サイズと最大サイズ
        /// </summary>
        [SerializeField, Range(0.01f, 10f)]
        float _minSize = 0.2f, _maxSize = 2f;

        /// <summary>
        /// 枝の色
        /// </summary>
        [SerializeField]
        Color _branchColor;

        #region 外部インタフェース

        public override VStructure CreateStructure(Matrix4x4 toWorld)
        {
            var structure = new VStructure();

            var position = toWorld.MultiplyPoint3x4(Vector2.zero);
            var angle = 90f * Mathf.Deg2Rad;

            // 最初の枝
            var origin = structure.AddParticle(position, 0.5f);
            var next = MakeBranch(structure, _maxDepth, origin, position, angle, _initialLength);

            // ピン止
            structure.AddAfterStep(new VPin(origin, structure.GetParticlePosition(origin)));
            structure.AddAfterStep(new VPin(next, structure.GetParticlePosition(next)));

            return structure;
        }

        #endregion

        #region 内部処理

        /// <summary>
        /// 枝を一つ作成する
        /// </summary>
        /// <returns></returns>
        int MakeBranch(
            VStructure structure,
            int depth,
            int parentID,
            Vector2 parentPosition,
            float angle,
            float length)
        {
            // パーティクルの大きさは幹と葉で分ける
            var size = 0f;
            Color32 color;
            if (depth == 1)
            {
                // 葉
                size = Random.Range(_minSize, _maxSize);
                color = _leafColor.Evaluate(Random.Range(0f, 1f));
            }
            else
            {
                // 幹
                size = 0f;
                color = _branchColor;
            }

            var position = parentPosition + Utilities.Angle2Vector(angle) * length;
            var currentID = structure.AddParticle(position, size, color);

            structure.AddEdge(parentID, currentID, _branchWidth, _branchColor);

            depth--;
            // 最大深度未満の場合は次の枝へ
            if (depth > 0)
            {
                // child.hide = true;

                // branch A
                int aID;
                if ((length * Random.Range(1f, 2f)) / _initialLength > lengthBranchAThreshold)
                {
                    aID = MakeBranch(structure, depth, currentID, position, angle + Random.Range(angleOffsetMin, angleOffsetMax), length * Random.Range(lengthMin1, lengthMax1));
                }
                else
                {
                    aID = MakeBranch(structure, depth, currentID, position, angle + Random.Range(angleOffsetMin, angleOffsetMax), length * Random.Range(lengthMin2, lengthMax2));
                }

                // branch B
                int bID;
                if ((length * Random.Range(1f, 2f)) / _initialLength > lengthBranchBThreshold)
                {
                    bID = MakeBranch(structure, depth, currentID, position, angle - Random.Range(angleOffsetMin, angleOffsetMax), length * Random.Range(lengthMin1, lengthMax1));
                }
                else
                {
                    bID = MakeBranch(structure, depth, currentID, position, angle - Random.Range(angleOffsetMin, angleOffsetMax), length * Random.Range(lengthMin2, lengthMax2));
                }

                structure.AddEdge(currentID, aID, _branchWidth);
                structure.AddEdge(currentID, bID, _branchWidth);

                structure.AddEdge(parentID, aID, 0.025f, _branchColor);
                structure.AddEdge(parentID, bID, 0.025f, _branchColor);
                structure.AddEdge(aID, bID, 0.025f, _branchColor);
            }

            return currentID;
        }

        #endregion
    }
}