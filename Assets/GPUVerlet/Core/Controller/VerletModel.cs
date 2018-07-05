using Seiro.GPUVerlet.Core.RawDatas;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Controller
{
	
	/// <summary>
	/// Verletを動かすためのモデル
	/// </summary>
	public class VerletModel : MonoBehaviour {

		#region 外部インタフェース

		/// <summary>
		/// 構造体を設定する
		/// </summary>
		/// <param name="s"></param>
		public void SetStructure(CompiledStructure s)
		{
			var coms = GetComponents<BaseVerletComponent>();
			for (var i = 0; i < coms.Length; ++i)
			{
				coms[i].SetStructure(s);
			}
		}

		#endregion

	}
}