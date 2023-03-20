

using nsParticleMotion.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace nsParticleMotion
{
  public class helpForm : Form
  {
    public string itemClickedName;
   
    private RichTextBox helpBox;

    public helpForm() => this.InitializeComponent();

    private void helpForm_Shown(object sender, EventArgs e)
    {
      string itemClickedName = this.itemClickedName;
      if (!(itemClickedName == "howToHelpItem"))
      {
        if (!(itemClickedName == "codeHelpItem"))
        {
          if (!(itemClickedName == "designerCodeHelpItem"))
            return;
          this.helpBox.Text = Resources.DesignCode;
        }
        else
          this.helpBox.Text = Resources.code;
      }
      else
        this.helpBox.Text = Resources.ParticleMotion;
    }

  

    private void InitializeComponent()
    {
      this.helpBox = new RichTextBox();
      this.SuspendLayout();
      this.helpBox.Dock = DockStyle.Fill;
      this.helpBox.Location = new Point(0, 0);
      this.helpBox.Name = "helpBox";
      this.helpBox.Size = new Size(546, 456);
      this.helpBox.TabIndex = 0;
      this.helpBox.Text = "";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(546, 456);
      this.Controls.Add((Control) this.helpBox);
      
      this.StartPosition = FormStartPosition.CenterScreen;
      
      this.Shown += new EventHandler(this.helpForm_Shown);
      this.ResumeLayout(false);
    }
  }
}
