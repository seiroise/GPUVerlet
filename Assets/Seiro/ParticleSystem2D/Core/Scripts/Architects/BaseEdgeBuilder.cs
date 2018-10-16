using Seiro.ParticleSystem2D.Core.RefDatas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.Architects
{

    /// <summary>
    /// 基底の建築用エッジ
    /// </summary>
    public class BaseEdgeBuilder : ScriptableObject
    {

        /// <summary>
        /// エッジを追加する
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public virtual RefEdge Add(RefStructure s, RefParticle a, RefParticle b) { return null; }

        /// <summary>
        /// エッジを追加する
        /// </summary>
        /// <param name="s"></param>
        /// <param name="aIdx"></param>
        /// <param name="bIdx"></param>
        /// <returns></returns>
        public virtual RefEdge Add(RefStructure s, int aIdx, int bIdx) { return null; }

    }
}