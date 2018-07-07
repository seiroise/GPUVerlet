using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seiro.GPUVerlet.Common
{

    /// <summary>
    /// インスペクター上でフラットに参照を表示するためのAttribute
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class FlatRefAttribute : PropertyAttribute
    {

        /// <summary>
        /// 表示はするが操作させたくない場合はtrueに
        /// </summary>
        public bool disable = false;

        /// <summary>
        /// 表示するプロパティの
        /// </summary>
        public int offset = 0;

        /// <summary>
        /// ホールド表示
        /// </summary>
        public bool hold = true;

#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(FlatRefAttribute))]
        class InternalDrawer : PropertyDrawer
        {

            static readonly float padding = 4f;

            // static readonly float holdWidth = 16f;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                // base.OnGUI(position, property, label);

                EditorGUI.BeginChangeCheck();

                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    var attr = attribute as FlatRefAttribute;

                    // 参照
                    var valRect = position.E_MinTop(EditorGUIUtility.singleLineHeight);
                    EditorGUI.ObjectField(valRect, property);

                    // ホールド
                    if (property.objectReferenceValue != null)
                    {
                        attr.hold = EditorGUI.Foldout(valRect, attr.hold, GUIContent.none);
                    }

                    if (attr.hold && property.objectReferenceValue)
                    {
                        // 表示部分は矩形内に表示する
                        var boxRect = position.E_Add(0f, EditorGUIUtility.singleLineHeight + padding, 0f, 0f);
                        GUI.Box(boxRect, GUIContent.none);

                        EditorGUI.BeginDisabledGroup(attr.disable);

                        // 参照先の保持しているパラメータ
                        var refRect = boxRect.E_MoveY(padding).E_Add(14f, 0f, -padding, 0f);
                        MyEditorUtility.DrawObjectNonLayout(refRect, property.objectReferenceValue, attr.offset);

                        EditorGUI.EndDisabledGroup();
                    }
                }
                else
                {
                    EditorGUI.PropertyField(position, property);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                // オブジェクトの高さ + 一行分 + padding * 2
                // var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var height = EditorGUIUtility.singleLineHeight;

                var attr = attribute as FlatRefAttribute;
                if (attr.hold && property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue)
                {
                    height += padding * 3f;
                    height += MyEditorUtility.GetDrawHeight(property.objectReferenceValue, attr.offset);
                }
                return height;
                // return base.GetPropertyHeight(property, label);
            }
        }
#endif
    }
}