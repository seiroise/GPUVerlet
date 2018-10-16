using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Demo
{

    /// <summary>
    /// 円形の構造物の建築士
    /// </summary>
	// [CreateAssetMenu(fileName = "CircleArchitect", menuName = "Seiro/GPUVerlet/CircleArchitect")]
    public class CircleArchitect : BaseArchitect
    {

        /// <summary>
        /// 円の半径
        /// </summary>
        [Range(0.1f, 50f)]
        public float radius = 5f;

        /// <summary>
        /// 円周の解像度
        /// </summary>
        [Range(5, 1024)]
        public int resolution = 60;

        /// <summary>
        /// 突っ張りの設定
        /// </summary>
        public int[] supports;

        public override VStructure CreateStructure(Matrix4x4 toWorld)
        {
            var s = new VStructure();

            var stepSize = Mathf.PI * 2f / resolution;

            // パーティクル
            for (var i = 0; i < resolution; ++i)
            {
                var step = stepSize * i;
                var x = Mathf.Cos(step) * radius;
                var y = Mathf.Sin(step) * radius;

                s.AddParticle(toWorld.MultiplyPoint3x4(new Vector2(x, y)), 0.2f);
            }

            // エッジ
            for (var i = 0; i < resolution; ++i)
            {
                // 前後のつなぎ
                s.AddEdge(i, (i + 1) % resolution);

                // 支持棒
                for (var j = 0; j < supports.Length; ++j)
                {
                    var i2 = (i + supports[j]) % resolution;
                    s.AddEdge(i, i2);
                }
            }

            return s;
        }
    }
}