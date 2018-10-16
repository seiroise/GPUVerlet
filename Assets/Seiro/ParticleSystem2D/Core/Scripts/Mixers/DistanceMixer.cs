using System.Collections.Generic;
using Seiro.ParticleSystem2D.Core.Architects;
using Seiro.ParticleSystem2D.Core.RefDatas;
using UnityEngine;
using UnityEngine.Assertions;

namespace Seiro.ParticleSystem2D.Core.Mixers
{

    /// <summary>
    /// 2つの構造体をつなげて、距離に応じて一つの構造体を作成する
    /// </summary>
    public static class DistanceMixer
    {

        #region 外部インタフェース

        /// <summary>
        /// 特定のパーティクルの距離によるミックスを行う。多分これが一番シンプルな方法何じゃないかな？
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dist"></param>
        /// <param name="edgeBuilder"></param>
        /// <returns></returns>
        public static RefStructure Mix(RefStructure a, RefStructure b, float dist, BaseEdgeBuilder edgeBuilder)
        {
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
            Assert.IsNotNull(edgeBuilder);

            var dst = RefStructure.CreateNew();

            // それぞれの構造体の中から一定の距離感にある。パーティクル同士をエッジでつなげる。

            var aParticles = a.GetParticles();
            var bParticles = b.GetParticles();

            var pairDict = new Dictionary<RefParticle, int[]>();

            for (var i = 0; i < aParticles.Length; ++i)
            {
                var t = aParticles[i];
                var neighbours = FindParticles(t, dist, bParticles);
                pairDict.Add(t, neighbours);
            }

            // とりあえずペアを見つけ出したので、つないでみる。
            // ここで注意なのがパーティクルがaとbのどちらに属しているのかということ。
            // それらを意識しながら、接続関係をまとめていく。

            var aCount = aParticles.Length;

            // 一旦すべてのパーティクルとエッジを追加する
            AddParticles(dst, aParticles);
            AddParticles(dst, bParticles);

            // エッジの追加にはパーティクルのUIDからIdxへのマッピング辞書が必要。
            var aEdges = a.GetEdges();
            var bEdges = b.GetEdges();

            var uid2IdxDictA = CreateUID2IdxDictionary(aParticles);
            var uid2IdxDictB = CreateUID2IdxDictionary(bParticles);

            // Debug.Log(uid2IdxDictA.Count);
            AddEdges(dst, aEdges, uid2IdxDictA, 0);
            AddEdges(dst, bEdges, uid2IdxDictB, aCount);

            // ここから新しい接続用のエッジを追加していく。

            foreach (var pair in pairDict)
            {
                if (pair.Value.Length == 0) continue;
                var idx = uid2IdxDictA[pair.Key.uid];

                for (var i = 0; i < pair.Value.Length; ++i)
                {
                    var tIdx = pair.Value[i] + aCount;
                    // 2つのインデックスが揃ったのでエッジを作る
                    edgeBuilder.Add(dst, idx, tIdx);
                }
            }

            return dst;
        }

        #endregion

        #region 内部処理

        /// <summary>
        /// 指定したパーティクルからある程度の距離にあるパーティクルの番号の配列を返す。
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="dist"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        static int[] FindParticles(RefParticle origin, float dist, RefParticle[] targets)
        {
            var dst = new List<int>();

            var position = origin.position;
            var minSqrDist = dist;

            for (var i = 0; i < targets.Length; ++i)
            {
                var sqrDist = (targets[i].position - position).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    dst.Add(i);
                }
            }

            return dst.ToArray();
        }

        /// <summary>
        /// 指定した構造体にパーティクルを追加する
        /// </summary>
        /// <param name="s"></param>
        /// <param name="targets"></param>
        static void AddParticles(RefStructure s, RefParticle[] targets)
        {
            Assert.IsNotNull(s);
            Assert.IsNotNull(targets);

            for (var i = 0; i < targets.Length; ++i)
            {
                var t = targets[i];
                s.AddParticle(t.position, t.size, t.color, t.materialID);
            }
        }

        /// <summary>
        /// 指定した構造体にエッジを追加する。エッジの追加には対応するUIDからIdxへのマッピング辞書が必要。
        /// </summary>
        /// <param name="s"></param>
        /// <param name="targets"></param>
        /// <param name="uid2IdxDict"></param>
        /// <param name="idxOffset"></param>
        static void AddEdges(RefStructure s, RefEdge[] targets, Dictionary<uint, int> uid2IdxDict, int idxOffset = 0)
        {
            Assert.IsNotNull(s);
            Assert.IsNotNull(targets);
            Assert.IsNotNull(uid2IdxDict);

            for (var i = 0; i < targets.Length; ++i)
            {
                var t = targets[i];
                var aIdx = uid2IdxDict[t.aUID] + idxOffset;
                var bIdx = uid2IdxDict[t.bUID] + idxOffset;
                s.AddEdge(aIdx, bIdx, t.width, t.color, t.materialID);
            }
        }

        /// <summary>
        /// パーティクルを一列に配置したときのuidとindexの辞書を作成する
        /// </summary>
        /// <param name="particles"></param>
        /// <returns></returns>
        static Dictionary<uint, int> CreateUID2IdxDictionary(RefParticle[] particles)
        {
            var dict = new Dictionary<uint, int>();

            for (var i = 0; i < particles.Length; ++i)
            {
                dict.Add(particles[i].uid, i);
            }

            return dict;
        }

        #endregion
    }
}