using Seiro.GPUVerlet.Core.Architects;
using Seiro.GPUVerlet.Core.Compilers;
using Seiro.GPUVerlet.Core.Components;
using Seiro.GPUVerlet.Core.Interactions;
using Seiro.GPUVerlet.Core.RefDatas;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// 簡易的なエディタ
    /// </summary>
    public class SimpleEditor : MonoBehaviour
    {

		public enum PlayerState
		{
			Play,
			Stop
		}

		public enum EditType
		{
			Particle,
			Edge
		}

        [SerializeField]
        VerletModel _model;

        [SerializeField]
        RefStructure _structure;

		[SerializeField]
		MaterialDictionary _materialDict;

		[Header("Builder")]

        [SerializeField]
        BaseParticleBuilder _particleBuilder;

		[SerializeField]
		BaseEdgeBuilder _edgeBuilder;

		[Header("Interaction")]

		[SerializeField]
		ParticlePicker _particlePicker;

		[Header("View")]

		[SerializeField]
		EventSystem _higherEventSystem;

		[SerializeField]
		Toggle _typeParticle;

		[SerializeField]
		Toggle _typeEdge;

		PlayerState _playerState;

		EditType _editType;

		RefParticle _prevHitten;

		bool _isDirty;

		#region MonoBehaviourイベント

		private void Start()
		{
			if (!_structure)
			{
				_structure = RefStructure.CreateNew();
			}

			LinkUIInputHandlers();
		}

		private void Update()
		{
			HandleMouseInput();
			CheckDirty();
		}

		#endregion

		#region 内部処理

		/// <summary>
		/// マウス入力の処理
		/// </summary>
		void HandleMouseInput()
		{
			if (_higherEventSystem && _higherEventSystem.IsPointerOverGameObject())
			{
				return;
			}

			if (Input.GetMouseButtonDown(0))
			{
				switch (_editType) {
					case EditType.Particle:
						HandleAddParticle();
						break;
					case EditType.Edge:
						HandleAddEdge();
						break;
				}
			}
		}

		/// <summary>
		/// パーティクルの追加処理
		/// </summary>
		void HandleAddParticle()
        {
            if (_structure)
            {
                var wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				var hitten = RefStructure.HitParticle(_structure, wpos);
				if (hitten == null)
				{
					_particleBuilder.Add(_structure, wpos);
					SetDirty();
				}
            }
        }

		/// <summary>
		/// エッジの追加処理
		/// </summary>
		void HandleAddEdge()
		{
			if (_structure)
			{
				var wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				var hitten = RefStructure.HitParticle(_structure, wpos);
				if (hitten != null) {
					if (_prevHitten == null)
					{
						_prevHitten = hitten;
					}
					else if(_prevHitten != hitten)
					{
						_edgeBuilder.Add(_structure, _prevHitten, hitten);
						_prevHitten = null;
						SetDirty();
					}
				}
			}
		}

		/// <summary>
		/// 何かしらの編集を行った場合はこの関数を呼び出すとそのフレームの最後に更新される
		/// </summary>
		void SetDirty()
		{
			_isDirty = true;
		}

		/// <summary>
		/// Dirtyフラグを確認する
		/// </summary>
		void CheckDirty()
		{
			if (_isDirty)
			{
				ApplyPreview();
			}
			_isDirty = false;
		}

        /// <summary>
        /// プレビューの更新
        /// </summary>
        void ApplyPreview()
        {
			if (_model && _structure && _materialDict)
			{
				var compiled = StructureCompiler.Compile(_structure, _materialDict);
				_model.SetStructure(compiled);
			}
        }

		/// <summary>
		/// UI要素の入力処理とハンドラを紐づける
		/// </summary>
		void LinkUIInputHandlers()
		{
			if (_typeParticle)
			{
				_typeParticle.onValueChanged.AddListener(HandleOnTypeParticle);
			}
			if (_typeEdge)
			{
				_typeEdge.onValueChanged.AddListener(HandleOnTypeEdge);
			}
		}

		/// <summary>
		/// typeでパーティクルが選択されたときの処理
		/// </summary>
		void HandleOnTypeParticle(bool v)
		{
			if (v)
			{
				_editType = EditType.Particle;
			}
		}

		/// <summary>
		/// typeでエッジが選択されたときのしょり
		/// </summary>
		void HandleOnTypeEdge(bool v)
		{
			if (v)
			{
				_editType = EditType.Edge;
			}
		}

		#endregion
	}
}
