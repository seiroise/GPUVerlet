using UnityEngine;

namespace Seiro.ParticleSystem2D
{

    /// <summary>
    /// パーティクル
    /// </summary>
    [System.Serializable]
    public struct VParticle
    {

        /// <summary>
        /// 座標
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// 1ステップ前の座標
        /// </summary>
        public Vector2 prevPosition;

        /// <summary>
        /// パーティクルの大きさ
        /// </summary>
        public float size;

        /// <summary>
        /// 色
        /// </summary>
        public Color color;

        public VParticle(Vector2 position, float size = 1f, Color32 color = default(Color32))
        {
            this.position = this.prevPosition = position;
            this.size = size;
            this.color = color;
        }
    }
}