using Seiro.GPUVerlet.Core.RefDatas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Architect
{

	/// <summary>
	/// 基底の建築用パーティクル
	/// </summary>
	public class BaseParticleBuilder : ScriptableObject
	{

		/// <summary>
		/// パーティクルを追加する
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public virtual RefParticle Add(RefStructure s, Vector2 p) { return null; }

	}
}