using Seiro.GPUVerlet.Common;
using Seiro.GPUVerlet.Core.Mixers;
using Seiro.GPUVerlet.Core.RefDatas;
using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// とりあえずミキサーを試す
    /// </summary>
    public class MixerDemo : MonoBehaviour
    {

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

        }

        #endregion

        #region 内部処理

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
    }
}