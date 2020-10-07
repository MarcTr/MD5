using System;
using System.Collections;

namespace MD5
{
    public class Algorithm
    {
        public static string HashText(string message, bool verbose)
        {
            // convert original message to big-endian bitarray
            BitArray msgLE = Utility.StrToBitArray(message);
            BitArray msgBE = msgLE.ToBigEndian();
            return HashBitArray(msgBE, verbose);
        }

        public static string HashFile(string filePath, bool verbose)
        {
            // convert file to big-endian bitarray
            BitArray msgLE = Utility.FileToBitArray(filePath);
            BitArray msgBE = msgLE.ToBigEndian();
            return HashBitArray(msgBE, verbose);
        }

        private static string HashBitArray(BitArray bitArrayBE, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine("Input as Bitarray (big-endian):");
                Utility.PrintBitArray(bitArrayBE);
                Console.WriteLine();
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }


            int paddedLength;

            BitArray paddedMessage = ApplyPadding(bitArrayBE, out paddedLength, verbose);
            BitArray lowEndianArr = AppendLength(bitArrayBE, paddedLength, paddedMessage, verbose);
            string output = Digest(lowEndianArr, verbose);

            return output;
        }



        private static BitArray ApplyPadding(BitArray message, out int paddedLength, bool verbose)
        {
            //  RFC 1321:
            //  Step 1. Append Padding Bits

            //  The message is "padded"(extended) so that its length(in bits) is
            //  congruent to 448, modulo 512. That is, the message is extended so
            //  that it is just 64 bits shy of being a multiple of 512 bits long.
            //  Padding is always performed, even if the length of the message is
            //  already congruent to 448, modulo 512.

            //  Padding is performed as follows: a single "1" bit is appended to the
            //  message, and then "0" bits are appended so that the length in bits of
            //  the padded message becomes congruent to 448, modulo 512. In all, at
            //  least one bit and at most 512 bits are appended.


            // find next multiple of 512
            int lenInBits = message.Length;
            paddedLength = lenInBits + (512 - lenInBits % 512);

            // subtract 64 Bits, those are filled up in Step 2
            paddedLength -= 64;

            // ensure that 64 Bits are available to the next multiple of 512
            // (example: if lenInBits is 496, the next limit would be 512, 512-496 = 16 free bits, but 64 free bits are needed)
            if (paddedLength <= lenInBits)
            {
                paddedLength += 512;
            }

            if (verbose)
            {
                Console.WriteLine();
                Console.WriteLine("---------------------");
                Console.WriteLine("RFC1321: Step 1. Append Padding Bits");
            }

            // prepare new array to hold the message with padding
            BitArray paddedMessage = new BitArray(paddedLength);

            // copy the original message into the new array
            int i = 0;
            for (; i < message.Length; i++)
            {
                paddedMessage[i] = message[i];
            }

            // append "1"-bit to the message
            paddedMessage[i] = true;

            // other bits are 0 by default, no manual padding needed

            if (verbose)
            {
                Console.WriteLine("Padding: Message length: {0}, appended  \"1\"-Bit, all other bits are padded with \"0\":", paddedLength);
                Utility.PrintBitArray(paddedMessage, message.Length);
                Console.WriteLine();
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }

            return paddedMessage;
        }


