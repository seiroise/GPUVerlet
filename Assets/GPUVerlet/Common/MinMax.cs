using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seiro.GPUVerlet.Common
{
	[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class MinMaxAttribute : PropertyAttribute
	{
		float minLimit;
		float maxLimit;

		public MinMaxAttribute(float minLimit = 0, float maxLimit = 1f)
		{
			this.minLimit = minLimit;
			this.maxLimit = maxLimit;
		}

#if UNITY_EDITOR

		[CustomPropertyDrawer(typeof(MinMaxAttribute))]
		internal sealed class InternalDrawer : PropertyDrawer
		{
			const int NUM_WIDTH = 80;
			const int PADDING = 5;

			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				// 一つのプロパティの塊とする
				using (new EditorGUI.PropertyScope(position, label, property))
				{
					MinMaxAttribute att = attribute as MinMaxAttribute;

					if (property.type.Equals("MinMax"))
					{

						// EditorGUI.BeginProperty(position, label, property);

						// インデントに対応
						// というかインデントサイズってどっかしらで参照できないんですかね？
						var widthDiff = position.width - (NUM_WIDTH + EditorGUI.indentLevel * 15f);

						// 全体の矩形領域(一行分増えるだけ)
						Rect entireRect = position;

						// 背景用の矩形領域(なぜかdrawrectはインデント付きじゃないとダメ、というか他は何で大丈夫？
						Rect bgRect = EditorGUI.IndentedRect(position);

						// ラベルの矩形領域
						Rect labelRect = entireRect.E_Ratio(0f, 0f, 1f, 0.5f); // .Move(0f, 0f, 0f, PADDING * 0.5f);

						// スライダー部分の矩形領域
						Rect valueRect = entireRect.E_Ratio(0f, 0.5f, 1f, 1f).E_Add(2f, 0f, -2f, -2f);

						// スライダーの部品の矩形領域
						Rect minRect = valueRect.E_Add(0f, 0f, -widthDiff, 0f);
						Rect maxRect = valueRect.E_Add(widthDiff, 0f, 0f, 0f);
						Rect sliderRect = valueRect.E_Add(NUM_WIDTH, 0f, -NUM_WIDTH, 0f);

						// 背景
						// EditorGUI.DrawRect(bgRect, new Color(0f, 0f, 0f, 0.1f));
						GUI.Box(bgRect, GUIContent.none);

						// ラベル
						EditorGUI.PrefixLabel(labelRect, GUIUtility.GetControlID(FocusType.Passive), label);

						//プロパティの取得
						SerializedProperty minProp = property.FindPropertyRelative("_min");
						SerializedProperty maxProp = property.FindPropertyRelative("_max");

						//値の取得
						float min = minProp.floatValue;
						float max = maxProp.floatValue;

						//プロパティのの描画
						min = Mathf.Clamp(EditorGUI.FloatField(minRect, min), att.minLimit, max);
						max = Mathf.Clamp(EditorGUI.FloatField(maxRect, max), min, att.maxLimit);

						EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, att.minLimit, att.maxLimit);

						//プロパティに値を設定
						minProp.floatValue = min;
						maxProp.floatValue = max;

						// 再描画
						EditorUtility.SetDirty(property.serializedObject.targetObject);
					}
					else
					{
						EditorGUI.PropertyField(position, property, label);
					}
				}
			}

			public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
			{
				// 2行分使うので
				return EditorGUIUtility.singleLineHeight * 2f + PADDING;
				// return base.GetPropertyHeight(property, label);
			}
		}
#endif

	}

	/// <summary>
	/// 最大値と最小値を保持する
	/// </summary>
	[System.Serializable]
	public class MinMax
	{
		/// <summary>
		/// 最小値
		/// </summary>
		[SerializeField]
		float _min;
		public float min { get { return _min; } set { _min = value; } }

		/// <summary>
		/// 最大値
		/// </summary>
		[SerializeField]
		float _max;
		public float max { get { return _max; } set { _max = value; } }

		/// <summary>
		/// min, maxの範囲内からランダムなfloatを返す
		/// </summary>
		public float random { get { return Random.Range(_min, _max); } }

		/// <summary>
		/// min,maxの範囲からランダムなintを返す
		/// </summary>
		public int randomInt { get { return Random.Range((int)_min, (int)_max); } }

		/// <summary>
		/// 最小値から最大値の幅を返す
		/// </summary>
		public float width { get { return Mathf.Abs(_max - _min); } }

		public MinMax(float min, float max)
		{
			_min = min;
			_max = max;
		}

		#region Functions

		/// <summary>
		/// min, maxの範囲にvalueをclampする
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public float Clamp(float value)
		{
			return Mathf.Clamp(value, _min, _max);
		}

		/// <summary>
		/// min, maxの範囲にvalueが存在する場合はtrueを返す
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool InRange(float value)
		{
			return _min < value && value < _max;
		}

		/// <summary>
		/// min, maxの範囲内をtで内挿した値を返す
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public float Lerp(float t)
		{
			return Mathf.Lerp(_min, _max, t);
		}

		/// <summary>
		/// min, maxの範囲内に対応するvalueの割合を返す
		/// </summary>
		/// <returns></returns>
		public float InverseLerp(float value)
		{
			return Mathf.InverseLerp(_min, _max, value);
		}

		#endregion
	}
}