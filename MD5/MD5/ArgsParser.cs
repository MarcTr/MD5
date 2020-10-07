using System;
using System.IO;

namespace MD5
{
    static class ArgsParser
    {
        public static string Input { set; get; }
        public static string InputFile { set; get; }
        public static bool RunTest { set; get; }
        public static bool Verbose { set; get; }

        public static void Parse(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                string cArg = args[i];
                if(cArg.Equals("-i"))
                {
                    if(i + 1 < args.Length)
                    {
                        Input = args[++i];
                    }
                    else
                    {
                        Console.Error.WriteLine("No input specified for argument -i");
                    }
                }
                else if(cArg.Equals("-f"))
                {
                    if (i + 1 < args.Length)
                    {
                        string fileArg = args[++i];
                        FileAttributes attributes = File.GetAttributes(fileArg);
                        if (!attributes.HasFlag(FileAttributes.Directory))
                            InputFile = fileArg;
                    }

                    if(InputFile == null) 
                    {
                        Console.Error.WriteLine("No valid input file specified for argument -f");
                    }
                }
                else if(cArg.Equals("-v"))
                {
                    Verbose = true;
                }
                else if(cArg.Equals("-x"))
                {
                    RunTest = true;
                }
                else
                {
                    Console.Error.WriteLine("Unknown argument {0}", cArg);
                }
            }
        }
    }
}
