using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Datas
{

	/// <summary>
	/// Particleとedgeからなる構造体
	/// </summary>
	public class StructureBasic
	{
		/// <summary>
		/// パーティクル
		/// </summary>
		Particle[] _particles;

		/// <summary>
		/// エッジ
		/// </summary>
		Edge[] _edges;
	}
}