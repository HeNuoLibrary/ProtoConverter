using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Protoconverter.Editor
{
    [Flags]
    public enum HeadFlag
    {
        None = 0,
        Bson = 1,
        Proto = 2,
    }

    public static class InnerProto2CS
    {
        private static readonly char[] splitChars = { ' ', '\t' };

        public static void Proto2CS(string namspace, string protoName, string inputPath, string outputPath)
        {
            string csPath = "";
            string proto = Path.Combine(inputPath, protoName);
            if (Path.GetExtension(proto) == ".txt")
            {
                string CSName = protoName.Replace(".proto.txt", "") + ".cs";
                csPath = Path.Combine(outputPath, CSName);
            }
            else if (Path.GetExtension(proto) == ".proto")
            {
                csPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(proto) + ".cs");
            }

            string s = File.ReadAllText(proto);

            StringBuilder sb = new StringBuilder();
            sb.Append("using System.Collections.Generic;\n");
            sb.Append($"namespace {namspace}\n");
            sb.Append("{\n");

            bool isMsgStart = false;
            string parentClass = "";
            foreach (string line in s.Split('\n'))
            {
                string newline = line.Trim();

                if (newline == "")
                {
                    continue;
                }

                if (newline.StartsWith("//"))
                {
                    sb.Append($"{newline}\n");
                }

                if (newline.StartsWith("message"))
                {
                    parentClass = "";
                    isMsgStart = true;
                    string msgName = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                    string[] ss = newline.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

                    if (ss.Length == 2)
                    {
                        parentClass = ss[1].Trim();
                    }

                    sb.Append($"\tpublic partial class {msgName}");
                    if (parentClass == "IActorMessage" || parentClass == "IActorRequest" || parentClass == "IActorResponse")
                    {
                        sb.Append($": {parentClass}\n");
                    }
                    else if (parentClass != "")
                    {
                        sb.Append($": {parentClass}\n");
                    }
                    else
                    {
                        sb.Append("\n");
                    }
                    continue;
                }

                if (isMsgStart)
                {
                    if (newline == "{")
                    {
                        sb.Append("\t{\n");
                        continue;
                    }

                    if (newline == "}")
                    {
                        isMsgStart = false;
                        sb.Append("\t}\n\n");
                        continue;
                    }

                    if (newline.Trim().StartsWith("//"))
                    {
                        sb.AppendLine(newline);
                        continue;
                    }

                    if (newline.Trim() != "" && newline != "}")
                    {
                        if (newline.StartsWith("repeated"))
                        {
                            Repeated(sb, namspace, newline);
                        }
                        else
                        {
                            Members(sb, newline, true);
                        }
                    }
                }
            }
            sb.Append("}\n");

            File.WriteAllText(csPath, sb.ToString());
        }

        private static void Repeated(StringBuilder sb, string ns, string newline)
        {
            try
            {
                int index = newline.IndexOf(";");
                newline = newline.Remove(index);
                string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                string type = ss[1];
                type = ConvertType(type);
                string name = ss[2];

                sb.Append($"\t\tpublic List<{type}> {name} = new List<{type}>();\n\n");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"{newline}\n {e}");
            }

        }

        private static string ConvertType(string type)
        {
            string typeCs = "";
            switch (type)
            {
                case "int16":
                    typeCs = "short";
                    break;
                case "int32":
                    typeCs = "int";
                    break;
                case "bytes":
                    typeCs = "byte[]";
                    break;
                case "uint32":
                    typeCs = "uint";
                    break;
                case "long":
                    typeCs = "long";
                    break;
                case "int64":
                    typeCs = "long";
                    break;
                case "uint64":
                    typeCs = "ulong";
                    break;
                case "uint16":
                    typeCs = "ushort";
                    break;
                default:
                    typeCs = type;
                    break;
            }
            return typeCs;
        }

        private static void Members(StringBuilder sb, string newline, bool isRequired)
        {
            try
            {
                int index = newline.IndexOf(";");
                newline = newline.Remove(index);
                string[] ss = newline.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                string type = ss[0];
                string name = ss[1];
                string typeCs = ConvertType(type);

                sb.Append($"\t\tpublic {typeCs} {name} {{ get; set; }}\n\n");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"{newline}\n {e}");
            }

        }
    }
}