        private static BitArray AppendLength(BitArray originalInput, int limit, BitArray paddedMessage, bool verbose)
        {

            //  RFC 1321:
            //  Step 2. Append Length

            //  A 64-bit representation of b (the length of the message before the
            //  padding bits were added) is appended to the result of the previous
            //  step. In the unlikely event that b is greater than 2^64, then only                  
            //  the low-order 64 bits of b are used. (These bits are appended as two
            //  32-bit words and appended low-order word first in accordance with the
            //  previous conventions.)

            //  At this point the resulting message (after padding with bits and with
            //  b) has a length that is an exact multiple of 512 bits. Equivalently,
            //  this message has a length that is an exact multiple of 16 (32-bit)
            //  words.Let M[0... N - 1] denote the words of the resulting message,
            //  where N is a multiple of 16.


            // prepare a new array that can hold the padded message so far and the length of the 
            // original message in 64-bit representation
            BitArray paddedMsgWLength = new BitArray(limit + 64);

            // copy the padded message into the new array
            int paddedMsgWLengthIndex = 0;
            for (; paddedMsgWLengthIndex < paddedMessage.Length; paddedMsgWLengthIndex++)
            {
                paddedMsgWLength[paddedMsgWLengthIndex] = paddedMessage[paddedMsgWLengthIndex];
            }


            // get 64-bit representation of length of original message
            byte[] msgLengthBytes = BitConverter.GetBytes(originalInput.Length);
            BitArray msgLengthBitsLE = new BitArray(msgLengthBytes);

            // TODO: possibly unnecessary
            // invert array 
            BitArray msgLengthBE = new BitArray(msgLengthBitsLE.Length);
            for (int i = 0; i < msgLengthBitsLE.Length; i++)
            {
                bool bit = msgLengthBitsLE[i];
                msgLengthBE[msgLengthBE.Length - 1 - i] = bit;
            }


            if (verbose)
            {
                Console.WriteLine();
                Console.WriteLine("---------------------");
                Console.WriteLine("RFC1321: Step 2. Append Length");
                Console.WriteLine("Append length of original message in 64-Bit representation to padded message:");
                Console.WriteLine("Length of original message in Bits: {0}", originalInput.Length);
                Console.WriteLine("Length of original message in big-endian representation:");
                Utility.PrintBitArray(msgLengthBE);
                Console.WriteLine();
            }


            // get low-order 32 bit word from message length
            BitArray lowOrderWord = new BitArray(32);
            for (int i = 0; i < 32; i++)
            {
                bool bitValue = false;

                if (msgLengthBE.Length - 1 - i >= 0)
                    bitValue = msgLengthBE[msgLengthBE.Length - 1 - i];


                lowOrderWord[lowOrderWord.Length - 1 - i] = bitValue;
            }

            // get high-order 32 bit word from message length
            BitArray highOrderWord = new BitArray(32);
            for (int i = 0; i < 32; i++)
            {
                bool bitValue = false;

                if (msgLengthBE.Length - 1 - i - 32 >= 0)
                    bitValue = msgLengthBE[msgLengthBE.Length - 1 - i - 32];


                highOrderWord[lowOrderWord.Length - 1 - i] = bitValue;
            }



            //  RFC 1321:
            //  Similarly, a sequence of bytes can be interpreted as a sequence of 32 - bit words, where each
            //  consecutive group of four bytes is interpreted as a word with the
            //  low-order (least significant) byte given first.


            // invert words, last 8 bit in word have to be the first now and so on
            BitArray lowOrderWordInv = lowOrderWord.InvertWordBitArray();
            BitArray highOrderWordInv = highOrderWord.InvertWordBitArray();

            if (verbose)
            {
                Console.WriteLine("RFC1321: The low order byte has to stand at the beginning of the 32-bit word");
                Console.WriteLine();
                Console.WriteLine("(Big Endian) LOW ORDER word of original message length in bits: ");
                Utility.PrintBitArray(lowOrderWordInv);
                Console.WriteLine();

                Console.WriteLine("(Big Endian) HIGH ORDER word of original message length in bits: ");
                Utility.PrintBitArray(highOrderWordInv);
                Console.WriteLine();
            }


            // append low order word to padded message first
            for (int i = 0; paddedMsgWLengthIndex < paddedMessage.Length + 32; paddedMsgWLengthIndex++)
            {
                paddedMsgWLength[paddedMsgWLengthIndex] = lowOrderWordInv[i];
                i++;
            }

            // append high order word to padded message second
            for (int i = 0; paddedMsgWLengthIndex < paddedMessage.Length + 64; paddedMsgWLengthIndex++)
            {
                paddedMsgWLength[paddedMsgWLengthIndex] = highOrderWordInv[i];
                i++;
            }


            if (verbose)
            {
                Console.WriteLine();
                Console.WriteLine("Original message padded to length of {0} bits with appended length of message:", (limit + 64));
                Utility.PrintBitArray(paddedMsgWLength, paddedMessage.Length);
                Console.WriteLine();
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }


            // convert to little endian
            // see also: https://crypto.stackexchange.com/questions/2099/why-do-all-hash-functions-use-big-endian-data
            BitArray paddedMsgWLengthLE = paddedMsgWLength.ToLittleEndian();

            if (verbose)
            {
                Console.WriteLine();
                Console.WriteLine("In Little-Endian Format (final message before digest):");
                Utility.PrintBitArray(paddedMsgWLengthLE);
                Console.WriteLine();
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
            }

            // return the fully padded message in little endian
            return paddedMsgWLengthLE;
        }


