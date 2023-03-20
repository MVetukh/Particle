

namespace Integrators
{
  public interface IIntegrator
  {
    ushort N { get; }

    TEquations Equations { get; }

    double this[ushort index] { get; set; }

    double Tolerance { get; set; }

    CallBackEventHandler CallBack { get; set; }

    bool IntegrateTo(double tEnd);
  }
}
