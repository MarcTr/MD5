using System;

namespace MD5
{
    public class Program
    {

        static int Main(string[] args)
        {
            ArgsParser.Parse(args);

            if( ArgsParser.Input == null && 
                ArgsParser.InputFile == null && 
                !ArgsParser.RunTest)
            {
                Console.Error.WriteLine("No input argument specified, use parameter -i <input>, -f <filePath> or -x");
                return 0;
            }


            if(ArgsParser.RunTest)
                RunTestSuite(ArgsParser.Verbose);

            if(ArgsParser.Input != null)
                RunMessageInput(ArgsParser.Input, ArgsParser.Verbose);

            if(ArgsParser.InputFile != null)
                RunFileInput(ArgsParser.InputFile, ArgsParser.Verbose);

            return 0;
        }

        private static void RunTestSuite(bool verbose)
        {
            string[] inputs = new string[] { 
                "", "a", "abc", "message digest", 
                "abcdefghijklmnopqrstuvwxyz", 
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 
                "12345678901234567890123456789012345678901234567890123456789012345678901234567890"
            };

            Console.WriteLine("MD5 test suite:");
            Array.ForEach(inputs, input =>
            {
                string output = Algorithm.HashText(input, verbose);
                Console.WriteLine("MD5 (\"{0}\") = {1}", input, output);
            });
        }

        private static void RunFileInput(string filePath, bool verbose)
        {
            // print parsed arguments
            Console.WriteLine("Verbose: {0}", verbose);
            Console.WriteLine("File: {0}", filePath);
            Console.WriteLine();

            // hash input & display output
            string output = Algorithm.HashFile(filePath, verbose);
            Console.WriteLine("MD5 (\"{0}\") = {1}", filePath, output);
        }

        private static void RunMessageInput(string message, bool verbose)
        {
            // print parsed arguments
            Console.WriteLine("Verbose: {0}", verbose);
            Console.WriteLine("Input: {0}", message);
            Console.WriteLine();

            // hash input & display output
            string output = Algorithm.HashText(message, verbose);
            Console.WriteLine("MD5 (\"{0}\") = {1}", message, output);
        }
    }
}
