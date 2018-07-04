using UnityEngine;

namespace Seiro.GPUVerlet.Core.Datas
{

	/// <summary>
	/// 位置を持つパーティクル
	/// </summary>
	[System.Serializable]
	public struct Particle
	{
		/// <summary>
		/// 現在の位置
		/// </summary>
		public Vector2 position;

		/// <summary>
		/// 古い位置
		/// </summary>
		public Vector2 oldPosition;

		public Particle(Vector2 position)
		{
			this.position = this.oldPosition = position;
		}
	}
}