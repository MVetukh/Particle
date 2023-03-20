

using System;

namespace Integrators.MechSystems
{
  public class MechSystem : IMechSystem
  {
    private string name;
    private ushort S;
    private Propagator fq;
    private Propagator fp;
    private double time = double.NaN;
    private QP[] state;
    private IIntegrator integrator;
    public readonly SimpleEventHandler integratorCreated;
    private Type integratorClass;

    public string Name
    {
      set => this.name = value == null || value == string.Empty ? base.ToString() + " " + this.GetHashCode().ToString() : value;
      get => this.name;
    }

    public override string ToString() => this.name;

    public ushort s => this.S;

    public Propagator Fq => this.fq;

    public Propagator Fp => this.fp;

    private void Equations(double t, double[] y, double[] f)
    {
      for (ushort index = 0; (int) index < 2 * (int) this.S; ++index)
        f[(int) index] = ((int) index & 1) == 1 ? this.fp((ushort) ((uint) index / 2U), t, y) : this.fq((ushort) ((uint) index / 2U), t, y);
    }

    public double Time => this.time;

    public QP[] State => this.state;

    public IIntegrator Integrator => this.integrator;

    public Type IntegratorClass
    {
      get => this.integratorClass;
      set
      {
        if (this.fq == null)
          throw new ApplicationException("\nПравая часть уравнений изменения координат не задана?!!");
        if (this.fp == null)
          throw new ApplicationException("\nПравая часть уравнений изменения импульсов не задана?!!");
        if (value == (Type) null)
          throw new ApplicationException("\nКласс интегратора не задан?!!");
        if (value == this.IntegratorClass)
          return;
        this.integratorClass = value;
        this.integrator = (IIntegrator) Activator.CreateInstance(this.integratorClass, (object) (ushort) (2U * (uint) this.S), (object) new TEquations(this.Equations));
        if (this.integratorCreated == null)
          return;
        this.integratorCreated((object) this);
      }
    }

    public MechSystem(
      ushort s,
      Propagator fq,
      Propagator fp,
      Type IntegratorClass,
      string name,
      SimpleEventHandler integratorCreated)
    {
      if (s == (ushort) 0)
        throw new ApplicationException("\nЧисло степеней свободы равно нулю?!!");
      if (fq == null)
        throw new ApplicationException("\nПравая часть уравнений изменения координат не задана?!!");
      if (fp == null)
        throw new ApplicationException("\nПравая часть уравнений изменения импульсов не задана?!!");
      this.S = s;
      this.state = new QP[(int) this.S];
      this.fq = fq;
      this.fp = fp;
      this.Name = name;
      this.integratorCreated = integratorCreated;
      this.IntegratorClass = IntegratorClass;
    }

    public MechSystem(ushort s, Propagator fq, Propagator fp, Type IntegratorClass, string name)
      : this(s, fq, fp, IntegratorClass, name, (SimpleEventHandler) null)
    {
    }

    public MechSystem(ushort s, Propagator fq, Propagator fp, Type IntegratorClass)
      : this(s, fq, fp, IntegratorClass, (string) null, (SimpleEventHandler) null)
    {
    }

    public void BeginFrom(double startTime, QP[] startState)
    {
      this.time = this.integrator[(ushort) 0] = startTime;
      for (ushort index = 1; (int) index <= 2 * (int) this.S; ++index)
        this.integrator[index] = ((int) index & 1) == 1 ? (this.state[((int) index - 1) / 2].q = startState[((int) index - 1) / 2].q) : (this.state[((int) index - 1) / 2].p = startState[((int) index - 1) / 2].p);
    }

    public bool PropagateTo(double EndTime)
    {
      bool flag = this.integrator.IntegrateTo(EndTime);
      this.time = this.integrator[(ushort) 0];
      for (ushort index = 0; (int) index < 2 * (int) this.S; ++index)
      {
        if (((int) index & 1) == 1)
          this.state[(int) index / 2].p = this.integrator[(ushort) ((uint) index + 1U)];
        else
          this.state[(int) index / 2].q = this.integrator[(ushort) ((uint) index + 1U)];
      }
      return flag;
    }
  }
}
