using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{

	/// <summary>
	/// 構造エディタのコントローラ
	/// </summary>
	public class StructureEditorController : MonoBehaviour
	{

		/// <summary>
		/// 構造体
		/// </summary>
		VStructure _structure;

		#region MonoBehaviourイベント

		private void Awake()
		{
			_structure = new VStructure();
		}

		#endregion

		#region 外部インタフェース

		/// <summary>
		/// 現段階での構造体を取得する
		/// </summary>
		/// <returns></returns>
		public VStructure GetStructure()
		{
			return null;
		}

		#endregion

	}
}