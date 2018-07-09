using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Seiro.GPUVerlet.Core.RefDatas
{

    /// <summary>
    /// 参照の構造体
    /// </summary>
    [CreateAssetMenu(menuName = "Seiro/GPUVerlet/RefStructure", fileName = "RefStructure")]
    public partial class RefStructure : ScriptableObject
    {

        [SerializeField, HideInInspector]
        List<RefParticle> _particles = null;

        [SerializeField, HideInInspector]
        List<RefEdge> _edges = null;

        [SerializeField, HideInInspector]
        uint _uidCounter = 0;

        #region static 外部インタフェース

        public static RefStructure CreateNew()
        {
            var s = CreateInstance<RefStructure>();

            s._particles = new List<RefParticle>();
            s._edges = new List<RefEdge>();

            return s;
        }

        #endregion

        #region 外部インタフェース

        /// <summary>
        /// パーティクルを追加する
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="materialID"></param>
        /// <returns></returns>
        public RefParticle AddParticle(Vector2 position, float size, Color color, string materialID)
        {
            var p = new RefParticle(GetNextUID(), position, size, color, materialID);
            _particles.Add(p);
            return p;
        }

        /// <summary>
        /// 指定したuidに紐付いているパーティクルを削除する
        /// </summary>
        /// <param name="uid"></param>
        public void RemoveParticle(RefParticle p)
        {

            if (p == null)
            {
                return;
            }

            // 紐付いているエッジを削除する
            var result = FindEdgesWithParticle(p.uid);
            for (var i = 0; i < result.Count; ++i)
            {
                _edges.Remove(result[i]);
            }
        }

        /// <summary>
        /// パーティクル参照を元にエッジを追加する
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="width"></param>
        /// <param name="color"></param>
        /// <param name="materialID"></param>
        /// <returns></returns>
        public RefEdge AddEdge(RefParticle a, RefParticle b, float width, Color color, string materialID)
        {
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);

            var e = new RefEdge(GetNextUID(), a.uid, b.uid, width, color, materialID);
            _edges.Add(e);
            return e;
        }

        /// <summary>
        /// パーティクル番号を元にエッジを追加する
        /// </summary>
        /// <param name="aIdx"></param>
        /// <param name="bIdx"></param>
        /// <param name="width"></param>
        /// <param name="color"></param>
        /// <param name="materialID"></param>
        /// <returns></returns>
        public RefEdge AddEdge(int aIdx, int bIdx, float width, Color color, string materialID)
        {
            var e = new RefEdge(GetNextUID(), GetParticleAt(aIdx).uid, GetParticleAt(bIdx).uid, width, color, materialID);
            _edges.Add(e);
            return e;
        }

        /// <summary>
        /// 指定したUIDに紐付いているエッジを削除する
        /// </summary>
        /// <param name="uid"></param>
        public void RemoveEdge(RefEdge edge)
        {
            _edges.Remove(edge);
        }

        /// <summary>
        /// パーティクル全体の座標変換を行う
        /// </summary>
        /// <param name="translation"></param>
        public void TranslateParticles(Matrix4x4 translation)
        {
            for (var i = 0; i < _particles.Count; ++i)
            {
                var t = _particles[i];
                var pos = translation.MultiplyPoint3x4(t.position);
                t.position = pos;
            }
        }

        /// <summary>
        /// パーティクルの配列を返す
        /// </summary>
        /// <returns></returns>
        public RefParticle[] GetParticles()
        {
            return _particles.ToArray();
        }

        /// <summary>
        /// エッジの配列を返す
        /// </summary>
        /// <returns></returns>
        public RefEdge[] GetEdges()
        {
            return _edges.ToArray();
        }

        /// <summary>
        /// 使用しているMaterialIDの配列を返す
        /// </summary>
        /// <returns></returns>
        public string[] GetUsingMaterialIDs()
        {
            var p = GetParticleUsingMaterialIDs();
            var e = GetEdgeUsingMaterialIDs();

            var mergedArray = new string[p.Length + e.Length];
            p.CopyTo(mergedArray, 0);
            e.CopyTo(mergedArray, p.Length);

            return mergedArray;
        }

        /// <summary>
        /// パーティクルで使用するMaterialIDの配列を返す
        /// </summary>
        /// <returns></returns>
        public string[] GetParticleUsingMaterialIDs()
        {
            var dst = new HashSet<string>();
            for (var i = 0; i < _particles.Count; ++i)
            {
                dst.Add(_particles[i].materialID);
            }
            return dst.ToArray();
        }

        /// <summary>
        /// エッジで使用するMaterialIDの配列を返す
        /// </summary>
        /// <returns></returns>
        public string[] GetEdgeUsingMaterialIDs()
        {
            var dst = new HashSet<string>();
            for (var i = 0; i < _edges.Count; ++i)
            {
                dst.Add(_edges[i].materialID);
            }
            return dst.ToArray();
        }

        /// <summary>
        /// Boundsを計算する、particleのsizeを考慮する
        /// </summary>
        /// <returns></returns>
        public Bounds CalcurateBounds()
        {
            if (_particles.Count == 0)
            {
                return default(Bounds);
            }

            var first = _particles[0];
            if (_particles.Count == 1)
            {
                return new Bounds(first.position, new Vector3(first.size, first.size));
            }

            Vector3 min, max;
            min = max = first.position;

            for (var i = 0; i < _particles.Count; ++i)
            {
                var t = _particles[i].position;
                var hs = _particles[i].size * 0.5f;

                if (min.x > t.x - hs) min.x = t.x - hs;
                if (min.y > t.y - hs) min.y = t.y - hs;
                if (max.x < t.x + hs) max.x = t.x + hs;
                if (max.y < t.y + hs) max.y = t.y + hs;
            }

            return new Bounds((min + max) * 0.5f, max - min);
        }

        #endregion

        #region 内部処理

        /// <summary>
        /// UIDからパーティクルを検索し、返す
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        RefParticle FindParticleFromUID(uint uid)
        {
            for (var i = 0; i < _particles.Count; ++i)
            {
                if (_particles[i].uid == uid)
                {
                    return _particles[i];
                }
            }
            return null;
        }

        /// <summary>
        /// パーティクル番号からパーティクルを返す
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        RefParticle GetParticleAt(int idx)
        {
            Assert.IsTrue(ContainsParticlesIdx(idx));

            return _particles[idx];
        }

        /// <summary>
        /// 指定したUIDに紐付いているパーティクルを含むエッジを全て探索する
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        List<RefEdge> FindEdgesWithParticle(uint uid)
        {
            var dst = new List<RefEdge>();

            for (var i = 0; i < _edges.Count; ++i)
            {
                if (_edges[i].ContainsParticle(uid))
                {
                    dst.Add(_edges[i]);
                }
            }

            return dst;
        }

        /// <summary>
        /// パーティクルインデックスのはみ出し確認
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        bool ContainsParticlesIdx(int idx)
        {
            return 0 <= idx && idx < _particles.Count;
        }

        /// <summary>
        /// 新しいUIGを取得する
        /// </summary>
        /// <returns></returns>
        uint GetNextUID()
        {
            return _uidCounter++;
        }

        #endregion
    }
}