using Seiro.GPUVerlet.Core.RefDatas;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core
{
	
	/// <summary>
	/// IDとMaterialのペアからなる辞書
	/// </summary>
	[CreateAssetMenu(menuName = "Seiro/GPUVerlet/MaterialDictionary", fileName = "MaterialDictionary")]
	public class MaterialDictionary : ScriptableObject
	{
		
		[SerializeField]
		List<RefMaterial> _items;

	}
}