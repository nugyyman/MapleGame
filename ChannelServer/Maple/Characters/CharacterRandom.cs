using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loki.IO;

namespace Loki.Maple.Characters
{
    class CharacterRandom
    {

        private uint[] mSeeds { get; set; }

        public CharacterRandom()
        {
            mSeeds = new uint[3];
            Random random = new Random();
            uint beginSeed = (uint)random.Next();
            ResetSeeds(beginSeed, beginSeed, beginSeed);

            random = null;
        }

        private void ResetSeeds(uint seed1, uint seed2, uint seed3)
        {
            mSeeds[0] = seed1 | 0x100000;
            mSeeds[1] = seed2 | 0x1000;

            mSeeds[2] = seed3 | 0x10;
        }

        private uint NextSeed()
        {
            mSeeds[0] = ((mSeeds[0] & 0xFFFFFFFE) << 12) ^ (((mSeeds[0] << 13) ^ mSeeds[0]) >> 19);
            mSeeds[1] = ((mSeeds[1] & 0xFFFFFFF8) << 4) ^ (((mSeeds[1] << 2) ^ mSeeds[1]) >> 25);
            mSeeds[2] = ((mSeeds[2] & 0xFFFFFFF0) << 17) ^ (((mSeeds[2] << 3) ^ mSeeds[2]) >> 11);
            return (mSeeds[0] ^ mSeeds[1] ^ mSeeds[2]);
        }

        public byte[] ToByteArray()
        {
            uint seed1 = NextSeed();
            uint seed2 = NextSeed();
            uint seed3 = NextSeed();

            ResetSeeds(seed1, seed2, seed3);

            using (ByteBuffer buffer = new ByteBuffer())
            {
                buffer.WriteUInt(seed1);
                buffer.WriteUInt(seed2);
                buffer.WriteUInt(seed3);

                buffer.Flip();
                return buffer.GetContent();
            }
        }
    }
}
