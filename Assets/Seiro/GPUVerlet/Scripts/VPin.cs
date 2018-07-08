using UnityEngine;

namespace Seiro.GPUVerlet
{

    /// <summary>
    /// パーティクルを特定の座標に固定するピン
    /// </summary>
    public class VPin : IExternalStep
    {

        /// <summary>
        /// 固定するパーティクルのID
        /// </summary>
        int _particleID;

        /// <summary>
        /// パーティクルを固定する座標
        /// </summary>
        Vector2 _position;

        public VPin(int particleID, Vector2 position)
        {
            _particleID = particleID;
            _position = position;
        }

        void IExternalStep.Step(VSimulator sim)
        {
            sim.SetParticlePosition(_particleID, _position);
        }
    }
}