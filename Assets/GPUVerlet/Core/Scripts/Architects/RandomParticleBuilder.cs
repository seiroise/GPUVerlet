using Seiro.GPUVerlet.Common;
using Seiro.GPUVerlet.Core.RefDatas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Architects
{

    /// <summary>
    /// パラメータをランダムに決めてパーティクルを作る
    /// </summary>
    [CreateAssetMenu(menuName = "Seiro/GPUVerlet/RandomParticleBuilder", fileName = "RandomParticleBuilder")]
    public class RandomParticleBuilder : BaseParticleBuilder
    {

        [SerializeField, MinMax(0f, 10f)]
        MinMax _size;

        [SerializeField]
        Gradient _color;

        [SerializeField]
        string _materialID = "Particle";

        public override RefParticle Add(RefStructure s, Vector2 p)
        {
            return s.AddParticle(p, _size.random, _color.E_Random(), _materialID);
        }
    }
}