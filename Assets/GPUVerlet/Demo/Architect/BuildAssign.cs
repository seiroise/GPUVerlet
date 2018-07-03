using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// 建築士の作った構造体をシミュレータに割り当てる
    /// </summary>
	[RequireComponent(typeof(VSimulator))]
    public class BuildAssign : MonoBehaviour
    {

        /// <summary>
        /// アーキテクト
        /// </summary>
        public BaseArchitect architect;

        void Start()
        {
            if (architect)
            {
                GetComponent<VSimulator>().SetStructure(architect.CreateStructure(transform.localToWorldMatrix));
            }
        }
    }
}