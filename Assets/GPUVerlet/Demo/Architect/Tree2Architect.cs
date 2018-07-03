using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// 樹木の設計士その2
    /// </summary>
	[CreateAssetMenu(menuName = "Seiro/GPUVerlet/Tree2Architect", fileName = "Tree2Architect")]
    public class Tree2Architect : BaseArchitect
    {

        /// <summary>
        /// サポート用のエッジ設定
        /// </summary>
        [System.Serializable]
        public class SupportEdge
        {
            /// <summary>
            /// サポートの間隔
            /// </summary>
            [Range(1, 100)]
            public int supportInterval = 2;

            /// <summary>
            /// サポートの幅
            /// </summary>
            [Range(0f, 1f)]
            public float supportWidth = 0.2f;
        }

        [Header("幹の設定")]

        /// <summary>
        /// 幹の分解能
        /// </summary>
        [SerializeField, Range(2, 100)]
        int _mainBranchResolution = 20;

        /// <summary>
        /// 基準長さ
        /// </summary>
        [SerializeField]
        float baseLength = 2f;

        /// <summary>
        /// 長さのカーブ
        /// </summary>
        [SerializeField]
        AnimationCurve _mainBranchLengthMultiplierhCurve;

        /// <summary>
        /// 基準角度
        /// </summary>
        [SerializeField]
        float baseAngle = 90f;

        /// <summary>
        /// 角度のカーブ
        /// </summary>
        [SerializeField]
        AnimationCurve _mainBranchDeltaAngleCurve;

        /// <summary>
        /// 角度差のカーブを反転させる
        /// </summary>
        [SerializeField]
        bool _inverseMainBranchDeltaAngle = false;

        /// <summary>
        /// 幹の幅
        /// </summary>
        [SerializeField, Range(0f, 5f)]
        float _mainBranchWidth = 0.1f;

        /// <summary>
        /// 幹の色
        /// </summary>
        [SerializeField]
        Gradient _mainBranchGradient;

        [Header("サポートの設定")]

        /// <summary>
        /// サポート
        /// </summary>
        [SerializeField]
        SupportEdge[] _supportEdges;

        [Header("葉の設定")]

        /// <summary>
        /// 葉っぱの基本的なサイズ
        /// </summary>
        [SerializeField, Range(0f, 10f)]
        float _baseLeafSize = 0.1f;

        /// <summary>
        /// 葉っぱのサイズの先端までのカーブ
        /// </summary>
        [SerializeField]
        AnimationCurve _leafSizeMultiplierCurve;

        /// <summary>
        /// 葉の色
        /// </summary>
        [SerializeField]
        Gradient _leafGradient;

        #region 外部インタフェース

        public override VStructure CreateStructure(Matrix4x4 toWorld)
        {
            var s = new VStructure();

            // まずは使用する座標のリストを作成
            var positions = new Vector2[_mainBranchResolution];

            Vector2 position = new Vector2(0f, 0f);

            positions[0] = toWorld.MultiplyPoint3x4(position);

            float length = baseLength;
            float angle = baseAngle;

            for (var i = 1; i < _mainBranchResolution; ++i)
            {
                var t = (float)i / _mainBranchResolution;

                var lengthMultiplier = _mainBranchLengthMultiplierhCurve.Evaluate(t);
                var deltaAngle = (_inverseMainBranchDeltaAngle ? -1 : 1) * _mainBranchDeltaAngleCurve.Evaluate(t);

                length = baseLength * lengthMultiplier;
                angle += deltaAngle;

                position += Utilities.Angle2Vector(angle * Mathf.Deg2Rad) * length;
                positions[i] = toWorld.MultiplyPoint3x4(position);
            }

            // 座標を元にパーティクルとエッジを追加
            for (var i = 0; i < positions.Length; ++i)
            {
                var t = (float)i / positions.Length;
                s.AddParticle(
                    positions[i],
                    _baseLeafSize * _leafSizeMultiplierCurve.Evaluate(t),
                    _leafGradient.Evaluate(Random.Range(0f, 1f))
                );
            }

            // エッジの作成
            for (var i = 1; i < positions.Length; ++i)
            {
                s.AddEdge(i - 1, i, _mainBranchWidth, _mainBranchGradient.Evaluate(Random.Range(0f, 1f)));

                // サポートの作成
                for (var j = 0; j < _supportEdges.Length; ++j)
                {
                    var t = _supportEdges[j];
                    if (i >= t.supportInterval)
                    {
                        s.AddEdge(i - t.supportInterval, i, t.supportWidth, _mainBranchGradient.Evaluate(Random.Range(0f, 1f)));
                    }
                }
            }

            // 根本をピンどめ
            s.AddAfterStep(new VPin(0, positions[0]));
            s.AddAfterStep(new VPin(1, positions[1]));

            return s;
        }

        #endregion
    }
}