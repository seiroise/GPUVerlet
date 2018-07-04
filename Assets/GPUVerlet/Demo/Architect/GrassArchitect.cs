using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// 比較的背の低い草みたいな感じ
    /// </summary>
	[CreateAssetMenu(menuName = "Seiro/GPUVerlet/GrassArchitect", fileName = "GrassArchitect")]
    public class GrassArchitect : BaseArchitect
    {

        [Header("葉の設定")]

        /// <summary>
        /// 葉の大きさ
        /// </summary>
        [SerializeField]
        Vector2 _leafSize;

        /// <summary>
        /// 葉の部分のパーティクルの大きさ
        /// </summary>
        [SerializeField]
        float _leafParticleSize = 0.1f;

        /// <summary>
        /// 葉の部分のパーティクルの色
        /// </summary>
        [SerializeField]
        Gradient _leafParticleGradient;

        /// <summary>
        /// 葉の部分のエッジの幅
        /// </summary>
        [SerializeField]
        float _leafEdgeWidth = 0.1f;

        /// <summary>
        /// 葉の部分のエッジの色
        /// </summary>
        [SerializeField]
        Gradient _leafEdgeGradient;

        [Header("地面とのつなぎの設定")]

        /// <summary>
        /// 地面との接続部分のエッジの幅
        /// </summary>
        [SerializeField]
        float _groundEdgeWidth = 0.05f;

        /// <summary>
        /// 地面との接続部分のエッジの色
        /// </summary>
        [SerializeField]
        Gradient _groundEdgeGradient;

        #region 外部インタフェース

        public override VStructure CreateStructure(Matrix4x4 toWorld)
        {
            var s = new VStructure();

            CreatePart(s);

            s.TranslateParticles(toWorld);

            return s;
        }

        #endregion

        #region 内部処理

        /// <summary>
        /// 一部を作成する
        /// </summary>
        /// <param name="s"></param>
        /// <param name="toWorld"></param>
        /// <param name="x"></param>
        void CreatePart(VStructure s)
        {
            // 葉の作成
            Vector2[] leafPositions = new Vector2[4];

            // 座標配列を作成
            leafPositions[0] = new Vector2(0f, 0f);
            leafPositions[1] = new Vector2(_leafSize.x * 0.5f, _leafSize.y * 0.5f);
            leafPositions[2] = new Vector2(0f, _leafSize.y);
            leafPositions[3] = new Vector2(-_leafSize.x * 0.5f, _leafSize.y * 0.5f);

            s.AddParticle(leafPositions[0], _leafParticleSize, _leafParticleGradient.Evaluate(Random.Range(0f, 1f)));
            s.AddParticle(leafPositions[1], _leafParticleSize, _leafParticleGradient.Evaluate(Random.Range(0f, 1f)));
            s.AddParticle(leafPositions[2], _leafParticleSize, _leafParticleGradient.Evaluate(Random.Range(0f, 1f)));
            s.AddParticle(leafPositions[3], _leafParticleSize, _leafParticleGradient.Evaluate(Random.Range(0f, 1f)));

            s.AddEdge(0, 1, _leafEdgeWidth, _leafEdgeGradient.Evaluate(Random.Range(0f, 1f)));
            s.AddEdge(1, 2, _leafEdgeWidth, _leafEdgeGradient.Evaluate(Random.Range(0f, 1f)));
            s.AddEdge(2, 3, _leafEdgeWidth, _leafEdgeGradient.Evaluate(Random.Range(0f, 1f)));
            s.AddEdge(3, 0, _leafEdgeWidth, _leafEdgeGradient.Evaluate(Random.Range(0f, 1f)));

            s.AddEdge(0, 2, _leafEdgeWidth, _leafEdgeGradient.Evaluate(Random.Range(0f, 1f)));
            s.AddEdge(1, 3, _leafEdgeWidth, _leafEdgeGradient.Evaluate(Random.Range(0f, 1f)));
        }

        #endregion
    }
}