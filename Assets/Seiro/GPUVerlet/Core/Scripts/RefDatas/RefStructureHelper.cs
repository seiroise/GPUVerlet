using UnityEngine;

namespace Seiro.GPUVerlet.Core.RefDatas
{

	/// <summary>
	/// こっちはどちらかというとヘルパー的な関数軍
	/// </summary>
    public partial class RefStructure : ScriptableObject
    {

        /// <summary>
        /// 指定した座標にヒットした最も近いパーティクルを返す
        /// </summary>
        /// <param name="s"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static RefParticle HitParticle(RefStructure s, Vector2 position)
        {
            float _minDist = float.MaxValue;
            RefParticle nearestHitten = null;

            for (var i = 0; i < s._particles.Count; ++i)
            {
                var t = s._particles[i];
                var dist = (t.position - position).magnitude;
                if (_minDist > dist && dist <= t.size)
                {
                    _minDist = dist;
                    nearestHitten = t;
                }
            }

            return nearestHitten;
        }

        /// <summary>
        /// 指定した座標にヒットした最も近いエッジを返す
        /// </summary>
        /// <param name="s"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static RefEdge HitEdge(RefStructure s, Vector2 position)
        {
            float _minDist = float.MaxValue;
            RefEdge nearestHitten = null;

            for (var i = 0; i < s._edges.Count; ++i)
            {
                var t = s._edges[i];
                var a = s.FindParticleFromUID(t.aUID);
                var b = s.FindParticleFromUID(t.bUID);
                var dist = Utilities.CalcMinDist2(position, a.position, b.position);
                if (_minDist > dist && dist <= t.width)
                {
                    _minDist = dist;
                    nearestHitten = t;
                }
            }

            return nearestHitten;
        }

		/// <summary>
		/// 簡易的なプレビューをGizmos上に描画する
		/// </summary>
		/// <param name="s"></param>
		public static void DrawSimplePreviewOnGizmos(RefStructure s)
		{
			for (var i = 0; i < s._particles.Count; ++i)
			{
				var t = s._particles[i];
				Gizmos.DrawWireSphere(t.position, t.size);
			}

			for (var i = 0; i < s._edges.Count; ++i)
			{
				
			}
		}
    }
}