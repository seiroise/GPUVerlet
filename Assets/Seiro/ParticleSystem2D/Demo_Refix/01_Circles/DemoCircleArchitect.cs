using System.Collections;
using System.Collections.Generic;
using Seiro.ParticleSystem2D.Common;
using Seiro.ParticleSystem2D.Core.Architects;
using Seiro.ParticleSystem2D.Core.RefDatas;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Demo
{

    /// <summary>
    /// 円形の設計士
    /// </summary>
    [CreateAssetMenu(fileName = "DemoCircleArchitect", menuName = "Seiro/GPUVerlet/DemoCircleArchitect")]
    public class DemoCircleArchitect : Core.Architects.BaseArchitect
    {

        [Header("基本設定")]

        /// <summary>
        /// 円の半径
        /// </summary>
        [SerializeField, Range(0.01f, 50f)]
        float _radius = 3f;

        /// <summary>
        /// 円周の解像度
        /// </summary>
        [SerializeField, Range(5, 1024)]
        int _resolution = 60;

        [Header("パーティクル設定")]

        /// <summary>
        ///  パーティクルを作る人
        /// </summary>
        [SerializeField, FlatRef(offset = 8)]
        BaseParticleBuilder _particleBuilder;

        [Header("エッジ設定")]

        /// <summary>
        /// エッジを作る人
        /// </summary>
        [SerializeField, FlatRef(offset = 8)]
        BaseEdgeBuilder _edgeBuilder;

        [Header("サポートエッジ設定")]

        /// <summary>
        /// サポートエッジを作る人
        /// </summary>
        [SerializeField, FlatRef(offset = 8)]
        BaseEdgeBuilder _supportEdgeBuilder;

        /// <summary>
        /// サポートエッジの間隔
        /// </summary>
        [SerializeField]
        int[] _supportIntervals;

        public override RefStructure Build(Matrix4x4 toWorld)
        {

            var s = RefStructure.CreateNew();

            var stepSize = Mathf.PI * 2f / _resolution;

            //パーティクルを配置
            var p = new RefParticle[_resolution];
            for (var i = 0; i < _resolution; ++i)
            {
                var t = stepSize * i;
                var x = Mathf.Cos(t) * _radius;
                var y = Mathf.Sin(t) * _radius;

                p[i] = _particleBuilder.Add(s, new Vector2(x, y));
            }

            // エッジを配置
            for (var i = 0; i < _resolution; ++i)
            {
                _edgeBuilder.Add(s, p[i], p[(i + 1) % _resolution]);

                for (var j = 0; j < _supportIntervals.Length; ++j)
                {
                    _supportEdgeBuilder.Add(s, p[i], p[(i + _supportIntervals[j]) % _resolution]);
                }
            }

            s.TranslateParticles(toWorld);

            return s;
        }
    }
}