using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Protoconverter.Editor
{
    /// <summary>
    /// 协议转换窗口 
    /// </summary>
    public class ProtoConverterWindow : EditorWindow
    {
        private ProtoConverter m_Controller;

        [MenuItem("Tools/Proto转换工具")]
        public static void AddWindow2()
        {
            ProtoConverterWindow window = GetWindow<ProtoConverterWindow>("协议转换工具", true);
            window.minSize = new Vector2(800f, 500f);
            window.m_Controller = new ProtoConverter();
        }

        private void OnGUI()
        {
            GUI.enabled = true;
            GUILayout.Space(10f);
            EditorGUILayout.LabelField("=======================说明====================");
            EditorGUILayout.LabelField("这个工具 将指定目录下的Proto文件 转换成C#文件 输出到指定文件夹下");
            EditorGUILayout.LabelField("==============================================");
            GUILayout.Space(10f);

            GUILayout.Space(5f);
            EditorGUILayout.LabelField("环境配置", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            m_Controller.ProtoDirectory = GUISelectProtoDirectory("协议文件夹", m_Controller.ProtoDirectory, "设置");
            m_Controller.OutputDirectory = GUISelectProtoDirectory("输出文件夹", m_Controller.OutputDirectory, "设置");

            EditorGUILayout.EndVertical();

            GUILayout.Space(5f);
            EditorGUILayout.LabelField("转换", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            GUIItem("内部到处使用,proto.txt也可转换", "C#转换", m_Controller.ConvertToCSharp);
            GUIItem("外部网络通讯使用,仅支持proto文件", "Exe转换", m_Controller.GenProtoCSFile);
            EditorGUILayout.EndVertical();

            GUILayout.Space(5f);
            EditorGUILayout.LabelField("导出", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            GUIItem("导出程序包  脱离编辑器使用  需要按照说明配置bat文件", "导出程序包", m_Controller.ExportPackage);
            EditorGUILayout.EndVertical();
        }

        private void GUIItem(string content, string button, Action onClick)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(content);
            if (GUILayout.Button(button, GUILayout.Width(150)))
            {
                onClick?.Invoke();
            }
            EditorGUILayout.EndHorizontal();
        }

        private string GUISelectProtoDirectory(string content, string directory, string selectButton)
        {
            EditorGUILayout.BeginHorizontal();
            directory = EditorGUILayout.TextField(content, directory);
            if (GUILayout.Button(selectButton, GUILayout.Width(100)))
            {
                string temp = EditorUtility.OpenFolderPanel(content, directory, string.Empty);
                if (!string.IsNullOrEmpty(temp)) directory = temp;
            }
            EditorGUILayout.EndHorizontal();

            return directory;
        }
    }

}