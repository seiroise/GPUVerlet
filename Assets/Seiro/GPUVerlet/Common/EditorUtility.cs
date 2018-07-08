#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Seiro.GPUVerlet.Common
{

    public static class MyEditorUtility
    {

        /// <summary>
        /// 指定したobjectのpropertyをNonLayoutで描画する。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="offset"></param>
        public static void DrawObjectNonLayout(Rect rect, Object target, int offset = 0)
        {
            if (!target) return;

            var sObj = new SerializedObject(target);
            sObj.Update();
            var props = sObj.GetIterator();
            var counter = 0;
            props.Next(true);

            var drawRect = rect.E_MinTop(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            while (props.Next(false))
            {
                counter++;

                // ScriptableObjectの場合は先頭から8番目はいらないのでそういうのに対応するため
                if (counter > offset)
                {
                    drawRect.height = GetDrawHeight(props);
                    EditorGUI.PropertyField(drawRect, props, true);
                    drawRect = drawRect.E_MoveY(EditorGUI.GetPropertyHeight(props) + EditorGUIUtility.standardVerticalSpacing);
                    // drawRect = drawRect.E_MoveY(EditorGUI.GetPropertyHeight(props) + EditorGUIUtility.standardVerticalSpacing);
                }
            }

            // 変更があればそれを適用
            if (EditorGUI.EndChangeCheck())
            {
                sObj.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 指定したSerializedPropertyの描画に必要な高さを計算する
        /// </summary>
        /// <param name="property"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static float GetDrawHeight(SerializedProperty property, int offset = 0)
        {
            if (property == null) return 0f;
            return EditorGUI.GetPropertyHeight(property);
        }

        /// <summary>
        /// 指定したObjectの描画に必要な高さを返す
        /// </summary>
        /// <param name="target"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static float GetDrawHeight(Object target, int offset = 0)
        {

            if (!target) return 0f;

            var sObj = new SerializedObject(target);
            var props = sObj.GetIterator();
            var counter = 0;
            props.Next(true);

            var height = 0f;
            while (props.Next(false))
            {
                counter++;
                // ScriptableObjectの場合は先頭から8番目はいらないのでそういうのに対応するため
                if (counter > offset)
                {
                    height += EditorGUI.GetPropertyHeight(props);
                    height += EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return height;
        }
    }
}

#endif