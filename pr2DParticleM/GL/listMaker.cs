
using System;
using System.Drawing;

namespace GL
{
  public static class listMaker
  {
    public const float torusMinSmallRadius = 0.01f;

    public static void BMPGlyphs(Font font, int deviceContext, ref uint glyphsBaseList)
    {
      if (glyphsBaseList != 0U && gl.IsList(glyphsBaseList))
        return;
      glyphsBaseList = gl.GenLists(256);
      IntPtr objectHandle = gl.SelectObject(deviceContext, font.ToHfont());
      if (!gl.UseFontBitmaps(deviceContext, 0, 256, glyphsBaseList))
        throw new Exception("Списки битовых карт символов не установлены.");
      gl.SelectObject(deviceContext, objectHandle);
    }

    public static void OutlineGlyphs(
      Font font,
      gl.GLYPHMETRICSFLOAT[] metrics,
      float deviation,
      float extrusion,
      int format,
      int deviceContext,
      ref uint outlineGlyphsBaseList)
    {
      if (outlineGlyphsBaseList != 0U && gl.IsList(outlineGlyphsBaseList))
        return;
      IntPtr objectHandle = gl.SelectObject(deviceContext, font.ToHfont());
      outlineGlyphsBaseList = gl.GenLists(256);
      if (!gl.UseFontOutlines(deviceContext, 0U, 256U, outlineGlyphsBaseList, deviation, extrusion, format, metrics))
        throw new Exception("Списки символов контурного шрифта не установлены.");
      gl.SelectObject(deviceContext, objectHandle);
    }

    public static void objLineAxes(int mode, uint outlineGlyphsBaseList, ref uint axesList)
    {
      if (axesList != 0U && gl.IsList(axesList))
      {
        if (mode != 4865)
          return;
        gl.CallList(axesList);
      }
      else
      {
        axesList = gl.GenLists(1);
        gl.NewList(axesList, mode);
        gl.Begin(1);
        gl.Color(0.0f, 1f, 0.0f);
        gl.Vertex(0.0f, -1f, 0.0f);
        gl.Vertex(0.0f, 1f, 0.0f);
        gl.Color(1f, 0.0f, 0.0f);
        gl.Vertex(-1f, 0.0f, 0.0f);
        gl.Vertex(1f, 0.0f, 0.0f);
        gl.Color(1f, 1f, 0.0f);
        gl.Vertex(0.0f, 0.0f, -1f);
        gl.Vertex(0.0f, 0.0f, 1f);
        gl.End();
        gl.PushAttrib(131080);
        gl.ListBase(outlineGlyphsBaseList);
        gl.Color(1f, 0.0f, 0.0f);
        gl.PushMatrix();
        gl.Translate(0.98f, 0.0f, 0.0f);
        gl.Rotate(90f, 0.0f, 1f, 0.0f);
        gl.Scale(0.1f, 0.1f, 0.1f);
        gl.CallLists(1, 5121, "X");
        gl.PopMatrix();
        gl.Color(0.0f, 1f, 0.0f);
        gl.PushMatrix();
        gl.Translate(0.0f, 0.98f, 0.0f);
        gl.Rotate(90f, 1f, 0.0f, 0.0f);
        gl.Rotate(90f, 0.0f, 0.0f, 1f);
        gl.Scale(0.1f, 0.1f, 0.1f);
        gl.CallLists(1, 5121, "Y");
        gl.PopMatrix();
        gl.Color(1f, 1f, 0.0f);
        gl.PushMatrix();
        gl.Translate(0.0f, 0.0f, 0.98f);
        gl.Scale(0.1f, 0.1f, 0.1f);
        gl.CallLists(1, 5121, "Z");
        gl.PopMatrix();
        gl.PopAttrib();
        gl.EndList();
      }
    }

