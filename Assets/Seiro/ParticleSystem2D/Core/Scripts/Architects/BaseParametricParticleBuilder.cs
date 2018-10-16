using Seiro.ParticleSystem2D.Core.RefDatas;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.Architects
{

    /// <summary>
    /// 基底のパラメータ依存なパーティクルビルダー
    /// </summary>
    public class BaseParametricParticleBuilder : ScriptableObject
    {

        /// <summary>
        /// パーティクルを追加する
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual RefParticle Add(RefStructure s, Vector2 p, float t) { return null; }
    }
}