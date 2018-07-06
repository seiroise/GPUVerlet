using Seiro.GPUVerlet.Core.RefDatas;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Compilers
{
	
	/// <summary>
	/// IDとMaterialのペアからなる辞書
	/// </summary>
	[CreateAssetMenu(menuName = "Seiro/GPUVerlet/MaterialDictionary", fileName = "MaterialDictionary")]
	public class MaterialDictionary : ScriptableObject
	{
		
		[SerializeField]
		List<RefMaterial> _items;

		#region 外部インタフェース

		/// <summary>
		/// 内容を辞書形式で書き出す
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, Material> ExportDictionry()
		{
			var dict = new Dictionary<string, Material>();

			for (var i = 0; i < _items.Count; ++i)
			{
				dict.Add(_items[i].id, _items[i].material);
			}

			return dict;
		}

		#endregion
	}
}