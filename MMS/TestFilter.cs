using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS
{
    internal class TestFilter : Filter
    {
        public static void Test_StandardDeviation()
        {
            byte[] val = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            int[] start_index = { 0, 0 };
            int width = 5, height = 5;
            int quadrant_size = 3;

            byte val_avg;
            byte ret_val = Filter.StandardDeviation(ref start_index, ref val, width, height, quadrant_size, out val_avg);

            ;
        }

        public static void Test_KuwaharaOperator()
        {
            byte[] val = { 233, 233, 233, 1, 2, 233, 233, 233, 3, 4, 233, 233, 233, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            int width = 5, height = 5;
            int quadrant_size = 3;

            byte ret_val = Filter.KuwaharaOperator(val, 2, 2, width, height, quadrant_size);
            ;
        }

        public static void Test_BurkesOperator()
        {
            byte[] value= { 1,2,3,4,5,6};
            int index = 1;
            int width=3;
            int size=6;
            int error=2;
            BurkesOperator(ref value, index, width, size, error);
            ;
        }
    }
}
