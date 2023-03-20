

using System;

namespace Integrators
{
  public abstract class TIntegrator : IIntegrator
  {
    public const double minTolerance = 1E-15;
    public const double maxTolerance = 0.01;
    public const double defTolerance = 1E-06;
    protected internal const double epsilon = 1E-16;
    protected internal ushort n;
    protected internal TEquations equations;
    protected internal double[] currY;
    protected internal double currt;
    protected internal double stepSize;
    protected internal ulong equationsCallNmb;
    protected internal double tolerance;
    protected internal double[] startLocalVector;
    protected internal sbyte direction;
    protected internal CallBackEventHandler callBack;

    public ushort N => this.n;

    public TEquations Equations => this.equations;

    public double StepSize => this.stepSize;

    public ulong EquationsCallNmb => this.equationsCallNmb;

    protected internal virtual void Reset()
    {
      this.stepSize = double.NaN;
      this.equationsCallNmb = 0UL;
    }

    public double this[ushort index]
    {
      get
      {
        if (index == (ushort) 0)
          return this.currt;
        return (int) index > (int) this.n ? double.NaN : this.currY[(int) index - 1];
      }
      set
      {
        if (index == (ushort) 0)
          this.currt = value;
        else if ((int) index <= (int) this.n)
          this.currY[(int) index - 1] = value;
        if (double.IsNaN(this.stepSize))
          return;
        this.Reset();
      }
    }

    public virtual double Tolerance
    {
      get => this.tolerance;
      set => this.tolerance = value > 0.01 ? 0.01 : (value < 1E-15 ? 1E-15 : value);
    }

    protected internal virtual void Initialize()
    {
      this.currY = new double[(int) this.n];
      for (ushort index = 0; (int) index < (int) this.n; ++index)
        this.currY[(int) index] = double.NaN;
      this.currt = double.NaN;
      this.startLocalVector = new double[(int) this.n];
      this.Tolerance = 1E-06;
    }

    public sbyte Direction => this.direction;

    public virtual CallBackEventHandler CallBack
    {
      get => this.callBack;
      set => this.callBack = value;
    }

    protected internal TIntegrator(ushort N, TEquations Equations, CallBackEventHandler CallBack)
    {
      this.n = N != (ushort) 0 ? N : throw new ApplicationException("\nЧисло уравнений равно нулю!?");
      this.equations = Equations != null ? Equations : throw new ApplicationException("\nУравнения не заданы!!!");
      this.Initialize();
      this.CallBack = CallBack;
    }

    protected internal TIntegrator(ushort N, TEquations Equations)
      : this(N, Equations, (CallBackEventHandler) null)
    {
    }

    protected internal virtual bool DoCallBack()
    {
      CallBackEventArgs e = new CallBackEventArgs();
      this.callBack((object) this, e);
      return e.Stop;
    }

    protected internal virtual void FirstEstimate(ref double df, ref double dy)
    {
      for (ushort index = 0; (int) index < (int) this.n; ++index)
      {
        double num1 = 1.0 / (1E-16 + this.tolerance * Math.Abs(this.currY[(int) index]));
        double num2 = this.startLocalVector[(int) index] * num1;
        df += num2 * num2;
        double num3 = this.currY[(int) index] * num1;
        dy += num3 * num3;
      }
    }

    protected internal virtual void SecondEstimate(ref double der2, double[] tempLocalVector)
    {
      for (ushort index = 0; (int) index < (int) this.n; ++index)
      {
        double num1 = 1.0 / (1E-16 + this.tolerance * Math.Abs(this.currY[(int) index]));
        double num2 = (tempLocalVector[(int) index] - this.startLocalVector[(int) index]) * num1;
        der2 += num2 * num2;
      }
    }

    private double StartStepSize(double MaxStepSize)
    {
      double df = 0.0;
      double dy = 0.0;
      this.FirstEstimate(ref df, ref dy);
      double num1 = Math.Abs(Math.Min(double.IsInfinity(df) || double.IsNaN(df) || df <= 1E-10 || double.IsInfinity(dy) || double.IsNaN(dy) || dy <= 1E-10 ? 1E-06 : Math.Sqrt(dy / df) * 0.01, MaxStepSize)) * (double) this.direction;
      double[] numArray = new double[(int) this.n];
      double[] y = new double[(int) this.n];
      for (ushort index = 0; (int) index < (int) this.n; ++index)
        y[(int) index] = this.currY[(int) index] + num1 * this.startLocalVector[(int) index];
      try
      {
        this.equations(this.currt + num1, y, numArray);
        ++this.equationsCallNmb;
      }
      catch
      {
        return double.NaN;
      }
      double der2 = 0.0;
      this.SecondEstimate(ref der2, numArray);
      double val1;
      if (double.IsInfinity(df) || double.IsNaN(df) || double.IsInfinity(der2) || double.IsNaN(der2))
      {
        val1 = Math.Max(1E-06, Math.Abs(num1) * 0.001);
      }
      else
      {
        der2 = Math.Sqrt(der2) / num1;
        if (double.IsInfinity(der2) || double.IsNaN(der2))
        {
          val1 = Math.Max(1E-06, Math.Abs(num1) * 0.001);
        }
        else
        {
          double num2 = Math.Max(Math.Abs(der2), Math.Sqrt(df));
          val1 = num2 > 1E-15 ? Math.Pow(0.01 / num2, 0.2) : Math.Max(1E-06, Math.Abs(num1) * 0.001);
        }
      }
      return Math.Abs(Math.Min(100.0 * num1, Math.Min(val1, MaxStepSize))) * (double) this.direction;
    }

    public bool IntegrateTo(double tEnd)
    {
      for (ushort index = 0; (int) index < (int) this.n + 1; ++index)
      {
        if (double.IsNaN(this[index]))
          throw new ApplicationException("\nНе заданы начальные условия интегрирования!");
      }
      if (tEnd - this.currt == 0.0)
        throw new ApplicationException("\nШаг интегрирования равен нулю?!");
      this.direction = tEnd - this.currt > 0.0 ? (sbyte) 1 : (sbyte) -1;
      try
      {
        this.Equations(this.currt, this.currY, this.startLocalVector);
        ++this.equationsCallNmb;
      }
      catch (Exception ex)
      {
        throw new ApplicationException(ex.Message + "\nОшибка при первом обращении к уравнениям!");
      }
      if (double.IsNaN(this.stepSize))
      {
        this.stepSize = this.StartStepSize(Math.Abs(tEnd - this.currt));
        if (double.IsNaN(this.stepSize) || double.IsInfinity(this.stepSize))
          throw new ApplicationException("\nОшибка при вычислении начального шага интегрирования!");
      }
      else
        this.stepSize = Math.Abs(this.stepSize) * (double) this.direction;
      return this.BasicLoop(tEnd);
    }

    protected internal abstract bool BasicLoop(double tEnd);
  }
}
