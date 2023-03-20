using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pr1DParticle2
{
    public partial class f1DParticle2 : Form
    {
        #region Объявление переменных
        const double stepTime = .01;
        const int sleepTime = 1;
        double x;
        double v ;
        double time;
        int pbHeight, pbWidth;
        String Problem;
        bool runFlag;
        private bool run;
        double scale;
        int linePos ;
        Rectangle lineRect ;
        static Size pSize = new Size(20, 20);
        static Size pOffset = new Size(-10, -10);
        Rectangle pSquare = new Rectangle(new Point(-2 * pSize.Width, -2 * pSize.Width), pSize);
        Rectangle pRect = new Rectangle(new Point(), f1DParticle2.pSize);
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripTextBox vBox;

        #endregion

        public f1DParticle2()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += new MouseEventHandler(this.PictureBox1_MouseWheel);
            ResizePanel();
        }


        private void ResizePanel()
        {
            pbWidth = pictureBox1.ClientSize.Width;
            scale = (double)pbWidth * 0.5;
            linePos = pictureBox1.ClientSize.Height / 2;
            lineRect = new Rectangle(new Point(0, linePos ), new Size(pbWidth, 4));
           
            ResetParticle();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            using (HatchBrush hatchBrush = new HatchBrush(HatchStyle.Cross, Color.DarkGray, Color.FromArgb(0, 0, 100)))
                e.Graphics.FillRectangle((Brush)hatchBrush, pictureBox1.ClientRectangle);
            using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(lineRect, Color.White, Color.Black, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle((Brush)linearGradientBrush, lineRect);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(pRect);
                using (PathGradientBrush pathGradientBrush = new PathGradientBrush(path))
                {
                    pathGradientBrush.CenterColor = Color.Yellow;
                    pathGradientBrush.SurroundColors = new Color[1]
                    {
            Color.Black
                    };
                    e.Graphics.FillPath((Brush)pathGradientBrush, path);
                }
            }
            if (this.v == 0.0)
                return;
            using (Pen pen = new Pen(Color.Red, 3f))
            {
                pen.EndCap = LineCap.ArrowAnchor;
                e.Graphics.DrawLine(pen, pRect.Location - f1DParticle2.pOffset, pRect.Location - f1DParticle2.pOffset + new Size((int)(10.0 * v), 0));
            }
        }
        void ResetPanelSize()
        {
            // Сохраняем новые размеры pictureBox
            pbHeight = pictureBox1.ClientSize.Height;
            pbWidth = pictureBox1.ClientSize.Width;
            // текущий масштаб
            scale = pbWidth / 2.0;
            // Текущий квадрат, в который вписано положение частицы
            // Положение левого верхнего угла квадрата вычисляется 
            // от текущего положения частицы x с учетом текущего масштаба
            pSquare.Location = new Point((int)((x + 1) * scale) - pSize.Width,
                        pbHeight / 2 - pSize.Width);

        }
        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            ResetPanelSize();
            ResizePanel();
            pictureBox1.Refresh();
        }

        #region Описание физики 
        private double Force()
        {
            if (Problem == "Свободная частица")
                return 0.0;
            if (Problem == "Частица в среде")
                return -v;
            if (Problem == "Частица в однородном поле")
                return 1.0;
            return Problem == "Осциллятор" ? -x : 0;//double.NaN;
        }
        private double PotentialEnergy()
        {
            if (Problem == "Свободная частица" || Problem == "Частица в среде")
                return 0.0;
            if (Problem == "Частица в однородном поле")
                return -x;
            return Problem == "Осциллятор" ? 0.5 * x * x : 0;//double.NaN;
        }
        private double Energy()
        { 
            return 0.5 * v * v + PotentialEnergy();
        }
        #endregion
       
        private void ResetParticle()
        {
            pRect.Location = new Point((int)((x + 1.0) * scale), linePos) + f1DParticle2.pOffset;
            ResetStatus();
           
        }
        
        private void ResetStatus()
        {
            timeLbl.Text = string.Format("time = {0:g2}; x = {1:g2}; v = {2:g2}; Energy = {3:g5}", (object)time, (object)x, (object)v, (object)Energy());
        }
        private void DoStep()
        {
            pictureBox1.Invalidate(lineRect);
            pictureBox1.Invalidate(pRect);
            RelocateParticle();
            pictureBox1.Invalidate(pRect);
            pictureBox1.Update();
        }
       void Run()
        {
            while (runFlag)
            {
                
                DoStep();                
                System.Threading.Thread.Sleep(sleepTime);               
                Application.DoEvents();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false; 
            Run();
        }

        private void startButton_Click(object sender, EventArgs e)
        {          
            runFlag = !runFlag;   // флаг движения опускается или поднимается
            startButton.Text = runFlag ? "Stop" : "Start"; // меняется надпись на кнопке
            // при включении движения обнуляется счетчик времени, и включается таймер
            if (runFlag)
            {
                time = 0;
                
                timer1.Enabled = true; //Включает таймер
            }
        }

        private void f1DParticle2_FormClosed(object sender, FormClosedEventArgs e)
        {
            // при закрытии приложения флаг движения опускается
            runFlag = false;
        }

        private void ProblemToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Problem = Text = e.ClickedItem.Text;
            time = 0.0;
            v = 1.0; 
            ResetParticle();
            pictureBox1.Refresh();
        }





        //void Run()
        //{
        //    while (this.run)
        //    {
        //        this.DoStep();
        //        Application.DoEvents();
        //    }
        //}
        private void RelocateParticle()
        {
            v += Force() * stepTime; //0.1*Math.Exp(-stepTime);
            time += stepTime;
           
            double vrtlx = x + stepTime * v;
            x = vrtlx;
            if (Math.Abs(vrtlx) > 1.0)
            {
                x = (2 * Math.Sign(v))- vrtlx;
                v = -v;
            }
            //else 
            //{
            //    x = vrtlx;
            //}
            ResetParticle();
        }


        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.run || !pRect.Contains(e.Location))
                return;
            this.time = 0.0;
            if (Math.Abs(v) <= 3.0)
                v += (double)e.Delta * 0.001;
            if (Math.Abs(v) > 3.0)
                v = (double)Math.Sign(v) * 3.0;
            this.ResetStatus();
            this.pictureBox1.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.run)
                return;
            this.Cursor = this.lineRect.Contains(e.Location) ? Cursors.Hand : (this.Cursor = Cursors.Default);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || this.run || this.Cursor != Cursors.Hand)
                return;
            this.x = (double)(e.X - this.pbWidth / 2) / this.scale;
            this.time = 0.0;
            this.v = v;
            this.ResetParticle();
            this.pictureBox1.Refresh();
        }


        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = this.run;
            this.vBox.Text = this.v.ToString("g3");
        }

        //void DoStep()
        //{
        //    time += stepTime;// Новое время
        //    v += Force(v, x, time) * stepTime;// Новая скорость
        //    double _vrtlx = x + stepTime * v;// Предполагаемое положение
        //    // при выходе за границы виртуальной координаты
        //    if (Math.Abs(_vrtlx) > 1)
        //    {
        //        // происходит отражение
        //        x = 2 * Math.Sign(v) - _vrtlx;
        //        v = -v;
        //    }
        //    else
        //        x = _vrtlx;
        //    // Прежнее положение частицы обновляется (стирается)
        //    pictureBox1.Invalidate(pSquare);
        //    // Прямоугольник частицы меняет свое положение
        //    pSquare.Location = new Point((int)((x + 1) * scale) - pRadius, pbHeight / 2 - pRadius);
        //    // Обновляется изображение частицы на pictureBox
        //    pictureBox1.Invalidate(pSquare);
        //    // Немедленное обновление изображения
        //    pictureBox1.Update();
        //    // Меняем текст в строке статуса 
        //    timeLbl.Text = "time = " + time.ToString("f") + "; x = " + x.ToString("f5") + "; v = " + v.ToString("f") +
        //            "; Energy=" + Energy(v, x, time).ToString("f5");
        //}
        //private void vBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode != Keys.Return)
        //        return;
        //    double result;
        //    if (!double.TryParse(this.vBox.Text, out result))
        //    {
        //        this.contextMenuStrip1.Hide();
        //    }
        //    else
        //    {
        //        this.v = result;
        //        this.ResetStatus();
        //        this.pictureBox1.Refresh();
        //        this.contextMenuStrip1.Hide();
        //    }
        //}


    }
}
