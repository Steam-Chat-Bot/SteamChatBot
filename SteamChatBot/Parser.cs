using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace SteamChatBot
{
    public static class Parser
    {
        private static JavaScriptSerializer json;
        private static JavaScriptSerializer JSON
        {
            get
            {
                return json ?? (json = new JavaScriptSerializer());
            }
        }

        public static Stream ToStream(this string @this)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static T ParseXML<T>(this string @this) where T : class
        {
            XmlRootAttribute root = new XmlRootAttribute();
            root.ElementName = "profile";
            root.IsNullable = true;

            var reader = XmlReader.Create(@this.Trim().ToStream(), new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Document
            });
            return new XmlSerializer(typeof(T), root).Deserialize(reader) as T;
        }

        public static T ParseJSON<T>(this string @this) where T : class
        {
            return JSON.Deserialize<T>(@this.Trim());
        }
    }
}
