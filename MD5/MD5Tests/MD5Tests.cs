using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MD5Tests
{
    [TestClass]
    public class MD5Tests
    {
        [TestMethod]
        public void Hashing()
        {
            // MD5 test suite according to RFC 1321
            string input0 = "";
            string output0 = MD5.Algorithm.HashText(input0, false);
            string output0Expected = "d41d8cd98f00b204e9800998ecf8427e";
            Assert.AreEqual(output0Expected, output0, true, "Wrong output for input {0}", input0);

            string input1 = "a";
            string output1 = MD5.Algorithm.HashText(input1, false);
            string output1Expected = "0cc175b9c0f1b6a831c399e269772661";
            Assert.AreEqual(output1Expected, output1, true, "Wrong output for input {0}", input1);

            string input2 = "abc";
            string output2 = MD5.Algorithm.HashText(input2, false);
            string output2Expected = "900150983cd24fb0d6963f7d28e17f72";
            Assert.AreEqual(output2Expected, output2, true, "Wrong output for input {0}", input2);

            string input3 = "message digest";
            string output3 = MD5.Algorithm.HashText(input3, false);
            string output3Expected = "f96b697d7cb7938d525a2f31aaf161d0";
            Assert.AreEqual(output3Expected, output3, true, "Wrong output for input {0}", input3);

            string input4 = "abcdefghijklmnopqrstuvwxyz";
            string output4 = MD5.Algorithm.HashText(input4, false);
            string output4Expected = "c3fcd3d76192e4007dfb496cca67e13b";
            Assert.AreEqual(output4Expected, output4, true, "Wrong output for input {0}", input4);

            string input5 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string output5 = MD5.Algorithm.HashText(input5, false);
            string output5Expected = "d174ab98d277d9f5a5611c2c9f419d9f";
            Assert.AreEqual(output5Expected, output5, true, "Wrong output for input {0}", input5);

            string input6 = "12345678901234567890123456789012345678901234567890123456789012345678901234567890";
            string output6 = MD5.Algorithm.HashText(input6, false);
            string output6Expected = "57edf4a22be3c955ac49da2e2107b67a";
            Assert.AreEqual(output6Expected, output6, true, "Wrong output for input {0}", input6);
        }
    }
}
