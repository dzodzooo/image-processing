using Cake.Core;
using Cake.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace MMS
{
    internal class Filter : Boja
    {
        private static readonly byte[] BURKES_WEIGHTS = { 8, 4, 2, 4, 8, 4, 2 };
        private static readonly byte BURKES_SHIFT_SIZE = 6;
        public static Bitmap Invert(Bitmap bmp)
        {
            if (bmp != null)
            {
                Bitmap img;
                if (bmp.PixelFormat != PixelFormat.Format24bppRgb)
                    img = Obrada.ConvertTo24bpp(bmp);
                else
                    img = bmp;

                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, img.PixelFormat);
                IntPtr ptr = data.Scan0;
                int size = Math.Abs(data.Stride) * img.Height;
                byte[] rgb = new byte[size];
                Marshal.Copy(ptr, rgb, 0, size);
                for (int i = 0; i < size; i++)
                {
                    rgb[i] = (byte)(255 - rgb[i]);
                }
                Marshal.Copy(rgb, 0, ptr, size);
                img.UnlockBits(data);
                return img;
            }
            return null;
        }

        public static Bitmap KuwaharaFilter(Bitmap bmp, int window_size=5)
        {
            if (bmp != null)
            {
                window_size += (1 - window_size % 2);
                Bitmap img;
                if (bmp.PixelFormat != PixelFormat.Format24bppRgb)
                    img = Obrada.ConvertTo24bpp(bmp);
                else
                    img = bmp;

                int[] hue;
                byte[] sat, val;
                Boja.RGBToHSV(img, img.Width, img.Height, out hue, out sat, out val);

                int index = 0 ;
                var quadrant_size = (int)Math.Ceiling(window_size / 2.0);
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        val[index] = KuwaharaOperator(val, x, y, img.Width, img.Height, quadrant_size);
                        index++;
                    }
                }

                Bitmap img_out;
                Boja.HSVToRGB(hue, sat, val, out img_out, img.Width, img.Height);
                return img_out;
            }
            return null;
        }

        protected static byte KuwaharaOperator(byte[] val, int x, int y, int width, int height, int quadrant_size)
        {
            int[] q1 = { Math.Max(x-quadrant_size+1,0),Math.Max(0,y-quadrant_size+1)};
            int[] q2 = { x, Math.Max(y - quadrant_size + 1,0) };
            int[] q3 = { Math.Max(x - quadrant_size + 1,0), y };
            int[] q4 = { x, y };

            byte[] q_avg=new byte[4];
            byte[] q_dev = new byte[4];

            q_dev[0] = StandardDeviation(ref q1, ref val, width, height, quadrant_size, out q_avg[0]);
            q_dev[1] = StandardDeviation(ref q2, ref val, width, height, quadrant_size, out q_avg[1]);
            q_dev[2] = StandardDeviation(ref q3, ref val, width, height, quadrant_size, out q_avg[2]);
            q_dev[3] = StandardDeviation(ref q4, ref val, width, height, quadrant_size, out q_avg[3]);

            byte[] min = new byte[2];
            min[0] = q_dev[0];
            min[1] = q_avg[0];

            for (int i = 1; i < 4; i++)
            {
                if (min[0] > q_dev[i])
                {
                    min[0] = q_dev[i];
                    min[1] = q_avg[i];
                }
            }
            return min[1];
        }

        protected static byte StandardDeviation(ref int[] start_index, ref byte[] val, int width, int height, int quadrant_size, out byte val_avg)
        {
            int mean = 0;
            int br = 0;
            int bound_x = Math.Min(width, start_index[0] + quadrant_size);
            int bound_y = Math.Min(height, start_index[1] + quadrant_size);

            for(int y = start_index[1]; y < bound_y; y++)
            {
                for(int x = start_index[0];x< bound_x; x++)
                {
                    mean += val[width*y+x];
                    br++;
                }
            }
            mean /= br;
            val_avg = (byte)mean;

            double deviations = 0;
            for (int y = start_index[1]; y < bound_y; y++)
            {
                for (int x = start_index[0]; x < bound_x; x++)
                {
                    deviations += Math.Pow((val[width * y + x] - mean), 2);
                }
            }
            double std_dev=deviations/br;
            return (byte)std_dev;
        }

        public static Bitmap BurkesDithering(Bitmap bmp)
        {
            if (bmp != null)
            {
                Bitmap img;
                if (bmp.PixelFormat != PixelFormat.Format24bppRgb)
                    img = Obrada.ConvertTo24bpp(bmp);
                else
                    img = bmp;
                int width=img.Width;
                int height=img.Height;
                int size_img=width*height;
                byte[] S, V;
                int[] H;
                Boja.RGBToHSV(img,width,height,out H, out S, out V);

                Bitmap img_out;
                int[] tmp = new int[size_img];
                

                int mid = (int)V.Average(x => (x));
                for(int i = 0; i < size_img; i ++)
                {
                    byte old_pixel = V[i];
                    V[i] = (byte)(V[i]>mid?100:0);
                    int error = old_pixel - V[i];
                    BurkesOperator(ref V, i, width, size_img, error);
                }
                byte[] dummy = new byte[size_img];
                int[] dummy_int = new int[size_img];
                Boja.HSVToRGB(dummy_int, dummy, V,out img_out,width,height);
                return img_out;
            }
            return null;
        }

        protected static void BurkesOperator(ref byte[] value,int index, int width, int size, int error)
        {
            int i = 0;
            int w = index % width;
            for (i = 1; i<3&&w+i<width; i++)
            {
                value[index + i] += (byte)(error * Filter.BURKES_WEIGHTS[i-1] >> BURKES_SHIFT_SIZE);
                if (value[index + i] > 100)
                {
                    value[index + i] = 100;
                }
            }
            index += width;
            if (index >= size) return;
            value[index] += (byte)(error * Filter.BURKES_WEIGHTS[2] >> BURKES_SHIFT_SIZE);
            if (value[index] > 100)
            {
                value[index] = 100;
            }

            for (i=1; i < 3; i++)
            {
                if (index % width - i > -1)
                {
                    value[index - i] += (byte)(error * Filter.BURKES_WEIGHTS[5- i]>>BURKES_SHIFT_SIZE);
                    if (value[index - i] > 100)
                    {
                        value[index - i] = 100;
                    }
                }
                if (index%width+i<width)
                {
                    value[index + i] += (byte)(error * Filter.BURKES_WEIGHTS[4+i] >> BURKES_SHIFT_SIZE);
                    if (value[index + i] >100)
                    {
                        value[index + i] = 100;
                    }

                }
            }


        }
    }
}
