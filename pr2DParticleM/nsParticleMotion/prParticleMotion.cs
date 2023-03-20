

using System;
using System.Windows.Forms;

namespace nsParticleMotion
{
  internal static class prParticleMotion
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new mainForm());
    }
  }
}