    public static void Sphere(int mode, int slices, int stacks, float alpha, ref uint sphereList)
    {
      if (sphereList != 0U && gl.IsList(sphereList))
      {
        if (mode != 4865)
          return;
        gl.CallList(sphereList);
      }
      else
      {
        float[] numArray1 = new float[stacks];
        float[] numArray2 = new float[stacks];
        float[] numArray3 = new float[slices + 1];
        float[] numArray4 = new float[slices + 1];
        for (int index = 0; index <= slices; ++index)
        {
          numArray3[index] = (float) Math.Sin(2.0 * Math.PI * (double) index / (double) slices);
          numArray4[index] = (float) Math.Cos(2.0 * Math.PI * (double) index / (double) slices);
        }
        for (int index = 0; index < stacks; ++index)
        {
          numArray1[index] = (float) Math.Sin(Math.PI * (double) index / (double) stacks);
          numArray2[index] = (float) Math.Cos(Math.PI * (double) index / (double) stacks);
        }
        sphereList = gl.GenLists(1);
        gl.NewList(sphereList, mode);
        gl.PushAttrib(1);
        gl.Begin(6);
        gl.Color(0.0f, 0.0f, 1f, alpha);
        gl.Normal(0.0f, 1f, 0.0f);
        gl.Vertex(0.0f, 1f, 0.0f);
        for (int index = 0; index <= slices; ++index)
        {
          gl.Normal(numArray1[1] * numArray3[index], numArray2[1], numArray1[1] * numArray4[index]);
          gl.Vertex(numArray1[1] * numArray3[index], numArray2[1], numArray1[1] * numArray4[index]);
        }
        gl.End();
        for (int index1 = 1; index1 < stacks - 1; ++index1)
        {
          if (index1 < stacks / 4)
          {
            float num;
            gl.Color(0.0f, num = 4f * (float) index1 / (float) stacks, 1f - num, alpha);
          }
          else if (index1 < stacks / 2)
          {
            float num;
            gl.Color(num = 4f * (float) (index1 - stacks / 4) / (float) stacks, 1f - num, 0.0f, alpha);
          }
          else if (index1 < 3 * stacks / 4)
          {
            float num;
            gl.Color(num = (float) (1.0 - 4.0 * (double) (index1 - stacks / 2) / (double) stacks), 1f - num, 0.0f, alpha);
          }
          else
          {
            float num;
            gl.Color(0.0f, num = (float) (1.0 - 4.0 * (double) (index1 - 3 * stacks / 4) / (double) stacks), 1f - num, alpha);
          }
          gl.Begin(5);
          for (int index2 = 0; index2 <= slices; ++index2)
          {
            gl.Normal(numArray1[index1] * numArray3[index2], numArray2[index1], numArray1[index1] * numArray4[index2]);
            gl.Vertex(numArray1[index1] * numArray3[index2], numArray2[index1], numArray1[index1] * numArray4[index2]);
            gl.Normal(numArray1[index1 + 1] * numArray3[index2], numArray2[index1 + 1], numArray1[index1 + 1] * numArray4[index2]);
            gl.Vertex(numArray1[index1 + 1] * numArray3[index2], numArray2[index1 + 1], numArray1[index1 + 1] * numArray4[index2]);
          }
          gl.End();
        }
        gl.Begin(6);
        gl.Normal(0.0f, -1f, 0.0f);
        gl.Vertex(0.0f, -1f, 0.0f);
        for (int index = slices; index >= 0; --index)
        {
          gl.Normal(numArray1[stacks - 1] * numArray3[index], numArray2[stacks - 1], numArray1[stacks - 1] * numArray4[index]);
          gl.Vertex(numArray1[stacks - 1] * numArray3[index], numArray2[stacks - 1], numArray1[stacks - 1] * numArray4[index]);
        }
        gl.End();
        gl.PopAttrib();
        gl.EndList();
      }
    }

