using Seiro.ParticleSystem2D.Common;
using Seiro.ParticleSystem2D.Core.Architects;
using Seiro.ParticleSystem2D.Core.RefDatas;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Demo
{

    /// <summary>
    /// デモ用の樹木のあアーキテクト
    /// </summary>
    [CreateAssetMenu(fileName = "DemoTreeArchitect", menuName = "Seiro/GPUVerlet/DemoTreeArchitect")]
    public class DemoTreeArchitect : Core.Architects.BaseArchitect
    {

        [Header("幹の基本設定")]

        [Tooltip("幹の最大深度"), SerializeField, MinMax(0f, 10f)]
        MinMax _maxDepth = new MinMax(3f, 6f);

        [Tooltip("幹の基本長"), SerializeField, MinMax(0f, 10f)]
        MinMax _baseBranchLength = new MinMax(4f, 5f);

        [Tooltip("幹のエッジを作るビルダー"), SerializeField, FlatRef(offset = 8)]
        BaseEdgeBuilder _branchEdgeBuilder = null;

        [Tooltip("幹のサポートエッジを作るビルダー"), SerializeField, FlatRef(offset = 8)]
        BaseEdgeBuilder _branchSupportEdgeBuilder = null;

        [Tooltip("幹のパーティクルを作るビルダー"), SerializeField, FlatRef(offset = 8)]
        BaseParticleBuilder _branchParticleBuilder = null;

        [Header("枝の分岐設定")]

        [Tooltip("分岐幹Aの角度"), SerializeField, MinMax(0f, 360f)]
        MinMax _angleoffsetA = new MinMax(20f, 40f);

        [Tooltip("分岐幹Aの長さの減衰率"), SerializeField, MinMax(0f, 1f)]
        MinMax _branchLengthDampingA = new MinMax(0.7f, 0.8f);

        [Tooltip("分岐幹Bの角度"), SerializeField, MinMax(0f, 360f)]
        MinMax _angleoffsetB = new MinMax(10f, 50f);

        [Tooltip("分岐幹Bの長さの減衰率"), SerializeField, MinMax(0f, 1f)]
        MinMax _branchLengthDampingB = new MinMax(0.6f, 0.9f);

        [Header("葉の設定")]

        [Tooltip("葉の長さ"), SerializeField, MinMax(0f, 10f)]
        MinMax _leafLength = new MinMax(1f, 2f);

        [Tooltip("葉のエッジを作るビルダー"), SerializeField, FlatRef(offset = 8)]
        BaseEdgeBuilder _leafEdgeBuilder = null;

        float _tempBaseBranchLength = 0f;

        public override RefStructure Build(Matrix4x4 toWorld)
        {
            var s = RefStructure.CreateNew();

            var origin = _branchParticleBuilder.Add(s, Vector2.zero);
            _tempBaseBranchLength = _baseBranchLength.random;

            MakeBranch(s, _maxDepth.randomInt, origin, 90f, _tempBaseBranchLength);

            s.TranslateParticles(toWorld);

            return s;
        }

        RefParticle MakeBranch(RefStructure s, int depth, RefParticle parent, float angle, float length)
        {
            var dir = Utilities.Angle2Vector(angle * Mathf.Deg2Rad);
            var pos = parent.position + dir * length;
            var current = _branchParticleBuilder.Add(s, pos);

            depth--;
            if (depth > 0)
            {
                /*var edge = */
                _branchEdgeBuilder.Add(s, parent, current);

                // 分岐していく
                RefParticle b1, b2;
                if (length * Random.Range(1f, 2f) / _tempBaseBranchLength > 1f)
                {
                    b1 = MakeBranch(s, depth, current, angle + _angleoffsetA.random, length * _branchLengthDampingA.random);
                }
                else
                {
                    b1 = MakeBranch(s, depth, current, angle + _angleoffsetB.random, length * _branchLengthDampingB.random);
                }

                if (length * Random.Range(1f, 2f) / _tempBaseBranchLength > 1f)
                {
                    b2 = MakeBranch(s, depth, current, angle - _angleoffsetA.random, length * _branchLengthDampingA.random);
                }
                else
                {
                    b2 = MakeBranch(s, depth, current, angle - _angleoffsetB.random, length * _branchLengthDampingB.random);
                }

                // サポート
                _branchSupportEdgeBuilder.Add(s, b1, b2);
                _branchSupportEdgeBuilder.Add(s, parent, b1);
                _branchSupportEdgeBuilder.Add(s, parent, b2);
            }
            else
            {
                current.position = parent.position + dir * _leafLength.random;

                /*var leaf = */
                _leafEdgeBuilder.Add(s, parent, current);
            }

            return current;
        }
    }
}