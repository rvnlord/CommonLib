﻿using System;

namespace CommonLib.Source.Common.Utils.TypeUtils
{
    public static class TUtils
    {
        public static void SwapIf<T>(Func<T, T, bool> cond, ref T o1, ref T o2)
        {
            if (cond == null)
                throw new ArgumentNullException(nameof(cond));

            if (!cond(o1, o2))
                return;

            var temp = o1;
            o1 = o2;
            o2 = temp;
        }
    }
}
