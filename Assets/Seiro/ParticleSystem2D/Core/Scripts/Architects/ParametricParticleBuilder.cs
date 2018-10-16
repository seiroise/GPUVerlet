using Seiro.ParticleSystem2D.Common;
using Seiro.ParticleSystem2D.Core.RefDatas;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.Architects
{

    /// <summary>
    /// パラメータ依存なパーティクルビルダー
    /// </summary>
	[CreateAssetMenu(menuName = "Seiro/GPUVerlet/ParametricParticleBuilder", fileName = "ParametricParticleBuilder")]
    public class ParametricParticleBuilder : BaseParametricParticleBuilder
    {

        [SerializeField, MinMax(0f, 10f)]
        MinMax _size;

        [SerializeField]
        Gradient _color;

        [SerializeField]
        string _materialID = "Particle";

        public override RefParticle Add(RefStructure s, Vector2 p, float t)
        {
            return s.AddParticle(p, _size.Lerp(t), _color.Evaluate(t), _materialID);
        }
    }
}