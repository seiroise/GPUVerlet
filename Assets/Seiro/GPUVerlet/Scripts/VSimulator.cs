using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seiro.GPUVerlet
{

    /// <summary>
    /// GPU上でverlet法をシミュレートする
    /// </summary>
    public class VSimulator : MonoBehaviour
    {

        const int SIMULATION_SIZE = 256;
        const string PARTICLE_KERNEL_NAME = "ParticleCS";
        const string EDGE_KERNEL_NAME = "EdgeCS";
        const string FORCE_FIELD_KERNEL_NAME = "ForceFieldCS";

        [Header("Compute Settings")]

        public ComputeShader verletCS;

        [Header("Rendering Settings")]

        public Bounds renderingBounds;
        public Mesh instancedMesh;
        public Material particleMat;
        public Material edgeMat;

        [Header("Simulation Parameters")]

        [SerializeField, Range(0.001f, 0.999f)]
        float _damping = 0.97f;

        [SerializeField, Range(1, 10)]
        int _iterations = 4;

        // 各種データ
        VParticle[] _particles;
        VEdge[] _edges;

        // 外部ステップ用
        IExternalStep[] _beforeSteps;	// エッジ解決前
        IExternalStep[] _afterSteps;	// エッジ解決後

        // バッファ
        ComputeBuffer _particlesBuffer;
        ComputeBuffer _edgesBuffer;

        // インスタンシング用の引数
        uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };
        ComputeBuffer _particleArgsBuffer;
        ComputeBuffer _edgeArgsBuffer;

        // 力場(xy: 座標)、(z: 力)
        Vector4 _forceField;

        // 準備できてるフラグ
        bool _ready = false;

        // マテリアルプロパティブロック
        MaterialPropertyBlock _particlePropBlock;
        MaterialPropertyBlock _edgePropBlock;

        #region MonoBehaviourイベント

        void Awake()
        {
            if (!CheckSupported())
            {
                Debug.LogWarning("Your device or GL is not supported Compute Shader or GPUInstancing.");
            }
        }

        void OnDestroy()
        {
            ReleaseBuffer();
        }

        void Update()
        {
            if (_ready)
            {
                RenderInstancedMesh();
            }
        }

        void FixedUpdate()
        {
            if (_ready)
            {
                // 結果的にCPU上で実行することになったのは無念としか言いようがない・・・
                // SimulateOnGPU();
                SimulateOnCPU();
            }
        }

        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            Handles.DrawWireDisc(new Vector3(_forceField.x, _forceField.y), Vector3.back, _forceField.z * 0.5f);
#endif
        }

        #endregion

        #region 外部インタフェース

        /// <summary>
        /// 構造を設定する
        /// </summary>
        /// <param name="s"></param>
        public void SetStructure(VStructure s)
        {
            _particles = s.GetParticles();
            _edges = s.GetEdges();

            _beforeSteps = s.GetBeforeSteps();
            _afterSteps = s.GetAfterSteps();

            BindBuffer();

            _ready = true;
        }

        /// <summary>
        /// 力場を設定する
        /// </summary>
        /// <param name="position"></param>
        /// <param name="strength"></param>
        public void SetForceField(Vector2 position, float strength)
        {
            _forceField.x = position.x;
            _forceField.y = position.y;
            _forceField.z = strength;
        }

        /// <summary>
        /// 最も近いパーティクルを探し、そのidを返す
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int FindNearestParticle(Vector2 pos, float range)
        {
            var nearest = -1;
            var minSqrDist = range * range;

            for (var i = 0; i < _particles.Length; ++i)
            {
                var p = _particles[i];
                var sqrDist = (p.position - pos).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    nearest = i;
                }
            }

            return nearest;
        }

        /// <summary>
        /// 指定したidのパーティクルの座標を設定する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        public void SetParticlePosition(int id, Vector2 pos)
        {
            _particles[id].position = pos;
        }

        #endregion

        #region 内部処理

        /// <summary>
        /// 実行環境が整っているか確認する
        /// </summary>
        /// <returns></returns>
        bool CheckSupported()
        {
            return SystemInfo.supportsComputeShaders && SystemInfo.supportsInstancing;
        }

        /// <summary>
        /// バッファの解放
        /// </summary>
        void ReleaseBuffer()
        {
            if (_particlesBuffer != null)
            {
                _particlesBuffer.Release();
                _particlesBuffer = null;
            }
            if (_edgesBuffer != null)
            {
                _edgesBuffer.Release();
                _edgesBuffer = null;
            }
            if (_particleArgsBuffer != null)
            {
                _particleArgsBuffer.Release();
                _particleArgsBuffer = null;
            }
            if (_edgeArgsBuffer != null)
            {
                _edgeArgsBuffer.Release();
                _edgeArgsBuffer = null;
            }
        }

        /// <summary>
        /// バッファにデータをバインドする
        /// </summary>
        void BindBuffer()
        {
            if (_particles == null || _edges == null)
            {
                return;
            }

            ReleaseBuffer();

            _particlesBuffer = new ComputeBuffer(_particles.Length, Marshal.SizeOf(typeof(VParticle)));
            _particlesBuffer.SetData(_particles);

            _edgesBuffer = new ComputeBuffer(_edges.Length, Marshal.SizeOf(typeof(VEdge)));
            _edgesBuffer.SetData(_edges);

            _particleArgsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            _edgeArgsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

            _particlePropBlock = new MaterialPropertyBlock();
            _edgePropBlock = new MaterialPropertyBlock();
        }

        /// <summary>
        /// GPU上でシミュレートする
        /// </summary>
        void SimulateOnGPU()
        {
            if (!verletCS)
            {
                return;
            }

            var cs = verletCS;
            int id = -1;
            int threadGroupSize = -1;

            if (cs.HasKernel(PARTICLE_KERNEL_NAME))
            {
                threadGroupSize = Mathf.CeilToInt(_particles.Length / SIMULATION_SIZE);
                id = cs.FindKernel(PARTICLE_KERNEL_NAME);
                cs.SetFloat("_Damping", _damping);
                cs.SetBuffer(id, "_ParticleDataBufferR", _particlesBuffer);
                cs.SetBuffer(id, "_ParticleDataBufferW", _particlesBuffer);

                cs.Dispatch(id, threadGroupSize, 1, 1);
            }
            if (cs.HasKernel(EDGE_KERNEL_NAME))
            {
                threadGroupSize = Mathf.CeilToInt(_edges.Length / SIMULATION_SIZE);
                id = cs.FindKernel(EDGE_KERNEL_NAME);
                cs.SetBuffer(id, "_ParticleDataBufferR", _particlesBuffer);
                cs.SetBuffer(id, "_ParticleDataBufferW", _particlesBuffer);
                cs.SetBuffer(id, "_EdgeDataBufferR", _edgesBuffer);

                cs.Dispatch(id, threadGroupSize, 1, 1);
            }
            /*
            if (cs.HasKernel(FORCE_FIELD_KERNEL_NAME))
            {
                threadGroupSize = Mathf.CeilToInt(_particles.Length / SIMULATION_SIZE);
                id = cs.FindKernel(FORCE_FIELD_KERNEL_NAME);
                cs.SetVector("_ForceField", _forceField);
                cs.SetBuffer(id, "_ParticleDataBufferR", _particlesBuffer);
                cs.SetBuffer(id, "_ParticleDataBufferW", _particlesBuffer);

                cs.Dispatch(id, threadGroupSize, 1, 1);
            }
			*/
        }

        /// <summary>
        /// CPU上でシミュレートする
        /// </summary>
        void SimulateOnCPU()
        {
            StepParticles();
            StepExternal(_beforeSteps);
            SolveEdges();
            StepExternal(_afterSteps);

            // バッファに値を設定する
            _particlesBuffer.SetData(_particles);
            _edgesBuffer.SetData(_edges);
        }

        /// <summary>
        /// パーティクル全体を1ステップシミュレートする
        /// </summary>
        void StepParticles()
        {
            for (var i = 0; i < _particles.Length; ++i)
            {
                StepParticle(i);
            }
        }

        /// <summary>
        /// パーティクルを1ステップシミュレートする
        /// </summary>
        /// <param name="p"></param>
        void StepParticle(int id)
        {
            var p = _particles[id];

            var dx = (p.position.x - p.prevPosition.x) * _damping;
            var dy = (p.position.y - p.prevPosition.y) * _damping;

            var nx = p.position.x + dx;
            var ny = p.position.y + dy;

            p.prevPosition.x = p.position.x;
            p.prevPosition.y = p.position.y;

            p.position.x = nx;
            p.position.y = ny;

            _particles[id] = p;
        }

        /// <summary>
        /// エッジによる座標調整を規定回数反復して解決する
        /// </summary>
        void SolveEdges()
        {
            for (var i = 0; i < _iterations; ++i)
            {
                StepEdges();
            }
        }

        /// <summary>
        /// エッジ全体を1ステップシミュレートする
        /// </summary>
        void StepEdges()
        {
            for (var i = 0; i < _edges.Length; ++i)
            {
                StepEdge(i);
            }
        }

        /// <summary>
        /// エッジを1ステップシミュレートする
        /// </summary>
        /// <param name="e"></param>
        void StepEdge(int id)
        {
            var e = _edges[id];

            var a = _particles[e.a];
            var b = _particles[e.b];

            float dx = a.position.x - b.position.x;
            float dy = a.position.y - b.position.y;

            var current = Mathf.Sqrt(dx * dx + dy * dy);
            var f = (current - e.length) / current;

            float ax = f * 0.5f * dx;
            float ay = f * 0.5f * dy;

            a.position.x -= ax;
            a.position.y -= ay;

            b.position.x += ax;
            b.position.y += ay;

            _particles[e.a] = a;
            _particles[e.b] = b;
        }

        /// <summary>
        /// 外部ステップを実行する
        /// </summary>
        /// <param name="steps"></param>
        void StepExternal(IExternalStep[] steps)
        {
            for (var i = 0; i < steps.Length; ++i)
            {
                steps[i].Step(this);
            }
        }

        /// <summary>
        /// インスタンシングを利用してメッシュを描画する
        /// </summary>
        void RenderInstancedMesh()
        {

            if (instancedMesh == null)
            {
                return;
            }

            // パーティクルの描画
            uint numIndices = instancedMesh.GetIndexCount(0);

            if (particleMat)
            {
                _args[0] = numIndices;
                _args[1] = (uint)_particles.Length;
                _particleArgsBuffer.SetData(_args);

                _particlePropBlock.SetBuffer("_ParticleDataBuffer", _particlesBuffer);

                // particleMat.SetBuffer("_ParticleDataBuffer", _particlesBuffer);

                Graphics.DrawMeshInstancedIndirect(instancedMesh, 0, particleMat, renderingBounds, _particleArgsBuffer, 0, _particlePropBlock);
            }

            // エッジの描画
            if (edgeMat)
            {
                _args[0] = numIndices;
                _args[1] = (uint)_edges.Length;
                _edgeArgsBuffer.SetData(_args);

                _edgePropBlock.SetBuffer("_ParticleDataBuffer", _particlesBuffer);
                _edgePropBlock.SetBuffer("_EdgeDataBuffer", _edgesBuffer);

                // edgeMat.SetBuffer("_ParticleDataBuffer", _particlesBuffer);
                // edgeMat.SetBuffer("_EdgeDataBuffer", _edgesBuffer);

                Graphics.DrawMeshInstancedIndirect(instancedMesh, 0, edgeMat, renderingBounds, _edgeArgsBuffer, 0, _edgePropBlock);
            }
        }

        #endregion
    }
}