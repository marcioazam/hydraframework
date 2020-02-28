using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydraFramework
{
    public class HydraTuple<T1, T2>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }

        internal HydraTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }

    public class HydraTuple<T1, T2, T3>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }

        internal HydraTuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }
    }

    public static class HydraTuple
    {
        public static HydraTuple<T1, T2> New<T1, T2>(T1 item1, T2 item2)
        {
            var tuple = new HydraTuple<T1, T2>(item1, item2);

            return tuple;
        }

        public static HydraTuple<T1, T2, T3> New<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            var tuple = new HydraTuple<T1, T2, T3>(item1, item2, item3);

            return tuple;
        }
    }
}
