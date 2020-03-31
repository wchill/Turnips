using System;
using System.Collections.Generic;
using System.Text;

namespace Turnips
{
    public class SeadRandom
    {
        private uint[] _mContext;

        public SeadRandom(uint seed = 42069)
        {
            _mContext = new uint[4];
            _mContext[0] = 0x6C078965 * (seed ^ (seed >> 30)) + 1;
            _mContext[1] = 0x6C078965 * (_mContext[0] ^ (_mContext[0] >> 30)) + 2;
            _mContext[2] = 0x6C078965 * (_mContext[1] ^ (_mContext[1] >> 30)) + 3;
            _mContext[3] = 0x6C078965 * (_mContext[2] ^ (_mContext[2] >> 30)) + 4;
        }

        public SeadRandom(uint seed1, uint seed2, uint seed3, uint seed4)
        {
            if ((seed1 | seed2 | seed3 | seed4) == 0) // seeds must not be all zero.
            {
                seed1 = 1;
                seed2 = 0x6C078967;
                seed3 = 0x714ACB41;
                seed4 = 0x48077044;
            }

            _mContext[0] = seed1;
            _mContext[1] = seed2;
            _mContext[2] = seed3;
            _mContext[3] = seed4;
        }

        public uint getU32()
        {
            uint n = _mContext[0] ^ (_mContext[0] << 11);

            _mContext[0] = _mContext[1];
            _mContext[1] = _mContext[2];
            _mContext[2] = _mContext[3];
            _mContext[3] = n ^ (n >> 8) ^ _mContext[3] ^ (_mContext[3] >> 19);

            return _mContext[3];
        }

        public ulong getU64()
        {
            uint n1 = _mContext[0] ^ (_mContext[0] << 11);
            uint n2 = _mContext[1];
            uint n3 = n1 ^ (n1 >> 8) ^ _mContext[3];

            _mContext[0] = _mContext[2];
            _mContext[1] = _mContext[3];
            _mContext[2] = n3 ^ (_mContext[3] >> 19);
            _mContext[3] = n2 ^ (n2 << 11) ^ ((n2 ^ (n2 << 11)) >> 8) ^ _mContext[2] ^ (n3 >> 19);

            return ((ulong)_mContext[2] << 32) | _mContext[3];
        }

        public uint[] getContext()
        {
            return _mContext;
        }
}
}
