using GL;
using Integrators;
using Integrators.MechSystems;
using System;
using Steema.TeeChart;
using Steema.TeeChart.Drawing;
using Steema.TeeChart.Styles;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Panel = System.Windows.Forms.Panel;

namespace nsParticleMotion
{
  public class mainForm : Form
  {
    private const int _pRadius = 10;
    private Color _pStartColor = Color.Yellow;
    private Color[] _pEndColors = new Color[1]
    {
      Color.Black
    };
    private Rectangle _pRect = new Rectangle(new Point(), new Size(20, 20));
    private Size _ltOffset = new Size(-10, -10);
    private mainForm.Vector2D _Pos = new mainForm.Vector2D(0.5, 0.5);
    private mainForm.Vector2D _V;
    private double _vModMax = 0.2 * Math.Sqrt(0.006);
    private Point _curPPos;
    private double _time;
    private const double _intConst = 0.003;
    private double _eMin = -0.003;
    private double _eMax = 0.003;
    private double _nu;
    private int _alphaBlend = 240;
    private Color _startColorGrad = Color.Black;
    private Color _endColorGrad = Color.White;
    private RectangleF _window = new RectangleF(0.0f, 0.0f, 1f, 1f);
    private IMechSystem _mechSyst;
    private QP[] _startState = new QP[2];
    private const int _alphaMin = 50;
    private int _pbWidth;
    private int _pbHeight;
    private Bitmap _bkgrBMP;
    private List<mainForm.Vector2D> _pPath = new List<mainForm.Vector2D>();
    private Rectangle _pxlRect = new Rectangle(new Point(), new Size(1, 1));
    private const float _wheelScale = 0.001f;
    private bool _isCtrlDown;
    private Point _cursorLocation;
    private mainForm.MotionState _motionState;
    private const double _stepTime = 1.0;
    private const int _seriesLengthMax = 1000;
    private const float _eScale = 1.5f;
    private const int _pointStacks = 50;
    private const int _pointSlices = 50;
    private const int _defFStacks = 200;
    private const int _defFSlices = 200;
    private int _fStacks = 200;
    private int _fSlices = 200;
    private const float _radiusEye = 0.001f;
    private glPort _port;
    private static Color _clearColor = Color.FromArgb(0, 0, 51);
    private double _cameraFi;
    private double _cameraSinFi;
    private double _cameraCosFi;
    private double _cameraTeta;
    private double _cameraSinTeta;
    private double _cameraCosTeta;
    private float _3DpRadius = 0.03f;
    private float _3DeMax;
    private Point _3DCursorPos;
    private uint _pList;
    private bool _isPListCreated;
    private uint _fList;
    private bool _isFListCreated;
    private uint _eList;
    private bool _isEListCreated;
    private float[] _enrgDiffuseReflectanceColor = new float[4]
    {
      0.0f,
      1f,
      0.5f,
      0.5f
    };
    private float[] _fDiffuseReflectanceColor = new float[4]
    {
      0.8f,
      0.8f,
      0.8f,
      1f
    };
    private float[] _pDiffuseReflectanceColor = new float[4]
    {
      0.8f,
      0.8f,
      0.0f,
      1f
    };
    private float[] sinfi = new float[51];
    private float[] cosfi = new float[51];
    private float[] sinteta = new float[50];
    private float[] costeta = new float[50];
    private bool _isLineView;
    private IContainer components;
    private PictureBox pictureBox1;
    private Timer timer1;
    private ToolStripContainer toolStripContainer1;
    private ToolStrip toolStrip1;
    private ToolStripButton breakButton;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel stLabelXY;
    private ToolStripStatusLabel stLabelP;
    private ToolStrip toolStrip2;
    private ToolStripButton closeButton;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem helpItem;
    private ToolStripTextBox nuBox;
    private ContextMenuStrip cntxtMenu;
    private ToolStripTextBox vModBox;
    private ToolStripButton startButton;
    private Panel panelGL;
    private TChart pmChart;
    private FastLine xSeries;
    private FastLine ySeries;
    private FastLine eSeries;
    private ToolStripMenuItem viewMenu;
    private ToolStripMenuItem _2DItem;
    private ToolStripMenuItem chartItem;
    private ToolStripMenuItem _3DItem;
    
    private ToolStripMenuItem codeHelpItem;
    private ToolStripMenuItem designerCodeHelpItem;
    private PictureBox pictureBox2;
    private CheckBox trBox;
    private ToolTip toolTip1;

    public mainForm()
    {
      this.InitializeComponent();
      ((Control) this.pictureBox1).KeyDown += new KeyEventHandler(this.mainForm_KeyDown);
      ((Control) this.pictureBox1).KeyUp += new KeyEventHandler(this.mainForm_KeyUp);
      if (SystemInformation.MouseWheelPresent)
      {
        this.pictureBox1.MouseWheel += new MouseEventHandler(this.pictureBox1_MouseWheel);
        this.panelGL.MouseWheel += new MouseEventHandler(this.panelGL_MouseWheel);
      }
      this._mechSyst = (IMechSystem) new MechSystem((ushort) 2, (Propagator) ((i, time, y) => y[2 * (int) i + 1]), new Propagator(this.fp), typeof (LWDoPri5));
      this.nuBox.Text = this._nu.ToString("f");
      this.RefreshStatusLabel();
      this.Init3DPort();
      this.ResetPanelsSize();
    }

    private void ResetPanelsSize()
    {
      int height = this.toolStripContainer1.ContentPanel.ClientSize.Height;
      int width = this.toolStripContainer1.ContentPanel.ClientSize.Width;
      int num = height < width ? height : width;
      PictureBox pictureBox1 = this.pictureBox1;
      Panel panelGl = this.panelGL;
      TChart pmChart = this.pmChart;
      Rectangle rectangle1 = new Rectangle(new Point((width - num) / 2, (height - num) / 2), new Size(num, num));
      Rectangle rectangle2 = rectangle1;
      ((Control) pmChart).Bounds = rectangle2;
      Rectangle rectangle3;
      Rectangle rectangle4 = rectangle3 = rectangle1;
      panelGl.Bounds = rectangle3;
      Rectangle rectangle5 = rectangle4;
      pictureBox1.Bounds = rectangle5;
    }

    private void RefreshStatusLabel() => this.stLabelP.Text = string.Format(" point:time={0:f};X={1:f};Y={2:f};Vx={3:f};Vy={4:f};Energy={5:f4}", (object) this._time, (object) this._Pos.X, (object) this._Pos.Y, (object) this._V.X, (object) this._V.Y, (object) this.Energy(this._V, this._Pos, this._time));

    private void toolStripContainer1_ContentPanel_Resize(object sender, EventArgs e) => this.ResetPanelsSize();

    private void closeButton_Click(object sender, EventArgs e) => this.Close();

    private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      this._motionState = mainForm.MotionState.stop;
      this._port.Dispose();
    }

    private void _2DItem_Click(object sender, EventArgs e)
    {
      ((Control) this.pmChart).Visible = this.panelGL.Visible = false;
      this.trBox.Visible = this.pictureBox1.Visible = true;
    }

    private void chartItem_Click(object sender, EventArgs e)
    {
      this.trBox.Visible = this.pictureBox1.Visible = this.panelGL.Visible = false;
      ((Control) this.pmChart).Visible = true;
    }

    private void nuBox_Click(object sender, EventArgs e)
    {
      this.SetMotionState(mainForm.MotionState.stop);
      this.pictureBox1.Refresh();
    }

    private void nuBox_MouseLeave(object sender, EventArgs e) => this._nu = double.Parse(this.nuBox.Text);

