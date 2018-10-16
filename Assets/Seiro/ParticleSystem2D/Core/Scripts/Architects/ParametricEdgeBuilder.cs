using Seiro.ParticleSystem2D.Common;
using Seiro.ParticleSystem2D.Core.RefDatas;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.Architects
{

    /// <summary>
    /// パラメータ依存なパーティクルビルダー
    /// </summary>
	[CreateAssetMenu(menuName = "Seiro/GPUVerlet/ParametricEdgeBuilder", fileName = "ParametricEdgeBuilder")]
    public class ParametricEdgeBuilder : BaseParametricEdgeBuilder
    {

        [SerializeField, MinMax(0f, 10f)]
        MinMax _width;

        [SerializeField]
        Gradient _color;

        [SerializeField]
        string _materialID = "Edge";

        public override RefEdge Add(RefStructure s, int aIdx, int bIdx, float t)
        {
            return s.AddEdge(aIdx, bIdx, _width.Lerp(t), _color.Evaluate(t), _materialID);
        }

        public override RefEdge Add(RefStructure s, RefParticle a, RefParticle b, float t)
        {
            return s.AddEdge(a, b, _width.Lerp(t), _color.Evaluate(t), _materialID);
        }
    }
}