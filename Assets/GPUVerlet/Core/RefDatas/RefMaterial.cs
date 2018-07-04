using UnityEngine;

namespace Seiro.GPUVerlet.Core.RefDatas
{

	/// <summary>
	/// 参照用マテリアル
	/// </summary>
	[System.Serializable]
	public class RefMaterial
	{
		public string id;
		public Material material;

		public RefMaterial(string id, Material material) {
			this.id = id;
			this.material = material;
		}
	}
}