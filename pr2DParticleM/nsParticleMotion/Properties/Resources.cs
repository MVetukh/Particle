

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace nsParticleMotion.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (nsParticleMotion.Properties.Resources.resourceMan == null)
          nsParticleMotion.Properties.Resources.resourceMan = new ResourceManager("nsParticleMotion.Properties.Resources", typeof (nsParticleMotion.Properties.Resources).Assembly);
        return nsParticleMotion.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => nsParticleMotion.Properties.Resources.resourceCulture;
      set => nsParticleMotion.Properties.Resources.resourceCulture = value;
    }

    internal static string code => nsParticleMotion.Properties.Resources.ResourceManager.GetString(nameof (code), nsParticleMotion.Properties.Resources.resourceCulture);

    internal static string DesignCode => nsParticleMotion.Properties.Resources.ResourceManager.GetString(nameof (DesignCode), nsParticleMotion.Properties.Resources.resourceCulture);

    internal static string ParticleMotion => nsParticleMotion.Properties.Resources.ResourceManager.GetString(nameof (ParticleMotion), nsParticleMotion.Properties.Resources.resourceCulture);
  }
}
