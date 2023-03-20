

using System;

namespace Integrators
{
  public class LWDoPri5 : TIntegrator
  {
    protected internal const double c2 = 0.2;
    protected internal const double c3 = 0.3;
    protected internal const double c4 = 0.8;
    protected internal const double c5 = 0.888888888888889;
    protected internal const double a21 = 0.2;
    protected internal const double a31 = 0.075;
    protected internal const double a32 = 0.225;
    protected internal const double a41 = 0.977777777777778;
    protected internal const double a42 = -3.73333333333333;
    protected internal const double a43 = 3.55555555555556;
    protected internal const double a51 = 2.9525986892242;
    protected internal const double a52 = -11.5957933241884;
    protected internal const double a53 = 9.82289285169944;
    protected internal const double a54 = -0.290809327846365;
    protected internal const double a61 = 2.84627525252525;
    protected internal const double a62 = -10.7575757575758;
    protected internal const double a63 = 8.90642271774347;
    protected internal const double a64 = 0.278409090909091;
    protected internal const double a65 = -0.273531303602058;
    protected internal const double a71 = 0.0911458333333333;
    protected internal const double a73 = 0.449236298292902;
    protected internal const double a74 = 0.651041666666667;
    protected internal const double a75 = -0.322376179245283;
    protected internal const double a76 = 0.130952380952381;
    protected internal const double e1 = 0.00123263888888889;
    protected internal const double e3 = -0.00425277029050614;
    protected internal const double e4 = 0.0369791666666667;
    protected internal const double e5 = -0.0508637971698113;
    protected internal const double e6 = 0.0419047619047619;
    protected internal const double e7 = -0.025;
    protected internal const double d1 = -1.12701756538628;
    protected internal const double d3 = 2.6754244843516;
    protected internal const double d4 = -5.6855269615885;
    protected internal const double d5 = 3.52193236792079;
    protected internal const double d6 = -1.76728125707575;
    protected internal const double d7 = 2.38246893177814;
    public const ulong DefAllowedStepsMaxNmb = 100000;
    protected internal ushort iasti;
    protected internal ushort nonsti;
    protected internal ulong rejectedStepsNmb;
    protected internal ulong usedStepsNmb;
    protected internal ulong acceptedStepsNmb;
    protected internal ulong allowedStepsMaxNmb;
    protected internal double predictedStepSize;
    private const double stablStepSizeCtrl = 0.04;
    protected internal ushort stiffnessTestPrm = 1000;
    protected internal bool isDenseOutput;
    protected internal double oldX;
    private const double safetyFactor = 0.9;
    private const double facc1 = 5.0;
    private const double facc2 = 0.1;
    private const double expo1 = 0.17;
    protected internal double[] finalLocalVector;
    protected internal double[] finalY;
    protected internal double[] localVector3;
    protected internal double[] localVector4;
    protected internal double[] localVector5;
    protected internal double[] localVector6;
    protected internal double[] Fysti;

    protected internal static double sign(double a, double b) => b <= 0.0 ? -Math.Abs(a) : Math.Abs(a);

    public ulong RejectedStepsNmb => this.rejectedStepsNmb;

    public ulong UsedStepsNmb => this.usedStepsNmb;

    public ulong AcceptedStepsNmb => this.acceptedStepsNmb;

    public virtual ulong AllowedStepsMaxNmb
    {
      get => this.allowedStepsMaxNmb;
      set
      {
        if ((long) value == (long) this.AllowedStepsMaxNmb || value <= 100UL)
          return;
        this.allowedStepsMaxNmb = value;
        if ((ulong) this.stiffnessTestPrm <= this.allowedStepsMaxNmb)
          return;
        this.stiffnessTestPrm = (ushort) (this.allowedStepsMaxNmb - 10UL);
      }
    }

    public double PredictedStepSize => this.predictedStepSize;

    public LWDoPri5(ushort N, TEquations Equations)
      : base(N, Equations)
    {
    }

    public LWDoPri5(ushort N, TEquations Equations, CallBackEventHandler CallBack)
      : base(N, Equations, CallBack)
    {
    }

    protected internal override void Reset()
    {
      this.iasti = (ushort) 0;
      this.nonsti = (ushort) 0;
      this.usedStepsNmb = 0UL;
      this.acceptedStepsNmb = 0UL;
      this.rejectedStepsNmb = 0UL;
      base.Reset();
    }

    protected internal override void Initialize()
    {
      base.Initialize();
      try
      {
        this.finalLocalVector = new double[(int) this.n];
        this.finalY = new double[(int) this.n];
        this.localVector3 = new double[(int) this.n];
        this.localVector4 = new double[(int) this.n];
        this.localVector5 = new double[(int) this.n];
        this.localVector6 = new double[(int) this.n];
        this.Fysti = new double[(int) this.n];
      }
      catch
      {
        throw new ApplicationException("\nNot enough free memory for the method.");
      }
      this.AllowedStepsMaxNmb = 100000UL;
    }

