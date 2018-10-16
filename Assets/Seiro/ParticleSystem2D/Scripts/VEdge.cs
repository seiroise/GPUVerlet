using UnityEngine;

namespace Seiro.ParticleSystem2D
{

    /// <summary>
    /// パーティクル間
    /// </summary>
    [System.Serializable]
    public struct VEdge
    {

        /// <summary>
        /// パーティクルaとb
        /// </summary>
        public int a, b;

        /// <summary>
        /// パーティクル間の距離
        /// </summary>
        public float length;

        /// <summary>
        /// 描画時の幅
        /// </summary>
        public float width;

        /// <summary>
        /// 色
        /// </summary>
        public Color color;

        public VEdge(int a, int b, float length, float width = 1f, Color color = default(Color))
        {
            this.a = a;
            this.b = b;
            this.length = length;
            this.width = width;
            this.color = color;
        }
    }
}