using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public static class TextureExtension
{
    // Texture fill
    public struct Point
    {
        public short x;
        public short y;
        public Point(short aX, short aY) { x = aX; y = aY; }
        public Point(int aX, int aY) : this((short)aX, (short)aY) { }
    }

    public static bool CompareColor32(Color32 colA, Color32 colB)
    {
        return (colA.r + colA.g + colA.b) == (colB.r + colB.g + colB.b);
    }

    public static class TextureFill
    {
        private static Color32[] colors;
        private static Color32[] colorsComp;

        public static void ReleaseFillCache()
        {
            colors = colorsComp = null;
            System.GC.Collect();
        }

        public static void FloodFillArea(Texture2D aTex, int aX, int aY, Color32 aFillColor)
        {
            int w = aTex.width;
            int h = aTex.height;

            if (colors == null)
                colors = aTex.GetPixels32();

            Color32 refCol = colors[aX + aY * w];
            Queue<Point> nodes = new Queue<Point>();
            nodes.Enqueue(new Point(aX, aY));
            while (nodes.Count > 0)
            {
                Point current = nodes.Dequeue();
                for (int i = current.x; i < w; i++)
                {
                    Color32 C = colors[i + current.y * w];
                    if (!CompareColor32(C, refCol) ||
                        CompareColor32(C, aFillColor))
                        break;
                    colors[i + current.y * w] = aFillColor;
                    if (current.y + 1 < h)
                    {
                        C = colors[i + current.y * w + w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colors[i + current.y * w - w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
                for (int i = current.x - 1; i >= 0; i--)
                {
                    Color32 C = colors[i + current.y * w];
                    if (!CompareColor32(C, refCol) ||
                       CompareColor32(C, aFillColor))
                        break;
                    colors[i + current.y * w] = aFillColor;
                    if (current.y + 1 < h)
                    {
                        C = colors[i + current.y * w + w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colors[i + current.y * w - w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
            }
            aTex.SetPixels32(colors);
        }

        public static void FloodFillArea(Texture2D aTex, Texture2D comparableTex, int aX, int aY, Color32 aFillColor)
        {
            int w = comparableTex.width;
            int h = comparableTex.height;

            if (colors == null)
                colors = aTex.GetPixels32();

            if (colorsComp == null)
                colorsComp = comparableTex.GetPixels32();

            Color32 refCol = colorsComp[aX + aY * w];
            if (refCol.r == 0 &&
                refCol.g == 0 &&
                refCol.b == 0)
                return;

            Queue<Point> nodes = new Queue<Point>();
            nodes.Enqueue(new Point(aX, aY));
            while (nodes.Count > 0)
            {
                Point current = nodes.Dequeue();
                for (int i = current.x; i < w; i++)
                {
                    Color32 C = colorsComp[i + current.y * w];
                    if (!CompareColor32(C, refCol) ||
                        CompareColor32(C, aFillColor))
                        break;

                    colors[i + current.y * w] = colorsComp[i + current.y * w] = aFillColor;

                    if (current.y + 1 < h)
                    {
                        C = colorsComp[i + current.y * w + w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colorsComp[i + current.y * w - w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
                for (int i = current.x - 1; i >= 0; i--)
                {
                    Color32 C = colorsComp[i + current.y * w];
                    if (!CompareColor32(C, refCol) ||
                       CompareColor32(C, aFillColor))
                        break;

                    colors[i + current.y * w] = colorsComp[i + current.y * w] = aFillColor;

                    if (current.y + 1 < h)
                    {
                        C = colorsComp[i + current.y * w + w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colorsComp[i + current.y * w - w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
            }

            aTex.SetPixels32(colors);
            comparableTex.SetPixels32(colorsComp);
        }

        public static void FloodFillAreaWithTexturePattern(Texture2D aTex, Texture2D comparableTex, int aX, int aY)
        {
            int w = comparableTex.width;
            int h = comparableTex.height;

            Color32 aFillColor = new Color(1, 1, 1, 1);

            if (colors == null)
                colors = aTex.GetPixels32();

            if (colorsComp == null)
                colorsComp = comparableTex.GetPixels32();

            Color32 refCol = colorsComp[aX + aY * w];
            if (refCol.r == 0 &&
                refCol.g == 0 &&
                refCol.b == 0)
                return;

            Queue<Point> nodes = new Queue<Point>();
            nodes.Enqueue(new Point(aX, aY));
            while (nodes.Count > 0)
            {
                Point current = nodes.Dequeue();
                for (int i = current.x; i < w; i++)
                {
                    Color32 C = colorsComp[i + current.y * w];

                    if (!CompareColor32(C, refCol) ||
                        CompareColor32(C, aFillColor))
                        break;

                    colors[i + current.y * w] = colorsComp[i + current.y * w] = aFillColor;

                    if (current.y + 1 < h)
                    {
                        C = colorsComp[i + current.y * w + w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colorsComp[i + current.y * w - w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
                for (int i = current.x - 1; i >= 0; i--)
                {
                    Color32 C = colorsComp[i + current.y * w];
                    if (!CompareColor32(C, refCol) ||
                       CompareColor32(C, aFillColor))
                        break;

                    colors[i + current.y * w] = colorsComp[i + current.y * w] = aFillColor;

                    if (current.y + 1 < h)
                    {
                        C = colorsComp[i + current.y * w + w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colorsComp[i + current.y * w - w];
                        if (CompareColor32(C, refCol) &&
                            !CompareColor32(C, aFillColor))
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
            }

            aTex.SetPixels32(colors);
            comparableTex.SetPixels32(colorsComp);
        }
    }

    // Texture scale
    public class ThreadData
    {
        public int start;
        public int end;
        public ThreadData(int s, int e)
        {
            start = s;
            end = e;
        }
    }

    private static Color[] texColors;
    private static Color[] newColors;
    private static int w;
    private static float ratioX;
    private static float ratioY;
    private static int w2;
    private static int finishCount;
    private static Mutex mutex;

    public static void ScalePoint(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, false);
    }

    public static void ScaleBilinear(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, true);
    }

    private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
    {
        texColors = tex.GetPixels();
        newColors = new Color[newWidth * newHeight];
        if (useBilinear)
        {
            ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
            ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
        }
        else
        {
            ratioX = ((float)tex.width) / newWidth;
            ratioY = ((float)tex.height) / newHeight;
        }
        w = tex.width;
        w2 = newWidth;
        var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
        var slice = newHeight / cores;

        finishCount = 0;
        if (mutex == null)
            mutex = new Mutex(false);

        if (cores > 1)
        { 
            int i = 0;
            ThreadData threadData;
            for (i = 0; i < cores - 1; i++)
            {
                threadData = new ThreadData(slice * i, slice * (i + 1));
                ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                Thread thread = new Thread(ts);
                thread.Start(threadData);
            }
            threadData = new ThreadData(slice * i, newHeight);
            if (useBilinear)
            {
                BilinearScale(threadData);
            }
            else
            {
                PointScale(threadData);
            }
            while (finishCount < cores)
            {
                Thread.Sleep(1);
            }
        }
        else
        {
            ThreadData threadData = new ThreadData(0, newHeight);
            if (useBilinear)
            {
                BilinearScale(threadData);
            }
            else
            {
                PointScale(threadData);
            }
        }

        tex.Resize(newWidth, newHeight);
        tex.SetPixels(newColors);
        tex.Apply();
    }

    public static void BilinearScale(System.Object obj)
    {
        ThreadData threadData = (ThreadData)obj;
        for (var y = threadData.start; y < threadData.end; y++)
        {
            int yFloor = (int)Mathf.Floor(y * ratioY);
            var y1 = yFloor * w;
            var y2 = (yFloor + 1) * w;
            var yw = y * w2;

            for (var x = 0; x < w2; x++)
            {
                int xFloor = (int)Mathf.Floor(x * ratioX);
                var xLerp = x * ratioX - xFloor;
                newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                                                       ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                                                       y * ratioY - yFloor);
            }
        }

        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }

    public static void PointScale(System.Object obj)
    {
        ThreadData threadData = (ThreadData)obj;
        for (var y = threadData.start; y < threadData.end; y++)
        {
            var thisY = (int)(ratioY * y) * w;
            var yw = y * w2;
            for (var x = 0; x < w2; x++)
            {
                newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
            }
        }

        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }

    private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
    {
        return new Color(c1.r + (c2.r - c1.r) * value,
                          c1.g + (c2.g - c1.g) * value,
                          c1.b + (c2.b - c1.b) * value,
                          c1.a + (c2.a - c1.a) * value);
    }
}