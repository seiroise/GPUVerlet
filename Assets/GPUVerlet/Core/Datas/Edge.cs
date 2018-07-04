namespace Seiro.GPUVerlet.Core.Datas
{
	
	/// <summary>
	/// パーティクル間のつながり
	/// </summary>
	[System.Serializable]
	public struct Edge
	{

		/// <summary>
		/// パーティクルAの番号
		/// </summary>
		public int aID;

		/// <summary>
		/// パーティクルBの番号
		/// </summary>
		public int bID;

		/// <summary>
		/// パーティクル間の静止距離
		/// </summary>
		public float restLength;

		public Edge(int aID, int bID, float restLength)
		{
			this.aID = aID;
			this.bID = bID;
			this.restLength = restLength;
		}
	}
}