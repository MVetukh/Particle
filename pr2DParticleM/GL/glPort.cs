

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GL
{
  public class glPort : IDisposable
  {
    private const uint PFD_DOUBLEBUFFER = 1;
    private const uint PFD_DRAW_TO_WINDOW = 4;
    private const uint PFD_SUPPORT_OPENGL = 32;
    private static int openGLdllHandle;
    private glPort.PIXELFORMATDESCRIPTOR pixelFD = new glPort.PIXELFORMATDESCRIPTOR();
    private int renderContext;
    private int controlHandle;

    public int deviceContext { private set; get; }

    [DllImport("GDI32")]
    private static extern int ChoosePixelFormat(int dvcContext, [MarshalAs(UnmanagedType.LPStruct)] glPort.PIXELFORMATDESCRIPTOR pfd);

    [DllImport("GDI32")]
    private static extern bool SetPixelFormat(
      int dvcContext,
      int pixelFormatIndex,
      [MarshalAs(UnmanagedType.LPStruct)] glPort.PIXELFORMATDESCRIPTOR pfd);

    [DllImport("User32")]
    private static extern int GetDC(int handle);

    [DllImport("User32")]
    private static extern int ReleaseDC(int handle, int dvcContext);

    [DllImport("OPENGL32.DLL")]
    private static extern int wglCreateContext(int dvcContext);

    [DllImport("OPENGL32.DLL")]
    private static extern bool wglDeleteContext(int rndContext);

    [DllImport("GDI32")]
    private static extern bool SwapBuffers(int dvcContext);

    [DllImport("OPENGL32.DLL")]
    private static extern bool wglMakeCurrent(int dvcContext, int rndContext);

    [DllImport("Kernel32")]
    public static extern int LoadLibrary(string funcname);

    public bool MakeCurrent() => glPort.wglMakeCurrent(this.deviceContext, this.renderContext);

    public bool SwapBuffers() => glPort.SwapBuffers(this.deviceContext);

    public glPort(Control control)
    {
      if ((this.deviceContext = glPort.GetDC(this.controlHandle = (int) control.Handle)) == 0)
        throw new Exception("Контекст устройства для данного окна не определен.");
      if (glPort.openGLdllHandle == 0 && (glPort.openGLdllHandle = glPort.LoadLibrary("OpenGL32.DLL")) == 0)
        throw new Exception("Библиотека OPENGL32 не включена в процесс.");
      int pixelFormatIndex = glPort.ChoosePixelFormat(this.deviceContext, this.pixelFD);
      if (pixelFormatIndex == 0)
        throw new Exception("Формат пикселей не определен.");
      if (!glPort.SetPixelFormat(this.deviceContext, pixelFormatIndex, this.pixelFD))
        throw new Exception("Формат пикселей не установлен.");
      if ((this.renderContext = glPort.wglCreateContext(this.deviceContext)) == 0)
        throw new Exception("Контекст воспроизведения не установлен.");
      if (!glPort.wglMakeCurrent(this.deviceContext, this.renderContext))
        throw new Exception("Контекст не установлен текущим.");
    }

    public void Dispose()
    {
      if (!glPort.wglMakeCurrent(0, 0))
        throw new Exception("Контекст не установлен нулем.");
      if (this.renderContext > 0 && !glPort.wglDeleteContext(this.renderContext))
        throw new Exception("Контекст воспроизведения не освобожден.");
      if (this.deviceContext > 0 && glPort.ReleaseDC(this.controlHandle, this.deviceContext) == 0)
        throw new Exception("Контекст устройства не освобожден.");
      this.renderContext = this.deviceContext = 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    private class PIXELFORMATDESCRIPTOR
    {
      internal short nSize = 40;
      internal short nVersion = 1;
      internal uint dwFlags = 37;
      internal byte iPixelType;
      internal byte cColorBits = 32;
      internal byte cRedBits;
      internal byte cRedShift;
      internal byte cGreenBits;
      internal byte cGreenShift;
      internal byte cBlueBits;
      internal byte cBlueShift;
      internal byte cAlphaBits = 8;
      internal byte cAlphaShift;
      internal byte cAccumBits = 8;
      internal byte cAccumRedBits;
      internal byte cAccumGreenBits;
      internal byte cAccumBlueBits;
      internal byte cAccumAlphaBits;
      internal byte cDepthBits = 32;
      internal byte cStencilBits = 8;
      internal byte cAuxBuffers;
      internal byte iLayerType;
      internal byte bReserved;
      internal int dwLayerMask;
      internal uint dwVisibleMask;
      internal uint dwDamageMask;
    }
  }
}
