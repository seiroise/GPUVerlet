using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Seiro.GPUVerlet.Core.RefDatas
{

    /// <summary>
    /// 参照の構造体
    /// </summary>
    [CreateAssetMenu(menuName = "Seiro/GPUVerlet/RefStructure", fileName = "RefStructure")]
    public class RefStructure : ScriptableObject
    {

        List<RefParticle> _particles;
        List<RefEdge> _edges;
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
        /// パーティクル全体の座標変換を行う
        /// </summary>
        /// <param name="translate"></param>
        public void TranslateParticles(Matrix4x4 translate)
        {
            for (var i = 0; i < _particles.Count; ++i)
            {
                var t = _particles[i];
                var pos = translate.MultiplyPoint3x4(t.position);
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