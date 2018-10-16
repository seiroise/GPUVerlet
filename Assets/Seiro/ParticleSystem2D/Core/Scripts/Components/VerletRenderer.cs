using Seiro.ParticleSystem2D.Core.RawDatas;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.Components
{

	/// <summary>
	/// Verletをレンダリングする
	/// </summary>
	public class VerletRenderer : BaseVerletComponent
	{

		/// <summary>
		/// インスタンシング用のメッシュ
		/// </summary>
		[SerializeField]
		Mesh _instancedMesh = null;

		/// <summary>
		/// 描画範囲
		/// </summary>
		[SerializeField]
		Bounds _renderingBounds;

		// 生データ
		RawDatas.Particle[] _particles = null;
		Edge[] _edges;

		// パーティクルのレンダリング用設定
		Material[] _particleMaterials;
		int[] _particleOffsets;
		uint[] _particleCounts;

		// エッジのレンダリング用設定
		Material[] _edgeMaterials;
		int[] _edgeOffsets;
		uint[] _edgeCounts;

		// インスタンシング用
		uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };
		ComputeBuffer[] _particleArgsBuffers;               // 描画時にどちらとも必要なのでバッファは二つ必要
		ComputeBuffer[] _edgeArgsBuffers;                   // 上記の理由で必要
		ComputeBuffer _particleBuffer;
		ComputeBuffer _edgeBuffer;

		// それぞれのマテリアルプロパティブロック
		MaterialPropertyBlock[] _particlePropBlocks;
		MaterialPropertyBlock[] _edgePropBlocks;

		#region MonoBehaviourイベント

		private void Awake()
		{
			ReleaseAllBuffers();
		}

		private void OnDestroy()
		{
			ReleaseAllBuffers();
		}

		#endregion

		#region 外部インタフェース

		/// <summary>
		/// 準備できているか
		/// </summary>
		/// <returns></returns>
		public override bool IsReady()
		{
			return _particles != null && _edges != null && (_particleBuffer != null || _edgeBuffer != null);
		}

		/// <summary>
		/// 構造体を設定する
		/// </summary>
		/// <param name="s"></param>
		public override void SetStructure(CompiledStructure s)
		{
			_particles = s.particles;
			_particleMaterials = s.particleMaterials;
			_particleOffsets = s.particleMaterialOffsets;
			_particleCounts = s.particleCounts;

			_edges = s.edges;
			_edgeMaterials = s.edgeMaterials;
			_edgeOffsets = s.edgeMaterialOffsets;
			_edgeCounts = s.edgeCounts;

			BindEmptyBuffer();
		}

		/// <summary>
		/// 描画する
		/// </summary>
		public void Render()
		{
			RenderInstancedMesh();
		}

		/// <summary>
		/// バッファを解放する
		/// </summary>
		public void ReleaseAllBuffers()
		{
			if (_particleBuffer != null)
			{
				_particleBuffer.Release();
				_particleBuffer = null;
			}
			if (_edgeBuffer != null)
			{
				_edgeBuffer.Release();
				_edgeBuffer = null;
			}
			if (_particleArgsBuffers != null)
			{
				ReleaseBufferArray(_particleArgsBuffers);
				_particleArgsBuffers = null;
			}
			if (_edgeArgsBuffers != null)
			{
				ReleaseBufferArray(_edgeArgsBuffers);
				_edgeArgsBuffers = null;
			}
		}

		#endregion

		#region 内部処理

		/// <summary>
		/// 空バッファをバインドする
		/// </summary>
		void BindEmptyBuffer()
		{
			ReleaseAllBuffers();

			if (_particles.Length > 0)
			{
				_particleBuffer = new ComputeBuffer(_particles.Length, Marshal.SizeOf(typeof(RawDatas.Particle)));
				_particleArgsBuffers = BindArgsBufferArray(_particleCounts);
				_particlePropBlocks = CreateMaterialPropertyBlocks(_particleCounts.Length);
			}

			if (_edges.Length > 0)
			{
				_edgeBuffer = new ComputeBuffer(_edges.Length, Marshal.SizeOf(typeof(Edge)));
				_edgeArgsBuffers = BindArgsBufferArray(_edgeCounts);
				_edgePropBlocks = CreateMaterialPropertyBlocks(_edgeCounts.Length);
			}
		}

		/// <summary>
		/// 引数バッファ配列を確保する
		/// </summary>
		/// <param name="counts"></param>
		/// <returns></returns>
		ComputeBuffer[] BindArgsBufferArray(uint[] counts)
		{
			var buffers = new ComputeBuffer[counts.Length];

			_args[0] = _instancedMesh.GetIndexCount(0);
			for (var i = 0; i < counts.Length; ++i)
			{
				_args[1] = counts[i];
				buffers[i] = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
				buffers[i].SetData(_args);
			}
			return buffers;
		}

		/// <summary>
		/// MaterialPropertyBlockの配列を作成する
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		MaterialPropertyBlock[] CreateMaterialPropertyBlocks(int count)
		{
			var blocks = new MaterialPropertyBlock[count];
			for (var i = 0; i < count; ++i)
			{
				blocks[i] = new MaterialPropertyBlock();
			}
			return blocks;
		}

		/// <summary>
		/// バッファの配列を解放する
		/// </summary>
		/// <param name="buffers"></param>
		void ReleaseBufferArray(ComputeBuffer[] buffers)
		{
			for (var i = 0; i < buffers.Length; ++i)
			{
				buffers[i].Release();
				buffers[i] = null;
			}
		}

		/// <summary>
		/// インスタンシングを利用してメッシュを描画する
		/// </summary>
		void RenderInstancedMesh()
		{

			if (_instancedMesh == null)
			{
				return;
			}

			// パーティクルの描画
			if (_particleBuffer != null)
			{
				_particleBuffer.SetData(_particles);
				for (var i = 0; i < _particleMaterials.Length; ++i)
				{
					_particlePropBlocks[i].SetBuffer("_ParticleDataBuffer", _particleBuffer);
					_particlePropBlocks[i].SetFloat("_Offset", _particleOffsets[i]);
					Graphics.DrawMeshInstancedIndirect(_instancedMesh, 0, _particleMaterials[i], _renderingBounds, _particleArgsBuffers[i], 0, _particlePropBlocks[i]);
				}
			}

			// エッジの描画
			if (_edgeBuffer != null)
			{
				_edgeBuffer.SetData(_edges);
				for (var i = 0; i < _edgeMaterials.Length; ++i)
				{
					_edgePropBlocks[i].SetBuffer("_ParticleDataBuffer", _particleBuffer);
					_edgePropBlocks[i].SetBuffer("_EdgeDataBuffer", _edgeBuffer);
					_edgePropBlocks[i].SetFloat("_Offset", _edgeOffsets[i]);
					Graphics.DrawMeshInstancedIndirect(_instancedMesh, 0, _edgeMaterials[i], _renderingBounds, _edgeArgsBuffers[i], 0, _edgePropBlocks[i]);
				}
			}
		}

		#endregion
	}
}