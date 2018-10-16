using Seiro.ParticleSystem2D.Core.Architects;
using Seiro.ParticleSystem2D.Core.Compilers;
using Seiro.ParticleSystem2D.Core.Components;
using Seiro.ParticleSystem2D.Core.Interactions;
using Seiro.ParticleSystem2D.Core.RefDatas;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seiro.ParticleSystem2D.Demo
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
            Select,
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
        BaseParticleBuilder[] _particleBuilders;

        [SerializeField]
        BaseEdgeBuilder _edgeBuilder;

        [SerializeField]
        BaseEdgeBuilder[] _edgeBuilders;

        [Header("Interaction")]

        [SerializeField]
        ParticlePicker _particlePicker;

        [SerializeField]
        MonoBehaviour _interactionScript;

        [Header("Left Side Bar")]

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
        Button _historyUndo;

        [SerializeField]
        Button _historyRedo;

        [SerializeField]
        Toggle _toolAdd;

        [SerializeField]
        Toggle _toolRemove;

        [SerializeField]
        Toggle _toolMove;

        [Header("Top Data Dock")]

        [SerializeField]
        Button _dataSave;

        [SerializeField]
        Button _dataLoad;

        [Header("Bottom Material Dock")]

        [SerializeField]
        Toggle _particleSlot1;

        [SerializeField]
        Toggle _particleSlot2;

        [SerializeField]
        Toggle _edgeSlot1;

        [SerializeField]
        Toggle _edgeSlot2;

        PlayerState _playerState = PlayerState.Stop;

        ElementType _elementType = ElementType.Particle;

        EditType _editType = EditType.Add;

        [HideInInspector]
        RefParticle _prevHitten = null;

        bool _isDirty;

        [HideInInspector]
        RefParticle _draggedParticle = null;

        [HideInInspector]
        RefParticle _draggedEdgesParticleA = null;
        [HideInInspector]
        RefParticle _draggedEdgesParticleB = null;

        Vector2 _draggedEdgesParticleOffsetA;
        Vector2 _draggedEdgesParticleOffsetB;

        List<string> _editHistory = null;
        int _editPointer = 0;

        static readonly string _outputDataPath = "Seiro/GPUVerlet/Demo_Refix/04_SimpleEditor/Output";

        #region MonoBehaviourイベント

        private void Start()
        {
            if (!_structure)
            {
                _structure = RefStructure.CreateNew();
            }

            _editHistory = new List<string>();

            LinkUIInputHandlers();

            // 初期状態を編集履歴に追加
            AddEditHistory();
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
                                HandleRemoveEdge();
                                break;
                        }
                        break;
                    case EditType.Move:
                        switch (_elementType)
                        {
                            case ElementType.Particle:
                                HandleStartMoveParticle();
                                break;
                            case ElementType.Edge:
                                HandleStartMoveEdge();
                                break;
                        }
                        break;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                switch (_editType)
                {
                    case EditType.Move:
                        switch (_elementType)
                        {
                            case ElementType.Particle:
                                HandleEndMoveParticle();
                                break;
                            case ElementType.Edge:
                                HandleEndMoveEdge();
                                break;
                        }
                        break;
                }
            }
            else
            {
                HandleDragParticle();
                HandleDragEdge();
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
                    AddEditHistory();
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
                if (hitten != null)
                {
                    if (_prevHitten == null)
                    {
                        _prevHitten = hitten;
                    }
                    else if (_prevHitten != hitten)
                    {
                        _edgeBuilder.Add(_structure, _prevHitten, hitten);
                        AddEditHistory();

                        _prevHitten = null;
                        SetDirty();
                    }
                }
            }
        }

        /// <summary>
        /// パーティクルの削除処理
        /// </summary>
        void HandleRemoveParticle()
        {
            if (_structure)
            {
                var wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hitten = RefStructure.HitParticle(_structure, wpos);
                if (hitten != null)
                {
                    _structure.RemoveParticle(hitten);
                    AddEditHistory();
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// エッジの削除処理
        /// </summary>
        void HandleRemoveEdge()
        {
            if (_structure)
            {
                var wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hitten = RefStructure.HitEdge(_structure, wpos);
                if (hitten != null)
                {
                    _structure.RemoveEdge(hitten);
                    AddEditHistory();
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// パーティクルの移動処理
        /// </summary>
        void HandleStartMoveParticle()
        {
            if (_structure)
            {
                var wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hitten = RefStructure.HitParticle(_structure, wpos);
                if (hitten != null)
                {
                    _draggedParticle = hitten;
                }
            }
        }

        /// <summary>
        /// エッジの移動処理
        /// </summary>
        void HandleStartMoveEdge()
        {
            if (_structure)
            {
                Vector2 wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hitten = RefStructure.HitEdge(_structure, wpos);
                if (hitten != null)
                {
                    _draggedEdgesParticleA = _structure.FindParticleFromUID(hitten.aUID);
                    _draggedEdgesParticleB = _structure.FindParticleFromUID(hitten.bUID);

                    _draggedEdgesParticleOffsetA = wpos - _draggedEdgesParticleA.position;
                    _draggedEdgesParticleOffsetB = wpos - _draggedEdgesParticleB.position;
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
        /// エッジのドラッグ処理
        /// </summary>
        void HandleDragEdge()
        {
            if (_draggedEdgesParticleA != null && _draggedEdgesParticleB != null)
            {
                Vector2 wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _draggedEdgesParticleA.position = wpos - _draggedEdgesParticleOffsetA;
                _draggedEdgesParticleB.position = wpos - _draggedEdgesParticleOffsetB;

                SetDirty();
            }
        }

        /// <summary>
        /// パーティクルの移動処理の終了
        /// </summary>
        void HandleEndMoveParticle()
        {
            if (_draggedParticle != null)
            {
                _draggedParticle = null;
                AddEditHistory();
            }
        }

        /// <summary>
        /// エッジの移動処理の終了
        /// </summary>
        void HandleEndMoveEdge()
        {
            if (_draggedEdgesParticleA != null && _draggedEdgesParticleB != null)
            {
                _draggedEdgesParticleA = null;
                _draggedEdgesParticleB = null;
                AddEditHistory();
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
                UpdateHistoryUIs();
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
        /// 編集履歴に現在の状態を追加する
        /// </summary>
        void AddEditHistory()
        {
            if (_editHistory == null)
            {
                _editHistory = new List<string>();
            }

            var json = JsonUtility.ToJson(_structure);

            // 編集ポインタが _editHistory.Count - 1ではない場合はそうなるように調整する
            if (_editPointer != _editHistory.Count - 1)
            {
                for (var i = _editHistory.Count - 1; i > _editPointer; --i)
                {
                    _editHistory.RemoveAt(i);
                }
            }

            _editHistory.Add(json);
            _editPointer = _editHistory.Count - 1;
        }

        /// <summary>
        /// Undoが可能？
        /// </summary>
        bool CanUndo()
        {
            return _editPointer > 0;
        }

        /// <summary>
        /// 編集履歴をもとに一つもとに戻る
        /// </summary>
        void UndoHistory()
        {
            if (CanUndo())
            {
                _editPointer--;

                RestoreStructureToHistory(_editPointer);

                UpdateHistoryUIs();
            }
        }

        /// <summary>
        /// Redoが可能?
        /// </summary>
        /// <returns></returns>
        bool CanRedo()
        {
            return (_editPointer + 1) < _editHistory.Count;
        }

        /// <summary>
        /// 編集履歴をもとに状態を戻す
        /// </summary>
        void RedoHistory()
        {
            if (CanRedo())
            {
                _editPointer++;

                RestoreStructureToHistory(_editPointer);

                UpdateHistoryUIs();
            }
        }

        /// <summary>
        /// 編集履歴をもとにその編集番号の状態を復元する
        /// </summary>
        /// <param name="pointer"></param>
        void RestoreStructureToHistory(int pointer)
        {
            if (0 <= pointer && pointer < _editHistory.Count)
            {
                var json = _editHistory[_editPointer];
                // Objectを継承しているものをjsonでdeserializeする場合はFromJsonOverwriteを使用
                JsonUtility.FromJsonOverwrite(json, _structure);
                SetDirty();
            }
        }

        /// <summary>
        /// 作業履歴ボタンの有効無効の更新
        /// </summary>
        void UpdateHistoryUIs()
        {
            if (_historyUndo)
            {
                _historyUndo.interactable = CanUndo();
            }
            if (_historyRedo)
            {
                _historyRedo.interactable = CanRedo();
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

            if (_historyUndo)
            {
                _historyUndo.onClick.AddListener(HandleClickedUndo);
            }
            if (_historyRedo)
            {
                _historyRedo.onClick.AddListener(HandleClickedRedo);
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

            if (_dataSave)
            {
                _dataSave.onClick.AddListener(HandleClickedSave);
            }
            if (_dataLoad)
            {
                _dataLoad.onClick.AddListener(HandleClickedLoad);
            }

            if (_particleSlot1)
            {
                _particleSlot1.onValueChanged.AddListener(HandleChangedParticleMaterialSlot1);
            }
            if (_particleSlot2)
            {
                _particleSlot2.onValueChanged.AddListener(HandleChangedParticleMaterialSlot2);
            }
            if (_edgeSlot1)
            {
                _edgeSlot1.onValueChanged.AddListener(HandleChangedEdgeMaterialSlot1);
            }
            if (_edgeSlot2)
            {
                _edgeSlot2.onValueChanged.AddListener(HandleChangedEdgeMaterialSlot2);
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
                // _particlePicker.enabled = true;
                _interactionScript.enabled = true;
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
                // _particlePicker.enabled = false;
                _interactionScript.enabled = false;

                SetDirty();
            }
        }

        /// <summary>
        /// Undoがクリックされたとき
        /// </summary>
        void HandleClickedUndo()
        {
            UndoHistory();
        }

        /// <summary>
        /// Redoがクリックされたとき
        /// </summary>
        void HandleClickedRedo()
        {
            RedoHistory();
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
        void HandleChangedEditMove(bool v)
        {
            if (v)
            {
                _editType = EditType.Move;
            }
        }

        /// <summary>
        /// saveボタンを押したとき
        /// </summary>
        void HandleClickedSave()
        {
#if UNITY_EDITOR
            var path = Application.dataPath + "/" + _outputDataPath;
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(_structure, uniquePath);
            AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// loadボタンを押したとき
        /// </summary>
        void HandleClickedLoad()
        {

        }

        /// <summary>
        /// Particle Material Slot 1が変化したとき
        /// </summary>
        /// <param name="v"></param>
        void HandleChangedParticleMaterialSlot1(bool v)
        {
            if (v)
            {
                _particleBuilder = _particleBuilders[0];
            }
        }

        /// <summary>
        /// Particle Material Slot 2が変化したとき
        /// </summary>
        /// <param name="v"></param>
        void HandleChangedParticleMaterialSlot2(bool v)
        {
            if (v)
            {
                _particleBuilder = _particleBuilders[1];
            }
        }

        /// <summary>
        /// Edge Material Slot 1が変化したとき
        /// </summary>
        /// <param name="v"></param>
        void HandleChangedEdgeMaterialSlot1(bool v)
        {
            if (v)
            {
                _edgeBuilder = _edgeBuilders[0];
            }
        }

        /// <summary>
        /// Edge Material Slot 2が変化したとき
        /// </summary>
        /// <param name="v"></param>
        void HandleChangedEdgeMaterialSlot2(bool v)
        {
            if (v)
            {
                _edgeBuilder = _edgeBuilders[1];
            }
        }

        #endregion
    }
}
