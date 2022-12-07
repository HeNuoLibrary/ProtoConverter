using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System;

namespace Protoconverter.Editor
{
    public static class ProcessHelper
    {
        public static Process Run(string exe, string arguments, string workingDirectory = ".", bool waitExit = false)
        {
            try
            {
                bool redirectStandardOutput = true;
                bool redirectStandardError = true;
                bool useShellExecute = false;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    redirectStandardOutput = false;
                    redirectStandardError = false;
                    useShellExecute = true;
                }

                if (waitExit)
                {
                    redirectStandardOutput = true;
                    redirectStandardError = true;
                    useShellExecute = false;
                }

                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = useShellExecute,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = redirectStandardOutput,
                    RedirectStandardError = redirectStandardError,
                };

                Process process = Process.Start(info);

                if (waitExit)
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"{process.StandardOutput.ReadToEnd()} {process.StandardError.ReadToEnd()}");
                    }
                }

                return process;
            }
            catch (Exception e)
            {
                throw new Exception($"dir: {Path.GetFullPath(workingDirectory)}, command: {exe} {arguments}", e);
            }
        }

        /// <summary>
        /// 打开控制台执行拼接完成的批处理命令字符串
        /// 使用案例如下
        /// ExecBatCommand(p =>
        /// {
        ///     p(@"net use \\10.32.11.21\ERPProject yintai@123 /user:yt\ERPDeployer");
        ///     // 这里连续写入的命令将依次在控制台窗口中得到体现
        ///     p("exit 0");
        /// });
        /// </summary>
        /// <param name="inputAction">需要执行的命令委托方法：每次调用 <paramref name="inputAction"/> 中的参数都会执行一次</param>
        public static void ExecBatCommand(string exe, Action<Action<string>> inputAction)
        {
            Process pro = null;
            StreamWriter sIn = null;
            StreamReader sOut = null;

            try
            {
                pro = new Process();
                pro.StartInfo.FileName = exe;
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.CreateNoWindow = true;
                pro.StartInfo.RedirectStandardInput = true;
                pro.StartInfo.RedirectStandardOutput = true;
                pro.StartInfo.RedirectStandardError = true;

                pro.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                pro.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

                pro.Start();
                sIn = pro.StandardInput;
                sIn.AutoFlush = true;

                pro.BeginOutputReadLine();
                inputAction(value => sIn.WriteLine(value));

                pro.WaitForExit();
            }
            finally
            {
                if (pro != null && !pro.HasExited)
                    pro.Kill();
                if (sIn != null)
                    sIn.Close();
                if (sOut != null)
                    sOut.Close();
                if (pro != null)
                    pro.Close();
            }
        }
    }
}