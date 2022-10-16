using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TcpClientDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("开始了！！！！！！");
            TcpTester tester = new TcpTester();
            Tester test = new Tester(tester);

            while (true)
            {
                tester.Send("1111111111");
                Thread.Sleep(100);
            }

        }
    }

    public class Tester
    {
        System.Timers.Timer timer;
        TcpTester tester;
        public Tester(TcpTester tester)
        {
            this.tester = tester;
            timer = new System.Timers.Timer();
            timer.Interval = 2000;
            timer.AutoReset = true;
            timer.Elapsed += StatusTimer_Elapsed;
            timer.Start();
        }

        private void StatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            tester.Send("22222222222");
        }
    }

    public class TcpTester
    {
        private TcpClient client;

        public TcpTester()
        {
            Int32 port = 13000;
            client = new TcpClient();
            client.Connect("127.0.0.1", port);
        }

        private static readonly object _lock = new object();

        public void Send(String message)
        {
            lock (_lock)
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                try
                {
                    // Create a TcpClient.
                    // Note, for this client to work you need to have a TcpServer
                    // connected to the same address as specified by the server, port
                    // combination.


                    // Translate the passed message into ASCII and store it as a Byte array.

                    // Get a client stream for reading and writing.
                    //  Stream stream = client.GetStream();

                    NetworkStream ns = client.GetStream();
                    ns.WriteTimeout = 128;
                    // Send the message to the connected TcpServer.
                    ns.Write(data, 0, data.Length);

                    Console.WriteLine("Sent: {0}", message);

                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("ArgumentNullException: {0}", e);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                try
                {
                    data = new Byte[256];

                    // String to store the response ASCII representation.
                    String responseData = String.Empty;
                    NetworkStream ns = client.GetStream();
                    ns.ReadTimeout = 128;
                    // Read the first batch of the TcpServer response bytes.
                    Int32 bytes = ns.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    if (responseData != message)
                    {
                        Console.WriteLine("线程混乱了: {0}", responseData);
                    }

                    // Close everything.
                    //stream.Close();
                    //client.Close();
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("ArgumentNullException: {0}", e);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }

            }
        }

    }
}
