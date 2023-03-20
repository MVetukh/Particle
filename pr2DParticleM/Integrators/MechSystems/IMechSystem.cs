

using System;

namespace Integrators.MechSystems
{
  public interface IMechSystem
  {
    string Name { set; get; }

    ushort s { get; }

    double Time { get; }

    QP[] State { get; }

    Propagator Fq { get; }

    Propagator Fp { get; }

    Type IntegratorClass { get; set; }

    IIntegrator Integrator { get; }

    void BeginFrom(double startTime, QP[] startState);

    bool PropagateTo(double EndTime);
  }
}
