using UnityEngine;

namespace Seiro.GPUVerlet.Core.RawDatas
{

    /// <summary>
    /// パーティクル間のつながり
    /// </summary>
    [System.Serializable]
    public struct Edge
    {

        public int aID;
        public int bID;
		public float restLength;

		public float width;
		public Color color;

        public Edge(int aID, int bID, float restLength, float width, Color color)
        {
            this.aID = aID;
            this.bID = bID;
            this.restLength = restLength;

			this.width = width;
			this.color = color;
        }
    }
}