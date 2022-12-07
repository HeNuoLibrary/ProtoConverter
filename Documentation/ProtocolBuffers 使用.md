# ProtocolBuffers 使用

### 下载

win版本编译器

![微信截图_20221201082651.png](83b6bdbeeaa175fa0d144886a373f9ed.png)

C#版本

![微信截图_20221201082331.png](5fed5f8800aab59d98e03121cff91417.png)

官方下载网站 : https://developers.google.com/protocol-buffers/docs/downloads

### 构建

解压protobuf-csharp.zip 找到protobuf-csharp\protobuf\csharp\src\Google.Protobuf.sln 工程
启动并 重新构建Protobuf.dll

### protocol转换Csharp

单个转换如下bat代码   批量转换看参考链接 <u>【Unity3d】Protobuffer与Json配置使用教程</u>

```
echo on
set Path=protogen.exe
%Path%  -i:base.proto    -o:base.cs
pause
```

### 使用消息 序列化与反序列化

```
    /// <summary>
    /// Proto协议消息 序列化与反序列化
    /// </summary>
    public static class ProtoTransfer
    {
        /// <summary>
        /// 序列化泛型类
        /// </summary>
        public static byte[] SerializeProtoBuf2<T>(T data) where T : class, ProtoBuf.IExtensible
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(ms, data);
                byte[] bytes = ms.ToArray();

                ms.Close();

                return bytes;
            }
        }
        public static byte[] SerializeProtoBuf3<T>(T data) where T : Google.Protobuf.IMessage
        {
            using (MemoryStream rawOutput = new MemoryStream())
            {
                Google.Protobuf.CodedOutputStream output = new Google.Protobuf.CodedOutputStream(rawOutput);
                //output.WriteRawVarint32((uint)len);
                output.WriteMessage(data);
                output.Flush();
                byte[] result = rawOutput.ToArray();

                return result;
            }
        }

        /// <summary>
        /// 序列化解析
        /// </summary>
        public static T DeserializeProtoBuf2<T>(MessageBuffer buffer) where T : class, ProtoBuf.IExtensible
        {
            return DeserializeProtoBuf2<T>(buffer.Body());
        }
        public static T DeserializeProtoBuf3<T>(MessageBuffer buffer) where T : Google.Protobuf.IMessage, new()
        {
            return DeserializeProtoBuf3<T>(buffer.Body());
        }

        /// <summary>
        /// 序列化解析
        /// </summary>
        public static T DeserializeProtoBuf2<T>(byte[] bytes) where T : class, ProtoBuf.IExtensible
        {
            if (bytes == null) return default(T);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                T t = ProtoBuf.Serializer.Deserialize<T>(ms);
                return t;
            }
        }
        public static T DeserializeProtoBuf3<T>(byte[] bytes) where T : Google.Protobuf.IMessage, new()
        {
            if (bytes == null) return default(T);
            Google.Protobuf.CodedInputStream stream = new Google.Protobuf.CodedInputStream(bytes);
            T msg = new T();
            stream.ReadMessage(msg);
            //msg= (T)msg.Descriptor.Parser.ParseFrom(dataBytes);
            return msg;
        }
      }
```

### 参考文章 

1. [【Unity3d】Protobuffer与Json配置使用教程](https://www.bilibili.com/video/BV13E411k75W/?spm_id_from=333.1007.top_right_bar_window_history.content.click&vd_source=fce87de82e171536b5b62345675848b6)
2. [Win10下通过批处理命令文件将proto文件转C#](https://blog.csdn.net/LAUGINJI/article/details/114424385)
3. [C# ProtoBuf 序列号和反序列化](https://blog.csdn.net/qq_28218253/article/details/79401135)
4. [C#使用Protocol Buffer(ProtoBuf)进行对象的序列化与反序列化](https://blog.csdn.net/e295166319/article/details/52806791)