using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace PipeServerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //管道名称，客户端与服务端要一致
            PipesReader("TheRocks");
        }

        private static void PipesReader(string pipeName)
        {
            try
            {
                var pipeReader = new NamedPipeServerStream(pipeName, PipeDirection.In);
                using (var reader = new StreamReader(pipeReader))
                {
                    pipeReader.WaitForConnection();
                    WriteLine("reader connected");

                    //const int BUFFERSIZE = 256;

                    bool completed = false;
                    while (!completed)
                    {
                        //byte[] buffer = new byte[BUFFERSIZE];
                        //int nRead = pipeReader.Read(buffer, 0, BUFFERSIZE);
                        //string line = Encoding.UTF8.GetString(buffer, 0, nRead);
                        string line = reader.ReadLine();
                        WriteLine(line);
                        if (line == "bye") completed = true;
                    }
                }
                WriteLine("completed reading");
                ReadLine();
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }
        }

    }
}
