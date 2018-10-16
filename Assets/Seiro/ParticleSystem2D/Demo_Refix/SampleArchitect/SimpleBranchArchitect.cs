using Seiro.ParticleSystem2D.Common;
using Seiro.ParticleSystem2D.Core.Architects;
using Seiro.ParticleSystem2D.Core.Mixers;
using Seiro.ParticleSystem2D.Core.RefDatas;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Demo
{

    /// <summary>
    /// 比較的シンプルな枝のアーキテクト
    /// </summary>
    [CreateAssetMenu(fileName = "SimpleBranch", menuName = "Seiro/GPUVerlet/Architect/SimpleBranch")]
    public class SimpleBranchArchitect : Core.Architects.BaseArchitect
    {

        [Tooltip("枝の最大深度"), SerializeField, MinMax(4f, 32f)]
        MinMax _branchMaxDepth = new MinMax(8, 16);

        [Header("Length")]

        [Tooltip("枝の基本長"), SerializeField]
        float _branchLengthFactor = 1f;

        [Tooltip("枝の長さのカーブ"), SerializeField]
        AnimationCurve _branchLengthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Angle")]

        [Tooltip("枝の基本の曲がり"), SerializeField]
        float _branchAngleFactor = 30f;

        [Tooltip("枝の曲がりのカーブ"), SerializeField]
        AnimationCurve _branchAngleCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);

        [Header("Support edge")]

        [Tooltip("支持枝の間隔"), SerializeField]
        int[] _supportEdgeInterval;

        [Header("Builder")]

        [Tooltip("枝のエッジを作るビルダー"), SerializeField, FlatRef(offset = 8)]
        BaseParametricEdgeBuilder _branchEdgeBuilder = null;

        [Tooltip("枝のパーティクルを作るビルダー"), SerializeField, FlatRef(offset = 8)]
        BaseParametricParticleBuilder _branchParticleBuilder = null;

        [Header("Sub branch architect")]

        [Tooltip("枝のアーキテクト"), SerializeField]
        Core.Architects.BaseArchitect _subBranchArchitect = null;

        [Tooltip("接続用のEdge Builder"), SerializeField]
        BaseEdgeBuilder _connectionEdgeBuilder = null;

        RefStructure _s;

        public override RefStructure Build(Matrix4x4 toWorld)
        {
            _s = RefStructure.CreateNew();
            var origin = _branchParticleBuilder.Add(_s, Vector2.zero, 0f);

            var maxDepth = _branchMaxDepth.randomInt;

            // 枝の作成
            MakeBranch(0, maxDepth, origin, 90f * Mathf.Deg2Rad);

            // 支持枝の作成
            var count = _s.GetParticleCount();
            for (int i = 0, n = _supportEdgeInterval.Length; i < n; ++i)
            {
                var interval = _supportEdgeInterval[i];
                for (int j = interval, n2 = count; j < n2; ++j)
                {
                    _branchEdgeBuilder.Add(_s, j - interval, j, 0f);
                }
            }

            _s.TranslateParticles(toWorld);

            return _s;
        }

        /// <summary>
        /// 枝を再起的に生成する
        /// </summary>
        /// <param name="s"></param>
        /// <param name="depth"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        RefParticle MakeBranch(int depth, int maxDepth, RefParticle parent, float angle)
        {
            var t = depth / (float)maxDepth;

            angle += (_branchAngleCurve.Evaluate(t) * _branchAngleFactor) * Mathf.Deg2Rad;
            var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            var length = _branchLengthCurve.Evaluate(t) * _branchLengthFactor;

            var pos = parent.position + dir * length;
            var current = _branchParticleBuilder.Add(_s, pos, t);

            _branchEdgeBuilder.Add(_s, parent, current, t);

            if (depth > maxDepth)
            {
                return current;
            }
            else
            {
                var child = MakeBranch(++depth, maxDepth, current, angle);

                // 枝を伸ばせる場合は枝を伸ばす
                if (_subBranchArchitect && _connectionEdgeBuilder)
                {
                    // Debug.Log("before: " + _s.GetParticleCount());
                    var subBranchMatrix = Matrix4x4.TRS(pos, Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward), Vector3.one);
                    var branch = _subBranchArchitect.Build(subBranchMatrix);
                    // var branch = _subBranchArchitect.Build(Matrix4x4.identity);
                    // Debug.Log(branch.GetParticleCount());
                    _s = DistanceMixer.Mix(_s, branch, 3f, _connectionEdgeBuilder);
                    // Debug.Log("after: " + _s.GetParticleCount());
                }

                return child;
            }
        }
    }
}