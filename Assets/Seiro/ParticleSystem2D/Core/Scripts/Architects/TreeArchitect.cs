using Seiro.ParticleSystem2D.Core.RefDatas;
using UnityEngine;

namespace Seiro.ParticleSystem2D.Core.Architects
{

    /// <summary>
    /// 木の設計士
    /// </summary>
    [CreateAssetMenu(fileName = "TreeArchitect", menuName = "Seiro/GPUVerlet/TreeArchitect")]
    public class TreeArchitect : BaseArchitect
    {

        [Header("幹の設定")]

        /// <summary>
        /// 幹の最大深度
        /// </summary>
        [SerializeField, Range(2, 100)]
        int _maxBranchDepth = 4;

        /// <summary>
        /// 基準となる幹の長さ
        /// </summary>
        [SerializeField, Range(0.01f, 10f)]
        float _basebranchLength = 2f;

        /// <summary>
        /// 幹の幅
        /// </summary>
        [SerializeField, Range(0f, 10f)]
        float _branchWidth = 0.1f;

        /// <summary>
        /// 幹の角度の変化分の乗数
        /// </summary>
        [SerializeField]
        float _branchAngleDeltaFactor = 10f;

        /// <summary>
        /// 幹の角度の変化分
        /// </summary>
        [SerializeField]
        AnimationCurve _branchAngleDeltaMultiplierCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        /// <summary>
        /// 幹の長さの減衰率
        /// </summary>
        [SerializeField]
        float _branchDamping;

        /// <summary>
        /// 幹の色
        /// </summary>
        [SerializeField]
        Gradient _branchColor;

        /// <summary>
        /// 幹のマテリアル
        /// </summary>
        [SerializeField]
        string _branchMaterialID;

        [Header("幹の接続箇所")]

        /// <summary>
        /// 幹の接続箇所の大きさ
        /// </summary>
        [SerializeField, Range(0f, 10f)]
        float _branchJointSize = 0.1f;

        /// <summary>
        /// 葉っぱの色
        /// </summary>
        [SerializeField]
        Gradient _branchJointColor;

        /// <summary>
        /// 幹の接続部分のマテリアル
        /// </summary>
        [SerializeField]
        string _branchJointMaterialID;

        [Header("葉の設定")]

        /// <summary>
        /// 葉の大きさ
        /// </summary>
        [SerializeField]
        Vector2 _leafSize = new Vector2(1f, 1f);

        /// <summary>
        /// 葉っぱの色
        /// </summary>
        [SerializeField]
        Gradient _leafColor;

        /// <summary>
        /// 葉っぱのマテリアル
        /// </summary>
        [SerializeField]
        string _leafMaterialID;

        #region 外部インタフェース

        public override RefStructure Build(Matrix4x4 toWorld)
        {
            var s = RefStructure.CreateNew();

            var origin = s.AddParticle(Vector2.zero, _branchJointSize, RandomGradient(_leafColor), _branchJointMaterialID);
            /*var next = */
            MakeBranch(s, _maxBranchDepth, origin, 90f, _basebranchLength);

            s.TranslateParticles(toWorld);

            return s;
        }

        #endregion

        #region 内部処理

        /// <summary>
        /// 枝を作成する
        /// </summary>
        /// <returns></returns>
        RefParticle MakeBranch(RefStructure s, int depth, RefParticle parent, float angle, float length)
        {
            var t = 1f - ((float)depth / _maxBranchDepth);

            if (depth > 0)
            {
                depth--;
                var position = parent.position + Utilities.Angle2Vector(angle * Mathf.Deg2Rad) * length;
                var current = s.AddParticle(position, _branchJointSize, RandomGradient(_leafColor), _branchJointMaterialID);

                // 次の枝へ
                var deltaAngle = _branchAngleDeltaFactor * _branchAngleDeltaMultiplierCurve.Evaluate(t);
                MakeBranch(s, depth, current, angle + deltaAngle, length * _branchDamping);

                s.AddEdge(parent, current, _branchWidth, RandomGradient(_branchColor), _branchMaterialID);

                if (Random.Range(0f, 1f) > 0.5f)
                {
                    MakeBranch(s, depth, current, angle + Random.Range(-60f, 60f), length * _branchDamping);
                }

                return current;
            }
            else
            {
                var position = parent.position + Utilities.Angle2Vector(angle * Mathf.Deg2Rad) * _leafSize.y;
                var current = s.AddParticle(position, 0f, RandomGradient(_branchJointColor), _branchJointMaterialID);

                // 本当の葉っぱ
                s.AddEdge(parent, current, _leafSize.x, RandomGradient(_leafColor), _leafMaterialID);

                return null;
            }
        }

        /// <summary>
        /// ランダムに色を一つ取得する
        /// </summary>
        /// <param name="grad"></param>
        /// <returns></returns>
        Color RandomGradient(Gradient grad)
        {
            return grad.Evaluate(Random.Range(0f, 1f));
        }

        #endregion
    }
}