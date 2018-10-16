using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Common
{

	/// <summary>
	/// UnityEngine.Rectの拡張関数
	/// </summary>
	public static class RectExtentions
	{
		/// <summary>
		/// 指定した矩形領域の指定した割合の領域を抜き出す
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		/// <returns></returns>
		public static Rect E_Ratio(this Rect rect, float left, float top, float right, float bottom)
		{

			return new Rect()
			{
				xMin = rect.xMin + (rect.width * left),
				yMin = rect.yMin + (rect.height * top),
				xMax = rect.xMin + (rect.width * right),
				yMax = rect.yMin + (rect.height * bottom)
			};
		}

		/// <summary>
		/// 指定した矩形領域のそれぞれの方向に加算する(移動ではないので注意)
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		/// <returns></returns>
		public static Rect E_Add(this Rect rect, float left, float top, float right, float bottom)
		{
			return new Rect()
			{
				xMin = rect.xMin + left,
				yMin = rect.yMin + top,
				xMax = rect.xMax + right,
				yMax = rect.yMax + bottom
			};
		}

		/// <summary>
		/// 指定した矩形領域をx軸方向に移動する
		/// </summary>
		/// <param name="Rect">Rect.</param>
		/// <param name="x">The x coordinate.</param>
		public static Rect E_MoveX(this Rect rect, float x)
		{
			return new Rect()
			{
				xMin = rect.xMin + x,
				yMin = rect.yMin,
				xMax = rect.xMax + x,
				yMax = rect.yMax
			};
		}

		/// <summary>
		/// 指定した矩形領域をy軸方向に移動する
		/// </summary>
		/// <param name="Rect">Rect.</param>
		/// <param name="y">The y coordinate.</param>
		public static Rect E_MoveY(this Rect rect, float y)
		{
			return new Rect()
			{
				xMin = rect.xMin,
				yMin = rect.yMin + y,
				xMax = rect.xMax,
				yMax = rect.yMax + y
			};
		}

		/// <summary>
		/// 指定した矩形領域の左側に最小値だけの幅を持つ矩形を返す
		/// </summary>
		/// <returns>The left.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="min">Minimum.</param>
		public static Rect E_MinLeft(this Rect rect, float min)
		{
			return new Rect()
			{
				xMin = rect.xMin,
				yMin = rect.yMin,
				xMax = rect.xMin + min,
				yMax = rect.yMax
			};
		}

		/// <summary>
		/// 指定した矩形領域の右側に最小値だけの幅を持つ矩形を返す
		/// </summary>
		/// <returns>The right.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="min">Minimum.</param>
		public static Rect E_MinRight(this Rect rect, float min)
		{
			return new Rect()
			{
				xMin = rect.xMax - min,
				yMin = rect.yMin,
				xMax = rect.xMax,
				yMax = rect.yMax
			};
		}

		/// <summary>
		/// 指定した矩形領域の上側に最小値だけの高さを持つ矩形を返す
		/// </summary>
		/// <returns>The top.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="min">Minimum.</param>
		public static Rect E_MinTop(this Rect rect, float min)
		{
			return new Rect()
			{
				xMin = rect.xMin,
				yMin = rect.yMin,
				xMax = rect.xMax,
				yMax = rect.yMin + min
			};
		}

		/// <summary>
		/// 指定した矩形領域の下側に最小値だけの高さを持つ矩形を返す
		/// </summary>
		/// <returns>The bottom.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="min">Minimum.</param>
		public static Rect E_MinBottom(this Rect rect, float min)
		{
			return new Rect()
			{
				xMin = rect.xMin,
				yMin = rect.yMax - min,
				xMax = rect.xMax,
				yMax = rect.yMax
			};
		}

		public static Vector2 E_TopLeft(this Rect self)
		{
			return new Vector2(self.xMin, self.yMax);
		}

		public static Vector2 E_TopRight(this Rect self)
		{
			return new Vector2(self.xMax, self.yMax);
		}

		public static Vector2 E_BottomLeft(this Rect self)
		{
			return new Vector2(self.xMin, self.yMin);
		}

		public static Vector2 E_BottomRight(this Rect self)
		{
			return new Vector2(self.xMax, self.yMin);
		}
	}
}