    public static void Frustum(
      int mode,
      int slices,
      int stacks,
      float topRadius,
      float alpha,
      ref uint frustumList)
    {
      if ((double) topRadius < 0.0 || (double) topRadius > 1.0)
        throw new Exception("Параметр topRadius должен лежать в интервале [0;1].");
      if (frustumList != 0U && gl.IsList(frustumList))
      {
        if (mode != 4865)
          return;
        gl.CallList(frustumList);
      }
      else
      {
        float nz = (float) Math.Cos(Math.Atan2(1.0, 1.0 - (double) topRadius));
        float[] numArray1 = new float[slices + 1];
        float[] numArray2 = new float[slices + 1];
        for (int index = 0; index <= slices; ++index)
        {
          numArray1[index] = (float) Math.Sin(2.0 * Math.PI * (double) index / (double) slices);
          numArray2[index] = (float) Math.Cos(2.0 * Math.PI * (double) index / (double) slices);
        }
        frustumList = gl.GenLists(1);
        gl.NewList(frustumList, mode);
        gl.PushAttrib(1);
        for (int index1 = 0; index1 < stacks; ++index1)
        {
          float num1 = (float) (1.0 - (double) index1 * (1.0 - (double) topRadius) / (double) stacks);
          float num2 = (float) (1.0 - (double) (index1 + 1) * (1.0 - (double) topRadius) / (double) stacks);
          float z1 = 1f * (float) index1 / (float) stacks;
          float z2 = 1f * (float) (index1 + 1) / (float) stacks;
          if (index1 < stacks / 2)
          {
            float num3;
            gl.Color(num3 = (float) (1.0 - 2.0 * (double) index1 / (double) stacks), 1f - num3, 0.0f, alpha);
          }
          else
          {
            float num4;
            gl.Color(0.0f, num4 = (float) (1.0 - 2.0 * (double) (index1 - stacks / 2) / (double) stacks), 1f - num4, alpha);
          }
          if ((double) topRadius == 0.0 && index1 == stacks - 1)
          {
            gl.Begin(6);
            gl.Vertex(0.0f, 0.0f, 1f);
            for (int index2 = slices; index2 >= 0; --index2)
            {
              float num5 = num1 * numArray2[index2];
              float num6 = num1 * numArray1[index2];
              gl.Normal(num5, num6, nz);
              gl.Vertex(num5, num6, z1);
            }
            gl.End();
          }
          else
          {
            gl.Begin(8);
            for (int index3 = slices; index3 >= 0; --index3)
            {
              float num7 = num1 * numArray2[index3];
              float num8 = num1 * numArray1[index3];
              float num9 = num2 * numArray2[index3];
              float num10 = num2 * numArray1[index3];
              gl.Normal(num7, num8, nz);
              gl.Vertex(num7, num8, z1);
              gl.Normal(num9, num10, nz);
              gl.Vertex(num9, num10, z2);
            }
            gl.End();
          }
        }
        gl.PopAttrib();
        gl.EndList();
      }
    }

    public static void Disk(
      int mode,
      int slices,
      int stacks,
      float intRadius,
      float alpha,
      ref uint diskList)
    {
      if ((double) intRadius < 0.0 || (double) intRadius >= 1.0)
        throw new Exception("Параметр intRadius должен лежать в интервале [0;1).");
      if (diskList != 0U && gl.IsList(diskList))
      {
        if (mode != 4865)
          return;
        gl.CallList(diskList);
      }
      else
      {
        float[] numArray1 = new float[slices + 1];
        float[] numArray2 = new float[slices + 1];
        for (int index = 0; index <= slices; ++index)
        {
          numArray1[index] = (float) Math.Sin(2.0 * Math.PI * (double) index / (double) slices);
          numArray2[index] = (float) Math.Cos(2.0 * Math.PI * (double) index / (double) slices);
        }
        diskList = gl.GenLists(1);
        gl.NewList(diskList, mode);
        gl.PushAttrib(1);
        gl.Normal(0.0f, 0.0f, 1f);
        for (int index1 = 0; index1 < stacks; ++index1)
        {
          float num1 = (float) (1.0 - (double) index1 * (1.0 - (double) intRadius) / (double) stacks);
          float num2 = (float) (1.0 - (double) (index1 + 1) * (1.0 - (double) intRadius) / (double) stacks);
          if (index1 < stacks / 2)
          {
            float num3;
            gl.Color(num3 = (float) (1.0 - 2.0 * (double) index1 / (double) stacks), 1f - num3, 0.0f, alpha);
          }
          else
          {
            float num4;
            gl.Color(0.0f, num4 = (float) (1.0 - 2.0 * (double) (index1 - stacks / 2) / (double) stacks), 1f - num4, alpha);
          }
          if ((double) intRadius == 0.0 && index1 == stacks - 1)
          {
            gl.Begin(6);
            gl.Vertex(0.0f, 0.0f);
            for (int index2 = slices; index2 >= 0; --index2)
              gl.Vertex(num1 * numArray2[index2], num1 * numArray1[index2]);
            gl.End();
          }
          else
          {
            gl.Begin(8);
            for (int index3 = slices; index3 >= 0; --index3)
            {
              gl.Vertex(num1 * numArray2[index3], num1 * numArray1[index3]);
              gl.Vertex(num2 * numArray2[index3], num2 * numArray1[index3]);
            }
            gl.End();
          }
        }
        gl.PopAttrib();
        gl.EndList();
      }
    }

