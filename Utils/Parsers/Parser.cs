using System.Diagnostics;
using System.Text;

namespace PodNoms.Api.Utils.Parsers
{
    public class Parser
    {
        protected Process _startProcess(string process, string arguments)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = process,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            return proc;
        }

        protected string _outputProcess(Process proc)
        {
            var line = new StringBuilder();
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                line.Append(proc.StandardOutput.ReadLine());
                // do something with line
            }
            return line.ToString();
        }
    }
}