namespace Seiro.ParticleSystem2D
{

    /// <summary>
    /// シミュレータに対して何かしら外部から処理を加えるためのインタフェース
    /// </summary>
    public interface IExternalStep
    {

        void Step(VSimulator sim);
    }
}