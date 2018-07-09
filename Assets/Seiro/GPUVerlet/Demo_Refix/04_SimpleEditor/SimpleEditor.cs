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

		public enum ElementType
		{
			Particle,
			Edge
		}

		public enum EditType
		{
			Add,
			Remove,
			Move,
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
		Toggle _playerPlay;

		[SerializeField]
		Toggle _playerStop;

		[SerializeField]
		Toggle _typeParticle;

		[SerializeField]
		Toggle _typeEdge;

		[SerializeField]
		Toggle _toolAdd;

		[SerializeField]
		Toggle _toolRemove;

		[SerializeField]
		Toggle _toolMove;

		PlayerState _playerState = PlayerState.Stop;

		ElementType _elementType = ElementType.Particle;

		EditType _editType = EditType.Add;

		RefParticle _prevHitten;

		bool _isDirty;

		Vector2 _draggedStartPosition;

		RefParticle _draggedParticle;

		RefEdge _draggedEdge;

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
			if (_playerState == PlayerState.Stop)
			{
				HandleMouseInput();
				CheckDirty();
			}
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
				switch (_editType)
				{
					case EditType.Add:
						switch (_elementType)
						{
							case ElementType.Particle:
								HandleAddParticle();
								break;
							case ElementType.Edge:
								HandleAddEdge();
								break;
						}
						break;
					case EditType.Remove:
						switch (_elementType)
						{
							case ElementType.Particle:
								HandleRemoveParticle();
								break;
							case ElementType.Edge:
								HandleremoveEdge();
								break;
						}
						break;
					case EditType.Move:
						switch (_elementType)
						{
							case ElementType.Particle:
								HandleMoveParticle();
								break;
							case ElementType.Edge:
								break;
						}
						break;
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				_draggedParticle = null;
				_draggedEdge = null;
			}

			HandleDragParticle();
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
		/// パーティクルの削除処理
		/// </summary>
		void HandleRemoveParticle() {
			if (_structure)
			{
				var wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				var hitten = RefStructure.HitParticle(_structure, wpos);
				if (hitten != null)
				{
					_structure.RemoveParticle(hitten);
					SetDirty();
				}
			}
		}

		/// <summary>
		/// エッジの削除処理
		/// </summary>
		void HandleremoveEdge()
		{
			if (_structure)
			{
				var wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				var hitten = RefStructure.HitEdge(_structure, wpos);
				if (hitten != null)
				{
					_structure.RemoveEdge(hitten);
					SetDirty();
				}
			}
		}

		/// <summary>
		/// パーティクルの移動処理
		/// </summary>
		void HandleMoveParticle()
		{
			if (_structure)
			{
				var wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				var hitten = RefStructure.HitParticle(_structure, wpos);
				if (hitten != null)
				{
					_draggedStartPosition = wpos;
					_draggedParticle = hitten;
				}
			}
		}

		/// <summary>
		/// パーティクルのドラッグ処理
		/// </summary>
		void HandleDragParticle()
		{
			if (_draggedParticle != null)
			{
				var wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				_draggedParticle.position = wpos;

				SetDirty();
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
			if (_playerPlay)
			{
				_playerPlay.onValueChanged.AddListener(HandleChangedPlayerPlay);
			}
			if (_playerStop)
			{
				_playerStop.onValueChanged.AddListener(HandleChangedPlayerStop);
			}
			if (_typeParticle)
			{
				_typeParticle.onValueChanged.AddListener(HandleOnTypeParticle);
			}
			if (_typeEdge)
			{
				_typeEdge.onValueChanged.AddListener(HandleOnTypeEdge);
			}
			if (_toolAdd)
			{
				_toolAdd.onValueChanged.AddListener(HandleChangedEditAdd);
			}
			if (_toolRemove)
			{
				_toolRemove.onValueChanged.AddListener(HandleChangedEditRemove);
			}
			if (_toolMove)
			{
				_toolMove.onValueChanged.AddListener(HandleChangedEditMove);
			}
		}

		/// <summary>
		/// Playerのplayが変化したとき
		/// </summary>
		/// <param name="v"></param>
		void HandleChangedPlayerPlay(bool v)
		{
			if (v)
			{
				_playerState = PlayerState.Play;
				_model.isSimulated = true;
				_particlePicker.enabled = true;
			}
		}

		/// <summary>
		/// Playerのstopが変化したとき
		/// </summary>
		/// <param name="v"></param>
		void HandleChangedPlayerStop(bool v)
		{
			if (v)
			{
				_playerState = PlayerState.Stop;
				_model.isSimulated = false;
				_particlePicker.enabled = false;

				SetDirty();
			}
		}

		/// <summary>
		/// typeでパーティクルが選択されたときの処理
		/// </summary>
		void HandleOnTypeParticle(bool v)
		{
			if (v)
			{
				_elementType = ElementType.Particle;
			}
		}

		/// <summary>
		/// typeでエッジが選択されたときのしょり
		/// </summary>
		void HandleOnTypeEdge(bool v)
		{
			if (v)
			{
				_elementType = ElementType.Edge;
			}
		}

		/// <summary>
		/// EditのAddが変化したとき
		/// </summary>
		/// <param name="v"></param>
		void HandleChangedEditAdd(bool v)
		{
			if (v)
			{
				_editType = EditType.Add;
			}
		}

		/// <summary>
		/// EditのRemoveが変化したとき
		/// </summary>
		/// <param name="v"></param>
		void HandleChangedEditRemove(bool v)
		{
			if (v)
			{
				_editType = EditType.Remove;
			}
		}

		/// <summary>
		/// EditのMoveが変化したとき
		/// </summary>
		/// <param name="v"></param>
		void HandleChangedEditMove(bool v) {
			if (v)
			{
				_editType = EditType.Move;
			}
		}

		#endregion
	}
}
