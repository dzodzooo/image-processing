using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MMS
{
    internal class Obrada : Boja
    {

        private int height, width;
        private Bitmap original;
        private string name;
        private string folder_out;

        private byte[,]? Y, U, V;
        bool YUV_set;

        public Obrada(string url)
        {
            Bitmap img;
            string[] tmp = url.Split('\\');
            if (tmp[tmp.Length - 1].Split('.')[1] == "photo")
            {
                img = Obrada.LoadFile(url);
            }
            else
            {
                img = new Bitmap(url);
            }
            if (img.PixelFormat != PixelFormat.Format24bppRgb)
                original = Obrada.ConvertTo24bpp(img);
            else
                original = img;

            width = img.Width;
            height = img.Height;
            name = url;
            YUV_set = false;

            folder_out = Environment.CurrentDirectory + "\\photos";          
        }

        public void SetOriginal(Bitmap bmp)
        {
            original = bmp;
            YUV_set = false;
        }
        public Bitmap GetOriginal()
        {
            return original;
        }
        public static Bitmap ConvertTo24bpp(Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }
        public void CompressFile(bool words_num_is_16)
        {
            if (!YUV_set)
            {
                Obrada.Downsample(width,height,original,out Y, out U, out V);
                
                YUV_set = true;
            }
            string path;
            path = this.folder_out + "\\" + this.name.Substring(this.name.LastIndexOf('\\') + 1).Split('.')[0] + "_.photo";
            var dict = CreateCodebook(words_num_is_16);

            using (var file = File.Open(path, FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(file);
                bw.Write(words_num_is_16);
                bw.Write(height);
                bw.Write(width);
                bw.Write(dict.Count);


                byte[] keys = dict.Keys.ToArray();

                foreach (byte x in keys)
                {
                    bw.Write(x);
                    bw.Write((byte)(dict[x].Length));
                    string[] tmp = new string[1];
                    tmp[0] = dict[x];
                    WriteDictToFile(tmp, bw);
                }
                int bits_num = 0;
                string[] encoded = Encode(dict, Y, height, width, words_num_is_16, ref bits_num);

                bits_num = 0;
                string extra = WriteToFile(encoded, bw, "");
                encoded = Encode(dict, U, height, width / 2 + width % 2, words_num_is_16, ref bits_num);
                extra = WriteToFile(encoded, bw, extra);
                encoded = Encode(dict, V, height, width / 2 + width % 2, words_num_is_16, ref bits_num);

                bits_num = 0;
                extra = WriteToFile(encoded, bw, extra);
                if (extra.Length > 0)
                {
                    extra += new string('0', 8 - extra.Length);
                    string[] tmp = new string[1];
                    tmp[0] = extra;
                    WriteToFile(tmp, bw, "");
                }
                bw.Close();
            }
        }
        private static void Downsample(int width, int height, Bitmap original, out byte[,] Y, out byte[,] U, out byte[,] V)
        {
            //RGBToYUV
            int width_half = width / 2;
            Y = new byte[height, width];
            U = new byte[height, width_half+width % 2];
            V = new byte[height, width_half + width % 2];
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

                    //odmah vrsimo downsample
                    if (j % 4 < 2)
                    {
                        U[i, tmp% width_half] = (byte)(128 + Math.Max(0, Math.Min(255, -0.169 * pixelR - 0.331 * pixelG + 0.500 * pixelB)));
                        V[i, tmp%width_half] = (byte)(128 + Math.Max(0, Math.Min(255, 0.5 * pixelR - 0.419 * pixelG - 0.081 * pixelB)));
                        tmp++;

                    }
                }
            }
        }

        private static void Downsample_byte(Bitmap original, out byte[,] Y, out byte[,] U, out byte[,] V)
        {
            //RGBToYUV
            int width = original.Width;
            int height = original.Height;

            int width_half = width / 2;

            Y = new byte[height, width];
            U = new byte[height, (width_half + width % 2)];
            V = new byte[height, (width_half + width % 2)];

            BitmapData data = original.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, original.PixelFormat);
            IntPtr ptr = data.Scan0;
            int size = Math.Abs(data.Stride) * height;
            byte[] bgr = new byte[size];
            Marshal.Copy(ptr, bgr, 0, size);
            int tmp = 0;
            int j = 0;
            int len = width * height;
            
            for (int i = 0; i < len; i += 3)
            {
                byte pixelB = bgr[i];
                byte pixelG = bgr[i + 1];
                byte pixelR = bgr[i + 2];
                Y[i/width,i%width] = (byte)(Math.Max(0, Math.Min(255, 0.299 * pixelR + 0.587 * pixelG + 0.114 * pixelB)));
                if (i % 4 == 0 || i % 4 == 3)
                {
                    
                    U[i/width,j%width_half] = (byte)(128 + Math.Max(0, Math.Min(255, -0.169 * pixelR - 0.331 * pixelG + 0.500 * pixelB)));
                    V[i/width,j%width_half] = (byte)(128 + Math.Max(0, Math.Min(255, 0.5 * pixelR - 0.419 * pixelG - 0.081 * pixelB)));
                    j ++;
                }
            }

            original.UnlockBits(data);
        }

        private Dictionary<byte, string> CreateCodebook(bool words_num_is_16)
        {
            Dictionary<byte, int> frequencies;
            if (words_num_is_16)
            {
                frequencies = this.Huffman16Dict();
            }
            else
            {
                frequencies = this.Huffman256Dict();
            }


            Node root = CreateCodeTree(frequencies);
            Dictionary<byte, string> Codebook = GetCodeWords(root);
            return Codebook;

        }

        private Dictionary<byte, int> Huffman16Dict()
        {
            Dictionary<byte, int> dict = new Dictionary<byte, int>();
            for (byte i = 0; i < 16; i++)
            {
                dict[i] = 0;
            }
            HuffmanAddToDict(Y, ref dict, height, width, true);
            HuffmanAddToDict(U, ref dict, height, width / 2 + width % 2, true);
            HuffmanAddToDict(V, ref dict, height, width / 2 + width % 2, true);
            for (byte i = 0; i < 16; i++)
                if (dict[i] == 0)
                    dict.Remove(i);
            return dict;
        }

        private Dictionary<byte, int> Huffman256Dict()
        {
            Dictionary<byte, int> dict = new Dictionary<byte, int>();
            dict[255] = 0;
            for (byte i = 0; i < 255; i++)
            {
                dict[i] = 0;
            }
            HuffmanAddToDict(Y, ref dict, height, width, false);
            HuffmanAddToDict(U, ref dict, height, width / 2 + width % 2, false);
            HuffmanAddToDict(V, ref dict, height, width / 2 + width % 2, false);
            for (byte i = 0; i < 255; i++)
            {
                if (dict[i] == 0)
                    dict.Remove(i);
            }
            return dict;
        }

        protected void HuffmanAddToDict(byte[,] to_encode, ref Dictionary<byte, int> dict, int height, int width, bool half)
        {

            if (half)
            {
                byte upper, low;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        upper = (byte)(to_encode[i, j] >> 4);
                        dict[upper]++;
                        low = (byte)(to_encode[i, j] & 15);
                        dict[low]++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        dict[to_encode[i, j]]++;
                    }
                }
            }

        }

        protected Node CreateCodeTree(Dictionary<byte, int> dict)
        {

            Dictionary<int, int> work_dict = new Dictionary<int, int>();
            foreach (var x in dict)
            {
                work_dict.Add(x.Key, x.Value);
            }
            int new_node_val = 256;
            Dictionary<int, Node> new_nodes = new Dictionary<int, Node>();
            while (work_dict.Count > 1)
            {

                KeyValuePair<int, int> tmp1 = work_dict.MinBy(d => d.Value);
                work_dict.Remove(tmp1.Key);
                KeyValuePair<int, int> tmp2 = work_dict.MinBy(d => d.Value);
                work_dict.Remove(tmp2.Key);

                Node root = new Node();
                root.key = new_node_val++;

                Node n;
                if (tmp1.Key < 256)
                {
                    n = new Node();
                    n.key = tmp1.Key;
                    root.left = n;
                }
                else
                {
                    root.left = new_nodes[tmp1.Key];
                }

                if (tmp2.Key < 256)
                {
                    n = new Node();
                    n.key = tmp2.Key;
                    root.right = n;
                }
                else
                {
                    root.right = new_nodes[tmp2.Key];
                }
                work_dict.Add(root.key, tmp1.Value + tmp2.Value);
                new_nodes.Add(root.key, root);
            }
            Node out_node = new_nodes[work_dict.ElementAt(0).Key];
            return out_node;
        }

        protected Dictionary<byte, string> GetCodeWords(Node root)
        {
            Dictionary<byte, string> codes = new Dictionary<byte, string>();
            Queue<Node> to_do = new Queue<Node>();
            to_do.Enqueue(root);
            int first = root.key;
            while (to_do.Count > 0)
            {
                Node tmp = to_do.Dequeue();
                if (tmp.left != null)
                {
                    to_do.Enqueue(tmp.left);
                    to_do.Enqueue(tmp.right);
                    tmp.left.code = tmp.code + "0";
                    tmp.right.code = tmp.code + "1";
                }
                else
                {
                    codes.Add((byte)tmp.key, "1" + tmp.code);
                }

            }
            return codes;

        }
        public static byte[,] Downsample(int width, int height, byte[,] ulaz)
        {
            int width_downsampled = width / 2 + width % 2;
            byte[,] downsampled = new byte[height, width_downsampled];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width_downsampled; j += 4)
                {
                    downsampled[i, j] = ulaz[i, j];
                    if (j < width_downsampled - 1)
                        downsampled[i, j + 1] = ulaz[i, j + 1];
                }
            }
            return downsampled;
        }









        protected string[] Encode(Dictionary<byte, string> codebook, byte[,] to_encode, int height, int width, bool words_num_is_16, ref int bits_num)
        {

            string[] out_value = new string[to_encode.Length];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (words_num_is_16)
                    {
                        out_value[i * width + j] = codebook[(byte)(to_encode[i, j] >> 4)];
                        out_value[i * width + j] += codebook[(byte)(to_encode[i, j] & 15)];
                        bits_num += out_value.Length;
                    }
                    else
                    {
                        out_value[i * width + j] = codebook[to_encode[i, j]];
                        bits_num += out_value.Length;
                    }
                }


            }
            return out_value;
        }



        private void WriteDictToFile(string[] values, BinaryWriter bw)
        {
            byte tmp;
            foreach (string x in values)
            {
                for (int i = 0; i < x.Length - x.Length % 8; i += 8)
                {
                    tmp = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        tmp <<= 1;
                        if (x[i + j] == '1')
                            tmp += 1;

                    }
                    bw.Write(tmp);
                }

                if (x.Length % 8 != 0)
                {
                    tmp = 0;
                    for (int j = x.Length - x.Length % 8; j < x.Length; j++)
                    {
                        tmp <<= 1;
                        if (x[j] == '1')
                            tmp += 1;
                    }
                    bw.Write(tmp);
                }
            }
        }

        private string WriteToFile(string[] values, BinaryWriter bw, string start)
        {
            byte tmp = 0;
            int i = 0;
            int j;
            string izlaz = "";
            if (start != "")
                values[0] = start + values[0];
            foreach (string x in values)
            {
                j = 0;
                while (j < x.Length)
                {
                    while (j < x.Length && i < 8)
                    {
                        izlaz += x[j];
                        tmp <<= 1;
                        if (x[j] == '1')
                            tmp += 1;
                        i++;
                        j++;
                    }
                    if (i == 8)
                    {

                        izlaz = "";
                        bw.Write(tmp);
                        tmp = 0;
                        i = 0;
                    }
                }
            }
            return izlaz;
        }






        public static Bitmap LoadFile(string url)
        {
            Bitmap original;
            using (var f = File.Open(url, FileMode.Open))
            {
                BinaryReader br = new BinaryReader(f);
                bool words_num_is_16 = br.ReadBoolean();
                int height = br.ReadInt32();
                int width = br.ReadInt32();
                original = new Bitmap(width, height);
                int dict_len = br.ReadInt32();
                int max_value_len;
                var codebook = Obrada.GetCodebook(br, dict_len, out max_value_len);

                string leftover = "";
                byte[,] Y, U, V;
                string start = "";

                List<byte> decoded = DecodeEverything(br, "", codebook, max_value_len, width, height);

                Y = new byte[height, width];
                U = new byte[height, width / 2 + width % 2];
                V = new byte[height, width / 2 + width % 2];

                for (int i = 0; i < width * height; i++)
                {
                    Y[i / width, i % width] = decoded[i];
                }
                int w = width / 2 + width % 2;
                int pom = width * height;
                for (int i = 0; i < w * height; i++)
                {
                    U[i / w, i % w] = decoded[pom + i];
                }
                pom += w * height;
                for (int i = 0; i < w * height; i++)
                {
                    V[i / w, i % w] = decoded[pom + i];
                }

                Obrada.YUVToRGB(height, width, out original, Y, U, V);


                br.Close();
            }
            return original;
        }

        public static List<byte> DecodeEverything(BinaryReader br, string start, Dictionary<string, byte> dict, int max_value_len, int width, int height)
        {
            int len = 2*width * height;
            int count = 0;
            List<byte> decoded = new List<byte>();
            string b = start;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                b += Obrada.ByteToString(br.ReadByte(), 8);
                int i = 1;
                bool fleg = false;
                while (true)
                {
                    while (i <= b.Length)
                    {
                        if (dict.ContainsKey(b.Substring(0, i)))
                        {
                            decoded.Add(dict[b.Substring(0, i)]);
                            b = b.Substring(i);
                            count++;
                            fleg = true;
                        }
                        i++;

                    }
                    i = 1;
                    if (!fleg) break;
                    fleg = false;
                }
                
                if (b.Length > max_value_len) throw new Exception("vrednost duza od najduze reci u recniku");
            }
            if (b.Length > 0 && decoded.Count != 2 * width * height) throw new Exception("greska");
            return decoded;

        }

        public static Dictionary<string, byte> GetCodebook(BinaryReader br, int dict_len, out int max_value_len)
        {
            max_value_len = 0;
            Dictionary<string, byte> dict = new Dictionary<string, byte>();
            for (int i = 0; i < dict_len; i++)
            {
                byte key = br.ReadByte();
                byte len_of_val = br.ReadByte();
                byte bytes_num = (byte)(len_of_val % 8 == 0 ? len_of_val / 8 : len_of_val / 8 + 1);

                byte[] value_in_bytes = br.ReadBytes(bytes_num);
                string value_as_string = "";

                for (int j = 0; j < bytes_num - 1; j++)
                {
                    value_as_string += Obrada.ByteToString(value_in_bytes[j], 8);
                }
                if (len_of_val % 8 == 0)
                    value_as_string += Obrada.ByteToString(value_in_bytes[bytes_num - 1], 8);
                else
                    value_as_string += Obrada.ByteToString(value_in_bytes[bytes_num - 1], (byte)(len_of_val % 8));
                dict.Add(value_as_string, key);
                if (value_as_string.Length > max_value_len)
                    max_value_len = value_as_string.Length;
            }
            return dict;
        }

        public static string ByteToString(byte to_do, byte len)
        {
            string s = "";
            while (len > 0)
            {
                s = (to_do % 2) + s;
                to_do /= 2;
                len--;
            }
            return s;
        }


    }
}
