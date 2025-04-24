using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS
{
    internal class TestObrada:Obrada
    {
        public TestObrada(string ime_fajla) : base(ime_fajla)
        {
        }
        public void Test()
        {
            byte[,] podaci = {
                { 1, 2 },
                { 3, 4 },
                { 3, 1 },
                { 3, 3 } };

            Dictionary<byte, int> frequencies = new Dictionary<byte, int>();
            for (int i = 0; i < 256; i++)
            {
                frequencies[(byte)i] = 0;
            }
            HuffmanAddToDict(podaci, ref frequencies, 4, 2, false);
            for (int i = 0; i < 256; i++)
                if (frequencies[(byte)i] == 0)
                    frequencies.Remove((byte)i);

            Node root = CreateCodeTree(frequencies);
            Dictionary<byte, string> Codebook = GetCodeWords(root);
            int bits_num = 0;
            string[] encoded = Encode(Codebook, podaci, 4, 2, false, ref bits_num);
            string s = "";
            foreach (var x in encoded)
            {
                s += x;
            }

            List<byte> decoded = new List<byte>();
            string start = "";
            //string leftover = "";
            //int max_bytes = 1;
            //int iter = 0;
            Dictionary<string, byte> Codebook2 = new Dictionary<string, byte>();
            foreach (var x in Codebook)
            {
                Codebook2.Add(x.Value, x.Key);
            }
            while (s.Length > 0)
            {
                string b = start + s.Substring(0, Math.Min(8, s.Length));
                s = s.Substring(Math.Min(8, s.Length));
                int i = 1;
                bool fleg = false;
                while (true)
                {
                    while (i <= b.Length)
                    {
                        if (Codebook2.ContainsKey(b.Substring(0, i)))
                        {
                            decoded.Add(Codebook2[b.Substring(0, i)]);
                            b = b.Substring(i);
                            fleg = true;
                        }
                        i++;

                    }
                    i = 1;
                    if (!fleg) break;
                    fleg = false;
                }
                if (s.Length > 0)
                {
                    start = b;

                }
                else
                {
                    if (b.Length > 0)
                        throw new Exception("doslo je do greske");
                }

            }
        }

    }
}
