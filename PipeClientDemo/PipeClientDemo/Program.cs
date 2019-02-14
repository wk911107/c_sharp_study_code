using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace PipeClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            PipeWriter("TheRocks");


        }

        public static void PipeWriter(string pipeName)
        {
            var pipeWriter = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
            using (var writer = new StreamWriter(pipeWriter))
            {
                pipeWriter.Connect();
                WriteLine("writer connected");

                bool completed = false;
                while (!completed)
                {
                    string input = ReadLine();
                    if (input == "bye") completed = true;

                    writer.WriteLine(input);
                    writer.Flush();
                }
            }
            WriteLine("completed writing");
        }

    }
}
