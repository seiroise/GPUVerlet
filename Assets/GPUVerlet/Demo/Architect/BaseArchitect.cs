using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{
    /// <summary>
    /// 設計士の基底
    /// </summary>
    public class BaseArchitect : ScriptableObject
    {

        #region 仮想インタフェース

        /// <summary>
        /// 何かしらの構造物を作成する
        /// </summary>
        /// <returns></returns>
        public virtual VStructure CreateStructure(Matrix4x4 toWorld) { return null; }

        #endregion
    }
}