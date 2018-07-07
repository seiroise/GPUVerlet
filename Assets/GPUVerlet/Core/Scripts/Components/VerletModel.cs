using Seiro.GPUVerlet.Core.RawDatas;
using UnityEngine;
using UnityEngine.Assertions;

namespace Seiro.GPUVerlet.Core.Components
{

    /// <summary>
    /// Verletを動かすためのモデル
    /// </summary>
    [RequireComponent(typeof(VerletRenderer), typeof(VerletSimulator))]
    [ExecuteInEditMode]
    public class VerletModel : MonoBehaviour
    {

        [SerializeField]
        CompiledStructure _structure;

        [SerializeField]
        VerletSimulator _simulator;

        [SerializeField]
        VerletRenderer _renderer;

        #region MonoBehaviourイベント

        private void Start()
        {
            if (_structure)
            {
                SetStructure(_structure);
            }
        }

        private void Update()
        {
            if (_renderer && _renderer.IsReady()) _renderer.Render();
            if (_simulator && _simulator.IsReady()) _simulator.Simulate();
        }

        #endregion

        #region 外部インタフェース

        /// <summary>
        /// 構造体を設定する
        /// </summary>
        /// <param name="s"></param>
        public void SetStructure(CompiledStructure s)
        {
            Assert.IsNotNull(s);
            Assert.IsNotNull(_simulator);
            Assert.IsNotNull(_renderer);

            _structure = s;

            _simulator.SetStructure(s);
            _renderer.SetStructure(s);
        }

        #endregion

    }
}