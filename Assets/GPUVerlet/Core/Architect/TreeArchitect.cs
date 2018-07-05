using Seiro.GPUVerlet.Core.RefDatas;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Architect
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
		[SerializeField, Range(2, 10)]
		int _maxBranchDepth = 4;

		/// <summary>
		/// 基準となる幹の長さ
		/// </summary>
		[SerializeField, Range(1f, 10f)]
		float _basebranchLength = 2f;

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

		/// <summary>
		/// 幹の接続部分のマテリアル
		/// </summary>
		[SerializeField]
		string _branchJointMaterialID;

		[Header("葉の設定")]

		/// <summary>
		/// 葉の大きさ
		/// </summary>
		[SerializeField, Range(0f, 10f)]
		float _leafSize = 0.5f;

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

		public override RefStructure Build()
		{
			var s = new RefStructure();

			var origin = s.AddParticle(Vector2.zero, _leafSize, RandomGradient(_leafColor), _branchJointMaterialID);
			var next = MakeBranch(s, _maxBranchDepth, origin, 90f, _basebranchLength);

			s.AddEdge(origin, next, 0.01f, RandomGradient(_branchColor), _branchMaterialID);

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
			var position = parent.position + Utilities.Angle2Vector(angle * Mathf.Deg2Rad);

			if (depth > 0)
			{
				depth--;

				var child = s.AddParticle(position, _leafSize, RandomGradient(_leafColor), _branchJointMaterialID);
				s.AddEdge(parent, child, 0.1f, RandomGradient(_branchColor), _branchMaterialID);

				MakeBranch(s, depth, child, angle, length * 0.9f);

				return child;
			}
			else
			{
				var child = s.AddParticle(position, _leafSize, RandomGradient(_leafColor), _branchJointMaterialID);

				// 本当の葉っぱ
				s.AddEdge(parent, child, 1f, RandomGradient(_branchColor), _leafMaterialID);

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