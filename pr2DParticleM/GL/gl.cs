
using System;
using System.Runtime.InteropServices;

namespace GL
{
  public static class gl
  {
    public const int COLOR_BUFFER_BIT = 16384;
    public const int DEPTH_BUFFER_BIT = 256;
    public const int STENCIL_BUFFER_BIT = 1024;
    public const int ACCUM_BUFFER_BIT = 512;
    public const int POINTS = 0;
    public const int LINES = 1;
    public const int LINE_LOOP = 2;
    public const int LINE_STRIP = 3;
    public const int TRIANGLES = 4;
    public const int TRIANGLE_STRIP = 5;
    public const int TRIANGLE_FAN = 6;
    public const int QUADS = 7;
    public const int QUAD_STRIP = 8;
    public const int POLYGON = 9;
    public const int CW = 2304;
    public const int CCW = 2305;
    public const int FRONT = 1028;
    public const int BACK = 1029;
    public const int FRONT_AND_BACK = 1032;
    public const int POINT = 6912;
    public const int LINE = 6913;
    public const int FILL = 6914;
    public const int POINT_SIZE = 2833;
    public const int POINT_SIZE_RANGE = 2834;
    public const int POINT_SIZE_GRANULARITY = 2835;
    public const int CURRENT_COLOR = 2816;
    public const int ALPHA_TEST_FUNC = 3009;
    public const int ALPHA_TEST_REF = 3010;
    public const int COLOR_WRITEMASK = 3107;
    public const int DEPTH_RANGE = 2928;
    public const int DEPTH_FUNC = 2932;
    public const int MATRIX_MODE = 2976;
    public const int LINE_WIDTH_RANGE = 2850;
    public const int LINE_WIDTH_GRANULARITY = 2851;
    public const int LINE_WIDTH = 2849;
    public const int LINE_STIPPLE_REPEAT = 2854;
    public const int LINE_STIPPLE_PATTERN = 2853;
    public const int CULL_FACE_MODE = 2885;
    public const int POLYGON_MODE = 2880;
    public const int BLEND_DST = 3040;
    public const int BLEND_SRC = 3041;
    public const int VIEWPORT = 2978;
    public const int MODELVIEW_MATRIX = 2982;
    public const int PROJECTION_MATRIX = 2983;
    public const int COLOR_CLEAR_VALUE = 3106;
    public const int SCISSOR_BOX = 3088;
    public const int DEPTH_CLEAR_VALUE = 2931;
    public const int STENCIL_CLEAR_VALUE = 2961;
    public const int STENCIL_FUNC = 2962;
    public const int STENCIL_REF = 2967;
    public const int STENCIL_VALUE_MASK = 2963;
    public const int STENCIL_FAIL = 2964;
    public const int STENCIL_PASS_DEPTH_FAIL = 2965;
    public const int STENCIL_PASS_DEPTH_PASS = 2966;
    public const int LIST_MODE = 2864;
    public const int LIST_INDEX = 2867;
    public const int LIST_BASE = 2866;
    public const int MAX_LIGHTS = 3377;
    public const int FRONT_FACE = 2886;
    public const int MAX_CLIP_PLANES = 3378;
    public const int RED_BITS = 3410;
    public const int GREEN_BITS = 3411;
    public const int BLUE_BITS = 3412;
    public const int ALPHA_BITS = 3413;
    public const int DEPTH_BITS = 3414;
    public const int STENCIL_BITS = 3415;
    public const int ACCUM_RED_BITS = 3416;
    public const int ACCUM_GREEN_BITS = 3417;
    public const int ACCUM_BLUE_BITS = 3418;
    public const int ACCUM_ALPHA_BITS = 3419;
    public const int ACCUM_CLEAR_VALUE = 2944;
    public const int EDGE_FLAG = 2883;
    public const int VERSION = 7938;
    public const int VENDOR = 7936;
    public const int RENDERER = 7937;
    public const int CURRENT_NORMAL = 2818;
    public const int COLOR_MATERIAL_FACE = 2901;
    public const int COLOR_MATERIAL_PARAMETER = 2902;
    public const int SHADE_MODEL = 2900;
    public const int CURRENT_RASTER_COLOR = 2820;
    public const int CURRENT_RASTER_POSITION = 2823;
    public const int CURRENT_RASTER_DISTANCE = 2825;
    public const int POINT_SMOOTH = 2832;
    public const int ALPHA_TEST = 3008;
    public const int DEPTH_TEST = 2929;
    public const int LINE_STIPPLE = 2852;
    public const int POLYGON_STIPPLE = 2882;
    public const int CULL_FACE = 2884;
    public const int BLEND = 3042;
    public const int STENCIL_TEST = 2960;
    public const int LIGHTING = 2896;
    public const int LIGHT0 = 16384;
    public const int LINE_SMOOTH = 2848;
    public const int SCISSOR_TEST = 3089;
    public const int COLOR_MATERIAL = 2903;
    public const int NORMALIZE = 2977;
    public const int POLYGON_SMOOTH = 2881;
    public const int DITHER = 3024;
    public const int NEVER = 512;
    public const int LESS = 513;
    public const int EQUAL = 514;
    public const int LEQUAL = 515;
    public const int GREATER = 516;
    public const int NOTEQUAL = 517;
    public const int GEQUAL = 518;
    public const int ALWAYS = 519;
    public const int SRC_ALPHA_SATURATE = 776;
    public const int DST_COLOR = 774;
    public const int ONE_MINUS_DST_COLOR = 775;
    public const int SRC_COLOR = 768;
    public const int ONE_MINUS_SRC_COLOR = 769;
    public const int ZERO = 0;
    public const int ONE = 1;
    public const int SRC_ALPHA = 770;
    public const int ONE_MINUS_SRC_ALPHA = 771;
    public const int DST_ALPHA = 772;
    public const int ONE_MINUS_DST_ALPHA = 773;
    public const int PROJECTION = 5889;
    public const int MODELVIEW = 5888;
    public const int COMPILE = 4864;
    public const int COMPILE_AND_EXECUTE = 4865;
    public const int UNSIGNED_BYTE = 5121;
    public const int FONT_LINES = 0;
    public const int FONT_POLYGONS = 1;
    public const int CLIP_PLANE0 = 12288;
    public const int KEEP = 7680;
    public const int REPLACE = 7681;
    public const int INVERT = 5386;
    public const int INCR = 7682;
    public const int DECR = 7683;
    public const int ALL_ATTRIB_BITS = 1048575;
    public const int ENABLE_BIT = 8192;
    public const int CURRENT_BIT = 1;
    public const int POINT_BIT = 2;
    public const int LINE_BIT = 4;
    public const int POLYGON_BIT = 8;
    public const int POLYGON_STIPPLE_BIT = 16;
    public const int LIST_BIT = 131072;
    public const int VIEWPORT_BIT = 2048;
    public const int LIGHTING_BIT = 64;
    public const int SCISSOR_BIT = 524288;
    public const int FLAT = 7424;
    public const int SMOOTH = 7425;
    public const int LIGHT_MODEL_LOCAL_VIEWER = 2897;
    public const int LIGHT_MODEL_TWO_SIDE = 2898;
    public const int LIGHT_MODEL_AMBIENT = 2899;
    public const int AMBIENT = 4608;
    public const int DIFFUSE = 4609;
    public const int SPECULAR = 4610;
    public const int EMISSION = 5632;
    public const int SHININESS = 5633;
    public const int AMBIENT_AND_DIFFUSE = 5634;
    public const int POSITION = 4611;
    public const int SPOT_DIRECTION = 4612;
    public const int SPOT_EXPONENT = 4613;
    public const int SPOT_CUTOFF = 4614;
    public const int CONSTANT_ATTENUATION = 4615;
    public const int LINEAR_ATTENUATION = 4616;
    public const int QUADRATIC_ATTENUATION = 4617;
    public const int FOG_BIT = 128;
    public const int FOG = 2912;
    public const int FOG_DENSITY = 2914;
    public const int FOG_START = 2915;
    public const int FOG_END = 2916;
    public const int FOG_MODE = 2917;
    public const int FOG_COLOR = 2918;
    public const int LINEAR = 9729;
    public const int EXP = 2048;
    public const int EXP2 = 2049;
    public const int ACCUM = 256;
    public const int LOAD = 257;
    public const int RETURN = 258;
    public const int MULT = 259;
    public const int ADD = 260;
    public const int VERTEX_ARRAY = 32884;
    public const int FLOAT = 5126;
    public const int DEPTH_COMPONENT = 6402;

