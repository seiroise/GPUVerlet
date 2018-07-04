using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// 構造体のプレビューを表示したい
    /// </summary>
    public class VStructurePreviewer : MonoBehaviour
    {

        /// <summary>
        /// プレビュー用の構造体
        /// </summary>
        [SerializeField]
        VStructureObject _structureObject;

        /// <summary>
        /// 構造体を作る設計士
        /// </summary>
        [SerializeField]
        BaseArchitect _architect;

        void OnDrawGizmos()
        {
            if (_structureObject)
            {
                _structureObject.DrawPreviewOnGizmos(transform.localToWorldMatrix);
            }
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(VStructurePreviewer))]
        class InternalEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                var self = target as VStructurePreviewer;

                if (GUILayout.Button("新しく生成"))
                {
                    if (self._architect)
                    {
                        self._structureObject = VStructureObject.CreateNew(self._architect.CreateStructure(Matrix4x4.identity));
                    }
                }
            }
        }
#endif
    }
}