    private void helpItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
      using (helpForm helpForm = new helpForm())
      {
        helpForm.itemClickedName = e.ClickedItem.Name;
        int num = (int) helpForm.ShowDialog();
      }
    }

    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
    {
      e.Cancel = this._motionState != mainForm.MotionState.stop || !this._pRect.IntersectsWith(new Rectangle(this.pictureBox1.PointToClient(Cursor.Position), new Size(1, 1)));
      if (e.Cancel)
        return;
      this.vModBox.Text = this._V.Mod.ToString("f");
    }

    private void vModBox_MouseLeave(object sender, EventArgs e)
    {
      this.Set_vMod();
      this.cntxtMenu.Close();
      this.Update_pbParticleImage(false);
    }

    private void Set_vMod()
    {
      double num;
      try
      {
        num = double.Parse(this.vModBox.Text);
      }
      catch
      {
        this.ErrorReaction("Набранная строка не является вещественным числом!!(Возможно,поставлена точка,вместо десятичной запятой?!)");
        return;
      }
      if (num < 0.0)
      {
        this.ErrorReaction("Модуль скорости не должен быть отрицательным!!");
      }
      else
      {
        this._V.Mod = num;
        this.RefreshStatusLabel();
      }
    }

    private void ErrorReaction(string message)
    {
      int num = (int) MessageBox.Show(message, Path.GetFileName(Application.ExecutablePath), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
      this.vModBox.Text = this._V.Mod.ToString("f");
    }

    private void break_Click(object sender, EventArgs e) => this.SetMotionState(this._motionState == mainForm.MotionState.run ? mainForm.MotionState.pause : mainForm.MotionState.run);

    private void startButton_Click(object sender, EventArgs e) => this.SetMotionState(this._motionState == mainForm.MotionState.stop ? mainForm.MotionState.run : mainForm.MotionState.stop);

    private void trBox_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.pictureBox1.Visible || this.trBox.Checked)
        return;
      this.pictureBox1.Refresh();
    }

    private double Energy(mainForm.Vector2D velocity, mainForm.Vector2D pos, double time) => 0.5 * velocity * velocity + this.PotentialEnergy(pos, time);

    private double PotentialEnergy(mainForm.Vector2D pos, double time) => 0.003 * Math.Cos(2.0 * Math.PI * this._nu * time) * Math.Sin(2.0 * Math.PI * pos.X) * Math.Cos(2.0 * Math.PI * pos.Y);

    private mainForm.Vector2D Force(mainForm.Vector2D pos, double time)
    {
      double num1 = 2.0 * Math.PI;
      double num2 = num1 * 0.003;
      double num3 = num1 * pos.X;
      double num4 = num1 * pos.Y;
      double num5 = Math.Cos(num1 * this._nu * time);
      return num2 * num5 * new mainForm.Vector2D(-Math.Cos(num3) * Math.Cos(num4), Math.Sin(num3) * Math.Sin(num4));
    }

    private double fp(ushort i, double time, double[] y) => i != (ushort) 0 ? this.Force(new mainForm.Vector2D(y[0], y[2]), time).Y : this.Force(new mainForm.Vector2D(y[0], y[2]), time).X;

    private void mainForm_KeyUp(object sender, KeyEventArgs e) => this._isCtrlDown = false;

    private void mainForm_KeyDown(object sender, KeyEventArgs e)
    {
      this._isCtrlDown = e.Control;
      if (e.KeyCode != Keys.Return)
        return;
      this.SetMotionState(this._motionState == mainForm.MotionState.stop || this._motionState == mainForm.MotionState.pause ? mainForm.MotionState.run : mainForm.MotionState.pause);
    }

    private void ResetPictureBoxBackground()
    {
      if (this._nu != 0.0)
        return;
      this.pictureBox2.Refresh();
      if (this._bkgrBMP != null)
        this._bkgrBMP.Dispose();
      this._bkgrBMP = new Bitmap(this._pbWidth, this._pbHeight);
      this.pictureBox2.DrawToBitmap(this._bkgrBMP, this.pictureBox2.ClientRectangle);
      this.pictureBox1.BackgroundImage = (Image) this._bkgrBMP;
    }

    private void pictureBox1_Resize(object sender, EventArgs e)
    {
      this.pictureBox2.ClientSize = this.pictureBox1.ClientSize;
      this._pbHeight = this.pictureBox1.ClientSize.Height;
      this._pbWidth = this.pictureBox1.ClientSize.Width;
      this.ResetPictureBoxBackground();
      this.ResetParticlePos();
      this.pictureBox1.Refresh();
    }

    private void pictureBox2_Paint(object sender, PaintEventArgs e)
    {
      using (HatchBrush hatchBrush = new HatchBrush(HatchStyle.Cross, Color.White))
        e.Graphics.FillRectangle((Brush) hatchBrush, this.pictureBox2.ClientRectangle);
      Blend blend = new Blend(this._pbHeight);
      float[] numArray1 = new float[this._pbHeight];
      for (int index = 0; index < this._pbHeight; ++index)
        numArray1[index] = (float) index / (float) (this._pbHeight - 1);
      blend.Positions = numArray1;
      float[] numArray2 = new float[this._pbHeight];
      mainForm.Vector2D pos = new mainForm.Vector2D();
      Rectangle rect = new Rectangle(0, 0, 1, this._pbHeight);
      for (int index1 = 0; index1 < this._pbWidth; ++index1)
      {
        pos.X = (double) this._window.Left + (double) this._window.Width * (double) index1 / (double) this._pbWidth;
        for (int index2 = 0; index2 < this._pbHeight; ++index2)
        {
          pos.Y = (double) this._window.Top + (double) this._window.Height * (double) numArray1[index2];
          double num = this.PotentialEnergy(pos, this._time);
          numArray2[index2] = num > this._eMax ? 1f : (num < this._eMin ? 0.0f : (float) ((num - this._eMin) / (this._eMax - this._eMin)));
        }
        blend.Factors = numArray2;
        using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rect, Color.FromArgb(this._alphaBlend, this._startColorGrad), Color.FromArgb(this._alphaBlend, this._endColorGrad), LinearGradientMode.Vertical))
        {
          linearGradientBrush.Blend = blend;
          e.Graphics.FillRectangle((Brush) linearGradientBrush, rect);
        }
        rect.Offset(1, 0);
      }
    }

    private void pictureBox1_Paint(object sender, PaintEventArgs e)
    {
      if (this._nu != 0.0)
      {
        using (HatchBrush hatchBrush = new HatchBrush(HatchStyle.Cross, Color.White))
          e.Graphics.FillRectangle((Brush) hatchBrush, this.pictureBox1.ClientRectangle);
        Blend blend = new Blend(this._pbHeight);
        float[] numArray1 = new float[this._pbHeight];
        for (int index = 0; index < this._pbHeight; ++index)
          numArray1[index] = (float) index / (float) (this._pbHeight - 1);
        blend.Positions = numArray1;
        float[] numArray2 = new float[this._pbHeight];
        mainForm.Vector2D pos = new mainForm.Vector2D();
        Rectangle rect = new Rectangle(0, 0, 1, this._pbHeight);
        for (int index1 = 0; index1 < this._pbWidth; ++index1)
        {
          pos.X = (double) this._window.Left + (double) this._window.Width * (double) index1 / (double) this._pbWidth;
          for (int index2 = 0; index2 < this._pbHeight; ++index2)
          {
            pos.Y = (double) this._window.Top + (double) this._window.Height * (double) numArray1[index2];
            double num = this.PotentialEnergy(pos, this._time);
            numArray2[index2] = num > this._eMax ? 1f : (num < this._eMin ? 0.0f : (float) ((num - this._eMin) / (this._eMax - this._eMin)));
          }
          blend.Factors = numArray2;
          using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rect, Color.FromArgb(this._alphaBlend, this._startColorGrad), Color.FromArgb(this._alphaBlend, this._endColorGrad), LinearGradientMode.Vertical))
          {
            linearGradientBrush.Blend = blend;
            e.Graphics.FillRectangle((Brush) linearGradientBrush, rect);
          }
          rect.Offset(1, 0);
        }
      }
      int alpha = (int) ((double) byte.MaxValue / (1.0 + this._V.Mod / this._vModMax));
      if (alpha < 50)
        alpha = 50;
      this._pStartColor = Color.FromArgb(alpha, this._pStartColor);
      this._pEndColors[0] = Color.FromArgb(alpha, this._pEndColors[0]);
      using (GraphicsPath path = new GraphicsPath())
      {
        path.AddEllipse(this._pRect);
        using (PathGradientBrush pathGradientBrush = new PathGradientBrush(path))
        {
          pathGradientBrush.CenterColor = this._pStartColor;
          pathGradientBrush.SurroundColors = this._pEndColors;
          e.Graphics.FillPath((Brush) pathGradientBrush, path);
        }
      }
      if (this._V.Mod != 0.0)
      {
        using (Pen pen = new Pen(Color.Red))
        {
          pen.EndCap = LineCap.ArrowAnchor;
          e.Graphics.DrawLine(pen, this._curPPos, this._curPPos + Size.Round(new SizeF(9.5f * (float) (this._V.X / this._V.Mod), 9.5f * (float) (this._V.Y / this._V.Mod))));
        }
      }
      if (this._motionState != mainForm.MotionState.run || !this.trBox.Checked || this._pPath.Count < 2)
        return;
      Point[] points = new Point[this._pPath.Count];
      for (int index3 = 0; index3 < this._pPath.Count; ++index3)
      {
        Point[] pointArray = points;
        int index4 = index3;
        mainForm.Vector2D vector2D = this._pPath[index3];
        double x = (vector2D.X - (double) this._window.Left) / (double) this._window.Width * (double) this._pbWidth;
        vector2D = this._pPath[index3];
        double y = (vector2D.Y - (double) this._window.Top) / (double) this._window.Height * (double) this._pbHeight;
        Point point = Point.Round(new PointF((float) x, (float) y));
        pointArray[index4] = point;
      }
      e.Graphics.DrawLines(Pens.Aqua, points);
    }

    private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
    {
      if (!this._isCtrlDown)
      {
        if (this._motionState != mainForm.MotionState.stop)
          return;
        if (this._V.Mod != 0.0 && this._pRect.IntersectsWith(new Rectangle(e.Location, new Size(1, 1))))
        {
          this._V.Azimuth -= Math.PI / 180.0 * (double) e.Delta / 40.0;
          this.RefreshStatusLabel();
          this.Update_pbParticleImage(false);
        }
        else
        {
          if (this._nu == 0.0)
            return;
          this._time += (double) e.Delta * 0.001;
          this.RefreshStatusLabel();
          this.Update_pbParticleImage(true);
        }
      }
      else
      {
        this._window.Inflate(new SizeF((float) ((double) this._window.Width * (double) e.Delta * (1.0 / 1000.0)), (float) ((double) this._window.Height * (double) e.Delta * (1.0 / 1000.0))));
        this.ResetPictureBoxBackground();
        this.ResetParticlePos();
        this.pictureBox1.Refresh();
        this._isFListCreated = false;
      }
    }

    private void pictureBox1_MouseEnter(object sender, EventArgs e)
    {
      this.pictureBox1.Focus();
      this.stLabelXY.Visible = true;
    }

    private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
    {
      this.stLabelXY.Text = string.Format("X={0};Y={1}", (object) (float) ((double) this._window.Left + (double) this._window.Width * (double) e.X / (double) this._pbWidth), (object) (float) ((double) this._window.Top + (double) this._window.Height * (double) e.Y / (double) this._pbHeight));
      if (e.Button != MouseButtons.Left || !this._isCtrlDown)
        return;
      this._window.Offset(new PointF(this._window.Width * (float) (this._cursorLocation.X - e.X) / (float) this._pbWidth, this._window.Height * (float) (this._cursorLocation.Y - e.Y) / (float) this._pbHeight));
      this._isFListCreated = false;
      this.ResetPictureBoxBackground();
      this.ResetParticlePos();
      this.pictureBox1.Refresh();
      this._cursorLocation = e.Location;
    }

    private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      if (this._motionState != mainForm.MotionState.pause && !this._isCtrlDown)
      {
        this.SetMotionState(mainForm.MotionState.stop);
        this.pictureBox1.Invalidate(this._pRect);
        this._Pos.X = (double) this._window.Left + (double) this._window.Width * (double) e.X / (double) this._pbWidth;
        this._Pos.Y = (double) this._window.Top + (double) this._window.Height * (double) e.Y / (double) this._pbHeight;
        this.RefreshStatusLabel();
        this._curPPos = e.Location;
        this._pRect.Location = e.Location + this._ltOffset;
        this.Update_pbParticleImage(false);
      }
      this._cursorLocation = e.Location;
    }

    private void pictureBox1_MouseLeave(object sender, EventArgs e) => this.stLabelXY.Visible = false;

    private void SetMotionState(mainForm.MotionState state)
    {
      bool flag = this._motionState == mainForm.MotionState.stop;
      this._motionState = state;
      switch (state)
      {
        case mainForm.MotionState.stop:
          this._pPath.Clear();
          this.breakButton.Enabled = false;
          this.nuBox.Enabled = true;
          this.startButton.Text = "Start";
          this._V.X = this._V.Y = 0.0;
          this._time = 0.0;
          this.RefreshStatusLabel();
          foreach (Series series in (CollectionBase) this.pmChart.Series)
            series.Clear();
          this.Update_pbParticleImage(true);
          this._isEListCreated = false;
          break;
        case mainForm.MotionState.pause:
          this.startButton.Enabled = false;
          this.breakButton.Text = "GoOn";
          this.Update_pbParticleImage(false);
          break;
        case mainForm.MotionState.run:
          if (this.pictureBox1.Visible)
            this.pictureBox1.Invalidate(this._pRect);
          this.startButton.Enabled = this.breakButton.Enabled = true;
          this.nuBox.Enabled = false;
          this.startButton.Text = "Stop";
          this.breakButton.Text = "Break";
          if (flag)
            this.Restart();
          this.timer1.Enabled = true;
          break;
      }
    }

    private void Update_pbParticleImage(bool refresh)
    {
      if (!this.pictureBox1.Visible)
        return;
      if (refresh)
      {
        this.ResetPictureBoxBackground();
        this.pictureBox1.Refresh();
      }
      else
      {
        this.pictureBox1.Invalidate(this._pRect);
        this.pictureBox1.Update();
      }
    }

    private void Restart()
    {
      this._startState[0].q = this._Pos.X;
      this._startState[0].p = this._V.X;
      this._startState[1].q = this._Pos.Y;
      this._startState[1].p = this._V.Y;
      this._mechSyst.BeginFrom(this._time, this._startState);
    }

    private void ResetParticlePos()
    {
      this._curPPos = Point.Round(new PointF(((float) this._Pos.X - this._window.Left) / this._window.Width * (float) this._pbWidth, ((float) this._Pos.Y - this._window.Top) / this._window.Height * (float) this._pbHeight));
      this._pRect.Location = this._curPPos + this._ltOffset;
    }

    private void DoStep()
    {
      ++this._time;
      this._mechSyst.PropagateTo(this._time);
      this._V.X = this._mechSyst.State[0].p;
      this._V.Y = this._mechSyst.State[1].p;
      this._Pos.X = this._mechSyst.State[0].q;
      this._Pos.Y = this._mechSyst.State[1].q;
      this.RefreshStatusLabel();
      if (((Series) this.xSeries).Count < 1000)
      {
        ((Series) this.xSeries).Add(this._time, this._Pos.X);
        ((Series) this.ySeries).Add(this._time, this._Pos.Y);
        ((Series) this.eSeries).Add(this._time, this.Energy(this._V, this._Pos, this._time));
      }
      if (this.pictureBox1.Visible && this._nu == 0.0)
        this.pictureBox1.Invalidate(this._pRect);
      this._pPath.Add(this._Pos);
      this.ResetParticlePos();
      this.Update_pbParticleImage(this._nu != 0.0);
      while (!this.pictureBox1.ClientRectangle.IntersectsWith(this._pRect))
      {
        this._window.Inflate(new SizeF(0.1f * this._window.Width, 0.1f * this._window.Height));
        this._pRect.Location = Point.Round(new PointF(((float) this._Pos.X - this._window.Left) / this._window.Width * (float) this._pbWidth, ((float) this._Pos.Y - this._window.Top) / this._window.Height * (float) this._pbHeight)) + this._ltOffset;
        this._isFListCreated = false;
        if (this.pictureBox1.Visible)
        {
          this.ResetPictureBoxBackground();
          this.pictureBox1.Refresh();
        }
      }
      if (!this.panelGL.Visible)
        return;
      this.RenderFrame();
    }

    private void run()
    {
      while (this._motionState == mainForm.MotionState.run)
      {
        this.DoStep();
        Application.DoEvents();
      }
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      this.timer1.Enabled = false;
      this.run();
    }

    private void panelGL_Paint(object sender, PaintEventArgs e) => this.RenderFrame();

    private void panelGL_SizeChanged(object sender, EventArgs e)
    {
      if (this._port == null)
        return;
      gl.Viewport(0, 0, this.panelGL.ClientSize.Width, this.panelGL.ClientSize.Height);
      this.InitProjection();
      if (!this.panelGL.Visible)
        return;
      this.RenderFrame();
    }

    private void panelGL_MouseEnter(object sender, EventArgs e) => this.panelGL.Focus();

    private void panelGL_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
        this._3DCursorPos = e.Location;
      if (e.Button != MouseButtons.Right)
        return;
      this._isLineView = !this._isLineView;
      this.RenderFrame();
    }

    private void panelGL_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left || !this.panelGL.ClientRectangle.Contains(e.Location))
        return;
      this._cameraFi += 2.0 * (double) (e.X - this._3DCursorPos.X) / (double) this.panelGL.ClientSize.Width * Math.PI;
      this._cameraTeta += -1.0 * (double) (e.Y - this._3DCursorPos.Y) / (double) this.panelGL.ClientSize.Height * Math.PI;
      this._3DCursorPos = e.Location;
      this._cameraCosTeta = Math.Cos(this._cameraTeta);
      this._cameraSinTeta = Math.Sin(this._cameraTeta);
      this._cameraSinFi = Math.Sin(this._cameraFi);
      this._cameraCosFi = Math.Cos(this._cameraFi);
      this.RenderFrame();
    }

    private void panelGL_MouseWheel(object sender, MouseEventArgs e)
    {
      this._window.Inflate(new SizeF((float) ((double) this._window.Width * (double) e.Delta * (1.0 / 1000.0)), (float) ((double) this._window.Height * (double) e.Delta * (1.0 / 1000.0))));
      this._fStacks = (int) (200.0 * (double) this._window.Width);
      this._fSlices = (int) (200.0 * (double) this._window.Height);
      this._isFListCreated = false;
      this.RenderFrame();
    }

    private void _3DItem_Click(object sender, EventArgs e)
    {
      this.trBox.Visible = this.pictureBox1.Visible = ((Control) this.pmChart).Visible = false;
      this.panelGL.Visible = true;
    }

    private void Init3DPort()
    {
      for (int index = 0; index <= 50; ++index)
      {
        this.sinfi[index] = (float) Math.Sin(2.0 * Math.PI * (double) index / 50.0);
        this.cosfi[index] = (float) Math.Cos(2.0 * Math.PI * (double) index / 50.0);
      }
      for (int index = 0; index < 50; ++index)
      {
        this.sinteta[index] = (float) Math.Sin(Math.PI * (double) index / 50.0);
        this.costeta[index] = (float) Math.Cos(Math.PI * (double) index / 50.0);
      }
      this._3DeMax = 1.5f * (float) Math.Max(Math.Abs(this._eMax), Math.Abs(this._eMin));
      this._port = new glPort((Control) this.panelGL);
      gl.ClearColor((float) mainForm._clearColor.R / (float) byte.MaxValue, (float) mainForm._clearColor.G / (float) byte.MaxValue, (float) mainForm._clearColor.B / (float) byte.MaxValue, 1f);
      gl.Enable(2929);
      gl.Enable(2896);
      gl.Enable(16384);
      gl.Enable(3042);
      gl.BlendFunc(770, 771);
      gl.LightModel(2898, 1);
      gl.LightModel(2897, 1);
      gl.Enable(2832);
      gl.Enable(2977);
      gl.Enable(2884);
      gl.Material(1032, 4609, this._fDiffuseReflectanceColor);
      this.InitializeSceneViewParameters();
    }

    private void InitializeSceneViewParameters()
    {
      this._cameraTeta = 0.0;
      this._cameraFi = 0.0;
      this._cameraCosTeta = Math.Cos(this._cameraTeta);
      this._cameraSinTeta = Math.Sin(this._cameraTeta);
      this._cameraSinFi = Math.Sin(this._cameraFi);
      this._cameraCosFi = Math.Cos(this._cameraFi);
    }

    private void InitProjection()
    {
      double num1 = (double) gl.Get(2978, 4)[2];
      double num2 = (double) gl.Get(2978, 4)[3];
      this._isFListCreated = this._isPListCreated = false;
      gl.MatrixMode(5889);
      gl.LoadIdentity();
      gl.Ortho(-1.0, 1.0, -1.0, 1.0, -1.0, 1.0);
      gl.MatrixMode(5888);
      gl.LoadIdentity();
    }

    private void BuildFrame()
    {
      gl.Clear(16640);
      gl.LoadIdentity();
      gl.LookAt(1.0 / 1000.0 * this._cameraCosTeta * this._cameraSinFi, 1.0 / 1000.0 * this._cameraSinTeta, 1.0 / 1000.0 * this._cameraCosTeta * this._cameraCosFi, 0.0, 0.0, 0.0, -this._cameraSinTeta * this._cameraSinFi, this._cameraCosTeta, -this._cameraSinTeta * this._cameraCosFi);
      this.RenderScene();
      gl.Finish();
    }

    private void RenderFrame()
    {
      this.BuildFrame();
      if (!this._port.SwapBuffers())
        throw new Exception("Не выполнен обмен буферов.");
      gl.Flush();
    }

    private void CreateNCallList(bool isCreatedList, uint listNmb, mainForm.CreateList createList)
    {
      if (!isCreatedList)
        createList();
      else
        gl.CallList(listNmb);
    }

    private void CreateFieldList()
    {
      float num1 = 2f / (float) this._fStacks;
      float num2 = -2f / (float) this._fSlices;
      if (gl.IsList(this._fList))
        gl.DeleteLists(this._fList, 1);
      this._fList = gl.GenLists(1);
      gl.NewList(this._fList, 4865);
      gl.PushAttrib(64);
      gl.Material(1032, 4609, this._fDiffuseReflectanceColor);
      mainForm.Vector2D pos = new mainForm.Vector2D();
      for (int index1 = 0; index1 < this._fSlices; ++index1)
      {
        float num3 = (float) index1 / (float) this._fSlices;
        pos.Y = (double) this._window.Top + (double) this._window.Height * (double) num3;
        float y = (float) pos.Y + this._window.Height / (float) this._fSlices;
        gl.Begin(5);
        for (int index2 = 0; index2 <= this._fStacks; ++index2)
        {
          float num4 = (float) index2 / (float) this._fStacks;
          pos.X = (double) this._window.Left + (double) this._window.Width * (double) num4;
          float x = (float) pos.X + this._window.Width / (float) this._fStacks;
          float z1 = (float) this.PotentialEnergy(pos, this._time) / this._3DeMax;
          float z2 = (float) this.PotentialEnergy(new mainForm.Vector2D(pos.X, (double) y), this._time) / this._3DeMax;
          float num5 = (float) this.PotentialEnergy(new mainForm.Vector2D((double) x, pos.Y), this._time) / this._3DeMax - z1;
          float num6 = z2 - z1;
          gl.Normal(num2 * num5, num1 * num6, -num1 * num2);
          gl.Vertex((float) (2.0 * (double) num4 - 1.0), (float) (1.0 - 2.0 * (double) num3), z1);
          gl.Vertex((float) (2.0 * (double) num4 - 1.0), (float) (1.0 - 2.0 * (double) num3) + num2, z2);
        }
        gl.End();
      }
      gl.PopAttrib();
      gl.EndList();
      this._isFListCreated = true;
    }

    private void CreatePList()
    {
      if (gl.IsList(this._pList))
        gl.DeleteLists(this._pList, 1);
      this._pList = gl.GenLists(1);
      gl.NewList(this._pList, 4865);
      gl.PushAttrib(64);
      gl.Material(1028, 4609, this._pDiffuseReflectanceColor);
      this.Sphere(this._3DpRadius);
      gl.PopAttrib();
      gl.EndList();
      this._isPListCreated = true;
    }

    private void CreateEnrgList()
    {
      if (gl.IsList(this._eList))
        gl.DeleteLists(this._eList, 1);
      this._eList = gl.GenLists(1);
      gl.NewList(this._eList, 4865);
      gl.PushAttrib(8256);
      gl.Disable(2884);
      gl.Material(1032, 4609, this._enrgDiffuseReflectanceColor);
      gl.Begin(7);
      float z = (float) this.Energy(this._V, this._Pos, this._time) / this._3DeMax;
      gl.Vertex(-1f, -1f, z);
      gl.Vertex(1f, -1f, z);
      gl.Vertex(1f, 1f, z);
      gl.Vertex(-1f, 1f, z);
      gl.End();
      gl.PopAttrib();
      gl.EndList();
      this._isEListCreated = true;
    }

    private void Sphere(float radius)
    {
      if ((double) radius <= 0.0)
        return;
      gl.Begin(6);
      gl.Normal(0.0f, radius, 0.0f);
      gl.Vertex(0.0f, radius, 0.0f);
      for (int index = 0; index <= 50; ++index)
      {
        gl.Normal(radius * this.sinteta[1] * this.sinfi[index], radius * this.costeta[1], radius * this.sinteta[1] * this.cosfi[index]);
        gl.Vertex(radius * this.sinteta[1] * this.sinfi[index], radius * this.costeta[1], radius * this.sinteta[1] * this.cosfi[index]);
      }
      gl.End();
      for (int index1 = 1; index1 < 49; ++index1)
      {
        gl.Begin(5);
        for (int index2 = 0; index2 <= 50; ++index2)
        {
          gl.Normal(radius * this.sinteta[index1] * this.sinfi[index2], radius * this.costeta[index1], radius * this.sinteta[index1] * this.cosfi[index2]);
          gl.Vertex(radius * this.sinteta[index1] * this.sinfi[index2], radius * this.costeta[index1], radius * this.sinteta[index1] * this.cosfi[index2]);
          gl.Normal(radius * this.sinteta[index1 + 1] * this.sinfi[index2], radius * this.costeta[index1 + 1], radius * this.sinteta[index1 + 1] * this.cosfi[index2]);
          gl.Vertex(radius * this.sinteta[index1 + 1] * this.sinfi[index2], radius * this.costeta[index1 + 1], radius * this.sinteta[index1 + 1] * this.cosfi[index2]);
        }
        gl.End();
      }
      gl.Begin(6);
      gl.Normal(0.0f, -radius, 0.0f);
      gl.Vertex(0.0f, -radius, 0.0f);
      for (int index = 50; index >= 0; --index)
      {
        gl.Normal(radius * this.sinteta[49] * this.sinfi[index], radius * this.costeta[49], radius * this.sinteta[49] * this.cosfi[index]);
        gl.Vertex(radius * this.sinteta[49] * this.sinfi[index], radius * this.costeta[49], radius * this.sinteta[49] * this.cosfi[index]);
      }
      gl.End();
    }

    private void RenderScene()
    {
      gl.PushAttrib(8);
      gl.PolygonMode(1028, this._isLineView ? 6913 : 6914);
      this.CreateNCallList(this._isFListCreated, this._fList, new mainForm.CreateList(this.CreateFieldList));
      gl.PopAttrib();
      gl.PushMatrix();
      gl.Translate((float) (2.0 * (this._Pos.X - (double) this._window.Left) / (double) this._window.Width - 1.0), (float) (1.0 - 2.0 * (this._Pos.Y - (double) this._window.Top) / (double) this._window.Height), (float) this.Energy(this._V, this._Pos, this._time) / this._3DeMax);
      this.CreateNCallList(this._isPListCreated, this._pList, new mainForm.CreateList(this.CreatePList));
      gl.PopMatrix();
      if (this._nu != 0.0)
        return;
      this.CreateNCallList(this._isEListCreated, this._eList, new mainForm.CreateList(this.CreateEnrgList));
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (mainForm));
      this.pictureBox1 = new PictureBox();
      this.cntxtMenu = new ContextMenuStrip(this.components);
      this.vModBox = new ToolStripTextBox();
      this.timer1 = new Timer(this.components);
      this.toolStripContainer1 = new ToolStripContainer();
      this.statusStrip1 = new StatusStrip();
      this.stLabelXY = new ToolStripStatusLabel();
      this.stLabelP = new ToolStripStatusLabel();
      this.trBox = new CheckBox();
      this.pictureBox2 = new PictureBox();
      this.pmChart = new TChart();
      this.xSeries = new FastLine();
      this.ySeries = new FastLine();
      this.eSeries = new FastLine();
      this.panelGL = new System.Windows.Forms.Panel();
      this.toolStrip1 = new ToolStrip();
      this.breakButton = new ToolStripButton();
      this.startButton = new ToolStripButton();
      this.toolStrip2 = new ToolStrip();
      this.closeButton = new ToolStripButton();
      this.menuStrip1 = new MenuStrip();
      this.viewMenu = new ToolStripMenuItem();
      this._2DItem = new ToolStripMenuItem();
      this.chartItem = new ToolStripMenuItem();
      this._3DItem = new ToolStripMenuItem();
      this.helpItem = new ToolStripMenuItem();
      
      this.codeHelpItem = new ToolStripMenuItem();
      this.designerCodeHelpItem = new ToolStripMenuItem();
      this.nuBox = new ToolStripTextBox();
      this.toolTip1 = new ToolTip(this.components);
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.cntxtMenu.SuspendLayout();
      this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
      this.toolStripContainer1.ContentPanel.SuspendLayout();
      this.toolStripContainer1.LeftToolStripPanel.SuspendLayout();
      this.toolStripContainer1.RightToolStripPanel.SuspendLayout();
      this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
      this.toolStripContainer1.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      ((ISupportInitialize) this.pictureBox2).BeginInit();
      this.toolStrip1.SuspendLayout();
      this.toolStrip2.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      this.pictureBox1.BackColor = Color.FromArgb(0, 0, 64);
      this.pictureBox1.ContextMenuStrip = this.cntxtMenu;
      this.pictureBox1.Location = new Point(140, 130);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(510, 409);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.Paint += new PaintEventHandler(this.pictureBox1_Paint);
      this.pictureBox1.MouseDown += new MouseEventHandler(this.pictureBox1_MouseDown);
      this.pictureBox1.MouseEnter += new EventHandler(this.pictureBox1_MouseEnter);
      this.pictureBox1.MouseLeave += new EventHandler(this.pictureBox1_MouseLeave);
      this.pictureBox1.MouseMove += new MouseEventHandler(this.pictureBox1_MouseMove);
      this.pictureBox1.Resize += new EventHandler(this.pictureBox1_Resize);
      this.cntxtMenu.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.vModBox
      });
      this.cntxtMenu.Name = "contextMenuStrip1";
      this.cntxtMenu.Size = new Size(161, 29);
      this.cntxtMenu.Opening += new CancelEventHandler(this.contextMenuStrip1_Opening);
      this.vModBox.Font = new Font("Segoe UI", 9f);
      this.vModBox.Name = "vModBox";
      this.vModBox.Size = new Size(100, 23);
      this.vModBox.Text = "0";
      this.vModBox.ToolTipText = "Модуль скорости частицы";
      this.vModBox.MouseLeave += new EventHandler(this.vModBox_MouseLeave);
      this.timer1.Interval = 10;
      this.timer1.Tick += new EventHandler(this.timer1_Tick);
      this.toolStripContainer1.BottomToolStripPanel.Controls.Add((Control) this.statusStrip1);
      this.toolStripContainer1.ContentPanel.BackColor = SystemColors.Control;
      this.toolStripContainer1.ContentPanel.Controls.Add((Control) this.trBox);
      this.toolStripContainer1.ContentPanel.Controls.Add((Control) this.pictureBox2);
      this.toolStripContainer1.ContentPanel.Controls.Add((Control) this.pmChart);
      this.toolStripContainer1.ContentPanel.Controls.Add((Control) this.panelGL);
      this.toolStripContainer1.ContentPanel.Controls.Add((Control) this.pictureBox1);
      this.toolStripContainer1.ContentPanel.Size = new Size(688, 624);
      this.toolStripContainer1.ContentPanel.Resize += new EventHandler(this.toolStripContainer1_ContentPanel_Resize);
      this.toolStripContainer1.Dock = DockStyle.Fill;
      this.toolStripContainer1.LeftToolStripPanel.Controls.Add((Control) this.toolStrip1);
      this.toolStripContainer1.Location = new Point(0, 0);
      this.toolStripContainer1.Name = "toolStripContainer1";
      this.toolStripContainer1.RightToolStripPanel.Controls.Add((Control) this.toolStrip2);
      this.toolStripContainer1.Size = new Size(770, 673);
      this.toolStripContainer1.TabIndex = 2;
      this.toolStripContainer1.Text = "toolStripContainer1";
      this.toolStripContainer1.TopToolStripPanel.Controls.Add((Control) this.menuStrip1);
      this.statusStrip1.Dock = DockStyle.None;
      this.statusStrip1.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.stLabelXY,
        (ToolStripItem) this.stLabelP
      });
      this.statusStrip1.Location = new Point(0, 0);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new Size(770, 22);
      this.statusStrip1.TabIndex = 0;
      this.stLabelXY.Name = "stLabelXY";
      this.stLabelXY.Size = new Size(0, 17);
      this.stLabelP.Name = "stLabelP";
      this.stLabelP.Size = new Size(0, 17);
      this.trBox.AutoSize = true;
      this.trBox.Checked = true;
      this.trBox.CheckState = CheckState.Checked;
      this.trBox.Location = new Point(3, 3);
      this.trBox.Name = "trBox";
      this.trBox.Size = new Size(62, 17);
      this.trBox.TabIndex = 4;
      this.trBox.Text = "Tracing";
      this.toolTip1.SetToolTip((Control) this.trBox, "Изображение траектории частицы");
      this.trBox.UseVisualStyleBackColor = true;
      this.trBox.CheckedChanged += new EventHandler(this.trBox_CheckedChanged);
      this.pictureBox2.BackColor = Color.FromArgb(0, 0, 64);
      this.pictureBox2.Location = new Point(143, 57);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new Size(100, 50);
      this.pictureBox2.TabIndex = 3;
      this.pictureBox2.TabStop = false;
      this.pictureBox2.Visible = false;
      this.pictureBox2.Paint += new PaintEventHandler(this.pictureBox2_Paint);
      this.pmChart.Aspect.ElevationFloat = 345.0;
      this.pmChart.Aspect.RotationFloat = 345.0;
      this.pmChart.Aspect.View3D = false;
      this.pmChart.Axes.Bottom.Automatic = true;
      ((ChartPen) this.pmChart.Axes.Bottom.Grid).Style = DashStyle.Dash;
      this.pmChart.Axes.Bottom.Grid.ZPosition = 0.0;
      ((TextShape) this.pmChart.Axes.Bottom.Labels).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Bottom.Labels).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.Bottom.Labels).Shadow.Visible = false;
      this.pmChart.Axes.Bottom.Title.Caption = "Time";
      ((TextShape) this.pmChart.Axes.Bottom.Title).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Bottom.Title).Font.Unit = GraphicsUnit.World;
      ((TextShape) this.pmChart.Axes.Bottom.Title).Lines = new string[1]
      {
        "Time"
      };
      ((Shape) this.pmChart.Axes.Bottom.Title).Shadow.Visible = false;
      this.pmChart.Axes.Depth.Automatic = true;
      ((ChartPen) this.pmChart.Axes.Depth.Grid).Style = DashStyle.Dash;
      this.pmChart.Axes.Depth.Grid.ZPosition = 0.0;
      ((TextShape) this.pmChart.Axes.Depth.Labels).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Depth.Labels).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.Depth.Labels).Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Depth.Title).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Depth.Title).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.Depth.Title).Shadow.Visible = false;
      this.pmChart.Axes.DepthTop.Automatic = true;
      ((ChartPen) this.pmChart.Axes.DepthTop.Grid).Style = DashStyle.Dash;
      this.pmChart.Axes.DepthTop.Grid.ZPosition = 0.0;
      ((TextShape) this.pmChart.Axes.DepthTop.Labels).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.DepthTop.Labels).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.DepthTop.Labels).Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.DepthTop.Title).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.DepthTop.Title).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.DepthTop.Title).Shadow.Visible = false;
      this.pmChart.Axes.Left.Automatic = true;
      ((ChartPen) this.pmChart.Axes.Left.Grid).Style = DashStyle.Dash;
      this.pmChart.Axes.Left.Grid.ZPosition = 0.0;
      ((TextShape) this.pmChart.Axes.Left.Labels).Font.Brush.Color = Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, 0);
      ((TextShape) this.pmChart.Axes.Left.Labels).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Left.Labels).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.Left.Labels).Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Left.Title).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Left.Title).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.Left.Title).Shadow.Visible = false;
      this.pmChart.Axes.Right.Automatic = true;
      ((ChartPen) this.pmChart.Axes.Right.Grid).Style = DashStyle.Dash;
      this.pmChart.Axes.Right.Grid.ZPosition = 0.0;
      ((TextShape) this.pmChart.Axes.Right.Labels).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Right.Labels).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.Right.Labels).Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Right.Title).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Right.Title).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.Right.Title).Shadow.Visible = false;
      this.pmChart.Axes.Top.Automatic = true;
      ((ChartPen) this.pmChart.Axes.Top.Grid).Style = DashStyle.Dash;
      this.pmChart.Axes.Top.Grid.ZPosition = 0.0;
      ((TextShape) this.pmChart.Axes.Top.Labels).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Top.Labels).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.Top.Labels).Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Top.Title).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Axes.Top.Title).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Axes.Top.Title).Shadow.Visible = false;
      ((Control) this.pmChart).Cursor = Cursors.Default;
      ((TextShape) this.pmChart.Footer).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Footer).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Footer).Shadow.Visible = false;
      ((TextShape) this.pmChart.Header).Font.Brush.Color = Color.FromArgb(0, (int) byte.MaxValue, (int) byte.MaxValue);
      ((TextShape) this.pmChart.Header).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Header).Font.Unit = GraphicsUnit.World;
      ((TextShape) this.pmChart.Header).Lines = new string[1]
      {
        "Particle Motion Charts"
      };
      ((Shape) this.pmChart.Header).Shadow.Visible = false;
      this.pmChart.Legend.Alignment = (LegendAlignments) 2;
      ((Shape) this.pmChart.Legend).Gradient.EndColor = Color.FromArgb(0, (int) byte.MaxValue, (int) byte.MaxValue);
      ((Shape) this.pmChart.Legend).Gradient.StartColor = Color.FromArgb(0, 0, 128);
      ((Shape) this.pmChart.Legend).Gradient.Visible = true;
      ((TextShape) this.pmChart.Legend).Font.Brush.Color = Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, 0);
      ((TextShape) this.pmChart.Legend).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Legend).Font.Unit = GraphicsUnit.World;
      this.pmChart.Legend.LegendStyle = (LegendStyles) 1;
      ((TextShape) this.pmChart.Legend.Title).Font.Bold = true;
      ((TextShape) this.pmChart.Legend.Title).Font.Brush.Color = Color.Yellow;
      ((TextShape) this.pmChart.Legend.Title).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.Legend.Title).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.Legend.Title).Pen.Visible = false;
      ((Shape) this.pmChart.Legend.Title).Shadow.Visible = false;
      ((Control) this.pmChart).Location = new Point(222, 221);
      ((Control) this.pmChart).Name = "pmChart";
      ((Shape) this.pmChart.Panel).Gradient.EndColor = Color.FromArgb(0, (int) byte.MaxValue, (int) byte.MaxValue);
      ((Shape) this.pmChart.Panel).Gradient.StartColor = Color.FromArgb(0, 0, 128);
      ((Shape) this.pmChart.Panel).Gradient.Visible = true;
      ((Shape) this.pmChart.Panel).Shadow.Visible = false;
      this.pmChart.Series.Add((Series) this.xSeries);
      this.pmChart.Series.Add((Series) this.ySeries);
      this.pmChart.Series.Add((Series) this.eSeries);
      ((Control) this.pmChart).Size = new Size(400, 250);
      ((TextShape) this.pmChart.SubFooter).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.SubFooter).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.SubFooter).Shadow.Visible = false;
      ((TextShape) this.pmChart.SubHeader).Font.Shadow.Visible = false;
      ((TextShape) this.pmChart.SubHeader).Font.Unit = GraphicsUnit.World;
      ((Shape) this.pmChart.SubHeader).Shadow.Visible = false;
      ((Control) this.pmChart).TabIndex = 2;
      ((Control) this.pmChart).Visible = false;
      ((Wall) this.pmChart.Walls.Back).AutoHide = false;
      ((Shape) this.pmChart.Walls.Back).Shadow.Visible = false;
      ((Wall) this.pmChart.Walls.Bottom).AutoHide = false;
      ((Shape) this.pmChart.Walls.Bottom).Shadow.Visible = false;
      ((Wall) this.pmChart.Walls.Left).AutoHide = false;
      ((Shape) this.pmChart.Walls.Left).Shadow.Visible = false;
      ((Wall) this.pmChart.Walls.Right).AutoHide = false;
      ((Shape) this.pmChart.Walls.Right).Shadow.Visible = false;
      this.pmChart.Walls.Visible = false;
      ((BaseLine) this.xSeries).LinePen.Color = Color.Red;
      ((BaseLine) this.xSeries).LinePen.Width = 2;
      ((Callout) ((Series) this.xSeries).Marks.Callout).ArrowHead = (ArrowHeadStyles) 0;
      ((Callout) ((Series) this.xSeries).Marks.Callout).ArrowHeadSize = 8;
      ((SeriesPointer) ((Series) this.xSeries).Marks.Callout).Brush.Color = Color.Black;
      ((Callout) ((Series) this.xSeries).Marks.Callout).Distance = 0;
      ((SeriesPointer) ((Series) this.xSeries).Marks.Callout).Draw3D = false;
      ((Series) this.xSeries).Marks.Callout.Length = 10;
      ((SeriesPointer) ((Series) this.xSeries).Marks.Callout).Style = (PointerStyles) 0;
      ((TextShape) ((Series) this.xSeries).Marks).Font.Shadow.Visible = false;
      ((TextShape) ((Series) this.xSeries).Marks).Font.Unit = GraphicsUnit.World;
      ((Series) this.xSeries).Title = "x(t)";
      ((Series) this.xSeries).XValues.DataMember = "X";
      ((Series) this.xSeries).XValues.Order = (ValueListOrder) 1;
      ((Series) this.xSeries).YValues.DataMember = "Y";
      ((BaseLine) this.ySeries).LinePen.Color = Color.Green;
      ((BaseLine) this.ySeries).LinePen.Width = 2;
      ((Callout) ((Series) this.ySeries).Marks.Callout).ArrowHead = (ArrowHeadStyles) 0;
      ((Callout) ((Series) this.ySeries).Marks.Callout).ArrowHeadSize = 8;
      ((SeriesPointer) ((Series) this.ySeries).Marks.Callout).Brush.Color = Color.Black;
      ((Callout) ((Series) this.ySeries).Marks.Callout).Distance = 0;
      ((SeriesPointer) ((Series) this.ySeries).Marks.Callout).Draw3D = false;
      ((Series) this.ySeries).Marks.Callout.Length = 10;
      ((SeriesPointer) ((Series) this.ySeries).Marks.Callout).Style = (PointerStyles) 0;
      ((TextShape) ((Series) this.ySeries).Marks).Font.Shadow.Visible = false;
      ((TextShape) ((Series) this.ySeries).Marks).Font.Unit = GraphicsUnit.World;
      ((Series) this.ySeries).Title = "y(t)";
      ((Series) this.ySeries).XValues.DataMember = "X";
      ((Series) this.ySeries).XValues.Order = (ValueListOrder) 1;
      ((Series) this.ySeries).YValues.DataMember = "Y";
      ((BaseLine) this.eSeries).LinePen.Color = Color.Yellow;
      ((BaseLine) this.eSeries).LinePen.Width = 2;
      ((Callout) ((Series) this.eSeries).Marks.Callout).ArrowHead = (ArrowHeadStyles) 0;
      ((Callout) ((Series) this.eSeries).Marks.Callout).ArrowHeadSize = 8;
      ((SeriesPointer) ((Series) this.eSeries).Marks.Callout).Brush.Color = Color.Black;
      ((Callout) ((Series) this.eSeries).Marks.Callout).Distance = 0;
      ((SeriesPointer) ((Series) this.eSeries).Marks.Callout).Draw3D = false;
      ((Series) this.eSeries).Marks.Callout.Length = 10;
      ((SeriesPointer) ((Series) this.eSeries).Marks.Callout).Style = (PointerStyles) 0;
      ((TextShape) ((Series) this.eSeries).Marks).Font.Shadow.Visible = false;
      ((TextShape) ((Series) this.eSeries).Marks).Font.Unit = GraphicsUnit.World;
      ((Series) this.eSeries).Title = "Energy";
      ((Series) this.eSeries).XValues.DataMember = "X";
      ((Series) this.eSeries).XValues.Order = (ValueListOrder) 1;
      ((Series) this.eSeries).YValues.DataMember = "Y";
      this.panelGL.Location = new Point(167, 140);
      this.panelGL.Name = "panelGL";
      this.panelGL.Size = new Size(200, 100);
      this.panelGL.TabIndex = 1;
      this.panelGL.Visible = false;
      this.panelGL.SizeChanged += new EventHandler(this.panelGL_SizeChanged);
      this.panelGL.Paint += new PaintEventHandler(this.panelGL_Paint);
      this.panelGL.MouseDown += new MouseEventHandler(this.panelGL_MouseDown);
      this.panelGL.MouseEnter += new EventHandler(this.panelGL_MouseEnter);
      this.panelGL.MouseMove += new MouseEventHandler(this.panelGL_MouseMove);
      this.toolStrip1.Dock = DockStyle.None;
      this.toolStrip1.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.breakButton,
        (ToolStripItem) this.startButton
      });
      this.toolStrip1.Location = new Point(0, 3);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(41, 55);
      this.toolStrip1.TabIndex = 0;
      this.breakButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.breakButton.Enabled = false;
      this.breakButton.ImageTransparentColor = Color.Magenta;
      this.breakButton.Name = "breakButton";
      this.breakButton.Size = new Size(39, 19);
      this.breakButton.Text = "Break";
      this.breakButton.Click += new EventHandler(this.break_Click);
      this.startButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.startButton.ImageTransparentColor = Color.Magenta;
      this.startButton.Name = "startButton";
      this.startButton.Size = new Size(39, 19);
      this.startButton.Text = "Start";
      this.startButton.Click += new EventHandler(this.startButton_Click);
      this.toolStrip2.Dock = DockStyle.None;
      this.toolStrip2.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.closeButton
      });
      this.toolStrip2.Location = new Point(0, 3);
      this.toolStrip2.Name = "toolStrip2";
      this.toolStrip2.Size = new Size(41, 33);
      this.toolStrip2.TabIndex = 0;
      this.closeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.closeButton.ImageTransparentColor = Color.Magenta;
      this.closeButton.Name = "closeButton";
      this.closeButton.Size = new Size(39, 19);
      this.closeButton.Text = "Close";
      this.closeButton.Click += new EventHandler(this.closeButton_Click);
      this.menuStrip1.Dock = DockStyle.None;
      this.menuStrip1.Items.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.viewMenu,
        (ToolStripItem) this.helpItem,
        (ToolStripItem) this.nuBox
      });
      this.menuStrip1.Location = new Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.ShowItemToolTips = true;
      this.menuStrip1.Size = new Size(770, 27);
      this.menuStrip1.TabIndex = 1;
      this.menuStrip1.Text = "menuStrip1";
      this.viewMenu.DropDownItems.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this._2DItem,
        (ToolStripItem) this.chartItem,
        (ToolStripItem) this._3DItem
      });
      this.viewMenu.Name = "viewMenu";
      this.viewMenu.Size = new Size(44, 23);
      this.viewMenu.Text = "View";
      this._2DItem.Name = "_2DItem";
      this._2DItem.Size = new Size(180, 22);
      this._2DItem.Text = "2D";
      this._2DItem.Click += new EventHandler(this._2DItem_Click);
      this.chartItem.Name = "chartItem";
      this.chartItem.Size = new Size(180, 22);
      this.chartItem.Text = "Chart";
      this.chartItem.Visible = false;
      this.chartItem.Click += new EventHandler(this.chartItem_Click);
      this._3DItem.Name = "_3DItem";
      this._3DItem.Size = new Size(180, 22);
      this._3DItem.Text = "3D";
      this._3DItem.Click += new EventHandler(this._3DItem_Click);
    
    
     
      
      this.codeHelpItem.Name = "codeHelpItem";
      this.codeHelpItem.Size = new Size(180, 22);
      this.codeHelpItem.Text = "Код программиста";
      this.codeHelpItem.Visible = false;
      this.designerCodeHelpItem.Name = "designerCodeHelpItem";
      this.designerCodeHelpItem.Size = new Size(180, 22);
      this.designerCodeHelpItem.Text = "Код дизайнера";
      this.designerCodeHelpItem.Visible = false;
      this.nuBox.Font = new Font("Segoe UI", 9f);
      this.nuBox.Name = "nuBox";
      this.nuBox.Size = new Size(100, 23);
      this.nuBox.ToolTipText = "Частота изменения поля";
      this.nuBox.Click += new EventHandler(this.nuBox_Click);
      this.nuBox.MouseLeave += new EventHandler(this.nuBox_MouseLeave);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(770, 673);
      this.Controls.Add((Control) this.toolStripContainer1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = nameof (mainForm);
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Particle Motion";
      this.FormClosed += new FormClosedEventHandler(this.mainForm_FormClosed);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.cntxtMenu.ResumeLayout(false);
      this.cntxtMenu.PerformLayout();
      this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
      this.toolStripContainer1.ContentPanel.ResumeLayout(false);
      this.toolStripContainer1.ContentPanel.PerformLayout();
      this.toolStripContainer1.LeftToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.LeftToolStripPanel.PerformLayout();
      this.toolStripContainer1.RightToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.RightToolStripPanel.PerformLayout();
      this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.PerformLayout();
      this.toolStripContainer1.ResumeLayout(false);
      this.toolStripContainer1.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      ((ISupportInitialize) this.pictureBox2).EndInit();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.toolStrip2.ResumeLayout(false);
      this.toolStrip2.PerformLayout();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
    }

    private struct Vector2D
    {
      private double _x;
      private double _y;
      private double _mod;
      private double _azimuth;

      internal double X
      {
        set
        {
          this._x = value;
          this._mod = Math.Sqrt(this * this);
          this._azimuth = Math.Atan2(this._y, this._x);
        }
        get => this._x;
      }

      internal double Y
      {
        set
        {
          this._y = value;
          this._mod = Math.Sqrt(this * this);
          this._azimuth = Math.Atan2(this._y, this._x);
        }
        get => this._y;
      }

      internal double Azimuth
      {
        set
        {
          this._azimuth = value;
          this._y = this._mod * Math.Sin(this._azimuth);
          this._x = this._mod * Math.Cos(this._azimuth);
        }
        get => this._azimuth;
      }

      internal double Mod
      {
        set
        {
          this._mod = value;
          this._y = this._mod * Math.Sin(this._azimuth);
          this._x = this._mod * Math.Cos(this._azimuth);
        }
        get => this._mod;
      }

      internal Vector2D(double x, double y)
      {
        this._x = x;
        this._y = y;
        this._mod = Math.Sqrt(this._x * this._x + this._y * this._y);
        this._azimuth = Math.Atan2(this._y, this._x);
      }

      internal Vector2D(SizeF size)
        : this((double) size.Width, (double) size.Height)
      {
      }

      public static explicit operator SizeF(mainForm.Vector2D vector) => new SizeF((float) vector.X, (float) vector.Y);

      public static mainForm.Vector2D operator -(mainForm.Vector2D vector) => new mainForm.Vector2D(-vector.X, -vector.Y);

      public static mainForm.Vector2D operator -(mainForm.Vector2D v1, mainForm.Vector2D v2) => v1 + -v2;

      public static mainForm.Vector2D operator +(mainForm.Vector2D v1, mainForm.Vector2D v2) => new mainForm.Vector2D(v1.X + v2.X, v1.Y + v2.Y);

      public static mainForm.Vector2D operator *(double a, mainForm.Vector2D v) => new mainForm.Vector2D(v.X * a, v.Y * a);

      public static mainForm.Vector2D operator *(mainForm.Vector2D v, double a) => a * v;

      public static mainForm.Vector2D operator /(mainForm.Vector2D v, double a) => v * (1.0 / a);

      public static double operator *(mainForm.Vector2D v1, mainForm.Vector2D v2) => v1.X * v2.X + v1.Y * v2.Y;
    }

    private enum MotionState
    {
      stop,
      pause,
      run,
    }

    private delegate void CreateList();
  }
}
