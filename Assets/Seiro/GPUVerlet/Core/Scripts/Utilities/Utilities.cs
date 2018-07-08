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