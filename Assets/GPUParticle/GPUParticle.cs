using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// GPUインスタンシングのテスト用
/// </summary>
public class GPUParticle : MonoBehaviour
{
    /// <summary>
    /// パーティクルデータ
    /// </summary>
    [System.Serializable]
    struct ParticleData
    {
        /// <summary>
        /// 座標
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// ｘがもとの半径、yが現在の半径
        /// </summary>
        public Vector2 radius;
    }

    /// <summary>
    /// スレッドグループのスレッドサイズ
    /// </summary>
    const int SIMULATION_BLOCK_SIZE = 256;

    /// <summary>
    /// 表示するオブジェクトの最大数
    /// </summary>
	[Range(32, 65536)]
    public int maxObjectNum = 256;

    /// <summary>
    /// 計算を行うComputesShader
    /// </summary>
    public ComputeShader particleCS;

    /// <summary>
    /// インスタンシングするメッシュ
    /// </summary>
    public Mesh instancedMesh;

    /// <summary>
    /// インスタンシングされたものを描画するマテリアル
    /// </summary>
    public Material instanceRenderMat;

    /// <summary>
    /// パーティクルバッファ
    /// </summary>
    ComputeBuffer _particleBuf;

    /// <summary>
    /// GPUインスタンシングのための引数(ComputeBufferを介して転送)
    /// </summary>
    uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };

    /// <summary>
    /// GPUインスタンシングのための引数バッファ
    /// </summary>
    ComputeBuffer _argsBuf;

    #region MonoBehaviourイベント

    void OnEnable()
    {
        if (CheckSupported())
        {
            BindBuffer();
        }
        else
        {
            Debug.LogWarning("Your device or GL is not supported Compute Shader or GPUInstancing.");
        }
    }

    void OnDisable()
    {
        ReleaseBuffer();
    }

    void Update()
    {
        Simulation();
        RenderInstancedMesh();
    }

    #endregion

    #region 内部処理

    /// <summary>
    /// 実行するための機能をサポートしているか
    /// </summary>
    /// <returns></returns>
    bool CheckSupported()
    {
        return SystemInfo.supportsComputeShaders && SystemInfo.supportsInstancing;
    }

    /// <summary>
    /// バッファのバインド
    /// </summary>
    void BindBuffer()
    {
        // ComputeShader用のパーティクルデータバッファの作成
        _particleBuf = new ComputeBuffer(maxObjectNum, Marshal.SizeOf(typeof(ParticleData)));

        // バッファに格納するデータの作成
        var particles = new ParticleData[maxObjectNum];
        for (var i = 0; i < maxObjectNum; ++i)
        {
            particles[i] = new ParticleData();
            particles[i].position = Random.insideUnitCircle * 10f;
            particles[i].radius.x = Random.Range(0.25f, 0.5f);
            particles[i].radius.y = 0f;
        }

        _particleBuf.SetData(particles);
        particles = null;

        // GPUInstancing用の引数バッファの作成
        _argsBuf = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
    }

    /// <summary>
    /// バッファの開放
    /// </summary>
    void ReleaseBuffer()
    {
        if (_particleBuf != null)
        {
            _particleBuf.Release();
            _particleBuf = null;
        }
        if (_argsBuf != null)
        {
            _argsBuf.Release();
            _argsBuf = null;
        }
    }

    /// <summary>
    /// ComputeShaderを介してシミュレート
    /// </summary>
    void Simulation()
    {
        ComputeShader cs = particleCS;
        int kernelID = -1;

        int threadGroupSize = Mathf.CeilToInt(maxObjectNum / SIMULATION_BLOCK_SIZE);

        if (cs.HasKernel("MainCS"))
        {
            kernelID = cs.FindKernel("MainCS");
            cs.SetFloat("_Time", Time.time);
            cs.SetBuffer(kernelID, "_ParticleDataBufferR", _particleBuf);
            cs.SetBuffer(kernelID, "_ParticleDataBuffer", _particleBuf);

            cs.Dispatch(kernelID, threadGroupSize, 1, 1);
        }
    }

    /// <summary>
    /// インスタンシングを利用してメッシュを描画する
    /// </summary>
    void RenderInstancedMesh()
    {
        if (instanceRenderMat == null || instancedMesh == null)
        {
            return;
        }

        // インスタン寝具用の引数データを作成してバッファに格納
        uint numIndices = instancedMesh.GetIndexCount(0);
        _args[0] = numIndices;
        _args[1] = (uint)maxObjectNum;
        _argsBuf.SetData(_args);

        // パーティクルバッファとマテリアルのヒモ付
        instanceRenderMat.SetBuffer("_ParticleDataBuffer", _particleBuf);

        // 境界領域を定義
        var bounds = new Bounds(
            new Vector3(0f, 0f),
            new Vector3(20f, 20f)
        );

        // メッシュをGPUインスタンシングして描画
        Graphics.DrawMeshInstancedIndirect(
            instancedMesh,
            0,
            instanceRenderMat,
            bounds,
            _argsBuf
        );
    }

    #endregion
}