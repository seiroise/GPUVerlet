using Seiro.GPUVerlet.Core.RefDatas;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Architect
{

	/// <summary>
	/// 基底の建築士
	/// </summary>
	public class BaseArchitect : ScriptableObject
	{

		#region 仮想インタフェース

		/// <summary>
		/// 構造体を作成する
		/// </summary>
		/// <returns></returns>
		public virtual RefStructure Build() { return null; }

		#endregion
	}
}