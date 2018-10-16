using UnityEngine;

namespace Seiro.ParticleSystem2D
{

    /// <summary>
    /// 便利な関数
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// 角度(radian)をベクトルに変換
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Vector2 Angle2Vector(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
    }
}