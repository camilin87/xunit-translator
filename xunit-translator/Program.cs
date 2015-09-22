using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xunit_translator
{
    class Program
    {
        static void Main(string[] args)
        {
            var updatedArgs = SanitizeCommandLineArguments(args);
            var process = CreateRealXUnitProcess(updatedArgs);
            SpawnAndWait(process);
        }

        private static void SpawnAndWait(Process process)
        {
            process.Start();
            
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();
            Environment.Exit(process.ExitCode);
        }

        private static Process CreateRealXUnitProcess(string updatedArgs)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("xunit.console.x86.exe", updatedArgs)
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            process.OutputDataReceived += process_RedirectOutput;
            process.ErrorDataReceived += process_RedirectOutput;
            return process;
        }

        private static string SanitizeCommandLineArguments(string[] args)
        {
            var updatedArgs = args
                .Select(ConvertArgumentValue)
                .Aggregate((i, j) => i + " " + j);
            return updatedArgs;
        }

        private static string ConvertArgumentValue(string arg)
        {
            if (string.Compare("/nunit", arg, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return "-nunit";
            }

            return arg;
        }

        static void process_RedirectOutput(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}
