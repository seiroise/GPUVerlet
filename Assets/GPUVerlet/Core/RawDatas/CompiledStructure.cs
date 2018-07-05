using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.RawDatas
{

    /// <summary>
    /// コンパイルされた構造体
    /// </summary>
    public sealed class CompiledStructure
    {

		public Particle[] particles;
		public Edge[] edges;

		public Material[] particleMaterials;
		public Material[] edgeMaterials;

		public int[] particleMaterialOffsets;
		public int[] edgeMaterialOffsets;

		public uint[] particleCounts;
		public uint[] edgeCounts;

		public CompiledStructure(
			Particle[] particles, Edge[] edges,
			Material[] particleMaterials, Material[] edgeMaterials,
			int[] particleMaterialOffsets, int[] edgeMaterialOffsets,
			uint[] particleCounts, uint[] edgeCounts
		) {
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