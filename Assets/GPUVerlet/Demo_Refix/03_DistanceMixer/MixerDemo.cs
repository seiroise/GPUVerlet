using Seiro.GPUVerlet.Common;
using Seiro.GPUVerlet.Core.Compilers;
using Seiro.GPUVerlet.Core.Components;
using Seiro.GPUVerlet.Core.Mixers;
using Seiro.GPUVerlet.Core.RefDatas;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// とりあえずミキサーを試す
    /// </summary>
    public class MixerDemo : MonoBehaviour
    {

        [SerializeField]
        VerletModel _model;

        [SerializeField]
        bool _autoBuildAndAssign = true;

        [SerializeField, FlatRef(offset = 8)]
        Core.Architects.BaseArchitect _a;

        [SerializeField, FlatRef(offset = 8)]
        Core.Architects.BaseArchitect _b;

        [SerializeField]
        Core.Architects.BaseEdgeBuilder _edgeBuilder;

        [SerializeField]
        Core.Compilers.MaterialDictionary _materialDict;

        #region MonoBehaviourイベント

        void Start()
        {
            if (_autoBuildAndAssign)
            {
                BuildAndAssign();
            }
        }

        #endregion

        #region 内部処理

        /// <summary>
        /// ビルドしてアサインする
        /// </summary>
        void BuildAndAssign()
        {
            var structure = BuildAndMix();
            var compiled = StructureCompiler.Compile(structure, _materialDict);
            _model.SetStructure(compiled);
        }

        /// <summary>
        /// ビルドしてミックスする
        /// </summary>
        /// <returns></returns>
        RefStructure BuildAndMix()
        {
            var mat = transform.localToWorldMatrix;
            var a = _a.Build(mat);
            var b = _b.Build(mat);

            var dst = DistanceMixer.Mix(a, b, 10f, _edgeBuilder);

            return dst;
        }

        #endregion

#if UNITY_EDITOR

        [CanEditMultipleObjects]
        [CustomEditor(typeof(MixerDemo))]
        class InternalEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                DrawMultiGUI();
            }

            void DrawMultiGUI()
            {
                var selves = new MixerDemo[targets.Length];
                for (var i = 0; i < targets.Length; ++i)
                {
                    selves[i] = targets[i] as MixerDemo;
                }

                if (GUILayout.Button("Build and Assign"))
                {
                    for (var i = 0; i < selves.Length; ++i)
                    {
                        selves[i].BuildAndAssign();
                    }
                }
            }
        }
#endif

    }
}