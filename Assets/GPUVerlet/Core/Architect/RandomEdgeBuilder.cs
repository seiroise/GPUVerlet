using Seiro.GPUVerlet.Common;
using Seiro.GPUVerlet.Core.RefDatas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Architect
{

	/// <summary>
	/// パラメータをランダムに決めてエッジを作る
	/// </summary>
	[CreateAssetMenu(menuName = "Seiro/GPUVerlet/RandomEdgeBuilder", fileName = "RandomEdgeBuilder")]
	public class RandomEdgeBuilder : BaseEdgeBuilder
	{

		[SerializeField, MinMax(0f, 10f)]
		MinMax _width;

		[SerializeField]
		Gradient _color;

		[SerializeField]
		string _materialID = "Edge";

		public override RefEdge Add(RefStructure s, RefParticle a, RefParticle b)
		{
			return s.AddEdge(a, b, _width.random, _color.E_Random(), _materialID);
		}
	}
}