using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;

namespace Remote
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var server = new TcpListener(5000);

            server.Start();
            var sock = server.AcceptTcpClient();
            var stream = sock.GetStream();

            var r = new Thread(() =>
            {
                var buf = new byte[1024];
                while (true)
                {
                    int c = stream.ReadAtLeast(buf, 1);
                    Console.Write(System.Text.Encoding.UTF8.GetString(buf, 0, c));
                }
            });

            var w = new Thread(() =>
            {
                var buf = new byte[1024];
                while (true)
                {
                    var c = (char)Console.Read();
                    while (c == -1) c = (char)Console.Read();

                    stream.Write(Encoding.UTF8.GetBytes(c.ToString()));
                }
            });

            r.Start();
            w.Start();

            while (true)
                Thread.Sleep(1000);
        }
    }
}