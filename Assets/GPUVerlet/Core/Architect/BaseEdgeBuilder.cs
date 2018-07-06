using Seiro.GPUVerlet.Core.RefDatas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Architect
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

	}
}