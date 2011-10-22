﻿namespace RandomAccessPerlinNoise
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class NoiseGenerator
    {
        private readonly long seed;
        private readonly double persistence;
        private readonly int levels;
        private readonly int[] size;
        private readonly bool smooth;
        private readonly IInterpolator interpolator;

        public NoiseGenerator(long seed, double persistence, int levels, int[] size, bool smooth, IInterpolator interpolator)
        {
            this.seed = seed;

            if (persistence < 0.0 || persistence > 1.0)
            {
                throw new ArgumentOutOfRangeException("persistence");
            }

            this.persistence = persistence;

            if (levels <= 0)
            {
                throw new ArgumentOutOfRangeException("levels");
            }

            this.levels = levels;

            this.smooth = smooth;

            if (size == null)
            {
                throw new ArgumentNullException("size");
            }
            else if (size.Length == 0)
            {
                throw new ArgumentOutOfRangeException("size");
            }

            this.size = size;

            if (interpolator == null)
            {
                throw new ArgumentNullException("interpolator");
            }

            this.interpolator = interpolator;
        }

        public void Fill(Array array, long[] location)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (array.Rank != this.size.Length)
            {
                throw new ArgumentOutOfRangeException("array");
            }

            var rand = GetRandom(this.seed, location);

            Fill(array, new int[array.Rank], 0, indices => rand.NextDouble());
        }

        private static void Fill<T>(Array array, int[] indices, int index, Func<int[], T> getValue)
        {
            var nextIndex = index + 1;

            if (index >= indices.Length)
            {
                array.SetValue(getValue(indices), indices);
            }
            else
            {
                var length = array.GetLength(index);

                for (int i = 0; i < length; i++)
                {
                    indices[index] = i;
                    Fill(array, indices, nextIndex, getValue);
                }
            }
        }

        private static CryptoPseudoRandom GetRandom(long seed, long[] location)
        {
            var a = BitConverter.GetBytes(seed);
            var len = a.Length;

            var seedBytes = new byte[len * (location.Length + 1)];
            Array.Copy(a, 0, seedBytes, 0, a.Length);

            for (int i = 0; i < location.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(location[i]), 0, seedBytes, len * (i + 1), len);
            }

            return new CryptoPseudoRandom(seedBytes);
        }
    }
}