    public static void Torus(
      int mode,
      int bigSlices,
      int smallSlices,
      float smallRadius,
      float alpha,
      ref uint torusList)
    {
      if ((double) smallRadius <= 0.00999999977648258 || (double) smallRadius >= 0.5)
        throw new Exception(string.Format("Малый радиус тора должен лежать в интервале ({0}0;0.5).", (object) 0.01f));
      if (torusList != 0U && gl.IsList(torusList))
      {
        if (mode != 4865)
          return;
        gl.CallList(torusList);
      }
      else
      {
        torusList = gl.GenLists(1);
        float[] numArray1 = new float[smallSlices + 1];
        float[] numArray2 = new float[smallSlices + 1];
        float[] numArray3 = new float[bigSlices + 1];
        float[] numArray4 = new float[bigSlices + 1];
        for (int index = 0; index <= smallSlices; ++index)
        {
          numArray1[index] = (float) Math.Sin(2.0 * Math.PI * (double) index / (double) smallSlices);
          numArray2[index] = (float) Math.Cos(2.0 * Math.PI * (double) index / (double) smallSlices);
        }
        for (int index = 0; index <= bigSlices; ++index)
        {
          numArray3[index] = (float) Math.Sin(2.0 * Math.PI * (double) index / (double) bigSlices);
          numArray4[index] = (float) Math.Cos(2.0 * Math.PI * (double) index / (double) bigSlices);
        }
        gl.NewList(torusList, mode);
        gl.PushAttrib(1);
        for (int index1 = 0; index1 < smallSlices; ++index1)
        {
          float num1 = (float) (1.0 - (double) smallRadius * (1.0 - (double) numArray2[index1]));
          float num2 = (float) (1.0 - (double) smallRadius * (1.0 - (double) numArray2[index1 + 1]));
          if (index1 < smallSlices / 4)
          {
            float num3;
            gl.Color(num3 = (float) (1.0 - 4.0 * (double) index1 / (double) smallSlices), 1f - num3, 0.0f, alpha);
          }
          else if (index1 < smallSlices / 2)
          {
            float num4;
            gl.Color(0.0f, num4 = (float) (1.0 - 4.0 * (double) (index1 - smallSlices / 4) / (double) smallSlices), 1f - num4, alpha);
          }
          else if (index1 < 3 * smallSlices / 4)
          {
            float num5;
            gl.Color(0.0f, num5 = 4f * (float) (index1 - smallSlices / 2) / (float) smallSlices, 1f - num5, alpha);
          }
          else
          {
            float num6;
            gl.Color(num6 = 4f * (float) (index1 - 3 * smallSlices / 4) / (float) smallSlices, 1f - num6, 0.0f, alpha);
          }
          gl.Begin(5);
          for (int index2 = bigSlices; index2 >= 0; --index2)
          {
            gl.Normal(numArray4[index2] * numArray2[index1], numArray2[index1] * numArray3[index2], numArray1[index1]);
            gl.Vertex(num1 * numArray4[index2], num1 * numArray3[index2], smallRadius * numArray1[index1]);
            gl.Normal(numArray4[index2] * numArray2[index1 + 1], numArray2[index1 + 1] * numArray3[index2], numArray1[index1 + 1]);
            gl.Vertex(num2 * numArray4[index2], num2 * numArray3[index2], smallRadius * numArray1[index1 + 1]);
          }
          gl.End();
        }
        gl.PopAttrib();
        gl.EndList();
      }
    }

    public static void CylinderAxe(
      int mode,
      float radius,
      float pointLength,
      float pointAngle,
      char label,
      uint bmpGlyphsBaseList,
      ref uint axeList)
    {
      float[] parameter = new float[4]{ 1f, 0.0f, 0.0f, 1f };
      if (axeList != 0U && gl.IsList(axeList))
      {
        if (mode != 4865)
          return;
        gl.CallList(axeList);
      }
      else
      {
        axeList = gl.GenLists(1);
        float num = (float) Math.Tan(Math.PI * (double) pointAngle / 180.0);
        uint frustumList1 = 0;
        uint frustumList2 = 0;
        listMaker.Frustum(4864, 100, 100, 1f, 1f, ref frustumList1);
        listMaker.Frustum(4864, 100, 100, 0.0f, 1f, ref frustumList2);
        gl.NewList(axeList, mode);
        gl.PushMatrix();
        gl.Scale(radius, radius, (float) (1.0 - (double) radius / (double) num));
        gl.CallList(frustumList1);
        gl.PopMatrix();
        gl.PushAttrib(64);
        gl.Material(1028, 5632, parameter);
        gl.PushMatrix();
        gl.Translate(0.0f, 0.0f, 1f - pointLength);
        gl.Scale(pointLength * num, pointLength * num, pointLength);
        gl.CallList(frustumList2);
        gl.PopMatrix();
        if (char.IsLetterOrDigit(label))
        {
          gl.PushAttrib(131073);
          gl.Color(0.0f, 1f, 1f);
          gl.RasterPosf(0.0f, 0.0f, 1f);
          gl.ListBase(bmpGlyphsBaseList);
          gl.CallLists(1, 5121, Convert.ToString(label));
          gl.PopAttrib();
        }
        gl.PopAttrib();
        gl.EndList();
      }
    }