    [DllImport("OPENGL32.DLL", EntryPoint = "glClear")]
    public static extern void Clear(int mask);

    [DllImport("OPENGL32.DLL", EntryPoint = "glClearColor")]
    public static extern void ClearColor(float red, float green, float blue, float alpha);

    [DllImport("OPENGL32.DLL", EntryPoint = "glClearDepth")]
    public static extern void ClearDepth(double depth);

    [DllImport("OPENGL32.DLL", EntryPoint = "glColorMask")]
    public static extern void ColorMask(bool red, bool green, bool blue, bool alpha);

    [DllImport("OPENGL32.DLL", EntryPoint = "glBegin")]
    public static extern void Begin(int mode);

    [DllImport("OPENGL32.DLL", EntryPoint = "glEnd")]
    public static extern void End();

    [DllImport("OPENGL32.DLL", EntryPoint = "glVertex2f")]
    public static extern void Vertex(float x, float y);

    [DllImport("OPENGL32.DLL", EntryPoint = "glVertex3f")]
    public static extern void Vertex(float x, float y, float z);

    [DllImport("OPENGL32.DLL", EntryPoint = "glVertex4f")]
    public static extern void Vertex(float x, float y, float z, float w);

    [DllImport("OPENGL32.DLL", EntryPoint = "glPointSize")]
    public static extern void PointSize(float size);

