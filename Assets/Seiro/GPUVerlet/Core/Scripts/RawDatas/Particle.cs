using UnityEngine;

namespace Seiro.GPUVerlet.Core.RawDatas
{

    /// <summary>
    /// 位置を持つパーティクル
    /// </summary>
    [System.Serializable]
    public struct Particle
    {
        public Vector2 position;
        public Vector2 oldPosition;
		public float size;
		public Color color;

        public Particle(Vector2 position, float size, Color color)
        {
            this.position = this.oldPosition = position;
			this.size = size;
			this.color = color;
        }
    }
}