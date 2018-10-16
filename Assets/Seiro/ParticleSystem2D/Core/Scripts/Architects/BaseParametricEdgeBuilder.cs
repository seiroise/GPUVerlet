using Seiro.ParticleSystem2D.Core.RefDatas;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.Architects
{
    /// <summary>
    /// 基底のパラメータ依存のエッジビルダー
    /// </summary>
    public class BaseParametricEdgeBuilder : ScriptableObject
    {

        /// <summary>
        /// エッジを追加する
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual RefEdge Add(RefStructure s, RefParticle a, RefParticle b, float t)
        {
            return null;
        }

        /// <summary>
        /// エッジを追加する
        /// </summary>
        /// <param name="s"></param>
        /// <param name="aIdx"></param>
        /// <param name="bIdx"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual RefEdge Add(RefStructure s, int aIdx, int bIdx, float t)
        {
            return null;
        }
    }
}