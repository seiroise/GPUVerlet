using Seiro.ParticleSystem2D.Core.RawDatas;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.Components
{

    /// <summary>
    /// 基底のVerletコンポーネント
    /// </summary>
    public class BaseVerletComponent : MonoBehaviour
    {

        #region 仮想インタフェース

        /// <summary>
        /// 準備できているか
        /// </summary>
        /// <returns></returns>
        public virtual bool IsReady() { return false; }

        /// <summary>
        /// 構造体を設定する
        /// </summary>
        /// <param name="s"></param>
        public virtual void SetStructure(CompiledStructure s) { }

        #endregion
    }
}