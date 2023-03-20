
using System;

namespace Integrators
{
  public class RK4 : TIntegrator
  {
    private double lowestAcc;
    private double[] tempY;
    private double[] tempLocalVector;
    private double[] oneStepLocalVector;
    private double[] firstHalfStepLocalVector;
    private double[] secondHalfStepLocalVector;

    public override double Tolerance
    {
      get => base.Tolerance;
      set
      {
        base.Tolerance = value;
        this.lowestAcc = 0.025 * this.tolerance;
      }
    }

    protected internal override void Initialize()
    {
      base.Initialize();
      try
      {
        this.oneStepLocalVector = new double[(int) this.n];
        this.firstHalfStepLocalVector = new double[(int) this.n];
        this.secondHalfStepLocalVector = new double[(int) this.n];
        this.tempY = new double[(int) this.n];
        this.tempLocalVector = new double[(int) this.n];
      }
      catch (Exception ex)
      {
        throw new ApplicationException(ex.Message + "\nПрерывание в методе Initialize.");
      }
    }

    public RK4(ushort N, TEquations Equations)
      : base(N, Equations)
    {
    }

    public RK4(ushort N, TEquations Equations, CallBackEventHandler CallBack)
      : base(N, Equations, CallBack)
    {
    }

    protected internal virtual void DoStep(
      double tStart,
      double currStep,
      double[] yStart,
      double[] yEnd)
    {
      double num = 0.5 * currStep;
      try
      {
        this.equations(tStart, yStart, this.tempLocalVector);
        ++this.equationsCallNmb;
        for (ushort index = 0; (int) index < (int) this.n; ++index)
        {
          yEnd[(int) index] = num * this.tempLocalVector[(int) index];
          this.tempY[(int) index] = yStart[(int) index] + yEnd[(int) index];
        }
        this.equations(tStart + num, this.tempY, this.tempLocalVector);
        ++this.equationsCallNmb;
        for (ushort index = 0; (int) index < (int) this.n; ++index)
        {
          this.tempY[(int) index] = yStart[(int) index] + num * this.tempLocalVector[(int) index];
          yEnd[(int) index] = yEnd[(int) index] + currStep * this.tempLocalVector[(int) index];
        }
        this.equations(tStart + num, this.tempY, this.tempLocalVector);
        ++this.equationsCallNmb;
        for (ushort index = 0; (int) index < (int) this.n; ++index)
        {
          this.tempY[(int) index] = yStart[(int) index] + currStep * this.tempLocalVector[(int) index];
          yEnd[(int) index] = yEnd[(int) index] + currStep * this.tempLocalVector[(int) index];
        }
        this.equations(tStart + currStep, this.tempY, this.tempLocalVector);
        ++this.equationsCallNmb;
      }
      catch (Exception ex)
      {
        throw new ApplicationException(ex.Message + "\nОшибка при обращении к уравнениям!");
      }
      for (ushort index = 0; (int) index < (int) this.n; ++index)
      {
        yEnd[(int) index] = yStart[(int) index] + 1.0 / 3.0 * (yEnd[(int) index] + num * this.tempLocalVector[(int) index]);
        if (double.IsNaN(yEnd[(int) index]) || double.IsInfinity(yEnd[(int) index]))
          throw new ApplicationException("\nКомпонента NaN или Infinity в методе DoStep!");
      }
    }

    protected internal override bool BasicLoop(double tEnd)
    {
      try
      {
        double currStep1 = this.stepSize;
        if (this.callBack != null && this.DoCallBack())
          return false;
        while (true)
        {
          bool flag1;
          double num;
          do
          {
            bool flag2;
            if ((this.currt + 1.25 * currStep1 - tEnd) * (double) this.direction >= 0.0)
            {
              currStep1 = tEnd - this.currt;
              flag2 = true;
            }
            else
              flag2 = false;
            flag1 = true;
            this.DoStep(this.currt, currStep1, this.currY, this.oneStepLocalVector);
            bool flag3;
            do
            {
              double currStep2 = 0.5 * currStep1;
              this.DoStep(this.currt, currStep2, this.currY, this.firstHalfStepLocalVector);
              this.DoStep(this.currt + currStep2, currStep2, this.firstHalfStepLocalVector, this.secondHalfStepLocalVector);
              num = 0.0;
              for (ushort index = 0; (int) index < (int) this.n; ++index)
              {
                if ((!double.IsInfinity(1.0 / this.oneStepLocalVector[(int) index]) || !double.IsInfinity(1.0 / this.secondHalfStepLocalVector[(int) index])) && this.oneStepLocalVector[(int) index] != this.secondHalfStepLocalVector[(int) index])
                {
                  double d = Math.Abs(this.oneStepLocalVector[(int) index] - this.secondHalfStepLocalVector[(int) index]);
                  if (d > 1E-16)
                    d = 2.0 * d / (Math.Abs(this.oneStepLocalVector[(int) index]) + Math.Abs(this.secondHalfStepLocalVector[(int) index]));
                  if (double.IsInfinity(d) || double.IsNaN(d))
                    throw new ApplicationException("\nПроблема при вычислении maxDiv");
                  if (d > num)
                    num = d;
                }
              }
              if (num > this.tolerance)
              {
                flag1 = false;
                currStep1 = currStep2;
                this.stepSize = currStep2;
                flag2 = false;
                this.firstHalfStepLocalVector.CopyTo((Array) this.oneStepLocalVector, 0);
              }
              flag3 = this.currt == 0.0 && Math.Abs(currStep1) <= 1E-16 || Math.Abs(currStep1) <= Math.Abs(this.currt) * 1E-16;
            }
            while (num > this.tolerance && !flag3);
            if (flag3)
              throw new ApplicationException("\nШаг слишком мал");
            for (ushort index = 0; (int) index < (int) this.n; ++index)
              this.currY[(int) index] = this.secondHalfStepLocalVector[(int) index] + 1.0 / 15.0 * (this.secondHalfStepLocalVector[(int) index] - this.oneStepLocalVector[(int) index]);
            if (!flag2)
            {
              this.currt += currStep1;
              if (this.callBack != null && this.DoCallBack())
                return false;
            }
            else
              goto label_31;
          }
          while (!(num < this.lowestAcc & flag1));
          currStep1 += currStep1;
          if (Math.Abs(currStep1) > Math.Abs(tEnd - this.currt))
            currStep1 = tEnd - this.currt;
          this.stepSize = currStep1;
        }
label_31:
        this.currt += currStep1;
        return this.callBack == null || !this.DoCallBack();
      }
      catch (Exception ex)
      {
        throw new ApplicationException(ex.Message + "\nBasicLoop прерван");
      }
    }
  }
}
