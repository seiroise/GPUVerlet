using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.ParticleSystem2D
{

    /// <summary>
    /// particleとedgeからなる構造体
    /// </summary>
	[System.Serializable]
    public class VStructure
    {

        /// <summary>
        /// パーティクル
        /// </summary>
        List<VParticle> _particles;

        /// <summary>
        /// エッジ
        /// </summary>
        List<VEdge> _edges;

        /// <summary>
        /// エッジ解決前の外部ステップ
        /// </summary>
        List<IExternalStep> _beforeSteps;

        /// <summary>
        /// エッジ解決後の外部ステップ
        /// </summary>
        List<IExternalStep> _afterSteps;

        public VStructure()
        {
            _particles = new List<VParticle>();
            _edges = new List<VEdge>();

            _beforeSteps = new List<IExternalStep>();
            _afterSteps = new List<IExternalStep>();
        }

        #region 外部インタフェース

        /// <summary>
        /// パーティクルを追加する
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public int AddParticle(Vector2 position, float size = 1f, Color32 color = default(Color32))
        {
            var p = new VParticle(position, size, color);
            _particles.Add(p);
            return _particles.Count - 1;
        }

        /// <summary>
        /// エッジを追加する
        /// </summary>
        /// <param name="aID"></param>
        /// <param name="bID"></param>
        /// <returns></returns>
        public int AddEdge(int aID, int bID, float width = 1f, Color color = default(Color))
        {
            if (InParticlesRange(aID) && InParticlesRange(bID))
            {
                var ap = _particles[aID].position;
                var bp = _particles[bID].position;
                var e = new VEdge(aID, bID, (ap - bp).magnitude, width, color);
                _edges.Add(e);
                return _edges.Count - 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// エッジ解決前のステップに追加する
        /// </summary>
        /// <param name="step"></param>
        public void AddBeforeStep(IExternalStep step)
        {
            _beforeSteps.Add(step);
        }

        /// <summary>
        /// エッジ解決後のステップに追加する
        /// </summary>
        /// <param name="step"></param>
        public void AddAfterStep(IExternalStep step)
        {
            _afterSteps.Add(step);
        }

        /// <summary>
        /// 指定したIDのパーティクルの座標を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector2 GetParticlePosition(int id)
        {
            return InParticlesRange(id) ? _particles[id].position : default(Vector2);
        }

        /// <summary>
        /// パーティクル配列を取得する
        /// </summary>
        /// <returns></returns>
        public VParticle[] GetParticles()
        {
            return _particles.ToArray();
        }

        /// <summary>
        /// エッジ配列を取得する
        /// </summary>
        /// <returns></returns>
        public VEdge[] GetEdges()
        {
            return _edges.ToArray();
        }

        /// <summary>
        /// エッジ解決前のステップ配列を取得する
        /// </summary>
        /// <returns></returns>
        public IExternalStep[] GetBeforeSteps()
        {
            return _beforeSteps.ToArray();
        }

        /// <summary>
        /// エッジ解決後のステップ配列を取得する
        /// </summary>
        /// <returns></returns>
        public IExternalStep[] GetAfterSteps()
        {
            return _afterSteps.ToArray();
        }

        #endregion

        #region 内部処理

        /// <summary>
        /// インデックスはみ出し確認
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InParticlesRange(int id)
        {
            return 0 <= id && id < _particles.Count;
        }

        #endregion
    }
}