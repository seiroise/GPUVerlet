using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet
{

    /// <summary>
    /// 構造をシリアライズして保持する
    /// </summary>
    public class VStructureObject : ScriptableObject
    {

        /// <summary>
        /// 構造
        /// </summary>
        [SerializeField]
        VStructure _structure;

        #region static外部インタフェース

        /// <summary>
        /// 構造を指定した新しく作成する。
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public static VStructureObject CreateNew(VStructure structure)
        {
            var n = ScriptableObject.CreateInstance<VStructureObject>();
            n._structure = structure;
            return n;
        }

        #endregion

        #region 外部インタフェース

        /// <summary>
        /// Gizmos上にプレビューを表示する
        /// </summary>
        public void DrawPreviewOnGizmos(Matrix4x4 toWorld)
        {
            if (_structure == null)
            {
                return;
            }

            var particles = _structure.GetParticles();
            for (var i = 0; i < particles.Length; ++i)
            {
                var t = particles[i];
                Gizmos.color = t.color;
                Gizmos.DrawWireSphere(toWorld.MultiplyPoint3x4(t.position), t.size * 0.5f);
            }

            var edges = _structure.GetEdges();
            for (var i = 0; i < edges.Length; ++i)
            {
                var t = edges[i];
                Gizmos.color = t.color;
                Gizmos.DrawLine(
                    toWorld.MultiplyPoint3x4(_structure.GetParticlePosition(t.a)),
                    toWorld.MultiplyPoint3x4(_structure.GetParticlePosition(t.b))
                );
            }
        }

        #endregion
    }
}