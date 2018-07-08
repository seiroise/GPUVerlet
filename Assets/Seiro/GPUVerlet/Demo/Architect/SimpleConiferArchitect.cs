using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// シンプルな針葉樹っぽい何かの建築士
    /// </summary>
	// [CreateAssetMenu(fileName = "SimpleConiferArchitect", menuName = "Seiro/GPUVerlet/SimpleConiferArchitect")]
    public class SimpleConiferArchitect : BaseArchitect
    {

        [Header("枝の設定")]

        /// <summary>
        /// 最大深度
        /// </summary>
        [SerializeField, Range(1, 20)]
        int _maxDepth = 5;

        /// <summary>
        /// 基本的な長さ
        /// </summary>
        [SerializeField, Range(0.01f, 100f)]
        float _baseLength = 2f;

        /// <summary>
        /// 長さの減衰率
        /// </summary>
        [SerializeField, Range(0.01f, 1f)]
        float _lengthDamping = 0.8f;

        /// <summary>
        /// 枝の色
        /// </summary>
        [SerializeField]
        Gradient _branchGradient;

        [Header("即")]

        /// <summary>
        /// 葉っぱの色
        /// </summary>
        [SerializeField]
        Gradient _leafGradient;

        /// <summary>
        /// サブブランチの枝の長さの定数
        /// </summary>
        [SerializeField]
        float _subBranchLengthMutiplier = 0.9f;

        /// <summary>
        /// サブブランチの生える角度
        /// </summary>
        [SerializeField]
        float _subBranchAngle = 50f;

        public override VStructure CreateStructure(Matrix4x4 toWorld)
        {
            var s = new VStructure();

            var position = toWorld.MultiplyPoint3x4(Vector2.zero);
            var angle = 90f * Mathf.Deg2Rad;

            var origin = s.AddParticle(position, 0.5f);
            var next = MakeBranch(s, _maxDepth, origin, origin, position, angle, _baseLength);

            s.AddAfterStep(new VPin(origin, s.GetParticlePosition(origin)));
            s.AddAfterStep(new VPin(next, s.GetParticlePosition(next)));

            return s;
        }

        int MakeBranch(VStructure s, int depth, int originID, int parentID, Vector2 parentPosition, float angle, float length)
        {
            var position = parentPosition + Utilities.Angle2Vector(angle) * length;

            var currentID = s.AddParticle(position, 0.1f);

            s.AddEdge(parentID, currentID, 0.1f, _branchGradient.Evaluate(Random.Range(0f, 1f)));

            depth--;

            if (depth > 0)
            {
                var branchAngle = (Random.Range(0f, 1f) < 0.5f ? _subBranchAngle : -_subBranchAngle) * Mathf.Rad2Deg;
                var branchDir = Utilities.Angle2Vector(angle + branchAngle) * length * _subBranchLengthMutiplier;
                var branchID = s.AddParticle(branchDir + position, 0.2f, _leafGradient.Evaluate(Random.Range(0f, 1f)));

                s.AddEdge(currentID, branchID, 0.1f, _branchGradient.Evaluate(Random.Range(0f, 1f)));
                s.AddEdge(parentID, branchID, 0.025f, _branchGradient.Evaluate(Random.Range(0f, 1f)));

                var nextID = MakeBranch(s, depth, originID, currentID, position, angle, length * _lengthDamping);

                s.AddEdge(parentID, nextID, 0.025f, _branchGradient.Evaluate(Random.Range(0f, 1f)));
            }

            return currentID;
        }
    }
}