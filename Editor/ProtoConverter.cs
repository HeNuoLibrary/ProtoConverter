using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace Protoconverter.Editor
{
    public class ProtoConverter
    {
        public static string PackageName { get; } = "com.henuo.protoconverter";
        public static string DataPath => $"Packages/{PackageName}/Data~/";

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

        // 转换
        public void ConvertToCSharp()
        {
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
                    Debug.Log($"proto消息转C# {Path.Combine(ProtoDirectory, protoName)} 转 {Path.Combine(OutputDirectory, CSName)}");
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

        // 导出DLL文件
        public void ExportProtobufDll()
        {
            string dll_1 = Path.Combine(DataPath, "Google.Protobuf.dll");
            string dll_2 = Path.Combine(DataPath, "protobuf-net.dll");
            string temp = EditorUtility.OpenFolderPanel("导出DLL文件", Application.dataPath, string.Empty);


            if (File.Exists(dll_1))
            {
                string outPath = Path.Combine(temp, "Google.Protobuf.dll");

                File.Copy(dll_1, outPath);

                EditorUtility.DisplayDialog("导出成功", $"输出路径 : {temp}", "确认");
            }
            else
            {
                EditorUtility.DisplayDialog("导出失败", "未找到导出文件", "确认");
            }


            if (File.Exists(dll_2))
            {
                string outPath = Path.Combine(temp, "Google.Protobuf.dll");

                File.Copy(dll_2, outPath);

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
    }
}