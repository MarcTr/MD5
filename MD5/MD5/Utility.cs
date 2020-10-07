using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MD5
{
    class Utility
    {
        public static BitArray StrToBitArray(string input)
        {
            // Convert input to byte array
            Encoding enc = Encoding.UTF8;
            byte[] inputByteArr = enc.GetBytes(input);

            // Convert input to bit array (little-endian)
            BitArray inputBitArrayLE = new BitArray(inputByteArr);
            return inputBitArrayLE;
        }

        public static BitArray FileToBitArray(string filePath)
        {
            // Convert input to byte array
            byte[] inputByteArr = File.ReadAllBytes(filePath);

            // Convert input to bit array (little-endian)
            BitArray inputBitArrayLE = new BitArray(inputByteArr);
            return inputBitArrayLE;
        }

        public static byte[] ConvertToByteArray(BitArray bits)
        {
            byte[] bytes = new byte[bits.Length / 8];
            bits.CopyTo(bytes, 0);
            return bytes;
        }

        public static void PrintBitArray(BitArray bitArray, int? markFrom = null)
        {
            for (int i = 0; i < bitArray.Length; i++)
            {
                // set color to mark relevant part of output
                if (markFrom != null && markFrom == i)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                bool bit = bitArray[i];
                Console.Write(bit == false ? "0" : "1");
            }

            // reset color
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine();
        }
    }
}