        private static string Digest(BitArray message, bool verbose)
        {
            // RFC 1321
            // Apply the message digest algorithm according to steps 3, 4 and 5


            // Step 3. Initialize MD Buffer
            // Initialize four-word MD buffer to compute the message digest
            uint A = 0x67452301;
            uint B = 0xefcdab89;
            uint C = 0x98badcfe;
            uint D = 0x10325476;

            if (verbose)
            {
                Console.WriteLine();
                Console.WriteLine("---------------------");
                Console.WriteLine("Digest-Algorithm:");
                Console.WriteLine("RFC1321: Step 3. Initialize MD Buffer");
                Console.WriteLine("A: {0:X}", A);
                Console.WriteLine("B: {0:X}", B);
                Console.WriteLine("C: {0:X}", C);
                Console.WriteLine("D: {0:X}", D);
                Console.WriteLine();
            }


            // Step 4. Process Message in 16-Word Blocks
            // Process 16 word blocks
            // Idea: for every 512 bits, copy block I into X 
            for (int i = 0; i < message.Length; i += 512)
            {

                if (verbose)
                {
                    Console.WriteLine("RFC1321: Step 4. Process Message in 16-Word Blocks");
                }


                BitArray block = new BitArray(512);
                for (int k = 0; k < 512; k++)
                {
                    block[k] = message[i + k];
                }


                uint[] X = new uint[16]; // each field is 1 byte
                byte[] blockByteArr = Utility.ConvertToByteArray(block);

                // Copy block i into X
                int xIndex = 0;
                for (int l = 0; l < blockByteArr.Length; l += 4)
                {
                    byte[] bytes = new byte[4];
                    bytes[0] = blockByteArr[l + 0];
                    bytes[1] = blockByteArr[l + 1];
                    bytes[2] = blockByteArr[l + 2];
                    bytes[3] = blockByteArr[l + 3];

                    X[xIndex] = BitConverter.ToUInt32(bytes, 0);
                    xIndex++;
                }


                // save the original values at the beginning of this block
                uint AA = A;
                uint BB = B;
                uint CC = C;
                uint DD = D;

                // Round 1 (16 operations)
                A = MD5Operations.Round1Op(A, B, C, D, 0, 7, 1, X);
                D = MD5Operations.Round1Op(D, A, B, C, 1, 12, 2, X);
                C = MD5Operations.Round1Op(C, D, A, B, 2, 17, 3, X);
                B = MD5Operations.Round1Op(B, C, D, A, 3, 22, 4, X);

                A = MD5Operations.Round1Op(A, B, C, D, 4, 7, 5, X);
                D = MD5Operations.Round1Op(D, A, B, C, 5, 12, 6, X);
                C = MD5Operations.Round1Op(C, D, A, B, 6, 17, 7, X);
                B = MD5Operations.Round1Op(B, C, D, A, 7, 22, 8, X);

                A = MD5Operations.Round1Op(A, B, C, D, 8, 7, 9, X);
                D = MD5Operations.Round1Op(D, A, B, C, 9, 12, 10, X);
                C = MD5Operations.Round1Op(C, D, A, B, 10, 17, 11, X);
                B = MD5Operations.Round1Op(B, C, D, A, 11, 22, 12, X);

                A = MD5Operations.Round1Op(A, B, C, D, 12, 7, 13, X);
                D = MD5Operations.Round1Op(D, A, B, C, 13, 12, 14, X);
                C = MD5Operations.Round1Op(C, D, A, B, 14, 17, 15, X);
                B = MD5Operations.Round1Op(B, C, D, A, 15, 22, 16, X);


                // Round 2 (16 operations)
                A = MD5Operations.Round2Op(A, B, C, D, 1, 5, 17, X);
                D = MD5Operations.Round2Op(D, A, B, C, 6, 9, 18, X);
                C = MD5Operations.Round2Op(C, D, A, B, 11, 14, 19, X);
                B = MD5Operations.Round2Op(B, C, D, A, 0, 20, 20, X);

                A = MD5Operations.Round2Op(A, B, C, D, 5, 5, 21, X);
                D = MD5Operations.Round2Op(D, A, B, C, 10, 9, 22, X);
                C = MD5Operations.Round2Op(C, D, A, B, 15, 14, 23, X);
                B = MD5Operations.Round2Op(B, C, D, A, 4, 20, 24, X);

                A = MD5Operations.Round2Op(A, B, C, D, 9, 5, 25, X);
                D = MD5Operations.Round2Op(D, A, B, C, 14, 9, 26, X);
                C = MD5Operations.Round2Op(C, D, A, B, 3, 14, 27, X);
                B = MD5Operations.Round2Op(B, C, D, A, 8, 20, 28, X);

                A = MD5Operations.Round2Op(A, B, C, D, 13, 5, 29, X);
                D = MD5Operations.Round2Op(D, A, B, C, 2, 9, 30, X);
                C = MD5Operations.Round2Op(C, D, A, B, 7, 14, 31, X);
                B = MD5Operations.Round2Op(B, C, D, A, 12, 20, 32, X);

                // Round 3 (16 operations)
                A = MD5Operations.Round3Op(A, B, C, D, 5, 4, 33, X);
                D = MD5Operations.Round3Op(D, A, B, C, 8, 11, 34, X);
                C = MD5Operations.Round3Op(C, D, A, B, 11, 16, 35, X);
                B = MD5Operations.Round3Op(B, C, D, A, 14, 23, 36, X);

                A = MD5Operations.Round3Op(A, B, C, D, 1, 4, 37, X);
                D = MD5Operations.Round3Op(D, A, B, C, 4, 11, 38, X);
                C = MD5Operations.Round3Op(C, D, A, B, 7, 16, 39, X);
                B = MD5Operations.Round3Op(B, C, D, A, 10, 23, 40, X);

                A = MD5Operations.Round3Op(A, B, C, D, 13, 4, 41, X);
                D = MD5Operations.Round3Op(D, A, B, C, 0, 11, 42, X);
                C = MD5Operations.Round3Op(C, D, A, B, 3, 16, 43, X);
                B = MD5Operations.Round3Op(B, C, D, A, 6, 23, 44, X);

                A = MD5Operations.Round3Op(A, B, C, D, 9, 4, 45, X);
                D = MD5Operations.Round3Op(D, A, B, C, 12, 11, 46, X);
                C = MD5Operations.Round3Op(C, D, A, B, 15, 16, 47, X);
                B = MD5Operations.Round3Op(B, C, D, A, 2, 23, 48, X);


                // Round 4 (16 operations)
                A = MD5Operations.Round4Op(A, B, C, D, 0, 6, 49, X);
                D = MD5Operations.Round4Op(D, A, B, C, 7, 10, 50, X);
                C = MD5Operations.Round4Op(C, D, A, B, 14, 15, 51, X);
                B = MD5Operations.Round4Op(B, C, D, A, 5, 21, 52, X);

                A = MD5Operations.Round4Op(A, B, C, D, 12, 6, 53, X);
                D = MD5Operations.Round4Op(D, A, B, C, 3, 10, 54, X);
                C = MD5Operations.Round4Op(C, D, A, B, 10, 15, 55, X);
                B = MD5Operations.Round4Op(B, C, D, A, 1, 21, 56, X);

                A = MD5Operations.Round4Op(A, B, C, D, 8, 6, 57, X);
                D = MD5Operations.Round4Op(D, A, B, C, 15, 10, 58, X);
                C = MD5Operations.Round4Op(C, D, A, B, 6, 15, 59, X);
                B = MD5Operations.Round4Op(B, C, D, A, 13, 21, 60, X);

                A = MD5Operations.Round4Op(A, B, C, D, 4, 6, 61, X);
                D = MD5Operations.Round4Op(D, A, B, C, 11, 10, 62, X);
                C = MD5Operations.Round4Op(C, D, A, B, 2, 15, 63, X);
                B = MD5Operations.Round4Op(B, C, D, A, 9, 21, 64, X);

                // increment each of the four registers by the value it had at the start of this block
                A = A + AA;
                B = B + BB;
                C = C + CC;
                D = D + DD;

                if (verbose)
                {
                    Console.WriteLine("Internal State at the beginning of the block:\tInternal State at the end of the block:");
                    Console.Write("A: {0:X}", AA);
                    Console.WriteLine("\t\t\t\t\tA: {0:X}", A);
                    Console.Write("B: {0:X}", BB);
                    Console.WriteLine("\t\t\t\t\tB: {0:X}", B);
                    Console.Write("C: {0:X}", CC);
                    Console.WriteLine("\t\t\t\t\tC: {0:X}", C);
                    Console.Write("D: {0:X}", DD);
                    Console.WriteLine("\t\t\t\t\tD: {0:X}", D);
                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
            }


            // Step 5
            // The message digest produced as output is A, B, C, D. That is, we
            // begin with the low-order byte of A, and end with the high - order byte
            // of D.
            if (verbose)
            {
                Console.WriteLine("RFC1321: Step 5. Output");
            }

            // convert to byte array
            byte[] ABytes = BitConverter.GetBytes(A);
            byte[] BBytes = BitConverter.GetBytes(B);
            byte[] CBytes = BitConverter.GetBytes(C);
            byte[] DBytes = BitConverter.GetBytes(D);

            // reverse bytes
            byte[] reversedBytesA = new byte[ABytes.Length];
            byte[] reversedBytesB = new byte[BBytes.Length];
            byte[] reversedBytesC = new byte[CBytes.Length];
            byte[] reversedBytesD = new byte[DBytes.Length];

            for (int k = ABytes.Length - 1, l = 0; k >= 0; k--, l++)
                reversedBytesA[l] = ABytes[k];

            for (int k = BBytes.Length - 1, l = 0; k >= 0; k--, l++)
                reversedBytesB[l] = BBytes[k];

            for (int k = CBytes.Length - 1, l = 0; k >= 0; k--, l++)
                reversedBytesC[l] = CBytes[k];

            for (int k = DBytes.Length - 1, l = 0; k >= 0; k--, l++)
                reversedBytesD[l] = DBytes[k];

            // build hashed output to display
            var p1 = BitConverter.ToInt32(reversedBytesA, 0);
            var p2 = BitConverter.ToInt32(reversedBytesB, 0);
            var p3 = BitConverter.ToInt32(reversedBytesC, 0);
            var p4 = BitConverter.ToInt32(reversedBytesD, 0);
            var res = p1.ToString("X8") + p2.ToString("X8") + p3.ToString("X8") + p4.ToString("X8");
            res = res.ToLower();


            return res;
        }
    }
}
