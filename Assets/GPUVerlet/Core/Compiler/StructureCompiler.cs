using Seiro.GPUVerlet.Core.RefDatas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Converters
{

	/// <summary>
	/// 構造体を実際に使用できる状態に仕上げる
	/// </summary>
	public class StructureConverter
	{

		#region 外部インタフェース

		/// <summary>
		/// コンパイルして使える状態に仕上げる
		/// </summary>
		/// <param name="structure"></param>
		/// <param name="materialDict"></param>
		public void Compile(RefStructure structure, MaterialDictionary materialDict)
		{
			var particles = structure.GetParticles();
			var edges = structure.GetEdges();
		}

		#endregion

	}
}