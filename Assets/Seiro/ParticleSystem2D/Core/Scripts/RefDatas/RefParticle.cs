using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.RefDatas
{

	/// <summary>
	/// パーティクルと物理情報と描画設定
	/// </summary>
	[System.Serializable]
	public class RefParticle
	{
		public uint uid;
		public Vector2 position;
		public float size;
		public Color color;
		public string materialID;

		public RefParticle(uint uid, Vector2 position, float size, Color color, string materialID)
		{
			this.uid = uid;
			this.position = position;
			this.size = size;
			this.color = color;
			this.materialID = materialID;
		}
	}
}