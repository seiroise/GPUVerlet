using Seiro.ParticleSystem2D.Core.RawDatas;
using Seiro.ParticleSystem2D.Core.RefDatas;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Seiro.ParticleSystem2D.Core.Compilers
{

	/// <summary>
	/// 構造体を実際に使用できる状態に仕上げる
	/// </summary>
	public static class StructureCompiler
	{

		#region 外部インタフェース

		/// <summary>
		/// 参照からなる構造体を実際に使用する構造体に変換する
		/// </summary>
		/// <param name="structure"></param>
		/// <param name="materialDict"></param>
		public static CompiledStructure Compile(RefStructure structure, MaterialDictionary materialDict)
		{
			// パーティクルのuidとindexの辞書を作成
			var refParticles = structure.GetParticles();
			var refEdges = structure.GetEdges();

			// 描画用マテリアルの順番を考慮する
			var matDict = materialDict.ExportDictionry();
			var particleMaterialList = CreateParticleMaterialList(refParticles, matDict);
			var edgeMaterialList = CreateEdgeMaterialList(refEdges, matDict);

			// パーティクルとエッジを使用マテリアルの順番に整列する
			var materialOrderRefParticles = AlignParticlesWithMaterialOrder(refParticles, particleMaterialList);
			var materialOrderRefEdges = AlignEdgesWithMaterialOrder(refEdges, edgeMaterialList);

			// 各種データを変換
			var indexDict = CreateUID2IndexDictionary(materialOrderRefParticles);
			var particles = CompileParticles(materialOrderRefParticles);
			var edges = CompileEdges(materialOrderRefEdges, materialOrderRefParticles, indexDict);

			// マテリアルとそれぞれのオフセットを配列に
			var particleMaterials = particleMaterialList.Select(x => x.Key).ToArray();
			var edgeMaterials = edgeMaterialList.Select(x => x.Key).ToArray();

			var particleOffsets = CreateMaterialOffsets(particleMaterialList);
			var edgeOffsets = CreateMaterialOffsets(edgeMaterialList);

			// それぞれのマテリアル毎のパーティクルとエッジの数を配列に
			var particleCounts = CalcCountEachMaterials(particleMaterialList);
			var edgeCounts = CalcCountEachMaterials(edgeMaterialList);

			// インスタンスを生成してデータを設定
			var compiled = UnityEngine.ScriptableObject.CreateInstance<CompiledStructure>();
			compiled.SetDatas(particles, edges, particleMaterials, edgeMaterials, particleOffsets, edgeOffsets, particleCounts, edgeCounts);

			return compiled;
		}

		#endregion

		#region 内部処理

		/// <summary>
		/// パーティクルを一列に配置したときのuidとindexの辞書を作成する
		/// </summary>
		/// <param name="particles"></param>
		/// <returns></returns>
		static Dictionary<uint, int> CreateUID2IndexDictionary(RefParticle[] particles)
		{
			var dict = new Dictionary<uint, int>();

			for (var i = 0; i < particles.Length; ++i)
			{
				dict.Add(particles[i].uid, i);
			}

			return dict;
		}

		/// <summary>
		/// マテリアルとパーティクル番号のリストを作成する
		/// </summary>
		/// <param name="particles"></param>
		/// <returns></returns>
		static List<KeyValuePair<UnityEngine.Material, List<int>>> CreateParticleMaterialList(RefParticle[] particles, Dictionary<string, UnityEngine.Material> matDict)
		{
			var dict = new Dictionary<UnityEngine.Material, List<int>>();

			for (var i = 0; i < particles.Length; ++i)
			{
				var t = particles[i];
				var material = matDict[t.materialID];

				if (!dict.ContainsKey(material))
				{
					dict.Add(material, new List<int>());
				}

				dict[material].Add(i);
			}

			var dst = new List<KeyValuePair<UnityEngine.Material, List<int>>>(dict.Select(x => x));

			return dst;
		}

		/// <summary>
		/// マテリアルとパーティクル番号のリストを作成する
		/// </summary>
		/// <param name="edges"></param>
		/// <returns></returns>
		static List<KeyValuePair<UnityEngine.Material, List<int>>> CreateEdgeMaterialList(RefEdge[] edges, Dictionary<string, UnityEngine.Material> matDict)
		{
			var dict = new Dictionary<UnityEngine.Material, List<int>>();

			for (var i = 0; i < edges.Length; ++i)
			{
				var t = edges[i];
				var material = matDict[t.materialID];

				if (!dict.ContainsKey(material))
				{
					dict.Add(material, new List<int>());
				}

				dict[material].Add(i);
			}

			var dst = new List<KeyValuePair<UnityEngine.Material, List<int>>>(dict.Select(x => x));

			return dst;
		}

		/// <summary>
		/// マテリアル順でパーティクルを整列する
		/// </summary>
		/// <param name="src"></param>
		/// <param name="matIndexList"></param>
		/// <returns></returns>
		static RefParticle[] AlignParticlesWithMaterialOrder(RefParticle[] src, List<KeyValuePair<UnityEngine.Material, List<int>>> matIndexList)
		{
			var dst = new List<RefParticle>();

			for (var i = 0; i < matIndexList.Count; ++i)
			{
				var t = matIndexList[i];
				for (var j = 0; j < t.Value.Count; ++j)
				{
					dst.Add(src[t.Value[j]]);
				}
			}

			return dst.ToArray();
		}

		/// <summary>
		/// マテリアル順でエッジを整列する
		/// </summary>
		/// <param name="src"></param>
		/// <param name="matIndexList"></param>
		/// <returns></returns>
		static RefEdge[] AlignEdgesWithMaterialOrder(RefEdge[] src, List<KeyValuePair<UnityEngine.Material, List<int>>> matIndexList)
		{
			var dst = new List<RefEdge>();

			for (var i = 0; i < matIndexList.Count; ++i)
			{
				var t = matIndexList[i];
				for (var j = 0; j < t.Value.Count; ++j)
				{
					dst.Add(src[t.Value[j]]);
				}
			}

			return dst.ToArray();
		}

		/// <summary>
		/// パーティクルを一列に整列する
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		static Particle[] CompileParticles(RefParticle[] src)
		{
			var dst = new Particle[src.Length];

			for (var i = 0; i < src.Length; ++i)
			{
				var t = src[i];
				dst[i] = new Particle(t.position, t.size, t.color);
			}

			return dst;
		}

		/// <summary>
		/// エッジのパーティクル参照先をuidからindexに変換する
		/// </summary>
		/// <param name="src"></param>
		/// <param name="indexDict"></param>
		/// <returns></returns>
		static Edge[] CompileEdges(RefEdge[] src, RefParticle[] particles, Dictionary<uint, int> indexDict)
		{
			var dst = new Edge[src.Length];

			for (var i = 0; i < src.Length; ++i)
			{
				var t = src[i];
				var aID = indexDict[t.aUID];
				var bID = indexDict[t.bUID];
				var a = particles[aID];
				var b = particles[bID];
				var e = new Edge(aID, bID, (b.position - a.position).magnitude, t.width, t.color);

				dst[i] = e;
			}

			return dst;
		}

		/// <summary>
		/// マテリアルのオフセット配列を作成する
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		static int[] CreateMaterialOffsets(List<KeyValuePair<UnityEngine.Material, List<int>>> src)
		{
			var dst = new int[src.Count];
			var sum = 0;
			for (var i = 1; i < src.Count; ++i)
			{
				sum += src[i-1].Value.Count;
				dst[i] = sum;
			}

			return dst;
		}

		/// <summary>
		/// マテリアルごとの要素数を数え上げる
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		static uint[] CalcCountEachMaterials(List<KeyValuePair<UnityEngine.Material, List<int>>> src)
		{
			var dst = new uint[src.Count];
			for (var i = 0; i < src.Count; ++i)
			{
				dst[i] = (uint)src[i].Value.Count;
			}

			return dst;
		}

		#endregion
	}
}