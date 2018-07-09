using Seiro.GPUVerlet.Core.Architects;
using Seiro.GPUVerlet.Core.Components;
using Seiro.GPUVerlet.Core.RefDatas;
using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// 簡易的なエディタ
    /// </summary>
    public class SimpleEditor : MonoBehaviour
    {

        [SerializeField]
        VerletModel _model;

        [SerializeField]
        RefStructure _structure;

        [SerializeField]
        BaseParticleBuilder _particleBuilder;

        #region 内部処理

        /// <summary>
        /// パーティクルの追加処理
        /// </summary>
        void HandleAddParticle()
        {
            if (_structure)
            {
                var wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _particleBuilder.Add(_structure, wpos);
            }
        }

        /// <summary>
        /// プレビューの更新
        /// </summary>
        void ApplyPreview()
        {

        }

        #endregion
    }
}