    public static void CylinderAxes(int mode, uint bmpGlyphsBaseList, ref uint axesList)
    {
      if (axesList != 0U && gl.IsList(axesList))
      {
        if (mode != 4865)
          return;
        gl.CallList(axesList);
      }
      else
      {
        axesList = gl.GenLists(1);
        uint axeList1 = 0;
        uint axeList2 = 0;
        uint axeList3 = 0;
        listMaker.CylinderAxe(4864, 0.008f, 0.1f, 20f, 'X', bmpGlyphsBaseList, ref axeList1);
        listMaker.CylinderAxe(4864, 0.008f, 0.1f, 20f, 'Y', bmpGlyphsBaseList, ref axeList2);
        listMaker.CylinderAxe(4864, 0.008f, 0.1f, 20f, 'Z', bmpGlyphsBaseList, ref axeList3);
        gl.NewList(axesList, mode);
        gl.PushMatrix();
        gl.Rotate(90f, 0.0f, 1f, 0.0f);
        gl.Scale(1f, 1f, 2f);
        gl.Translate(0.0f, 0.0f, -0.5f);
        gl.CallList(axeList1);
        gl.PopMatrix();
        gl.PushMatrix();
        gl.Rotate(-90f, 1f, 0.0f, 0.0f);
        gl.Scale(1f, 1f, 2f);
        gl.Translate(0.0f, 0.0f, -0.5f);
        gl.CallList(axeList2);
        gl.PopMatrix();
        gl.PushMatrix();
        gl.Scale(1f, 1f, 2f);
        gl.Translate(0.0f, 0.0f, -0.5f);
        gl.CallList(axeList3);
        gl.PopMatrix();
        gl.EndList();
      }
    }

    public static void Star(int mode, ref uint starList)
    {
      if (starList != 0U && gl.IsList(starList))
      {
        if (mode != 4865)
          return;
        gl.CallList(starList);
      }
      else
      {
        starList = gl.GenLists(1);
        gl.NewList(starList, mode);
        gl.Begin(9);
        gl.Vertex(-0.5f * (float) Math.Sin(9.0 * Math.PI / 5.0), 0.5f * (float) Math.Cos(9.0 * Math.PI / 5.0));
        gl.Vertex(0.0f, 1f);
        gl.Vertex(-0.5f * (float) Math.Sin(Math.PI / 5.0), 0.5f * (float) Math.Cos(Math.PI / 5.0));
        gl.Vertex(-(float) Math.Sin(2.0 * Math.PI / 5.0), (float) Math.Cos(2.0 * Math.PI / 5.0));
        gl.Vertex(-0.5f * (float) Math.Sin(3.0 * Math.PI / 5.0), 0.5f * (float) Math.Cos(3.0 * Math.PI / 5.0));
        gl.Vertex(-(float) Math.Sin(4.0 * Math.PI / 5.0), (float) Math.Cos(4.0 * Math.PI / 5.0));
        gl.Vertex(0.0f, -0.5f);
        gl.Vertex(-(float) Math.Sin(6.0 * Math.PI / 5.0), (float) Math.Cos(6.0 * Math.PI / 5.0));
        gl.Vertex(-0.5f * (float) Math.Sin(7.0 * Math.PI / 5.0), 0.5f * (float) Math.Cos(7.0 * Math.PI / 5.0));
        gl.Vertex(-(float) Math.Sin(8.0 * Math.PI / 5.0), (float) Math.Cos(8.0 * Math.PI / 5.0));
        gl.End();
        gl.EndList();
      }
    }

    public static void Cardioid(int mode, int vertices, ref uint cardioidList)
    {
      if (cardioidList != 0U && gl.IsList(cardioidList))
      {
        if (mode != 4865)
          return;
        gl.CallList(cardioidList);
      }
      else
      {
        cardioidList = gl.GenLists(1);
        gl.NewList(cardioidList, mode);
        gl.Begin(9);
        for (int index = 0; index < vertices; ++index)
        {
          double num1 = 2.0 * Math.PI * (double) index / (double) vertices;
          float num2 = (float) Math.Cos(num1);
          float num3 = 1f + num2;
          gl.Vertex(0.75f * num3 * (float) Math.Sin(num1), (float) (0.75 * (-(double) num3 * (double) num2 + 1.0)));
        }
        gl.End();
        gl.EndList();
      }
    }
  }
}
