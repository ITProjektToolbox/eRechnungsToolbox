using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;

namespace PipeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFilePath = @"C:\XRechnungsDrucker\logDebug.txt";

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "XRechnungsDruckerPipe", PipeDirection.Out))
            {
                try
                {
                    var inputLines = new List<string>();
                    string s;
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFilePath))
                    {
                        while (null != (s = Console.ReadLine()))
                        {
                            inputLines.Add(s);
                            file.WriteLine(s);
                        }
                    }
                    pipeClient.Connect(3000);

                    using (StreamWriter sr = new StreamWriter(pipeClient))
                    {
                        foreach (var line in inputLines)
                        {
                            sr.WriteLine(line);
                        }
                    }
                }

                catch (Exception e)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFilePath))
                    {
                        file.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}