    [DllImport("OPENGL32.DLL", EntryPoint = "glColor3f")]
    public static extern void Color(float red, float green, float blue);

    [DllImport("OPENGL32.DLL", EntryPoint = "glColor4f")]
    public static extern void Color(float red, float green, float blue, float alpha);

    [DllImport("OPENGL32.DLL", EntryPoint = "glLineWidth")]
    public static extern void LineWidth(float width);

    [DllImport("OPENGL32.DLL", EntryPoint = "glLineStipple")]
    public static extern void LineStipple(int factor, ushort pattern);

    [DllImport("OPENGL32.DLL", EntryPoint = "glPolygonStipple")]
    public static extern void PolygonStipple(byte[] mask);

    [DllImport("OPENGL32.DLL", EntryPoint = "glFrontFace")]
    public static extern void FrontFace(int mode);

    [DllImport("OPENGL32.DLL", EntryPoint = "glPolygonMode")]
    public static extern void PolygonMode(int face, int mode);

    [DllImport("OPENGL32.DLL", EntryPoint = "glEdgeFlag")]
    public static extern void EdgeFlag(bool flag);

    [DllImport("OPENGL32.DLL", EntryPoint = "glCullFace")]
    public static extern void CullFace(int mode);

    [DllImport("OPENGL32.DLL")]
    private static extern void glGetFloatv(int prmName, float[] parameters);

    public static float[] Get(int prmName, int arrayLength)
    {
      float[] parameters = new float[arrayLength];
      gl.glGetFloatv(prmName, parameters);
      return parameters;
    }

    [DllImport("OPENGL32.DLL")]
    private static extern IntPtr glGetString(int name);

    public static string GetString(int name) => Marshal.PtrToStringAnsi(gl.glGetString(name));

    [DllImport("OPENGL32.DLL", EntryPoint = "glGetPolygonStipple")]
    public static extern void GetPolygonStipple(byte[] mask);

    [DllImport("OPENGL32.DLL", EntryPoint = "glGetClipPlane")]
    public static extern void GetClipPlane(int plane, double[] equation);

    [DllImport("OPENGL32.DLL", EntryPoint = "glEnable")]
    public static extern void Enable(int capability);

    [DllImport("OPENGL32.DLL", EntryPoint = "glDisable")]
    public static extern void Disable(int capability);

    [DllImport("OPENGL32.DLL", EntryPoint = "glIsEnabled")]
    public static extern bool IsEnabled(int capability);

    [DllImport("OPENGL32.DLL", EntryPoint = "glAlphaFunc")]
    public static extern void AlphaFunc(int function, float referenceValue);

    [DllImport("OPENGL32.DLL", EntryPoint = "glBlendFunc")]
    public static extern void BlendFunc(int sfactor, int dfactor);

    [DllImport("OPENGL32.DLL", EntryPoint = "glDepthMask")]
    public static extern void DepthMask(bool flag);

    [DllImport("OPENGL32.DLL", EntryPoint = "glDepthRange")]
    public static extern void DepthRange(double near, double far);

    [DllImport("OPENGL32.DLL", EntryPoint = "glDepthFunc")]
    public static extern void DepthFunc(int function);

    [DllImport("OPENGL32.DLL", EntryPoint = "glViewport")]
    public static extern void Viewport(int x, int y, int width, int height);

    [DllImport("OPENGL32.DLL", EntryPoint = "glMatrixMode")]
    public static extern void MatrixMode(int mode);

    [DllImport("OPENGL32.DLL", EntryPoint = "glLoadIdentity")]
    public static extern void LoadIdentity();

