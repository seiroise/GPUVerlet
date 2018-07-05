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

		public CompiledStructure(
			Particle[] particles, Edge[] edges,
			Material[] particleMaterials, Material[] edgeMaterials,
			int[] particleMaterialOffsets, int[] edgeMaterialOffsets
		) {
			this.particles = particles;
			this.edges = edges;
			this.particleMaterials = particleMaterials;
			this.edgeMaterials = edgeMaterials;
			this.particleMaterialOffsets = particleMaterialOffsets;
			this.edgeMaterialOffsets = edgeMaterialOffsets;
		}
    }
}