    protected internal virtual void PrepareDenseOutputVectors(bool first1_4)
    {
    }

    protected internal virtual double ErrorEstimation()
    {
      double num1 = 0.0;
      for (ushort index = 0; (int) index < (int) this.n; ++index)
      {
        double d = 1.0 / (1E-16 + this.tolerance * Math.Max(Math.Abs(this.currY[(int) index]), Math.Abs(this.finalY[(int) index])));
        if (double.IsInfinity(d) || double.IsNaN(d))
          throw new ApplicationException("\nError estimation's problem in LWDoPri5!");
        double num2 = this.localVector4[(int) index] * d;
        num1 += num2 * num2;
      }
      return num1;
    }

    protected internal override bool BasicLoop(double tEnd)
    {
      double num1 = 0.0;
      double x = 0.0001;
      bool flag1 = false;
      bool flag2 = false;
      this.oldX = this.currt;
      while (this.usedStepsNmb <= this.allowedStepsMaxNmb)
      {
        if (this.currt != 0.0 && 0.1 * Math.Abs(this.stepSize) <= Math.Abs(this.currt) * 1E-16 || this.currt == 0.0 && 0.1 * Math.Abs(this.stepSize) <= 1E-16)
        {
          this.predictedStepSize = this.stepSize;
          throw new ApplicationException(string.Format("\nExit of dopri5 at x = {0}, step size too small = {1}.", (object) this.currt, (object) this.stepSize));
        }
        if ((this.currt + 1.01 * this.stepSize - tEnd) * (double) this.direction > 0.0)
        {
          this.stepSize = tEnd - this.currt;
          flag1 = true;
        }
        ++this.usedStepsNmb;
        for (ushort index = 0; (int) index < (int) this.n; ++index)
        {
          this.finalY[(int) index] = this.currY[(int) index] + this.stepSize * 0.2 * this.startLocalVector[(int) index];
          if (double.IsNaN(this.finalY[(int) index]) || double.IsInfinity(this.finalY[(int) index]))
            throw new ApplicationException("\nA component is NaN or Infinity!");
        }
        try
        {
          this.equations(this.currt + 0.2 * this.stepSize, this.finalY, this.finalLocalVector);
        }
        catch
        {
          throw new ApplicationException("\nSmth wrong in equations!");
        }
        for (ushort index = 0; (int) index < (int) this.n; ++index)
        {
          this.finalY[(int) index] = this.currY[(int) index] + this.stepSize * (0.075 * this.startLocalVector[(int) index] + 0.225 * this.finalLocalVector[(int) index]);
          if (double.IsNaN(this.finalY[(int) index]) || double.IsInfinity(this.finalY[(int) index]))
            throw new ApplicationException("\nA component is NaN or Infinity!");
        }
        try
        {
          this.equations(this.currt + 0.3 * this.stepSize, this.finalY, this.localVector3);
        }
        catch
        {
          throw new ApplicationException("\nSmth wrong in equations!");
        }
        for (ushort index = 0; (int) index < (int) this.n; ++index)
        {
          this.finalY[(int) index] = this.currY[(int) index] + this.stepSize * (44.0 / 45.0 * this.startLocalVector[(int) index] + -56.0 / 15.0 * this.finalLocalVector[(int) index] + 32.0 / 9.0 * this.localVector3[(int) index]);
          if (double.IsNaN(this.finalY[(int) index]) || double.IsInfinity(this.finalY[(int) index]))
            throw new ApplicationException("\nA component is NaN or Infinity!");
        }
        try
        {
          this.equations(this.currt + 0.8 * this.stepSize, this.finalY, this.localVector4);
        }
        catch
        {
          throw new ApplicationException("\nSmth wrong in equations!");
        }
        for (ushort index = 0; (int) index < (int) this.n; ++index)
        {
          this.finalY[(int) index] = this.currY[(int) index] + this.stepSize * (2.9525986892242 * this.startLocalVector[(int) index] + -11.5957933241884 * this.finalLocalVector[(int) index] + 9.82289285169944 * this.localVector3[(int) index] + -212.0 / 729.0 * this.localVector4[(int) index]);
          if (double.IsNaN(this.finalY[(int) index]) || double.IsInfinity(this.finalY[(int) index]))
            throw new ApplicationException("\nA component is NaN or Infinity!");
        }
        try
        {
          this.equations(this.currt + 8.0 / 9.0 * this.stepSize, this.finalY, this.localVector5);
        }
        catch
        {
          throw new ApplicationException("\nSmth wrong in equations!");
        }
        for (ushort index = 0; (int) index < (int) this.n; ++index)
        {
          this.Fysti[(int) index] = this.currY[(int) index] + this.stepSize * (2.84627525252525 * this.startLocalVector[(int) index] + -355.0 / 33.0 * this.finalLocalVector[(int) index] + 8.90642271774347 * this.localVector3[(int) index] + 49.0 / 176.0 * this.localVector4[(int) index] + -0.273531303602058 * this.localVector5[(int) index]);
          if (double.IsNaN(this.Fysti[(int) index]) || double.IsInfinity(this.Fysti[(int) index]))
            throw new ApplicationException("\nA component is NaN or Infinity!");
        }
        double t = this.currt + this.stepSize;
        try
        {
          this.equations(t, this.Fysti, this.localVector6);
        }
        catch
        {
          throw new ApplicationException("\nSmth wrong in equations!");
        }
        for (ushort index = 0; (int) index < (int) this.n; ++index)
        {
          this.finalY[(int) index] = this.currY[(int) index] + this.stepSize * (35.0 / 384.0 * this.startLocalVector[(int) index] + 0.449236298292902 * this.localVector3[(int) index] + 125.0 / 192.0 * this.localVector4[(int) index] + -0.322376179245283 * this.localVector5[(int) index] + 11.0 / 84.0 * this.localVector6[(int) index]);
          if (double.IsNaN(this.finalY[(int) index]) || double.IsInfinity(this.finalY[(int) index]))
            throw new ApplicationException("\nA component is NaN or Infinity!");
        }
        try
        {
          this.equations(t, this.finalY, this.finalLocalVector);
        }
        catch
        {
          throw new ApplicationException("\nSmth wrong in equations!");
        }
        if (this.isDenseOutput)
          this.PrepareDenseOutputVectors(false);
        for (ushort index = 0; (int) index < (int) this.n; ++index)
        {
          this.localVector4[(int) index] = this.stepSize * (0.00123263888888889 * this.startLocalVector[(int) index] + -0.00425277029050614 * this.localVector3[(int) index] + 0.0369791666666667 * this.localVector4[(int) index] + -0.0508637971698113 * this.localVector5[(int) index] + 22.0 / 525.0 * this.localVector6[(int) index] + -0.025 * this.finalLocalVector[(int) index]);
          if (double.IsNaN(this.localVector4[(int) index]) || double.IsInfinity(this.localVector4[(int) index]))
            throw new ApplicationException("\nA component is NaN or Infinity!");
        }
        this.equationsCallNmb += 6UL;
        double num2 = Math.Sqrt(this.ErrorEstimation() / (double) this.n);
        double num3 = Math.Pow(num2, 0.17);
        double num4 = this.stepSize / Math.Max(0.1, Math.Min(5.0, num3 / Math.Pow(x, 0.04) / 0.9));
        if (num2 <= 1.0)
        {
          x = Math.Max(num2, 0.0001);
          ++this.acceptedStepsNmb;
          if (this.acceptedStepsNmb % (ulong) this.stiffnessTestPrm == 0UL || this.iasti > (ushort) 0)
          {
            double num5 = 0.0;
            double num6 = 0.0;
            for (ushort index = 0; (int) index < (int) this.n; ++index)
            {
              double num7 = this.finalLocalVector[(int) index] - this.localVector6[(int) index];
              num5 += num7 * num7;
              double num8 = this.finalY[(int) index] - this.Fysti[(int) index];
              num6 += num8 * num8;
            }
            if (num6 > 0.0)
              num1 = this.stepSize * Math.Sqrt(num5 / num6);
            if (num1 > 3.25)
            {
              this.nonsti = (ushort) 0;
              ++this.iasti;
              if (this.iasti == (ushort) 15)
              {
                this.predictedStepSize = this.stepSize;
                throw new ApplicationException(string.Format("\nThe problem seems to become stiff at x = {0}!", (object) this.currt));
              }
            }
            else
            {
              ++this.nonsti;
              if (this.nonsti == (ushort) 6)
                this.iasti = (ushort) 0;
            }
          }
          if (this.isDenseOutput)
            this.PrepareDenseOutputVectors(true);
          this.finalLocalVector.CopyTo((Array) this.startLocalVector, 0);
          this.finalY.CopyTo((Array) this.currY, 0);
          this.oldX = this.currt;
          this.currt = t;
          if (this.callBack != null && this.DoCallBack())
            return false;
          if (flag1)
          {
            this.predictedStepSize = num4;
            return true;
          }
          if (Math.Abs(num4) > Math.Abs(tEnd - this.currt))
            num4 = tEnd - this.currt;
          if (flag2)
            num4 = (double) this.direction * Math.Min(Math.Abs(num4), Math.Abs(this.stepSize));
          flag2 = false;
        }
        else
        {
          num4 = this.stepSize / Math.Min(5.0, num3 / 0.9);
          flag2 = true;
          if (this.acceptedStepsNmb >= 1UL)
            ++this.rejectedStepsNmb;
          flag1 = false;
        }
        this.stepSize = num4;
      }
      this.predictedStepSize = this.stepSize;
      throw new ApplicationException(string.Format("\nExit of dopri5 at x ={0} , more than AllowedStepsMaxNmb = {1} are needed.", (object) this.currt, (object) this.AllowedStepsMaxNmb));
    }
  }
}
