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
		/// 対象のオブジェクトを配列に変換する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="targets"></param>
		/// <returns></returns>
		public static T[] CastObjectArray<T>(IEnumerator<T> targets) where T : Object
		{
			return null;
		}
	}
}