using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MMS
{
    internal class Boja
    {
        protected static void YUVToRGB(int height, int width, out Bitmap original, byte[,] Y, byte[,] U, byte[,] V)
        {
            var R = new byte[height, width];
            var G = new byte[height, width];
            var B = new byte[height, width];

            double tmp;
            original = new Bitmap(width, height);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j % 2 == 0)
                    {
                        U[i, j / 2] -= 128;
                        V[i, j / 2] -= 128;
                    }

                    R[i, j] = (byte)(Math.Max(Math.Min(Y[i, j] + 1.14 * V[i,j/2], 255), 0));

                    tmp = Y[i,j]-0.343*U[i,j/2]-0.711*V[i,j/2];
                    G[i, j] = (byte)(Math.Max(Math.Min(tmp, 255), 0));

                    tmp = Y[i,j]+1.765*U[i,j/2];
                    B[i, j] = (byte)(Math.Max(Math.Min(tmp, 255), 0));

                    original.SetPixel(j, i, Color.FromArgb(R[i, j], G[i, j], B[i, j]));
                }
            }
        }

        protected static void YUVToRGB_byte(int height, int width, out Bitmap original, byte[,] Y, byte[,] U, byte[,] V)
        {
            var R = new byte[height, width];
            var G = new byte[height, width];
            var B = new byte[height, width];

            double tmp;
            original = new Bitmap(width, height);
            BitmapData data = original.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            IntPtr ptr = data.Scan0;
            int size = Math.Abs(data.Stride) * height;
            byte[] bgr = new byte[size];
            int index = width*height-1;
            Marshal.Copy(ptr, bgr, 0, size);
            tmp = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j % 2 == 0)
                    {
                        U[i, j / 2] -= 128;
                        V[i, j / 2] -= 128;
                    }

                    R[i, j] = (byte)(Math.Max(Math.Min(Y[i, j] + 1.14 * V[i, j / 2], 255), 0));

                    tmp = Y[i, j] - 0.343 * U[i, j / 2] - 0.711 * V[i, j / 2];
                    G[i, j] = (byte)(Math.Max(Math.Min(tmp, 255), 0));

                    tmp = Y[i, j] + 1.765 * U[i, j / 2];
                    B[i, j] = (byte)(Math.Max(Math.Min(tmp, 255), 0));

                    bgr[index--] = B[i, j];
                    bgr[index--] = G[i, j];
                    bgr[index--] = R[i, j];
                }
            }
            Marshal.Copy(bgr, 0, ptr, size);
            original.UnlockBits(data);
        }


        private static void RGBToYUV(int width, int height, Bitmap original, out byte[,] Y, out byte[,] U, out byte[,] V)
        {
            Y = new byte[height, width];
            U = new byte[height, width];
            V = new byte[height, width];
            int tmp;
            byte pixelR, pixelG, pixelB;
            for (int i = 0; i < height; i++)
            {
                tmp = 0;
                for (int j = 0; j < width; j++)
                {
                    Color clr = original.GetPixel(j, i);
                    pixelG = clr.G;
                    pixelB = clr.B;
                    pixelR = clr.R;

                    Y[i, j] = (byte)(Math.Max(0, Math.Min(255, 0.299 * pixelR + 0.587 * pixelG + 0.114 * pixelB)));
                    U[i, j] = (byte)(Math.Max(0, Math.Min(255, 0.492 * (pixelB -Y[i,j]))));
                    V[i, j] = (byte)(Math.Max(0, Math.Min(255, 0.877 * (pixelR -Y[i,j]))));

                }
            }
        }

        protected static void RGBToYUV_byte(Bitmap original, out byte[] Y, out byte[] U, out byte[] V)
        {
            int width = original.Width;
            int height = original.Height;

            Y = new byte[height * width];
            U = new byte[height * width];
            V = new byte[height * width];

            BitmapData data = original.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, original.PixelFormat);
            IntPtr ptr = data.Scan0;
            int size = Math.Abs(data.Stride) * height;
            byte[] bgr = new byte[size];
            Marshal.Copy(ptr, bgr, 0, size);
            int tmp = 0;
            for (int i = 0; i < size; i += 3)
            {
                byte pixelB = bgr[i];
                byte pixelG = bgr[i + 1];
                byte pixelR = bgr[i + 2];
                Y[tmp] = (byte)(Math.Max(0, Math.Min(255, 0.299 * pixelR + 0.587 * pixelG + 0.114 * pixelB)));
                U[tmp] = (byte)(128 - Math.Max(0, Math.Min(255, -0.169 * pixelR - 0.331 * pixelG + 0.500 * pixelB)));
                V[tmp++] = (byte)(128 + Math.Max(0, Math.Min(255, 0.5 * pixelR - 0.419 * pixelG - 0.081 * pixelB)));


            }

            original.UnlockBits(data);
        }

        
        



        protected static void HSVToRGB(int[] hue, byte[] sat, byte[] val, out Bitmap bmp, int width, int height)
        {
            bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = data.Scan0;
            int size = Math.Abs(data.Stride) * height;
            byte[] rgb = new byte[size];
            Marshal.Copy(ptr, rgb, 0, size);
            int len = width * height;
            for (int i = 0; i < len; i++)
            {
                int[] tmp = { hue[i], sat[i], val[i] };
                Color clr = Boja.HSVToRGB_clr(tmp);
                int j = i * 3;
                rgb[j] = clr.B;
                rgb[j + 1] = clr.G;
                rgb[j + 2] = clr.R;
            }
            Marshal.Copy(rgb, 0, ptr, size);
            bmp.UnlockBits(data);
        }

        protected static void RGBToHSV(Bitmap bmp, int width, int height, out int[] hue, out byte[] sat, out byte[] val)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = data.Scan0;
            int size = Math.Abs(data.Stride) * height;
            byte[] rgb = new byte[size];
            Marshal.Copy(ptr, rgb, 0, size);

            int len = width * height;
            hue = new int[len];
            sat = new byte[len];
            val = new byte[len];

            int index = 0;
            int rgb_len = len * 3;
            for (int i = 0; i < rgb_len; i += 3)
            {
                Color clr = Color.FromArgb(rgb[i + 2], rgb[i + 1], rgb[i]);
                int[] pixel_hsv = Boja.RGBToHSV_clr(clr);
                hue[index] = pixel_hsv[0];
                sat[index] = (byte)pixel_hsv[1];
                val[index] = (byte)pixel_hsv[2];
                index++;
            }
            Marshal.Copy(rgb, 0, ptr, size);
            bmp.UnlockBits(data);
        }
     
        private static Color HSVToRGB_clr(int[] hsv)
        {
            double hue = hsv[0] / 60.0;
            double saturation = hsv[1] / 100.0;
            double value = hsv[2] / 100.0;

            int hi = (int)Math.Floor(hue) % 6;
            double f = hue - Math.Floor(hue);

            value = value * 255;
            byte v = (byte)Math.Max(0, Math.Min(255, Convert.ToInt32(value)));
            byte p = (byte)Math.Max(0, Math.Min(255, Convert.ToInt32(value * (1 - saturation))));
            byte q = (byte)Math.Max(0, Math.Min(255, Convert.ToInt32(value * (1 - f * saturation))));
            byte t = (byte)Math.Max(0, Math.Min(255, Convert.ToInt32(value * (1 - (1 - f) * saturation))));
            Color ret_val;

            if (hi == 0)
                ret_val = Color.FromArgb(v, t, p);
            else if (hi == 1)
                ret_val = Color.FromArgb(q, v, p);
            else if (hi == 2)
                ret_val = Color.FromArgb(p, v, t);
            else if (hi == 3)
                ret_val = Color.FromArgb(p, q, v);
            else if (hi == 4)
                ret_val = Color.FromArgb(t, p, v);
            else
                ret_val = Color.FromArgb(v, p, q);
            return ret_val;
        }
        
        private static int[] RGBToHSV_clr(Color rgb)
        {
            int[] hsv = new int[3];
            var r = rgb.R / 255.0;
            var g = rgb.G / 255.0;
            var b = rgb.B / 255.0;

            double cmax = Math.Max(r, Math.Max(g, b));
            double cmin = Math.Min(r, Math.Min(g, b));
            double diff = cmax - cmin;
            double h = 0, s;

            if (diff == 0) h = 0;
            else if (cmax == r)
                h = (60 * ((g - b) / diff) + 360) % 360;
            else if (cmax == g)
                h = (60 * (((b - r) / diff) + 2)) % 360;
            else if (cmax == b)
                h = (60 * ((r - g) / diff) + 240) % 360;

            if (cmax == 0)
                s = 0;
            else
                s = (diff / cmax) * 100;

            double v = cmax * 100;

            hsv[0] = (int)Math.Max(0, Math.Min(360, h));
            hsv[1] = (int)Math.Max(0, Math.Min(100, s));
            hsv[2] = (int)Math.Max(0, Math.Min(100, v));

            return hsv;
        }
    }
}
