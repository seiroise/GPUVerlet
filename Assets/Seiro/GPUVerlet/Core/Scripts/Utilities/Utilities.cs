using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core
{

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

        /// <summary>
        /// p1とp2からなる線分とp0の最短距離の二乗を計算する。詳しいことは下のリンク先を見たほうがいいかもしれない。
 		/// https://qiita.com/yellow_73/items/bcd4e150e7caa0210ee6
		/// </summary>
  		/// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float CalcMinDist2(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            var a = p2.x - p1.x;
            var b = p2.y - p1.y;
            var a2 = a * a;
            var b2 = b * b;
            var r2 = a2 + b2;
            var ux = p1.x - p0.x;
            var uy = p1.y - p0.y;

            var tt = -(a * ux + b * uy);

            if (tt < 0f)
            {
                return (ux * ux) + (uy * uy);
            }
            if (tt > r2)
            {
                return (p2.x - p0.x) * (p2.x - p0.x) + (p2.y - p0.y) * (p2.y - p0.y);
            }

            var f1 = a * uy - b * ux;
            return (f1 * f1) / r2;
        }

        /// <summary>
        /// Gradientからランダムに色を選択する
        /// </summary>
        /// <param name="grad"></param>
        /// <returns></returns>
        public static Color E_Random(this Gradient grad)
        {
            return grad.Evaluate(Random.Range(0f, 1f));
        }
    }
}