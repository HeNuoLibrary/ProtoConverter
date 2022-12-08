using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;

namespace Protoconverter.Editor
{
    public class ProtoConverter
    {
        public static string PackageName { get; } = "com.henuo.protoconverter";
        public static string DataPath => $"Packages/{PackageName}/Data~/";
        public static string ProjectDir { get; } = Directory.GetParent(Application.dataPath).ToString();
        private string mProtoDirectory = "";
        private string mOutputDirectory = "";
        public string ProtoDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(mProtoDirectory))
                {
                    mProtoDirectory = EditorPrefs.GetString("ProtoDirectory", "");
                }

                return mProtoDirectory;
            }
            set
            {
                if (mProtoDirectory != value)
                {
                    mProtoDirectory = value;
                    EditorPrefs.SetString("ProtoDirectory", mProtoDirectory);
                }
                EditorPrefs.SetString("ProtoDirectory", value);
            }
        }
        public string OutputDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(mOutputDirectory))
                {
                    mOutputDirectory = EditorPrefs.GetString("OutputDirectory", "");
                }

                return mOutputDirectory;
            }
            set
            {
                if (mOutputDirectory != value)
                {
                    mOutputDirectory = value;
                    EditorPrefs.SetString("OutputDirectory", mOutputDirectory);
                }
            }
        }

        public string Protocexe { get; set; }

        public ProtoConverter()
        {
            //string desktop = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string protoGen = Path.Combine(ProjectDir, "ProtoGen");
            if (!Directory.Exists(protoGen))
            {
                Directory.CreateDirectory(protoGen);
            }
            Protocexe = Path.Combine(protoGen, "protoc.exe").Replace("/", "\\");
            if (!File.Exists(Protocexe))
            {
                string inputPath = Path.Combine(DataPath, "protoc.exe");
                // 拷贝文件到C盘的用户AppData路径 权限问题 无法启动
                File.Copy(inputPath, Protocexe, true);

                UnityEngine.Debug.Log($"安装 protoc.exe 程序到{Protocexe}");
            }
        }

        // 转换
        public void ConvertToCSharp()
        {
            if (!Directory.Exists(ProtoDirectory))
            {
                UnityEngine.Debug.LogError("不存在Proto文件夹");
                return;
            }
            if (!Directory.Exists(OutputDirectory))
            {
                UnityEngine.Debug.LogError("不存在输出文件夹");
                return;
            }

            foreach (var file in Directory.GetFiles(ProtoDirectory))
            {
                if (Path.GetExtension(file) == ".txt" || Path.GetExtension(file) == ".proto")
                {
                    string protoName = Path.GetFileName(file);
                    string CSName = "";
                    if (Path.GetExtension(file) == ".txt")
                    {
                        CSName = protoName.Replace(".proto.txt", "") + ".cs";
                    }
                    else if (Path.GetExtension(file) == ".proto")
                    {
                        CSName = protoName.Replace(".proto", "") + ".cs";
                    }
                    InnerProto2CS.Proto2CS("ProtolMessage", protoName, ProtoDirectory, OutputDirectory);
                    UnityEngine.Debug.Log($"proto消息转C# {Path.Combine(ProtoDirectory, protoName)} 转 {Path.Combine(OutputDirectory, CSName)}");
                }
            }
        }

        // 导出程序包
        public void ExportPackage()
        {
            string inputPath = Path.Combine(DataPath, "ProtoPackage.rar");
            if (File.Exists(inputPath))
            {
                string temp = EditorUtility.OpenFolderPanel("导出程序包", Application.dataPath, string.Empty);
                string outPath = Path.Combine(temp, "ProtoPackage.rar");

                File.Copy(inputPath, outPath);

                EditorUtility.DisplayDialog("导出成功", $"输出路径 : {temp}", "确认");
            }
            else
            {
                EditorUtility.DisplayDialog("导出失败", "未找到导出文件", "确认");
            }
        }

        public static void CopyDir(string src, string dst, bool log = false)
        {
            if (log)
            {
                UnityEngine.Debug.Log($"CopyDir {src} => {dst}");
            }

            Directory.CreateDirectory(dst);
            foreach (var file in Directory.GetFiles(src))
            {
                File.Copy(file, $"{dst}/{Path.GetFileName(file)}");
            }
            foreach (var subDir in Directory.GetDirectories(src))
            {
                CopyDir(subDir, $"{dst}/{Path.GetFileName(subDir)}");
            }
        }

        public void GenProtoCSFile()
        {
            if (!Directory.Exists(ProtoDirectory))
            {
                UnityEngine.Debug.LogError("不存在Proto文件夹");
                return;
            }
            if (!Directory.Exists(OutputDirectory))
            {
                UnityEngine.Debug.LogError("不存在输出文件夹");
                return;
            }

            DirectoryInfo folder = new DirectoryInfo(ProtoDirectory); // Proto所在路径
            FileInfo[] files = folder.GetFiles("*.proto");
            List<string> cmds = new List<string>();
            foreach (FileInfo file in files)
            {
                string protoPath = ProtoDirectory.Replace("/", "\\");
                string outputPath = OutputDirectory.Replace("/", "\\");
                string cmd = Protocexe + " --csharp_out=" + outputPath + " -I " + protoPath + " " + file.FullName;
                UnityEngine.Debug.Log(cmd);
                cmds.Add(cmd);
            }
            Cmd(cmds);

            UnityEngine.Debug.Log("proto convert to CS succeed !!");

            AssetDatabase.Refresh();
        }
        public void Cmd(List<string> cmds)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = ".";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += OutputHandler;
            process.ErrorDataReceived += ErrorDataHandler;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            for (int i = 0; i < cmds.Count; i++)
            {
                process.StandardInput.WriteLine(cmds[i]);
            }
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();
        }
        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                UnityEngine.Debug.Log(outLine.Data);
            }
        }
        private void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                UnityEngine.Debug.LogError(outLine.Data);
            }
        }
    }
}