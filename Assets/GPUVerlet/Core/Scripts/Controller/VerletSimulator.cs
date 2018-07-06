using UnityEngine;
using Seiro.GPUVerlet.Core.RawDatas;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seiro.GPUVerlet.Core.Controller
{

    /// <summary>
    /// Verletにのっとってシミュレーションする。
    /// </summary>
    public class VerletSimulator : BaseVerletComponent
    {

        [SerializeField, Range(0f, 1f)]
        float _damping = 0.98f;
        [SerializeField, Range(1, 10)]
        int _iterations = 3;

        RawDatas.Particle[] _particles;
        Edge[] _edges;

        #region 外部インタフェース

        /// <summary>
        /// 構造体を設定する
        /// </summary>
        /// <param name="s"></param>
        public override void SetStructure(CompiledStructure s)
        {
            _particles = s.particles;
            _edges = s.edges;
        }

        /// <summary>
        /// 全体を1ステップすすめる
        /// </summary>
        public void Simulate()
        {
            StepParticles();
            SolveEdges();
        }

        #endregion

        #region 内部処理

        /// <summary>
        /// パーティクル全体を1ステップシミュレートする
        /// </summary>
        void StepParticles()
        {
            for (var i = 0; i < _particles.Length; ++i)
            {
                StepParticle(i);
            }
        }

        /// <summary>
        /// パーティクルを1ステップシミュレートする
        /// </summary>
        /// <param name="p"></param>
        void StepParticle(int id)
        {
            var p = _particles[id];

            var dx = (p.position.x - p.oldPosition.x) * _damping;
            var dy = (p.position.y - p.oldPosition.y) * _damping;

            var nx = p.position.x + dx;
            var ny = p.position.y + dy;

            p.oldPosition.x = p.position.x;
            p.oldPosition.y = p.position.y;

            p.position.x = nx;
            p.position.y = ny;

            _particles[id] = p;
        }

        /// <summary>
        /// エッジによる座標調整を規定回数反復して解決する
        /// </summary>
        void SolveEdges()
        {
            for (var i = 0; i < _iterations; ++i)
            {
                StepEdges();
            }
        }

        /// <summary>
        /// エッジ全体を1ステップシミュレートする
        /// </summary>
        void StepEdges()
        {
            for (var i = 0; i < _edges.Length; ++i)
            {
                StepEdge(i);
            }
        }

        /// <summary>
        /// エッジを1ステップシミュレートする
        /// </summary>
        /// <param name="e"></param>
        void StepEdge(int id)
        {
            var e = _edges[id];

            var a = _particles[e.aID];
            var b = _particles[e.bID];

            float dx = a.position.x - b.position.x;
            float dy = a.position.y - b.position.y;

            var current = Mathf.Sqrt(dx * dx + dy * dy);
            var f = (current - e.restLength) / current;

            float ax = f * 0.5f * dx;
            float ay = f * 0.5f * dy;

            a.position.x -= ax;
            a.position.y -= ay;

            b.position.x += ax;
            b.position.y += ay;

            _particles[e.aID] = a;
            _particles[e.bID] = b;
        }

        #endregion

#if UNITY_EDITOR

        [CanEditMultipleObjects]
        [CustomEditor(typeof(VerletSimulator))]
        class InternalEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                var selves = new VerletSimulator[targets.Length];
                for (var i = 0; i < targets.Length; ++i)
                {
                    selves[i] = targets[i] as VerletSimulator;
                }

                var particleSum = 0;
                for (var i = 0; i < selves.Length; ++i)
                {
                    particleSum += selves[i]._particles.Length;
                }
                var edgeSum = 0;
                for (var i = 0; i < selves.Length; ++i)
                {
                    edgeSum += selves[i]._edges.Length;
                }

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("Particle count : " + particleSum);
                EditorGUILayout.LabelField("Edge count : " + edgeSum);

                EditorGUILayout.EndVertical();
            }
        }

#endif
    }
}