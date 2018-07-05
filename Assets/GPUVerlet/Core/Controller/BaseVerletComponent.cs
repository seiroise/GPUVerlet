using Seiro.GPUVerlet.Core.RawDatas;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Controller
{

	/// <summary>
	/// 基底のVerletコンポーネント
	/// </summary>
	public class BaseVerletComponent : MonoBehaviour
	{

		#region 仮想インタフェース

		/// <summary>
		/// 構造体を設定する
		/// </summary>
		/// <param name="s"></param>
		public virtual void SetStructure(CompiledStructure s) { }

		#endregion
	}
}