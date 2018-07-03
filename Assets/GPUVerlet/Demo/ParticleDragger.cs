using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{

    /// <summary>
    /// パーティクルをドラッグする
    /// </summary>
	[RequireComponent(typeof(VSimulator))]
    public class ParticleDragger : MonoBehaviour
    {

        [SerializeField, Range(0.1f, 5f)]
        float _range = 2f;

        VSimulator _sim;

        int _draggingID = -1;

        private void Awake()
        {
            _sim = GetComponent<VSimulator>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var pos = Input.mousePosition;
                pos.z = -Camera.main.transform.position.z;
                var wPos = Camera.main.ScreenToWorldPoint(pos);
                _draggingID = _sim.FindNearestParticle(wPos, _range);
            }

            if (Input.GetMouseButton(0) && _draggingID >= 0)
            {
                var pos = Input.mousePosition;
                pos.z = -Camera.main.transform.position.z;
                var wPos = Camera.main.ScreenToWorldPoint(pos);
                _sim.SetParticlePosition(_draggingID, wPos);
            }
        }
    }
}