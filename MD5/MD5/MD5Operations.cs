namespace MD5
{
    static class MD5Operations
    {
        private static readonly uint[] T =
        {   
            0xd76aa478,0xe8c7b756,0x242070db,0xc1bdceee,
            0xf57c0faf,0x4787c62a,0xa8304613,0xfd469501,
            0x698098d8,0x8b44f7af,0xffff5bb1,0x895cd7be,
            0x6b901122,0xfd987193,0xa679438e,0x49b40821,
            0xf61e2562,0xc040b340,0x265e5a51,0xe9b6c7aa,
            0xd62f105d,0x2441453,0xd8a1e681,0xe7d3fbc8,
            0x21e1cde6,0xc33707d6,0xf4d50d87,0x455a14ed,
            0xa9e3e905,0xfcefa3f8,0x676f02d9,0x8d2a4c8a,
            0xfffa3942,0x8771f681,0x6d9d6122,0xfde5380c,
            0xa4beea44,0x4bdecfa9,0xf6bb4b60,0xbebfbc70,
            0x289b7ec6,0xeaa127fa,0xd4ef3085,0x4881d05,
            0xd9d4d039,0xe6db99e5,0x1fa27cf8,0xc4ac5665,
            0xf4292244,0x432aff97,0xab9423a7,0xfc93a039,
            0x655b59c3,0x8f0ccc92,0xffeff47d,0x85845dd1,
            0x6fa87e4f,0xfe2ce6e0,0xa3014314,0x4e0811a1,
            0xf7537e82,0xbd3af235,0x2ad7d2bb,0xeb86d391
        };


        public static uint Round1Op(uint a, uint b, uint c, uint d, uint k, ushort s, uint i, uint[] X)
        {
            uint temp = (a + F(b, c, d) + X[k] + T[i - 1]);
            uint temp2 = ((temp >> 32 - s) | (temp << s)); // bitwise rotation
            a = b + temp2;

            return a;
        }

        public static uint Round2Op(uint a, uint b, uint c, uint d, uint k, ushort s, uint i, uint[] X)
        {
            var temp = (a + G(b, c, d) + X[k] + T[i - 1]);
            var temp2 = ((temp >> 32 - s) | (temp << s)); // bitwise rotation
            a = b + temp2;

            return a;
        }

        public static uint Round3Op(uint a, uint b, uint c, uint d, uint k, ushort s, uint i, uint[] X)
        {
            var temp = (a + H(b, c, d) + X[k] + T[i - 1]);
            var temp2 = ((temp >> 32 - s) | (temp << s)); // bitwise rotation
            a = b + temp2;

            return a;
        }

        public static uint Round4Op(uint a, uint b, uint c, uint d, uint k, ushort s, uint i, uint[] X)
        {
            var temp = (a + I(b, c, d) + X[k] + T[i - 1]);
            var temp2 = ((temp >> 32 - s) | (temp << s)); // bitwise rotation
            a = b + temp2;

            return a;
        }

        private static uint F(uint X, uint Y, uint Z)
        {
            // X, Y and Z are 32 bit words, output is 32 bit word 
            return (X & Y) | (~X & Z); 
        }

        private static uint G(uint X, uint Y, uint Z)
        {
            // X, Y and Z are 32 bit words, output is 32 bit word 
            return (X & Z) | (Y & ~Z);
        }

        private static uint H(uint X, uint Y, uint Z)
        {
            // X, Y and Z are 32 bit words, output is 32 bit word 
            return X ^ Y ^ Z; 
        }

        private static uint I(uint X, uint Y, uint Z)
        {
            // X, Y and Z are 32 bit words, output is 32 bit word 
            return Y ^ (X | ~Z);
        }
    }
}
