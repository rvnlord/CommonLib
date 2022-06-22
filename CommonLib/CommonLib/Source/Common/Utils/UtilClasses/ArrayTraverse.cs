﻿using System;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    internal class ArrayTraverse
    {
        public int[] Position;
        private readonly int[] _maxLengths;

        public ArrayTraverse(Array array)
        {
            _maxLengths = new int[array.Rank];
            for (var i = 0; i < array.Rank; ++i)
                _maxLengths[i] = array.GetLength(i) - 1;
            Position = new int[array.Rank];
        }

        public bool Step()
        {
            for (var i = 0; i < Position.Length; ++i)
            {
                if (Position[i] >= _maxLengths[i]) 
                    continue;

                Position[i]++;
                for (var j = 0; j < i; j++)
                    Position[j] = 0;
                return true;
            }
            return false;
        }
    }
}

