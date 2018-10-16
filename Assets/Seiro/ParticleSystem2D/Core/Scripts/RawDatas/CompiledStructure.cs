using System.Collections.Generic;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.RawDatas
{

    /// <summary>
    /// コンパイルされた構造体
    /// </summary>
	[System.Serializable]
    public sealed class CompiledStructure : ScriptableObject
    {

        public Particle[] particles;
        public Edge[] edges;

        public Material[] particleMaterials;
        public Material[] edgeMaterials;

        public int[] particleMaterialOffsets;
        public int[] edgeMaterialOffsets;

        public uint[] particleCounts;
        public uint[] edgeCounts;

        /// <summary>
        /// それぞれのデータを設定する
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="edges"></param>
        /// <param name="particleMaterials"></param>
        /// <param name="edgeMaterials"></param>
        /// <param name="particleMaterialOffsets"></param>
        /// <param name="edgeMaterialOffsets"></param>
        /// <param name="particleCounts"></param>
        /// <param name="edgeCounts"></param>
        public void SetDatas(
            Particle[] particles, Edge[] edges,
            Material[] particleMaterials, Material[] edgeMaterials,
            int[] particleMaterialOffsets, int[] edgeMaterialOffsets,
            uint[] particleCounts, uint[] edgeCounts
        )
        {
            this.particles = particles;
            this.edges = edges;
            this.particleMaterials = particleMaterials;
            this.edgeMaterials = edgeMaterials;
            this.particleMaterialOffsets = particleMaterialOffsets;
            this.edgeMaterialOffsets = edgeMaterialOffsets;
            this.particleCounts = particleCounts;
            this.edgeCounts = edgeCounts;
        }
    }
}