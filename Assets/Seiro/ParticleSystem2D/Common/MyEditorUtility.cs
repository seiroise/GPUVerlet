#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Seiro.ParticleSystem2D.Common
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

        /// <summary>
        /// GameObjectにDirtyフラグを立てる
        /// </summary>
        /// <param name="obj"></param>
        public static void DirtyObject(GameObject obj)
        {
            if (!IsPrefabObject(obj))
            {
                // プレハブ出ない場合はSceneのDirtyを立てる
                Scene scene = GetSceneFromTransform(obj.transform);
                if (scene.IsValid())
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
                }
            }
            else
            {
                EditorUtility.SetDirty(obj);
            }
        }

        /// <summary>
        /// Transformの所属するSceneを返す
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Scene GetSceneFromTransform(Transform trans)
        {
            if (trans)
            {
                var rootObject = trans.root.gameObject;
                for (int i = 0, n = SceneManager.sceneCount; i < n; ++i)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    var roots = scene.GetRootGameObjects();
                    for (int i2 = 0, n2 = roots.Length; i2 < n2; ++i2)
                    {
                        if (roots[i2] == rootObject)
                        {
                            return scene;
                        }
                    }
                }
            }

            // 該当なし、SceneSettingsに多分設定されていない
            return default(Scene);
        }

        /// <summary>
        /// プレハブかどうかの確認
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsPrefabObject(GameObject obj)
        {
            return (obj != null && PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab);
        }
    }
}

#endif