    [DllImport("GLU32.DLL", EntryPoint = "gluOrtho2D")]
    public static extern void Ortho(double left, double right, double bottom, double top);

    [DllImport("OPENGL32.DLL", EntryPoint = "glOrtho")]
    public static extern void Ortho(
      double left,
      double right,
      double bottom,
      double top,
      double near,
      double far);

    [DllImport("GLU32.DLL", EntryPoint = "gluPerspective")]
    public static extern void Perspective(double fovy, double aspect, double near, double far);

    [DllImport("OPENGL32.DLL", EntryPoint = "glFrustum")]
    public static extern void Frustum(
      double left,
      double right,
      double bottom,
      double top,
      double near,
      double far);

    [DllImport("OPENGL32.DLL", EntryPoint = "glTranslatef")]
    public static extern void Translate(float x, float y, float z);

    [DllImport("OPENGL32.DLL", EntryPoint = "glRotatef")]
    public static extern void Rotate(float angle, float x, float y, float z);

    [DllImport("OPENGL32.DLL", EntryPoint = "glScalef")]
    public static extern void Scale(float x, float y, float z);

    [DllImport("GLU32.DLL", EntryPoint = "gluLookAt")]
    public static extern void LookAt(
      double eyex,
      double eyey,
      double eyez,
      double centerx,
      double centery,
      double centerz,
      double upx,
      double upy,
      double upz);

    [DllImport("GLU32.DLL", EntryPoint = "gluProject")]
    public static extern void Project(
      double objx,
      double objy,
      double objz,
      double[] modelMatrix,
      double[] projMatrix,
      int[] viewport,
      out double winx,
      out double winy,
      out double winz);

    [DllImport("GLU32.DLL", EntryPoint = "gluUnProject")]
    public static extern void UnProject(
      double winx,
      double winy,
      double winz,
      double[] modelMatrix,
      double[] projMatrix,
      int[] viewport,
      out double objx,
      out double objy,
      out double objz);

    [DllImport("OPENGL32.DLL", EntryPoint = "glPopMatrix")]
    public static extern void PopMatrix();

    [DllImport("OPENGL32.DLL", EntryPoint = "glPushMatrix")]
    public static extern void PushMatrix();

    [DllImport("OPENGL32.DLL", EntryPoint = "glIsList")]
    public static extern bool IsList(uint list);

    [DllImport("OPENGL32.DLL", EntryPoint = "glDeleteLists")]
    public static extern void DeleteLists(uint list, int range);

    [DllImport("OPENGL32.DLL", EntryPoint = "glGenLists")]
    public static extern uint GenLists(int range);

    [DllImport("OPENGL32.DLL", EntryPoint = "glNewList")]
    public static extern void NewList(uint list, int mode);

    [DllImport("OPENGL32.DLL", EntryPoint = "glEndList")]
    public static extern void EndList();

    [DllImport("OPENGL32.DLL", EntryPoint = "glCallList")]
    public static extern void CallList(uint list);

    [DllImport("OPENGL32.DLL", EntryPoint = "glCallLists")]
    public static extern void CallLists(int n, int type, string lists);

    [DllImport("OPENGL32.DLL", EntryPoint = "glListBase")]
    public static extern void ListBase(uint basevalue);

    [DllImport("OPENGL32.DLL", EntryPoint = "glRasterPos3f")]
    public static extern void RasterPosf(float x, float y, float z);

    [DllImport("GDI32")]
    public static extern int SetTextColor(int deviceContext, int Color);

    [DllImport("GDI32")]
    public static extern int SetBkColor(int deviceContext, int Color);

    [DllImport("GDI32")]
    public static extern bool ExtTextOut(
      int deviceContext,
      int x,
      int y,
      uint options,
      IntPtr rect,
      string text,
      uint count,
      IntPtr spacing);

    [DllImport("GDI32")]
    public static extern IntPtr SelectObject(int deviceContext, IntPtr objectHandle);

    [DllImport("OPENGL32.DLL", EntryPoint = "wglUseFontBitmaps")]
    public static extern bool UseFontBitmaps(
      int deviceContext,
      int start,
      int count,
      uint listBase);

    [DllImport("OPENGL32.DLL", EntryPoint = "wglUseFontOutlines")]
    public static extern bool UseFontOutlines(
      int deviceContext,
      uint first,
      uint count,
      uint listBase,
      float deviation,
      float extrusion,
      int format,
      gl.GLYPHMETRICSFLOAT[] glyphs);

