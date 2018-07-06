using Seiro.GPUVerlet.Core.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Core.Interactions
{

	/// <summary>
	/// 対象のシミュレータからParticleをつかみだす
	/// </summary>
	public class ParticlePicker : MonoBehaviour
	{

		[SerializeField]
		VerletSimulator _target;

		[SerializeField]
		Camera _camera;

		[SerializeField, Range(0f, 5f)]
		float _pickRadius = 0.5f;

		int _overlappedIdx;

		#region MonoBehaviourイベント

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				var pos = _camera.ScreenPointToRay(Input.mousePosition).origin;
				_overlappedIdx = FindOverlappedParticle(pos);
			}
			else if (Input.GetMouseButtonUp(0))
			{
				_overlappedIdx = -1;
			}

			if (_overlappedIdx >= 0)
			{
				var pos = _camera.ScreenPointToRay(Input.mousePosition).origin;
				_target.SetParticlePosition(_overlappedIdx, pos);
			}
		}

		#endregion

		#region 内部処理

		/// <summary>
		/// 指定した座標に最も近いパーティクルの番号を取得する
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		int FindOverlappedParticle(Vector2 position)
		{
			if (!_target && !_target.IsReady())
			{
				return -1;
			}

			int minIdx = -1;
			float minSqrLength = _pickRadius * _pickRadius;

			for (var i = 0; i < _target.GetParticleCount(); ++i)
			{
				var sqrLength = (_target.GetParticlePosition(i) - position).sqrMagnitude;
				if (sqrLength < minSqrLength)
				{
					minIdx = i;
					minSqrLength = sqrLength;
				}
			}
			return minIdx;
		}

		#endregion
	}
}