    [DllImport("OPENGL32.DLL", EntryPoint = "glClipPlane")]
    public static extern void ClipPlane(int plane, double[] equation);

    [DllImport("OPENGL32.DLL", EntryPoint = "glStencilOp")]
    public static extern void StencilOp(int sFail, int zFail, int szPass);

    [DllImport("OPENGL32.DLL", EntryPoint = "glStencilFunc")]
    public static extern void StencilFunc(int func, int refvalue, uint mask);

    [DllImport("OPENGL32.DLL", EntryPoint = "glClearStencil")]
    public static extern void ClearStencil(int s);

    [DllImport("OPENGL32.DLL", EntryPoint = "glPushAttrib")]
    public static extern void PushAttrib(int mask);

    [DllImport("OPENGL32.DLL", EntryPoint = "glPopAttrib")]
    public static extern void PopAttrib();

    [DllImport("OPENGL32.DLL", EntryPoint = "glShadeModel")]
    public static extern void ShadeModel(int mode);

    [DllImport("OPENGL32.DLL", EntryPoint = "glLightModelfv")]
    public static extern void LightModel(int pname, float[] fparams);

    [DllImport("OPENGL32.DLL", EntryPoint = "glLightModeli")]
    public static extern void LightModel(int pname, int param);

    [DllImport("OPENGL32.DLL", EntryPoint = "glMaterialfv")]
    public static extern void Material(int face, int name, float[] parameter);

    [DllImport("OPENGL32.DLL", EntryPoint = "glLightfv")]
    public static extern void Light(int light, int name, float[] parameter);

    [DllImport("OPENGL32.DLL", EntryPoint = "glGetLightfv")]
    public static extern void GetLight(int light, int pname, float[] lparams);

    [DllImport("OPENGL32.DLL", EntryPoint = "glNormal3f")]
    public static extern void Normal(float nx, float ny, float nz);

    [DllImport("OPENGL32.DLL", EntryPoint = "glColorMaterial")]
    public static extern void ColorMaterial(int face, int mode);

    [DllImport("OPENGL32.DLL", EntryPoint = "glGetMaterialfv")]
    public static extern void GetMaterial(int face, int pname, float[] fparams);

    [DllImport("OPENGL32.DLL", EntryPoint = "glFogf")]
    public static extern void Fog(int pname, float param);

    [DllImport("OPENGL32.DLL", EntryPoint = "glFogfv")]
    public static extern void Fog(int pname, float[] fogparams);

    [DllImport("OPENGL32.DLL", EntryPoint = "glAccum")]
    public static extern void Accum(int op, float value);

    [DllImport("OPENGL32.DLL", EntryPoint = "glClearAccum")]
    public static extern void ClearAccum(float red, float green, float blue, float alpha);

    [DllImport("OPENGL32.DLL", EntryPoint = "glScissor")]
    public static extern void Scissor(int x, int y, int width, int height);

    [DllImport("OPENGL32.DLL", EntryPoint = "glRectf")]
    public static extern void Rect(float x1, float y1, float x2, float y2);

    [DllImport("OPENGL32.DLL", EntryPoint = "glVertexPointer")]
    public static extern void VertexPointer(int size, int type, int stride, IntPtr pointer);

    public static void VertexPointer(int size, int type, int stride, Array array)
    {
      GCHandle gcHandle = GCHandle.Alloc((object) array, GCHandleType.Pinned);
      try
      {
        gl.VertexPointer(size, type, stride, gcHandle.AddrOfPinnedObject());
      }
      finally
      {
        gcHandle.Free();
      }
    }

    [DllImport("OPENGL32.DLL", EntryPoint = "glDrawArrays")]
    public static extern void DrawArrays(int mode, int first, int count);

    [DllImport("OPENGL32.DLL", EntryPoint = "glFlush")]
    public static extern void Flush();

    [DllImport("OPENGL32.DLL", EntryPoint = "glFinish")]
    public static extern void Finish();

    [DllImport("OPENGL32.DLL", EntryPoint = "glReadPixels")]
    public static extern void ReadPixels(
      int x,
      int y,
      int width,
      int height,
      int format,
      int type,
      IntPtr pixels);

    public struct GLYPHMETRICSFLOAT
    {
      public float gmfBlackBoxX;
      public float gmfBlackBoxY;
      public float gmfptGlyphOriginX;
      public float gmfptGlyphOriginY;
      public float gmfCellIncX;
      public float gmfCellIncY;
    }
  }
}
